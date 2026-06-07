using Pindou.Application.Common;

namespace Pindou.Application.DTOs.User;

public class UserListQuery : QueryRequest
{
    public bool? IsMember { get; set; }
    public string? Platform { get; set; }
    public DateTime? RegisterStartTime { get; set; }
    public DateTime? RegisterEndTime { get; set; }
}

public class UserListDto
{
    public string Id { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Phone { get; set; }
    public string Gender { get; set; } = "unknown";
    public string? City { get; set; }
    public bool IsMember { get; set; }
    public DateTime? MemberExpireTime { get; set; }
    public string Status { get; set; } = "active";
    public DateTime CreateTime { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public int DiagramCount { get; set; }
    public int PostCount { get; set; }
}

public class UpdateUserRequest
{
    public string? Nickname { get; set; }
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Gender { get; set; }
}
