using Pindou.Application.Common;

namespace Pindou.Application.DTOs.Operation;

/**
 *@Author: 西成峰
 *@CreateTime: 2026-06-19
 *@Description: 运营管理模块 DTO
 */

#region Banner

/// <summary>
/// Banner DTO（APP端查询）
/// </summary>
public class BannerDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string LinkType { get; set; } = string.Empty;
    public string LinkValue { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int Sort { get; set; }
    public string Status { get; set; } = "active";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

/// <summary>
/// Banner 管理端列表 DTO
/// </summary>
public class BannerAdminDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string LinkType { get; set; } = string.Empty;
    public string? LinkValue { get; set; }
    public string Position { get; set; } = string.Empty;
    public int Sort { get; set; }
    public string Status { get; set; } = "active";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
}

/// <summary>
/// Banner 分页查询参数
/// </summary>
public class BannerListQuery : PageRequest
{
    /// <summary>位置筛选: home_top/template_top</summary>
    public string? Position { get; set; }

    /// <summary>状态筛选: active/inactive</summary>
    public string? Status { get; set; }
}

/// <summary>
/// 创建/更新 Banner 请求
/// </summary>
public class CreateBannerRequest
{
    /// <summary>标题</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>图片URL</summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>跳转类型: none/template/special/web/activity</summary>
    public string LinkType { get; set; } = "none";

    /// <summary>跳转值</summary>
    public string? LinkValue { get; set; }

    /// <summary>位置: home/template</summary>
    public string Position { get; set; } = "home";

    /// <summary>排序</summary>
    public int Sort { get; set; }

    /// <summary>开始时间</summary>
    public DateTime StartTime { get; set; }

    /// <summary>结束时间</summary>
    public DateTime EndTime { get; set; }
}

#endregion

#region 专题

/// <summary>
/// 专题列表/详情 DTO
/// </summary>
public class SpecialTopicDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CoverUrl { get; set; } = string.Empty;
    public List<string> TemplateIds { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    /// <summary>状态: 0-下架 1-上架</summary>
    public int Status { get; set; }
    public DateTime CreateTime { get; set; }
}

/// <summary>
/// 专题分页查询参数
/// </summary>
public class SpecialTopicListQuery : PageRequest
{
    /// <summary>状态筛选: 0-下架 1-上架</summary>
    public int? Status { get; set; }

    /// <summary>关键字搜索(专题名称)</summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// 创建专题请求
/// </summary>
public class CreateSpecialTopicRequest
{
    /// <summary>专题名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>描述</summary>
    public string? Description { get; set; }

    /// <summary>封面图URL</summary>
    public string CoverUrl { get; set; } = string.Empty;

    /// <summary>关联模板ID列表</summary>
    public List<string> TemplateIds { get; set; } = new();

    /// <summary>开始时间</summary>
    public DateTime StartTime { get; set; }

    /// <summary>结束时间</summary>
    public DateTime EndTime { get; set; }
}

/// <summary>
/// 更新专题请求
/// </summary>
public class UpdateSpecialTopicRequest
{
    /// <summary>专题名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>描述</summary>
    public string? Description { get; set; }

    /// <summary>封面图URL</summary>
    public string CoverUrl { get; set; } = string.Empty;

    /// <summary>关联模板ID列表</summary>
    public List<string> TemplateIds { get; set; } = new();

    /// <summary>开始时间</summary>
    public DateTime StartTime { get; set; }

    /// <summary>结束时间</summary>
    public DateTime EndTime { get; set; }

    /// <summary>状态: 0-下架 1-上架（仅状态切换时使用）</summary>
    public int? Status { get; set; }
}

#endregion

#region 话题

/// <summary>
/// 管理端话题列表/详情 DTO
/// </summary>
public class TopicAdminDto
{
    public string Id { get; set; } = string.Empty;
    public string TopicId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public int IsOfficial { get; set; }
    public string Status { get; set; } = "active";
    public int PostCount { get; set; }
    public int ParticipantCount { get; set; }
    public DateTime CreateTime { get; set; }
}

/// <summary>
/// 话题分页查询参数
/// </summary>
public class TopicListQuery : PageRequest
{
    /// <summary>状态筛选: active/closed</summary>
    public string? Status { get; set; }

    /// <summary>是否官方: 0-用户 1-官方</summary>
    public int? IsOfficial { get; set; }

    /// <summary>关键字搜索(话题名称)</summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// 创建话题请求
/// </summary>
public class CreateTopicAdminRequest
{
    /// <summary>话题名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>描述</summary>
    public string? Description { get; set; }

    /// <summary>封面图URL</summary>
    public string? CoverUrl { get; set; }

    /// <summary>是否官方: 0-用户 1-官方</summary>
    public int IsOfficial { get; set; }
}

/// <summary>
/// 更新话题请求
/// </summary>
public class UpdateTopicAdminRequest
{
    /// <summary>话题名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>描述</summary>
    public string? Description { get; set; }

    /// <summary>封面图URL</summary>
    public string? CoverUrl { get; set; }

    /// <summary>是否官方: 0-用户 1-官方</summary>
    public int IsOfficial { get; set; }
}

#endregion

#region 推送

/// <summary>
/// 推送记录管理端列表 DTO
/// </summary>
public class PushRecordDto
{
    public string Id { get; set; } = string.Empty;
    public string PushId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string PushType { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string? TargetParam { get; set; }
    public List<string> Channels { get; set; } = new();
    public DateTime? ScheduleTime { get; set; }
    public DateTime? SendTime { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public int ClickCount { get; set; }
    public string Status { get; set; } = "draft";
    public DateTime CreateTime { get; set; }
}

/// <summary>
/// 推送记录分页查询参数
/// </summary>
public class PushListQuery : PageRequest
{
    /// <summary>状态筛选</summary>
    public string? Status { get; set; }

    /// <summary>推送类型</summary>
    public string? PushType { get; set; }

    /// <summary>目标类型</summary>
    public string? TargetType { get; set; }

    /// <summary>关键字搜索(标题)</summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// 发送推送请求
/// </summary>
public class SendPushRequest
{
    /// <summary>标题</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>内容</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>推送类型: system/activity/marketing</summary>
    public string PushType { get; set; } = "system";

    /// <summary>目标类型: all/tag/user</summary>
    public string TargetType { get; set; } = "all";

    /// <summary>目标参数(标签或用户ID列表,JSON)</summary>
    public string? TargetParam { get; set; }

    /// <summary>目标用户ID列表(当TargetType=user时)</summary>
    public List<string>? TargetIds { get; set; }

    /// <summary>推送渠道: app/sms/email</summary>
    public List<string> Channels { get; set; } = new() { "app" };
}

/// <summary>
/// 定时推送请求
/// </summary>
public class SchedulePushRequest : SendPushRequest
{
    /// <summary>定时发送时间</summary>
    public DateTime ScheduleTime { get; set; }
}

#endregion
