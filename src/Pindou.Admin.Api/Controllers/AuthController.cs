using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Admin;
using Pindou.Application.Interfaces.Admin;
using Pindou.Shared.Attributes;

namespace Pindou.Admin.Api.Controllers;

[ApiController]
[Route("api/admin/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAdminAuthService _authService;
    public AuthController(IAdminAuthService authService) { _authService = authService; }

    /// <summary>获取验证码key</summary>
    [HttpGet("captcha")]
    [AllowAnonymous]
    public async Task<ApiResponse<CaptchaResponse>> Captcha()
    {
        var key = await _authService.GenerateCaptchaAsync();
        return ApiResponse<CaptchaResponse>.Ok(new CaptchaResponse
        {
            CaptchaKey = key,
            CaptchaImage = $"captcha:{key}" // 实际由前端展示
        });
    }

    /// <summary>登录</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ApiResponse<AdminLoginResponse>> Login([FromBody] AdminLoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var data = await _authService.LoginAsync(request, ip);
        return ApiResponse<AdminLoginResponse>.Ok(data);
    }

    /// <summary>登出</summary>
    [HttpPost("logout")]
    public async Task<ApiResponse> Logout()
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        await _authService.LogoutAsync(adminId);
        return ApiResponse.Ok();
    }

    /// <summary>当前用户</summary>
    [HttpGet("current")]
    public async Task<ApiResponse<AdminUserInfo>> Current()
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        var data = await _authService.GetCurrentUserAsync(adminId);
        return ApiResponse<AdminUserInfo>.Ok(data);
    }
}
