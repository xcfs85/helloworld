namespace Pindou.Infrastructure.ExternalServices.AI;

/// <summary>
/// AI生图任务参数
/// </summary>
public class AiGenerationRequest
{
    public string UserId { get; set; } = string.Empty;
    public string TaskId { get; set; } = string.Empty;
    public string SourceImageUrl { get; set; } = string.Empty;
    public string BoardSize { get; set; } = "29x29";
    public string Difficulty { get; set; } = "easy";
    public string Style { get; set; } = "pixel";
    public Dictionary<string, object>? Options { get; set; }
}

/// <summary>
/// AI生图结果
/// </summary>
public class AiGenerationResult
{
    public bool Success { get; set; }
    public string? DiagramId { get; set; }
    public string? PreviewUrl { get; set; }
    public string? PreviewNoGridUrl { get; set; }
    public int BeadCount { get; set; }
    public int ColorCount { get; set; }
    public List<ColorMapping>? ColorInfos { get; set; }
    public string? ErrorMessage { get; set; }
    public int Duration { get; set; }
}

public class ColorMapping
{
    public int ColorIndex { get; set; }
    public string ColorCode { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string Rgb { get; set; } = string.Empty;
    public int BeadCount { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// AI生图服务接口
/// </summary>
public interface IAiGenerationService
{
    Task<string> SubmitAsync(AiGenerationRequest request);
    Task<AiGenerationResult> GenerateSyncAsync(AiGenerationRequest request);
    Task<(int Progress, string? Stage)> GetProgressAsync(string taskId);
    Task CancelAsync(string taskId);
}
