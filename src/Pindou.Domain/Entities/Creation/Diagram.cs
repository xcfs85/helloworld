using SqlSugar;
using Pindou.Domain.Common;

namespace Pindou.Domain.Entities.Creation;

/// <summary>
/// 拼豆图纸表
/// </summary>
[SugarTable("diagrams")]
public class Diagram : UuidEntity
{
    /// <summary>用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>图纸名称</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>状态:draft/completed/beaded</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'draft'")]
    public string Status { get; set; } = "draft";

    /// <summary>源图片URL</summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string SourceImageUrl { get; set; } = string.Empty;

    /// <summary>预览图URL</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? PreviewUrl { get; set; }

    /// <summary>无格线预览</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? PreviewNoGridUrl { get; set; }

    /// <summary>底板规格</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string BoardSize { get; set; } = string.Empty;

    /// <summary>总颗数</summary>
    [SugarColumn(IsNullable = false)]
    public int BeadCount { get; set; }

    /// <summary>难度</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Difficulty { get; set; } = string.Empty;

    /// <summary>风格</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Style { get; set; } = string.Empty;

    /// <summary>色号数量</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int TotalColors { get; set; }

    /// <summary>豆子总数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int TotalBeads { get; set; }

    /// <summary>标签数组(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Tags { get; set; }

    /// <summary>版本号</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public int Version { get; set; } = 1;

    /// <summary>来源:create/template</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'create'")]
    public string SourceType { get; set; } = "create";

    /// <summary>来源模板ID</summary>
    [SugarColumn(Length = 36, IsNullable = true)]
    public string? TemplateId { get; set; }
}

/// <summary>
/// 色号信息表
/// </summary>
[SugarTable("color_infos")]
public class ColorInfo : UuidEntity
{
    /// <summary>图纸ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string DiagramId { get; set; } = string.Empty;

    /// <summary>颜色序号</summary>
    [SugarColumn(IsNullable = false)]
    public int ColorIndex { get; set; }

    /// <summary>色号(如M01)</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string ColorCode { get; set; } = string.Empty;

    /// <summary>颜色名称</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string ColorName { get; set; } = string.Empty;

    /// <summary>RGB值</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Rgb { get; set; } = string.Empty;

    /// <summary>颗数</summary>
    [SugarColumn(IsNullable = false)]
    public int BeadCount { get; set; }

    /// <summary>占比</summary>
    [SugarColumn(Length = 10, IsNullable = false)]
    public decimal Percentage { get; set; }

    /// <summary>位置范围</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Position { get; set; }
}

/// <summary>
/// 生成任务表
/// </summary>
[SugarTable("diagram_tasks")]
public class DiagramTask : UuidEntity
{
    /// <summary>用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>关联图纸ID</summary>
    [SugarColumn(Length = 36, IsNullable = true)]
    public string? DiagramId { get; set; }

    /// <summary>状态:pending/processing/completed/failed</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'pending'")]
    public string Status { get; set; } = "pending";

    /// <summary>进度0-100</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int Progress { get; set; }

    /// <summary>当前阶段</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? CurrentStage { get; set; }

    /// <summary>源图片</summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string SourceImageUrl { get; set; } = string.Empty;

    /// <summary>生成参数(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Params { get; set; } = "{}";

    /// <summary>错误信息</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? ErrorMessage { get; set; }

    /// <summary>是否同步执行</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public bool IsSync { get; set; }

    /// <summary>完成时间</summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? CompleteTime { get; set; }
}
