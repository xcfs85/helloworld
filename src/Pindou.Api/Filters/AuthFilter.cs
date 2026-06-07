using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pindou.Application.Common;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Pindou.Shared.Attributes;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Pindou.Api.Filters;

/// <summary>
/// 用户认证过滤器
/// </summary>
public class AuthFilter : IAsyncActionFilter
{
    private readonly ICacheService _cache;
    private readonly JwtOptions _jwt;

    public AuthFilter(ICacheService cache, IOptions<JwtOptions> jwt)
    {
        _cache = cache;
        _jwt = jwt.Value;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
        {
            await next();
            return;
        }

        // 从Header或Query获取Token
        var token = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token["Bearer ".Length..].Trim();
        }
        else
        {
            token = context.HttpContext.Request.Query["token"].ToString();
        }

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new ObjectResult(ApiResponse.Fail("未授权", ErrorCodes.TokenInvalid))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
            tokenHandler.ValidateToken(token, new TokenValidationParameters
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

            var jwtToken = (JwtSecurityToken)validatedToken;
            var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
            if (tokenType != "access")
            {
                context.Result = new ObjectResult(ApiResponse.Fail("非访问令牌", ErrorCodes.TokenInvalid))
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                context.HttpContext.Items["UserId"] = userId;
            }
        }
        catch (SecurityTokenExpiredException)
        {
            context.Result = new ObjectResult(ApiResponse.Fail("Token已过期", ErrorCodes.TokenExpired))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }
        catch
        {
            context.Result = new ObjectResult(ApiResponse.Fail("Token无效", ErrorCodes.TokenInvalid))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        await next();
    }
}
