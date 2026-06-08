using System.Text.Json;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.DTOs.Messaging;
using Pindou.Application.DTOs.Operation;
using Pindou.Application.Interfaces.Messaging;
using Pindou.Domain.Entities.Messaging;
using Pindou.Domain.Entities.Operation;
using Pindou.Infrastructure.Repositories;
using SqlSugar;

namespace Pindou.Infrastructure.Services.Messaging;

public class MessageService : IMessageService
{
    private readonly IRepository<Message> _messageRepo;
    private readonly IRepository<MessageSetting> _settingRepo;

    public MessageService(IRepository<Message> messageRepo, IRepository<MessageSetting> settingRepo)
    {
        _messageRepo = messageRepo;
        _settingRepo = settingRepo;
    }

    public async Task SendAsync(string userId, string type, string title, string content, object? extras = null)
    {
        var setting = await _settingRepo.FirstOrDefaultAsync(s => s.UserId == userId);
        if (setting != null)
        {
            var enabled = type switch
            {
                "comment" => setting.CommentEnabled,
                "like" => setting.LikeEnabled,
                "follow" => setting.FollowEnabled,
                "at" => setting.AtEnabled,
                "system" => setting.SystemEnabled,
                "marketing" => setting.MarketingEnabled,
                _ => true
            };
            if (!enabled) return;

            if (setting.QuietHoursEnabled && IsInQuietHours(setting.QuietHoursStart, setting.QuietHoursEnd))
                return;
        }

        await _messageRepo.InsertAsync(new Message
        {
            UserId = userId,
            Type = type,
            Title = title,
            Content = content,
            Extras = extras != null ? JsonSerializer.Serialize(extras) : null
        });
    }

    public async Task<PagedResult<MessageDto>> GetMessagesAsync(string userId, string? type, PageRequest request)
    {
        var exp = Expressionable.Create<Message>().And(m => m.UserId == userId);
        if (!string.IsNullOrWhiteSpace(type))
            exp.And(m => m.Type == type);

        var (list, total) = await _messageRepo.GetPagedAsync(
            exp.ToExpression(),
            request.Page,
            request.Size,
            m => m.CreateTime,
            true);

        var result = new PagedResult<MessageDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<MessageDto>()
        };

        foreach (var msg in list)
        {
            object? extras = null;
            if (!string.IsNullOrWhiteSpace(msg.Extras))
            {
                try { extras = JsonSerializer.Deserialize<object>(msg.Extras); }
                catch { }
            }

            result.List.Add(new MessageDto
            {
                Id = msg.Id,
                Type = msg.Type,
                Title = msg.Title,
                Content = msg.Content,
                Extras = extras,
                IsRead = msg.IsRead,
                CreateTime = msg.CreateTime
            });
        }

        return result;
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _messageRepo.CountAsync(m => m.UserId == userId && !m.IsRead);
    }

    public async Task<bool> MarkReadAsync(string userId, string messageId)
    {
        var msg = await _messageRepo.GetByIdAsync(messageId);
        if (msg == null) throw new BizException("消息不存在", ErrorCodes.NotFound);
        if (msg.UserId != userId) throw new BizException("无权操作", ErrorCodes.NoPermission);

        msg.IsRead = true;
        msg.UpdateTime = DateTime.Now;
        return await _messageRepo.UpdateAsync(msg);
    }

    public async Task<bool> MarkAllReadAsync(string userId)
    {
        var unreadMessages = await _messageRepo.GetListAsync(m => m.UserId == userId && !m.IsRead);
        if (unreadMessages.Count == 0) return true;

        foreach (var msg in unreadMessages)
        {
            msg.IsRead = true;
            msg.UpdateTime = DateTime.Now;
        }
        return await _messageRepo.UpdateRangeAsync(unreadMessages);
    }

    public async Task<MessageSettingDto> GetSettingsAsync(string userId)
    {
        var setting = await _settingRepo.FirstOrDefaultAsync(s => s.UserId == userId);
        if (setting == null)
        {
            setting = new MessageSetting { UserId = userId };
            await _settingRepo.InsertAsync(setting);
        }

        return new MessageSettingDto
        {
            CommentEnabled = setting.CommentEnabled,
            LikeEnabled = setting.LikeEnabled,
            FollowEnabled = setting.FollowEnabled,
            AtEnabled = setting.AtEnabled,
            SystemEnabled = setting.SystemEnabled,
            MarketingEnabled = setting.MarketingEnabled,
            QuietHoursEnabled = setting.QuietHoursEnabled,
            QuietHoursStart = setting.QuietHoursStart,
            QuietHoursEnd = setting.QuietHoursEnd
        };
    }

    public async Task<MessageSettingDto> UpdateSettingsAsync(string userId, UpdateMessageSettingRequest request)
    {
        var setting = await _settingRepo.FirstOrDefaultAsync(s => s.UserId == userId);
        if (setting == null)
        {
            setting = new MessageSetting { UserId = userId };
            await _settingRepo.InsertAsync(setting);
        }

        if (request.CommentEnabled.HasValue) setting.CommentEnabled = request.CommentEnabled.Value;
        if (request.LikeEnabled.HasValue) setting.LikeEnabled = request.LikeEnabled.Value;
        if (request.FollowEnabled.HasValue) setting.FollowEnabled = request.FollowEnabled.Value;
        if (request.AtEnabled.HasValue) setting.AtEnabled = request.AtEnabled.Value;
        if (request.SystemEnabled.HasValue) setting.SystemEnabled = request.SystemEnabled.Value;
        if (request.MarketingEnabled.HasValue) setting.MarketingEnabled = request.MarketingEnabled.Value;
        if (request.QuietHoursEnabled.HasValue) setting.QuietHoursEnabled = request.QuietHoursEnabled.Value;
        if (request.QuietHoursStart != null) setting.QuietHoursStart = request.QuietHoursStart;
        if (request.QuietHoursEnd != null) setting.QuietHoursEnd = request.QuietHoursEnd;

        setting.UpdateTime = DateTime.Now;
        await _settingRepo.UpdateAsync(setting);

        return new MessageSettingDto
        {
            CommentEnabled = setting.CommentEnabled,
            LikeEnabled = setting.LikeEnabled,
            FollowEnabled = setting.FollowEnabled,
            AtEnabled = setting.AtEnabled,
            SystemEnabled = setting.SystemEnabled,
            MarketingEnabled = setting.MarketingEnabled,
            QuietHoursEnabled = setting.QuietHoursEnabled,
            QuietHoursStart = setting.QuietHoursStart,
            QuietHoursEnd = setting.QuietHoursEnd
        };
    }

    private static bool IsInQuietHours(string start, string end)
    {
        if (!TimeSpan.TryParse(start, out var s) || !TimeSpan.TryParse(end, out var e))
            return false;
        var now = DateTime.Now.TimeOfDay;
        return s < e ? (now >= s && now <= e) : (now >= s || now <= e);
    }
}

public class OperationService : IOperationService
{
    private readonly IRepository<Banner> _bannerRepo;
    private readonly IRepository<SpecialTopic> _specialTopicRepo;
    private readonly IRepository<OperationTopic> _topicRepo;
    private readonly IRepository<PushRecord> _pushRepo;

    public OperationService(
        IRepository<Banner> bannerRepo,
        IRepository<SpecialTopic> specialTopicRepo,
        IRepository<OperationTopic> topicRepo,
        IRepository<PushRecord> pushRepo)
    {
        _bannerRepo = bannerRepo;
        _specialTopicRepo = specialTopicRepo;
        _topicRepo = topicRepo;
        _pushRepo = pushRepo;
    }

    public async Task<List<BannerDto>> GetActiveBannersAsync(string position)
    {
        var now = DateTime.Now;
        var banners = await _bannerRepo.GetListAsync(
            b => b.Position == position && b.Status == "active" && b.StartTime <= now && b.EndTime >= now,
            nameof(Banner.Sort),
            false);

        return banners.Select(b => new BannerDto
        {
            Id = b.Id,
            Title = b.Title,
            ImageUrl = b.ImageUrl,
            LinkType = b.LinkType,
            LinkValue = b.LinkValue,
            Position = b.Position,
            Sort = b.Sort
        }).ToList();
    }

    public async Task<PagedResult<SpecialTopicDto>> GetActiveSpecialTopicsAsync(PageRequest request)
    {
        var now = DateTime.Now;
        var (list, total) = await _specialTopicRepo.GetPagedAsync(
            s => s.Status == 1 && s.StartTime <= now && s.EndTime >= now,
            request.Page,
            request.Size,
            s => s.StartTime,
            true);

        var result = new PagedResult<SpecialTopicDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<SpecialTopicDto>()
        };

        foreach (var topic in list)
        {
            var templateIds = new List<string>();
            if (!string.IsNullOrWhiteSpace(topic.TemplateIds))
            {
                try { templateIds = JsonSerializer.Deserialize<List<string>>(topic.TemplateIds) ?? new(); }
                catch { }
            }

            result.List.Add(new SpecialTopicDto
            {
                Id = topic.Id,
                Name = topic.Name,
                Description = topic.Description,
                CoverUrl = topic.CoverUrl,
                TemplateIds = templateIds,
                StartTime = topic.StartTime,
                EndTime = topic.EndTime
            });
        }

        return result;
    }

    public async Task<SpecialTopicDto> GetSpecialTopicAsync(string id)
    {
        var topic = await _specialTopicRepo.GetByIdAsync(id);
        if (topic == null) throw new BizException("专题不存在", ErrorCodes.NotFound);

        var templateIds = new List<string>();
        if (!string.IsNullOrWhiteSpace(topic.TemplateIds))
        {
            try { templateIds = JsonSerializer.Deserialize<List<string>>(topic.TemplateIds) ?? new(); }
            catch { }
        }

        return new SpecialTopicDto
        {
            Id = topic.Id,
            Name = topic.Name,
            Description = topic.Description,
            CoverUrl = topic.CoverUrl,
            TemplateIds = templateIds,
            StartTime = topic.StartTime,
            EndTime = topic.EndTime
        };
    }

    public async Task<List<TopicDto>> GetOfficialTopicsAsync()
    {
        var topics = await _topicRepo.GetListAsync(t => t.IsOfficial == 1 && t.Status == "active");
        return topics.Select(t => new TopicDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            CoverUrl = t.CoverUrl,
            PostCount = t.PostCount
        }).ToList();
    }

    public async Task<int> GetActivePushCountAsync()
    {
        return await _pushRepo.CountAsync(p => p.Status == "pending" || p.Status == "sending");
    }
}