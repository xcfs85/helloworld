using SqlSugar;
using Pindou.Domain.Common;

namespace Pindou.Domain.Entities.System;

/// <summary>
/// 系统配置表
/// </summary>
[SugarTable("system_configs")]
public class SystemConfig : UuidEntity
{
    /// <summary>配置键</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>配置值</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? ConfigValue { get; set; }

    /// <summary>类型:string/number/json/boolean</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? ConfigType { get; set; }

    /// <summary>描述</summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>状态:0-禁用 1-启用</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public int Status { get; set; }
}

/// <summary>
/// MARD色号表
/// </summary>
[SugarTable("mard_colors")]
public class MardColor : UuidEntity
{
    /// <summary>色号(如M01)</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string ColorNo { get; set; } = string.Empty;

    /// <summary>颜色名称</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string ColorName { get; set; } = string.Empty;

    /// <summary>RGB值</summary>
    [SugarColumn(Length = 10, IsNullable = false)]
    public string Rgb { get; set; } = string.Empty;

    /// <summary>LAB值</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Lab { get; set; }

    /// <summary>分类:red/orange/yellow/green/blue/purple/gray/black/white/special</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Category { get; set; }

    /// <summary>是否常用:0-否 1-是</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int IsCommon { get; set; }

    /// <summary>状态:0-禁用 1-启用</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public int Status { get; set; }
}

/// <summary>
/// 耗材套装表
/// </summary>
[SugarTable("bead_kits")]
public class BeadKit : UuidEntity
{
    /// <summary>套装ID</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string KitId { get; set; } = string.Empty;

    /// <summary>套装名称</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string KitName { get; set; } = string.Empty;

    /// <summary>品牌</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'MARD'")]
    public string Brand { get; set; } = "MARD";

    /// <summary>色号数量</summary>
    [SugarColumn(IsNullable = false)]
    public int ColorCount { get; set; }

    /// <summary>豆子总数</summary>
    [SugarColumn(IsNullable = false)]
    public int BeadCount { get; set; }

    /// <summary>价格</summary>
    [SugarColumn(Length = 10, IsNullable = false)]
    public decimal Price { get; set; }

    /// <summary>购买链接</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? PurchaseUrl { get; set; }

    /// <summary>状态:0-禁用 1-启用</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public int Status { get; set; }
}

/// <summary>
/// 敏感词表
/// </summary>
[SugarTable("sensitive_words")]
public class SensitiveWord : UuidEntity
{
    /// <summary>敏感词</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Word { get; set; } = string.Empty;

    /// <summary>级别:1-警告 2-替换 3-拦截</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "2")]
    public int Level { get; set; }

    /// <summary>类型:politics/porn/violence/ad/other</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Type { get; set; } = string.Empty;

    /// <summary>替换词</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ReplaceWord { get; set; }

    /// <summary>状态:0-禁用 1-启用</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public int Status { get; set; }
}

/// <summary>
/// 举报表
/// </summary>
[SugarTable("reports")]
public class Report : UuidEntity
{
    /// <summary>举报ID</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string ReportId { get; set; } = string.Empty;

    /// <summary>举报者ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string ReporterId { get; set; } = string.Empty;

    /// <summary>目标类型:post/comment/user</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string TargetType { get; set; } = string.Empty;

    /// <summary>目标ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string TargetId { get; set; } = string.Empty;

    /// <summary>被举报用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string TargetUserId { get; set; } = string.Empty;

    /// <summary>举报原因</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>详情内容</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Content { get; set; }

    /// <summary>图片数组(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Images { get; set; }

    /// <summary>状态:pending/ignored/warned/ban_content/ban_user</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'pending'")]
    public string Status { get; set; } = "pending";

    /// <summary>处理结果</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? HandleResult { get; set; }

    /// <summary>处理人ID</summary>
    [SugarColumn(Length = 36, IsNullable = true)]
    public string? HandlerId { get; set; }

    /// <summary>处理时间</summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? HandleTime { get; set; }
}

/// <summary>
/// 帖子审核记录表
/// </summary>
[SugarTable("post_review_logs")]
public class PostReviewLog : UuidEntity
{
    /// <summary>帖子ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string PostId { get; set; } = string.Empty;

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
