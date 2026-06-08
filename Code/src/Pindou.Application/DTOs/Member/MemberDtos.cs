using Pindou.Application.Common;

namespace Pindou.Application.DTOs.Member;

public class MemberProductDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int DailyGenerations { get; set; }
    public List<string>? Features { get; set; }
}

public class CreateOrderRequest
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductType { get; set; } = "member";
    public string? PayMethod { get; set; }
}

public class OrderDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderNo { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "pending";
    public string? PayMethod { get; set; }
    public DateTime? PayTime { get; set; }
    public DateTime CreateTime { get; set; }
}

public class OpenMemberRequest
{
    public string UserId { get; set; } = string.Empty;
    public string MemberType { get; set; } = "month";
    public int DurationDays { get; set; } = 30;
    public string? Reason { get; set; }
}

public class MemberStatusDto
{
    public bool IsMember { get; set; }
    public DateTime? ExpireTime { get; set; }
    public int RemainingDays { get; set; }
    public int DailyGenerations { get; set; }
    public int UsedToday { get; set; }
    public int RemainingToday { get; set; }
}

public class MemberRecordDto
{
    public string Id { get; set; } = string.Empty;
    public string MemberType { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime ExpireTime { get; set; }
    public DateTime CreateTime { get; set; }
}

public class OrderQuery : PageRequest
{
    public string? ProductType { get; set; }
    public string? PayMethod { get; set; }
}
