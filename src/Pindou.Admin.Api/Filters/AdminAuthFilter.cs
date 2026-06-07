using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pindou.Application.Common;
using Pindou.Application.Interfaces.Admin;
using Pindou.Application.Interfaces.User;
using Pindou.Infrastructure.Options;
using Pindou.Shared.Attributes;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace Pindou.Admin.Api.Filters;

/// <summary>
/// 后台管理员认证
/// </summary>
public class AdminAuthFilter : IAsyncActionFilter
{
    private readonly JwtOptions _jwt;
    private readonly IRoleService _roleService;

    public AdminAuthFilter(IOptions<JwtOptions> jwt, IRoleService roleService)
    {
        _jwt = jwt.Value;
        _roleService = roleService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
        {
            await next();
            return;
        }

        var token = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            token = token["Bearer ".Length..].Trim();
        if (string.IsNullOrEmpty(token))
        {
            context.Result = Unauthorized("未授权", ErrorCodes.TokenInvalid);
            return;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwt.Issuer,
                ValidAudience = _jwt.Audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromSeconds(30)
            }, out SecurityToken validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;
            var tokenType = jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
            if (tokenType != "admin")
            {
                context.Result = Unauthorized("非管理员令牌", ErrorCodes.TokenInvalid);
                return;
            }

            var adminId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var roleId = jwt.Claims.FirstOrDefault(c => c.Type == "role_id")?.Value;
            if (!string.IsNullOrEmpty(adminId)) context.HttpContext.Items["AdminId"] = adminId;
            if (!string.IsNullOrEmpty(roleId)) context.HttpContext.Items["RoleId"] = roleId;

            // 权限验证
            var permission = endpoint?.Metadata.GetMetadata<PermissionAttribute>();
            if (permission != null)
            {
                if (roleId == "1") // 超级管理员
                {
                    // 全部通过
                }
                else
                {
                    var permissions = await _roleService.GetPermissionsAsync(long.Parse(roleId ?? "0"));
                    if (permissions != null && permissions.Contains(permission.Code))
                    {
                        // 通过
                    }
                    else if (permissions == null || !permissions.Contains("*"))
                    {
                        context.Result = new ObjectResult(ApiResponse<object>.Fail("无操作权限", ErrorCodes.NoPermission))
                        {
                            StatusCode = StatusCodes.Status403Forbidden
                        };
                        return;
                    }
                }
            }
        }
        catch (SecurityTokenExpiredException)
        {
            context.Result = Unauthorized("Token已过期", ErrorCodes.TokenExpired);
            return;
        }
        catch
        {
            context.Result = Unauthorized("Token无效", ErrorCodes.TokenInvalid);
            return;
        }

        await next();
    }

    private IActionResult Unauthorized(string msg, int code) =>
        new ObjectResult(ApiResponse<object>.Fail(msg, code))
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
}
