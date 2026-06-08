namespace Pindou.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<DTOs.Auth.SmsSendResponse> SendSmsAsync(DTOs.Auth.SmsSendRequest request);
    Task<DTOs.Auth.LoginResponse> PhoneLoginAsync(DTOs.Auth.PhoneLoginRequest request, string? ip = null);
    Task<DTOs.Auth.LoginResponse> WechatLoginAsync(DTOs.Auth.WechatLoginRequest request, string? ip = null);
    Task<DTOs.Auth.LoginResponse> AppleLoginAsync(DTOs.Auth.AppleLoginRequest request, string? ip = null);
    Task<DTOs.Auth.LoginResponse> GuestLoginAsync(DTOs.Auth.GuestLoginRequest request, string? ip = null);
    Task<DTOs.Auth.LoginResponse> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string userId, string? deviceId = null);
    Task<DTOs.Auth.UserInfo> GetCurrentUserAsync(string userId);
}
