using Pindou.Application.Common;

namespace Pindou.Application.DTOs.Template;

public class TemplateQuery : PageRequest
{
    public string? CategoryId { get; set; }
    public string? Difficulty { get; set; }
    public string? BoardSize { get; set; }
    public bool? IsFeatured { get; set; }
    public string? Keyword { get; set; }
    public List<string>? Tags { get; set; }
}

public class TemplateDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public List<string>? Tags { get; set; }
    public string CoverUrl { get; set; } = string.Empty;
    public string BoardSize { get; set; } = string.Empty;
    public int BeadCount { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public int TotalColors { get; set; }
    public string SourceType { get; set; } = "official";
    public string? CreatorName { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int UseCount { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsFavorited { get; set; }
}

public class TemplateDetailDto : TemplateDto
{
    public List<string> PreviewUrls { get; set; } = new();
    public string? CreatorId { get; set; }
    public DateTime CreateTime { get; set; }
}

public class TemplateCategoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public int TemplateCount { get; set; }
}

public class TemplateTagDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Type { get; set; }
    public int UseCount { get; set; }
}
