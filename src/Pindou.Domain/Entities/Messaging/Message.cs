using SqlSugar;
using Pindou.Domain.Common;

namespace Pindou.Domain.Entities.Messaging;

/// <summary>
/// 消息表
/// </summary>
[SugarTable("messages")]
public class Message : UuidEntity
{
    /// <summary>接收用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>类型:comment/like/follow/at/system</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Type { get; set; } = string.Empty;

    /// <summary>标题</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Title { get; set; } = string.Empty;

    /// <summary>内容</summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string Content { get; set; } = string.Empty;

    /// <summary>扩展数据(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Extras { get; set; }

    /// <summary>是否已读</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public bool IsRead { get; set; }
}

/// <summary>
/// 用户消息设置表
/// </summary>
[SugarTable("message_settings")]
public class MessageSetting : UuidEntity
{
    /// <summary>用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>评论通知</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public bool CommentEnabled { get; set; } = true;

    /// <summary>点赞通知</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public bool LikeEnabled { get; set; } = true;

    /// <summary>关注通知</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public bool FollowEnabled { get; set; } = true;

    /// <summary>@通知</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public bool AtEnabled { get; set; } = true;

    /// <summary>系统通知</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public bool SystemEnabled { get; set; } = true;

    /// <summary>营销通知</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public bool MarketingEnabled { get; set; } = true;

    /// <summary>勿扰模式</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public bool QuietHoursEnabled { get; set; }

    /// <summary>勿扰开始</summary>
    [SugarColumn(Length = 10, IsNullable = false, DefaultValue = "'22:00'")]
    public string QuietHoursStart { get; set; } = "22:00";

    /// <summary>勿扰结束</summary>
    [SugarColumn(Length = 10, IsNullable = false, DefaultValue = "'08:00'")]
    public string QuietHoursEnd { get; set; } = "08:00";
}
