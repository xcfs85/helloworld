using Pindou.Application.Common;
using Pindou.Application.DTOs.Operation;

namespace Pindou.Application.Interfaces.Operation;

/**
 *@Author: 西成峰
 *@CreateTime: 2026-06-27
 *@Description: 推送服务接口
 */

/// <summary>
/// 推送服务接口
/// </summary>
public interface IPushService
{
    /// <summary>推送记录分页列表</summary>
    Task<PagedResult<PushRecordDto>> GetPushListAsync(PushListQuery query);

    /// <summary>获取推送详情</summary>
    Task<PushRecordDto> GetPushAsync(string id);

    /// <summary>创建推送(草稿)</summary>
    Task<string> CreatePushAsync(SendPushRequest request);

    /// <summary>立即发送推送</summary>
    Task<string> SendPushAsync(SendPushRequest request);

    /// <summary>定时推送</summary>
    Task<string> SchedulePushAsync(SchedulePushRequest request);

    /// <summary>取消定时推送</summary>
    Task<bool> CancelPushAsync(string id);

    /// <summary>重新发送失败的推送</summary>
    Task<bool> RetryPushAsync(string id);
}
