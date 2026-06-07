namespace Pindou.Application.DTOs.Auth;

/// <summary>
/// 发送短信验证码请求
/// </summary>
public class SmsSendRequest
{
    public string Phone { get; set; } = string.Empty;
    /// <summary>场景: login/register/reset</summary>
    public string Scene { get; set; } = "login";
}

public class SmsSendResponse
{
    public int ExpireSeconds { get; set; }
    public int RemainingTimes { get; set; }
}

/// <summary>
/// 手机号验证码登录请求
/// </summary>
public class PhoneLoginRequest
{
    public string Phone { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? DeviceId { get; set; }
}

public class WechatLoginRequest
{
    public string Code { get; set; } = string.Empty;
    public string? DeviceId { get; set; }
}

public class AppleLoginRequest
{
    public string IdentityToken { get; set; } = string.Empty;
    public string? DeviceId { get; set; }
}

public class GuestLoginRequest
{
    public string? DeviceId { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpireTime { get; set; }
    public UserInfo User { get; set; } = new();
}

public class UserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Gender { get; set; } = "unknown";
    public string? Phone { get; set; }
    public bool IsMember { get; set; }
    public DateTime? MemberExpireTime { get; set; }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class LogoutRequest
{
    public string? DeviceId { get; set; }
}
