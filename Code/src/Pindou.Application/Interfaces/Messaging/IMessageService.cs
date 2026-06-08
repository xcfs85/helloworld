using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.DTOs.Messaging;
using Pindou.Application.DTOs.Operation;

namespace Pindou.Application.Interfaces.Messaging;

public interface IMessageService
{
    Task SendAsync(string userId, string type, string title, string content, object? extras = null);
    Task<PagedResult<MessageDto>> GetMessagesAsync(string userId, string? type, PageRequest request);
    Task<int> GetUnreadCountAsync(string userId);
    Task<bool> MarkReadAsync(string userId, string messageId);
    Task<bool> MarkAllReadAsync(string userId);
    Task<MessageSettingDto> GetSettingsAsync(string userId);
    Task<MessageSettingDto> UpdateSettingsAsync(string userId, UpdateMessageSettingRequest request);
}

public interface IOperationService
{
    Task<List<BannerDto>> GetActiveBannersAsync(string position);
    Task<PagedResult<SpecialTopicDto>> GetActiveSpecialTopicsAsync(PageRequest request);
    Task<SpecialTopicDto> GetSpecialTopicAsync(string id);
    Task<List<TopicDto>> GetOfficialTopicsAsync();
    Task<int> GetActivePushCountAsync();
}
