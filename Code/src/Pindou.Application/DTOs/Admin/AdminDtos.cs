using Pindou.Application.Common;

namespace Pindou.Application.DTOs.Admin;

public class AdminLoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Captcha { get; set; }
    public string? CaptchaKey { get; set; }
}

public class AdminLoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpireTime { get; set; }
    public AdminUserInfo User { get; set; } = new();
}

public class AdminUserInfo
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public long RoleId { get; set; }
    public string? RoleName { get; set; }
    public List<string> Permissions { get; set; } = new();
    public DateTime? LastLoginTime { get; set; }
    public string? LastLoginIp { get; set; }
}

public class CaptchaResponse
{
    public string CaptchaKey { get; set; } = string.Empty;
    public string CaptchaImage { get; set; } = string.Empty;
}

public class AdminUserQuery : PageRequest
{
    public long? RoleId { get; set; }
    public int? Status { get; set; }
}

public class AdminUserListDto
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public long RoleId { get; set; }
    public string? RoleName { get; set; }
    public int Status { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public string? LastLoginIp { get; set; }
    public DateTime CreateTime { get; set; }
}

public class AdminUserDetailDto : AdminUserListDto
{
    public DateTime? UpdateTime { get; set; }
}

public class CreateAdminUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public long RoleId { get; set; }
    public int Status { get; set; } = 1;
}

public class UpdateAdminUserRequest
{
    public string? Nickname { get; set; }
    public long? RoleId { get; set; }
    public int? Status { get; set; }
}

public class RoleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Permissions { get; set; } = new();
    public DateTime CreateTime { get; set; }
}

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class OperationLogDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Method { get; set; }
    public string? Params { get; set; }
    public string? Ip { get; set; }
    public DateTime CreateTime { get; set; }
}

public class OperationLogQuery : PageRequest
{
    public long? UserId { get; set; }
    public string? Operation { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
