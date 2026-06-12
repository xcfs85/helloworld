using SqlSugar;
using Pindou.Domain.Common;

namespace Pindou.Domain.Entities.Community;

/// <summary>
/// 帖子表
/// </summary>
[SugarTable("posts")]
public class Post : UuidEntity
{
    /// <summary>作者ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>类型:work/request/tutorial/discussion</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Type { get; set; } = string.Empty;

    /// <summary>标题</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Title { get; set; } = string.Empty;

    /// <summary>正文内容</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Content { get; set; } = string.Empty;

    /// <summary>媒体数组(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Media { get; set; } = "[]";

    /// <summary>话题ID数组(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? TopicIds { get; set; }

    /// <summary>拼豆参数(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? BeadParams { get; set; }

    /// <summary>关联图纸ID</summary>
    [SugarColumn(Length = 36, IsNullable = true)]
    public string? DiagramId { get; set; }

    /// <summary>点赞数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int LikeCount { get; set; }

    /// <summary>评论数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int CommentCount { get; set; }

    /// <summary>收藏数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int FavoriteCount { get; set; }

    /// <summary>浏览数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int ViewCount { get; set; }

    /// <summary>状态:pending/active/deleted</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'active'")]
    public string Status { get; set; } = "active";

    /// <summary>审核状态:pending/approved/rejected</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'pending'")]
    public string ReviewStatus { get; set; } = "pending";

    /// <summary>拒绝原因</summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReviewReason { get; set; }

    /// <summary>AI风险等级:none/low/mid/high</summary>
    [SugarColumn(Length = 10, IsNullable = false, DefaultValue = "'none'")]
    public string RiskLevel { get; set; } = "none";

    /// <summary>AI风险标签(JSON数组)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? RiskTags { get; set; }

    /// <summary>发布时间</summary>
    [SugarColumn(IsNullable = false)]
    public DateTime PublishTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 评论表
/// </summary>
[SugarTable("comments")]
public class Comment : UuidEntity
{
    /// <summary>帖子ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string PostId { get; set; } = string.Empty;

    /// <summary>评论者ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>父评论ID</summary>
    [SugarColumn(Length = 36, IsNullable = true)]
    public string? ParentId { get; set; }

    /// <summary>回复目标用户ID</summary>
    [SugarColumn(Length = 36, IsNullable = true)]
    public string? ReplyToUserId { get; set; }

    /// <summary>评论内容</summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string Content { get; set; } = string.Empty;

    /// <summary>点赞数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int LikeCount { get; set; }

    /// <summary>状态</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'active'")]
    public string Status { get; set; } = "active";
}

/// <summary>
/// 点赞表
/// </summary>
[SugarTable("likes")]
public class Like : UuidEntity
{
    /// <summary>用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>目标ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string TargetId { get; set; } = string.Empty;

    /// <summary>目标类型:post/comment</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string TargetType { get; set; } = string.Empty;
}

/// <summary>
/// 收藏表
/// </summary>
[SugarTable("favorites")]
public class Favorite : UuidEntity
{
    /// <summary>用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>目标ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string TargetId { get; set; } = string.Empty;

    /// <summary>目标类型:post/template</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string TargetType { get; set; } = string.Empty;
}

/// <summary>
/// 关注表
/// </summary>
[SugarTable("follows")]
public class Follow : UuidEntity
{
    /// <summary>关注者ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>被关注者ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string FollowUserId { get; set; } = string.Empty;
}

/// <summary>
/// 话题表
/// </summary>
[SugarTable("topics")]
public class Topic : UuidEntity
{
    /// <summary>话题名称</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>描述</summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>封面图</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? CoverUrl { get; set; }

    /// <summary>帖子数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int PostCount { get; set; }

    /// <summary>状态</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'active'")]
    public string Status { get; set; } = "active";

    /// <summary>是否热门</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public bool IsHot { get; set; }
}
