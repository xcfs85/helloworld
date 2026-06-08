using Pindou.Application.Common;

namespace Pindou.Application.DTOs.Creation;

public class CreateDiagramRequest
{
    public string SourceImageUrl { get; set; } = string.Empty;
    /// <summary>底板规格 如29x29</summary>
    public string BoardSize { get; set; } = "29x29";
    /// <summary>难度 easy/medium/hard/expert</summary>
    public string Difficulty { get; set; } = "easy";
    /// <summary>风格 pixel/cartoon/realistic/chibi</summary>
    public string Style { get; set; } = "pixel";
    public List<string>? Tags { get; set; }
    public string? TemplateId { get; set; }
    /// <summary>是否同步(≤5000颗)</summary>
    public bool IsSync { get; set; }
    /// <summary>图片处理参数</summary>
    public ImageProcessOptions? Options { get; set; }
}

public class ImageProcessOptions
{
    public int? Brightness { get; set; }
    public int? Contrast { get; set; }
    public int? Saturation { get; set; }
    public bool? Denoise { get; set; }
}

public class GenerationStatusResponse
{
    public string TaskId { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public int Progress { get; set; }
    public string? CurrentStage { get; set; }
    public string? DiagramId { get; set; }
    public string? ErrorMessage { get; set; }
}

public class DiagramDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public string? PreviewUrl { get; set; }
    public string? PreviewNoGridUrl { get; set; }
    public string BoardSize { get; set; } = string.Empty;
    public int BeadCount { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public int TotalColors { get; set; }
    public List<string>? Tags { get; set; }
    public string SourceType { get; set; } = "create";
    public DateTime CreateTime { get; set; }
}

public class DiagramDetailDto : DiagramDto
{
    public string SourceImageUrl { get; set; } = string.Empty;
    public int TotalBeads { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<ColorInfoDto> ColorInfos { get; set; } = new();
}

public class ColorInfoDto
{
    public int ColorIndex { get; set; }
    public string ColorCode { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string Rgb { get; set; } = string.Empty;
    public int BeadCount { get; set; }
    public decimal Percentage { get; set; }
}

public class ExportDiagramRequest
{
    public string DiagramId { get; set; } = string.Empty;
    /// <summary>格式 png/excel/pdf</summary>
    public string Format { get; set; } = "png";
    public bool WithGrid { get; set; } = true;
    public bool WithColorTable { get; set; } = true;
    /// <summary>推荐耗材套装ID</summary>
    public string? KitId { get; set; }
}
