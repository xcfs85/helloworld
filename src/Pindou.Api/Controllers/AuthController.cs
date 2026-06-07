using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Auth;
using Pindou.Application.Interfaces.Auth;
using Pindou.Application.Interfaces.User;
using Pindou.Shared.Attributes;

namespace Pindou.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    /// <summary>发送短信验证码</summary>
    [HttpPost("sms/send")]
    [AllowAnonymous]
    public async Task<ApiResponse<SmsSendResponse>> SendSms([FromBody] SmsSendRequest request)
    {
        var data = await _authService.SendSmsAsync(request);
        return ApiResponse<SmsSendResponse>.Ok(data);
    }

    /// <summary>手机号验证码登录</summary>
    [HttpPost("phone/login")]
    [AllowAnonymous]
    public async Task<ApiResponse<LoginResponse>> PhoneLogin([FromBody] PhoneLoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var data = await _authService.PhoneLoginAsync(request, ip);
        return ApiResponse<LoginResponse>.Ok(data);
    }

    /// <summary>微信登录</summary>
    [HttpPost("wechat/login")]
    [AllowAnonymous]
    public async Task<ApiResponse<LoginResponse>> WechatLogin([FromBody] WechatLoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var data = await _authService.WechatLoginAsync(request, ip);
        return ApiResponse<LoginResponse>.Ok(data);
    }

    /// <summary>Apple登录</summary>
    [HttpPost("apple/login")]
    [AllowAnonymous]
    public async Task<ApiResponse<LoginResponse>> AppleLogin([FromBody] AppleLoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var data = await _authService.AppleLoginAsync(request, ip);
        return ApiResponse<LoginResponse>.Ok(data);
    }

    /// <summary>游客登录</summary>
    [HttpPost("guest/login")]
    [AllowAnonymous]
    public async Task<ApiResponse<LoginResponse>> GuestLogin([FromBody] GuestLoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var data = await _authService.GuestLoginAsync(request, ip);
        return ApiResponse<LoginResponse>.Ok(data);
    }

    /// <summary>刷新Token</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ApiResponse<LoginResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var data = await _authService.RefreshTokenAsync(request.RefreshToken);
        return ApiResponse<LoginResponse>.Ok(data);
    }

    /// <summary>登出</summary>
    [HttpPost("logout")]
    public async Task<ApiResponse> Logout()
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _authService.LogoutAsync(userId);
        return ApiResponse.Ok();
    }

    /// <summary>获取当前用户信息</summary>
    [HttpGet("current")]
    public async Task<ApiResponse<UserInfo>> Current()
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _authService.GetCurrentUserAsync(userId);
        return ApiResponse<UserInfo>.Ok(data);
    }
}
