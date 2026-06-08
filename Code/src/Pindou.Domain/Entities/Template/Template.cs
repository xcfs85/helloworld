using SqlSugar;
using Pindou.Domain.Common;

namespace Pindou.Domain.Entities.Template;

/// <summary>
/// 模板表
/// </summary>
[SugarTable("templates")]
public class Template : UuidEntity
{
    /// <summary>模板名称</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>分类ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>标签数组(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Tags { get; set; }

    /// <summary>封面图URL</summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string CoverUrl { get; set; } = string.Empty;

    /// <summary>预览图数组(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string PreviewUrls { get; set; } = "[]";

    /// <summary>底板规格</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string BoardSize { get; set; } = string.Empty;

    /// <summary>总颗数</summary>
    [SugarColumn(IsNullable = false)]
    public int BeadCount { get; set; }

    /// <summary>难度</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Difficulty { get; set; } = string.Empty;

    /// <summary>色号数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int TotalColors { get; set; }

    /// <summary>来源:official/creator</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'official'")]
    public string SourceType { get; set; } = "official";

    /// <summary>创作者用户ID</summary>
    [SugarColumn(Length = 36, IsNullable = true)]
    public string? CreatorId { get; set; }

    /// <summary>创作者名称</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? CreatorName { get; set; }

    /// <summary>浏览数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int ViewCount { get; set; }

    /// <summary>点赞数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int LikeCount { get; set; }

    /// <summary>使用数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int UseCount { get; set; }

    /// <summary>状态:pending/active/deleted</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'pending'")]
    public string Status { get; set; } = "pending";

    /// <summary>审核状态</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'pending'")]
    public string ReviewStatus { get; set; } = "pending";

    /// <summary>拒绝原因</summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReviewReason { get; set; }

    /// <summary>是否精选</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public bool IsFeatured { get; set; }
}

/// <summary>
/// 模板分类表
/// </summary>
[SugarTable("template_categories")]
public class TemplateCategory : UuidEntity
{
    /// <summary>分类名称</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>图标</summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Icon { get; set; }

    /// <summary>排序</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int Sort { get; set; }
}

/// <summary>
/// 模板收藏表
/// </summary>
[SugarTable("template_favorites")]
public class TemplateFavorite : UuidEntity
{
    /// <summary>用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>模板ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string TemplateId { get; set; } = string.Empty;
}

/// <summary>
/// 模板标签表
/// </summary>
[SugarTable("template_tags")]
public class TemplateTag : UuidEntity
{
    /// <summary>标签名称</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>所属分类</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Category { get; set; }

    /// <summary>标签类型:theme/style/difficulty</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Type { get; set; }

    /// <summary>使用次数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int UseCount { get; set; }

    /// <summary>状态:0-禁用 1-启用</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public int Status { get; set; }
}

/// <summary>
/// 模板审核记录表
/// </summary>
[SugarTable("template_review_logs")]
public class TemplateReviewLog : UuidEntity
{
    /// <summary>模板ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>审核人ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string ReviewerId { get; set; } = string.Empty;

    /// <summary>操作:approve/reject</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Action { get; set; } = string.Empty;

    /// <summary>拒绝原因</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Reason { get; set; }

    /// <summary>备注</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}
