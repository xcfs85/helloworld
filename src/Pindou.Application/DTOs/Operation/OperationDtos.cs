using Pindou.Application.Common;

namespace Pindou.Application.DTOs.Operation;

public class BannerDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string LinkType { get; set; } = string.Empty;
    public string LinkValue { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int Sort { get; set; }
}

public class SpecialTopicDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CoverUrl { get; set; } = string.Empty;
    public List<string> TemplateIds { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class CreateSpecialTopicRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CoverUrl { get; set; } = string.Empty;
    public List<string> TemplateIds { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
