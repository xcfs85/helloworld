using SqlSugar;
using Pindou.Domain.Common;

namespace Pindou.Domain.Entities.Operation;

/// <summary>
/// Banner表
/// </summary>
[SugarTable("banners")]
public class Banner : UuidEntity
{
    /// <summary>标题</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Title { get; set; } = string.Empty;

    /// <summary>图片URL</summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>跳转类型:url/post/template</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string LinkType { get; set; } = string.Empty;

    /// <summary>跳转值</summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string LinkValue { get; set; } = string.Empty;

    /// <summary>位置:home_top/template_top</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Position { get; set; } = string.Empty;

    /// <summary>排序</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int Sort { get; set; }

    /// <summary>状态</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'active'")]
    public string Status { get; set; } = "active";

    /// <summary>开始时间</summary>
    [SugarColumn(IsNullable = false)]
    public DateTime StartTime { get; set; }

    /// <summary>结束时间</summary>
    [SugarColumn(IsNullable = false)]
    public DateTime EndTime { get; set; }
}

/// <summary>
/// 运营话题表
/// </summary>
[SugarTable("operation_topics")]
public class OperationTopic : UuidEntity
{
    /// <summary>话题标识</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string TopicId { get; set; } = string.Empty;

    /// <summary>话题名称</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>描述</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>封面图</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? CoverUrl { get; set; }

    /// <summary>是否官方:0-用户创建 1-官方创建</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int IsOfficial { get; set; }

    /// <summary>状态:active/closed</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'active'")]
    public string Status { get; set; } = "active";

    /// <summary>帖子数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int PostCount { get; set; }

    /// <summary>参与人数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int ParticipantCount { get; set; }
}

/// <summary>
/// 专题表
/// </summary>
[SugarTable("special_topics")]
public class SpecialTopic : UuidEntity
{
    /// <summary>专题名称</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>描述</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>封面图URL</summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string CoverUrl { get; set; } = string.Empty;

    /// <summary>关联模板ID数组(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? TemplateIds { get; set; }

    /// <summary>开始时间</summary>
    [SugarColumn(IsNullable = false)]
    public DateTime StartTime { get; set; }

    /// <summary>结束时间</summary>
    [SugarColumn(IsNullable = false)]
    public DateTime EndTime { get; set; }

    /// <summary>状态:0-下架 1-上架</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public int Status { get; set; }
}

/// <summary>
/// 推送记录表
/// </summary>
[SugarTable("push_records")]
public class PushRecord : UuidEntity
{
    /// <summary>推送ID</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string PushId { get; set; } = string.Empty;

    /// <summary>标题</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Title { get; set; } = string.Empty;

    /// <summary>内容</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Content { get; set; } = string.Empty;

    /// <summary>推送类型:system/activity</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string PushType { get; set; } = string.Empty;

    /// <summary>目标类型:all/tag/user</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string TargetType { get; set; } = string.Empty;

    /// <summary>目标参数</summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? TargetParam { get; set; }

    /// <summary>定时发送时间</summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? ScheduleTime { get; set; }

    /// <summary>实际发送时间</summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? SendTime { get; set; }

    /// <summary>推送渠道(JSON数组): ["app","sms","email"]</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Channels { get; set; }

    /// <summary>推送总数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int TotalCount { get; set; }

    /// <summary>成功数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int SuccessCount { get; set; }

    /// <summary>失败数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int FailCount { get; set; }

    /// <summary>点击数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int ClickCount { get; set; }

    /// <summary>状态:draft/pending/sending/sent/failed/canceled</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'pending'")]
    public string Status { get; set; } = "pending";
}
