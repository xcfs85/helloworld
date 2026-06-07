using Pindou.Application.Common;

namespace Pindou.Application.DTOs.Messaging;

public class MessageDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public object? Extras { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreateTime { get; set; }
}

public class MessageSettingDto
{
    public bool CommentEnabled { get; set; } = true;
    public bool LikeEnabled { get; set; } = true;
    public bool FollowEnabled { get; set; } = true;
    public bool AtEnabled { get; set; } = true;
    public bool SystemEnabled { get; set; } = true;
    public bool MarketingEnabled { get; set; } = true;
    public bool QuietHoursEnabled { get; set; }
    public string QuietHoursStart { get; set; } = "22:00";
    public string QuietHoursEnd { get; set; } = "08:00";
}

public class UpdateMessageSettingRequest
{
    public bool? CommentEnabled { get; set; }
    public bool? LikeEnabled { get; set; }
    public bool? FollowEnabled { get; set; }
    public bool? AtEnabled { get; set; }
    public bool? SystemEnabled { get; set; }
    public bool? MarketingEnabled { get; set; }
    public bool? QuietHoursEnabled { get; set; }
    public string? QuietHoursStart { get; set; }
    public string? QuietHoursEnd { get; set; }
}
