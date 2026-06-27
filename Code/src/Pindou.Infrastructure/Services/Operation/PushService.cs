using System.Text.Json;
using Microsoft.Extensions.Logging;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Operation;
using Pindou.Application.Interfaces.Operation;
using Pindou.Application.Interfaces.System;
using Pindou.Domain.Entities.Operation;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.ExternalServices.Push;
using Pindou.Infrastructure.Repositories;
using SqlSugar;
using DomainUser = Pindou.Domain.Entities.User.User;
using DomainDevice = Pindou.Domain.Entities.User.Device;

/**
 *@Author: 西成峰
 *@CreateTime: 2026-06-27
 *@Description: 推送服务实现
 */

namespace Pindou.Infrastructure.Services.Operation;

/// <summary>
/// 推送服务实现
/// </summary>
public class PushService : IPushService
{
    private readonly IRepository<PushRecord> _pushRepo;
    private readonly IRepository<DomainUser> _userRepo;
    private readonly IRepository<DomainDevice> _deviceRepo;
    private readonly IEnumerable<IPushChannelProvider> _channelProviders;
    private readonly ISystemConfigService _configService;
    private readonly ILogger<PushService> _logger;

    public PushService(
        IRepository<PushRecord> pushRepo,
        IRepository<DomainUser> userRepo,
        IRepository<DomainDevice> deviceRepo,
        IEnumerable<IPushChannelProvider> channelProviders,
        ISystemConfigService configService,
        ILogger<PushService> logger)
    {
        _pushRepo = pushRepo;
        _userRepo = userRepo;
        _deviceRepo = deviceRepo;
        _channelProviders = channelProviders;
        _configService = configService;
        _logger = logger;
    }

    /// <summary>推送记录分页列表</summary>
    public async Task<PagedResult<PushRecordDto>> GetPushListAsync(PushListQuery query)
    {
        var exp = Expressionable.Create<PushRecord>();

        if (!string.IsNullOrWhiteSpace(query.Status))
            exp.And(p => p.Status == query.Status);

        if (!string.IsNullOrWhiteSpace(query.PushType))
            exp.And(p => p.PushType == query.PushType);

        if (!string.IsNullOrWhiteSpace(query.TargetType))
            exp.And(p => p.TargetType == query.TargetType);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            exp.And(p => p.Title.Contains(query.Keyword));

        var (list, total) = await _pushRepo.GetPagedAsync(
            exp.ToExpression(),
            query.Page,
            query.Size,
            p => p.CreateTime,
            true);

        return new PagedResult<PushRecordDto>
        {
            Page = query.Page,
            Size = query.Size,
            Total = total,
            List = list.Select(MapToDto).ToList()
        };
    }

    /// <summary>获取推送详情</summary>
    public async Task<PushRecordDto> GetPushAsync(string id)
    {
        var record = await _pushRepo.GetByIdAsync(id);
        if (record == null) throw new BizException("推送记录不存在", ErrorCodes.NotFound);
        return MapToDto(record);
    }

    /// <summary>创建推送(草稿)</summary>
    public async Task<string> CreatePushAsync(SendPushRequest request)
    {
        ValidateRequest(request);

        var record = BuildPushRecord(request, "draft");
        await _pushRepo.InsertAsync(record);

        _logger.LogInformation("创建推送草稿: PushId={PushId}, Title={Title}", record.PushId, record.Title);
        return record.Id;
    }

    /// <summary>立即发送推送</summary>
    public async Task<string> SendPushAsync(SendPushRequest request)
    {
        ValidateRequest(request);

        var record = BuildPushRecord(request, "sending");
        await _pushRepo.InsertAsync(record);

        _logger.LogInformation("开始发送推送: PushId={PushId}, Title={Title}", record.PushId, record.Title);

        // 异步执行推送发送
        _ = ExecutePushAsync(record, request);

        return record.Id;
    }

    /// <summary>定时推送</summary>
    public async Task<string> SchedulePushAsync(SchedulePushRequest request)
    {
        ValidateRequest(request);

        if (request.ScheduleTime <= DateTime.Now)
            throw new BizException("定时推送时间必须大于当前时间", ErrorCodes.ParamError);

        var record = BuildPushRecord(request, "pending");
        record.ScheduleTime = request.ScheduleTime;
        await _pushRepo.InsertAsync(record);

        _logger.LogInformation("创建定时推送: PushId={PushId}, ScheduleTime={ScheduleTime}",
            record.PushId, record.ScheduleTime);
        return record.Id;
    }

    /// <summary>取消定时推送</summary>
    public async Task<bool> CancelPushAsync(string id)
    {
        var record = await _pushRepo.GetByIdAsync(id);
        if (record == null) throw new BizException("推送记录不存在", ErrorCodes.NotFound);

        if (record.Status != "pending")
            throw new BizException("仅待发送状态的推送可以取消", ErrorCodes.BadRequest);

        record.Status = "canceled";
        record.UpdateTime = DateTime.Now;
        return await _pushRepo.UpdateAsync(record);
    }

    /// <summary>重新发送失败的推送</summary>
    public async Task<bool> RetryPushAsync(string id)
    {
        var record = await _pushRepo.GetByIdAsync(id);
        if (record == null) throw new BizException("推送记录不存在", ErrorCodes.NotFound);

        if (record.Status != "failed")
            throw new BizException("仅失败状态的推送可以重试", ErrorCodes.BadRequest);

        record.Status = "sending";
        record.UpdateTime = DateTime.Now;
        await _pushRepo.UpdateAsync(record);

        // 重新构建发送请求并执行
        var request = new SendPushRequest
        {
            Title = record.Title,
            Content = record.Content,
            PushType = record.PushType,
            TargetType = record.TargetType,
            TargetParam = record.TargetParam,
            Channels = ParseChannels(record.Channels)
        };

        _ = ExecutePushAsync(record, request);

        return true;
    }

    /// <summary>
    /// 异步执行推送发送
    /// </summary>
    private async Task ExecutePushAsync(PushRecord record, SendPushRequest request)
    {
        try
        {
            var totalCount = 0;
            var successCount = 0;
            var failCount = 0;

            // 收集目标用户信息
            var targetUsers = await GetTargetUsersAsync(request.TargetType, request.TargetParam, request.TargetIds);
            var devices = await GetTargetDevicesAsync(targetUsers);

            foreach (var channelName in request.Channels)
            {
                var provider = _channelProviders.FirstOrDefault(p => p.Channel == channelName);
                if (provider == null)
                {
                    _logger.LogWarning("未找到推送渠道提供者: {Channel}", channelName);
                    continue;
                }

                // 构建渠道消息
                var message = BuildChannelMessage(request, channelName, targetUsers, devices);

                try
                {
                    var result = await provider.SendAsync(message);
                    totalCount += result.TotalCount;
                    successCount += result.SuccessCount;
                    failCount += result.FailCount;

                    _logger.LogInformation("推送渠道 {Channel} 发送完成: Success={Success}, Fail={Fail}",
                        channelName, result.SuccessCount, result.FailCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "推送渠道 {Channel} 发送异常", channelName);
                    failCount += targetUsers.Count;
                }
            }

            // 更新推送记录
            var latestRecord = await _pushRepo.GetByIdAsync(record.Id);
            if (latestRecord != null)
            {
                latestRecord.TotalCount = totalCount;
                latestRecord.SuccessCount = successCount;
                latestRecord.FailCount = failCount;
                latestRecord.SendTime = DateTime.Now;
                latestRecord.Status = failCount == 0 ? "sent" : (successCount > 0 ? "sent" : "failed");
                latestRecord.UpdateTime = DateTime.Now;
                await _pushRepo.UpdateAsync(latestRecord);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "推送发送异常: PushId={PushId}", record.PushId);

            // 更新为失败状态
            var latestRecord = await _pushRepo.GetByIdAsync(record.Id);
            if (latestRecord != null)
            {
                latestRecord.Status = "failed";
                latestRecord.UpdateTime = DateTime.Now;
                await _pushRepo.UpdateAsync(latestRecord);
            }
        }
    }

    /// <summary>
    /// 获取目标用户列表
    /// </summary>
    private async Task<List<DomainUser>> GetTargetUsersAsync(string targetType, string? targetParam, List<string>? targetIds)
    {
        return targetType switch
        {
            "all" => await _userRepo.GetListAsync(u => u.Status == "active"),
            "user" when targetIds?.Count > 0 => await _userRepo.GetListAsync(u => targetIds.Contains(u.Id)),
            "tag" => await GetUsersByTagAsync(targetParam),
            _ => await _userRepo.GetListAsync(u => u.Status == "active")
        };
    }

    /// <summary>
    /// 按标签获取用户（首期返回全部活跃用户，后续对接标签系统）
    /// </summary>
    private async Task<List<DomainUser>> GetUsersByTagAsync(string? tag)
    {
        // TODO: 对接用户标签系统
        _logger.LogInformation("按标签获取用户: Tag={Tag}, 暂返回全部活跃用户", tag);
        return await _userRepo.GetListAsync(u => u.Status == "active");
    }

    /// <summary>
    /// 获取目标用户的设备列表
    /// </summary>
    private async Task<List<DomainDevice>> GetTargetDevicesAsync(List<DomainUser> users)
    {
        var userIds = users.Select(u => u.Id).ToList();
        if (userIds.Count == 0) return new List<DomainDevice>();

        return await _deviceRepo.GetListAsync(d => userIds.Contains(d.UserId) && d.PushToken != null);
    }

    /// <summary>
    /// 构建渠道消息
    /// </summary>
    private static PushChannelMessage BuildChannelMessage(
        SendPushRequest request,
        string channelName,
        List<DomainUser> targetUsers,
        List<DomainDevice> devices)
    {
        var message = new PushChannelMessage
        {
            Title = request.Title,
            Content = request.Content,
            TargetType = request.TargetType,
            TargetParam = request.TargetParam,
            TargetUserIds = targetUsers.Select(u => u.Id).ToList()
        };

        switch (channelName)
        {
            case "app":
                // App推送使用 registration_id（即 PushToken）
                message.RegistrationIds = devices
                    .Where(d => !string.IsNullOrEmpty(d.PushToken))
                    .Select(d => d.PushToken!)
                    .Distinct()
                    .ToList();
                break;

            case "sms":
                // 短信推送使用用户手机号
                message.PhoneNumbers = targetUsers
                    .Where(u => !string.IsNullOrEmpty(u.Phone))
                    .Select(u => u.Phone!)
                    .Distinct()
                    .ToList();
                break;

            case "email":
                // 邮件推送：当前 User 实体无 Email 字段，暂从 TargetParam 解析
                if (!string.IsNullOrEmpty(request.TargetParam))
                {
                    try
                    {
                        message.EmailAddresses = JsonSerializer.Deserialize<List<string>>(request.TargetParam);
                    }
                    catch
                    {
                        message.EmailAddresses = new List<string> { request.TargetParam };
                    }
                }
                break;
        }

        return message;
    }

    /// <summary>
    /// 构建推送记录实体
    /// </summary>
    private static PushRecord BuildPushRecord(SendPushRequest request, string status)
    {
        return new PushRecord
        {
            PushId = Guid.NewGuid().ToString("N")[..16],
            Title = request.Title,
            Content = request.Content,
            PushType = request.PushType,
            TargetType = request.TargetType,
            TargetParam = request.TargetIds?.Count > 0
                ? JsonSerializer.Serialize(request.TargetIds)
                : request.TargetParam,
            Channels = request.Channels.Count > 0
                ? JsonSerializer.Serialize(request.Channels)
                : null,
            Status = status
        };
    }

    /// <summary>
    /// 验证推送请求参数
    /// </summary>
    private static void ValidateRequest(SendPushRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new BizException("推送标题不能为空", ErrorCodes.ParamError);
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new BizException("推送内容不能为空", ErrorCodes.ParamError);
        if (request.Channels == null || request.Channels.Count == 0)
            throw new BizException("至少选择一个推送渠道", ErrorCodes.ParamError);
    }

    /// <summary>
    /// 解析 Channels JSON 字符串
    /// </summary>
    private static List<string> ParseChannels(string? channels)
    {
        if (string.IsNullOrEmpty(channels)) return new List<string> { "app" };
        try
        {
            return JsonSerializer.Deserialize<List<string>>(channels) ?? new List<string> { "app" };
        }
        catch
        {
            return new List<string> { "app" };
        }
    }

    /// <summary>
    /// PushRecord 实体映射为 DTO
    /// </summary>
    private static PushRecordDto MapToDto(PushRecord record)
    {
        return new PushRecordDto
        {
            Id = record.Id,
            PushId = record.PushId,
            Title = record.Title,
            Content = record.Content,
            PushType = record.PushType,
            TargetType = record.TargetType,
            TargetParam = record.TargetParam,
            Channels = ParseChannels(record.Channels),
            ScheduleTime = record.ScheduleTime,
            SendTime = record.SendTime,
            TotalCount = record.TotalCount,
            SuccessCount = record.SuccessCount,
            FailCount = record.FailCount,
            ClickCount = record.ClickCount,
            Status = record.Status,
            CreateTime = record.CreateTime
        };
    }
}
