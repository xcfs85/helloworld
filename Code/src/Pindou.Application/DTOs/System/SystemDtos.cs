using Pindou.Application.Common;

namespace Pindou.Application.DTOs.System;

public class SystemConfigDto
{
    public string Id { get; set; } = string.Empty;
    public string ConfigKey { get; set; } = string.Empty;
    public string? ConfigValue { get; set; }
    public string? ConfigType { get; set; }
    public string? Description { get; set; }
    public int Status { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
}

public class MardColorDto
{
    public string Id { get; set; } = string.Empty;
    public string ColorNo { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string Rgb { get; set; } = string.Empty;
    public string? Lab { get; set; }
    public string? Category { get; set; }
    public bool IsCommon { get; set; }
}

public class BeadKitDto
{
    public string Id { get; set; } = string.Empty;
    public string KitId { get; set; } = string.Empty;
    public string KitName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int ColorCount { get; set; }
    public int BeadCount { get; set; }
    public decimal Price { get; set; }
    public string? PurchaseUrl { get; set; }
}

public class SensitiveWordDto
{
    public string Id { get; set; } = string.Empty;
    public string Word { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? ReplaceWord { get; set; }
    public int Status { get; set; }
}

public class AddSensitiveWordRequest
{
    public string Word { get; set; } = string.Empty;
    public int Level { get; set; } = 2;
    public string Type { get; set; } = "other";
    public string? ReplaceWord { get; set; }
    public int Status { get; set; } = 1;
}

public class ReportDto
{
    public string Id { get; set; } = string.Empty;
    public string ReportId { get; set; } = string.Empty;
    public string ReporterId { get; set; } = string.Empty;
    public string ReporterName { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
    public string TargetUserName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? Content { get; set; }
    public List<string>? Images { get; set; }
    public string Status { get; set; } = "pending";
    public string? HandleResult { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime? HandleTime { get; set; }
}

public class ReportQuery : PageRequest
{
    public string? TargetType { get; set; }
    public string? Status { get; set; }
    public string? Reason { get; set; }
}
