using Pindou.Application.Common;
using Pindou.Application.DTOs.Messaging;
using Pindou.Application.Interfaces.Messaging;
using Pindou.Application.DTOs.Operation;
using Pindou.Domain.Entities.Messaging;
using Pindou.Domain.Entities.Operation;
using Pindou.Infrastructure.Repositories;

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
        // 1. 检查消息设置
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

            // 勿扰时段
            if (setting.QuietHoursEnabled && IsInQuietHours(setting.QuietHoursStart, setting.QuietHoursEnd))
                return;
        }

        await _messageRepo.InsertAsync(new Message
        {
            UserId = userId,
            Type = type,
            Title = title,
            Content = content,
            Extras = extras != null ? System.Text.Json.JsonSerializer.Serialize(extras) : null
        });
    }

    public Task<PagedResult<MessageDto>> GetMessagesAsync(string userId, string? type, PageRequest request) { throw new NotImplementedException(); }
    public Task<int> GetUnreadCountAsync(string userId) { throw new NotImplementedException(); }
    public Task<bool> MarkReadAsync(string userId, string messageId) { throw new NotImplementedException(); }
    public Task<bool> MarkAllReadAsync(string userId) { throw new NotImplementedException(); }
    public Task<MessageSettingDto> GetSettingsAsync(string userId) { throw new NotImplementedException(); }
    public Task<MessageSettingDto> UpdateSettingsAsync(string userId, UpdateMessageSettingRequest request) { throw new NotImplementedException(); }

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
    public OperationService(
        IRepository<Banner> bannerRepo,
        IRepository<SpecialTopic> specialTopicRepo,
        IRepository<OperationTopic> topicRepo)
    {
        _bannerRepo = bannerRepo;
        _specialTopicRepo = specialTopicRepo;
        _topicRepo = topicRepo;
    }

    public Task<List<BannerDto>> GetActiveBannersAsync(string position) { throw new NotImplementedException(); }
    public Task<PagedResult<SpecialTopicDto>> GetActiveSpecialTopicsAsync(PageRequest request) { throw new NotImplementedException(); }
    public Task<SpecialTopicDto> GetSpecialTopicAsync(string id) { throw new NotImplementedException(); }
    public Task<List<TopicDto>> GetOfficialTopicsAsync() { throw new NotImplementedException(); }
    public Task<int> GetActivePushCountAsync() { throw new NotImplementedException(); }
}
