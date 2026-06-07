using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Auth;
using Pindou.Application.Interfaces.Auth;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Options;
using Pindou.Infrastructure.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Pindou.Infrastructure.Services.Auth;

/// <summary>
/// 认证服务实现
/// </summary>
public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Token> _tokenRepo;
    private readonly ICacheService _cache;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthService> _logger;
    private readonly ISmsService _smsService;

    public AuthService(
        IRepository<User> userRepo,
        IRepository<Token> tokenRepo,
        ICacheService cache,
        IOptions<JwtOptions> jwtOptions,
        ILogger<AuthService> logger,
        ISmsService smsService)
    {
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;
        _cache = cache;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
        _smsService = smsService;
    }

    public async Task<SmsSendResponse> SendSmsAsync(SmsSendRequest request)
    {
        // 1. 频率限制 1分钟
        var rateKey = $"sms:rate:{request.Phone}";
        if (await _cache.ExistsAsync(rateKey))
        {
            throw new BizException("发送过于频繁，请稍后重试", 3001);
        }
        // 2. 每日上限
        var dailyKey = $"sms:daily:{request.Phone}:{DateTime.Now:yyyyMMdd}";
        var dailyCount = await _cache.GetAsync<long>(dailyKey);
        if (dailyCount >= 10)
        {
            throw new BizException("今日发送次数已达上限", 3002);
        }

        // 3. 生成6位验证码
        var code = Random.Shared.Next(100000, 999999).ToString();
        await _cache.SetAsync($"sms:code:{request.Phone}:{request.Scene}", code, TimeSpan.FromMinutes(5));
        await _cache.SetAsync(rateKey, 1, TimeSpan.FromMinutes(1));
        await _cache.IncrementAsync(dailyKey, 1, TimeSpan.FromDays(1));

        // 4. 调用短信服务
        await _smsService.SendCodeAsync(request.Phone, code, request.Scene);

        return new SmsSendResponse
        {
            ExpireSeconds = 300,
            RemainingTimes = 10 - (int)dailyCount - 1
        };
    }

    public async Task<LoginResponse> PhoneLoginAsync(PhoneLoginRequest request, string? ip = null)
    {
        if (string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.Code))
            throw new BizException("手机号或验证码不能为空", 1002);

        // 1. 验证验证码
        var codeKey = $"sms:code:{request.Phone}:login";
        var cachedCode = await _cache.GetStringAsync(codeKey);
        if (string.IsNullOrEmpty(cachedCode))
            throw new BizException("验证码已过期", 2013);
        if (cachedCode != request.Code)
            throw new BizException("验证码错误", 2013);
        await _cache.RemoveAsync(codeKey);

        // 2. 查询用户
        var user = await _userRepo.FirstOrDefaultAsync(u => u.Phone == request.Phone);
        if (user == null)
        {
            // 自动注册
            user = new User
            {
                Phone = request.Phone,
                Nickname = $"拼豆用户{request.Phone[^4..]}",
                Status = "active"
            };
            await _userRepo.InsertAsync(user);
        }
        if (user.Status == "disabled")
            throw new BizException("账号已禁用", 2011);

        // 3. 颁发Token
        user.LastLoginTime = DateTime.Now;
        user.LastLoginIp = ip;
        await _userRepo.UpdateAsync(user);

        return await BuildLoginResponseAsync(user, request.DeviceId);
    }

    public Task<LoginResponse> WechatLoginAsync(WechatLoginRequest request, string? ip = null)
    {
        // 实现微信授权登录
        throw new NotImplementedException("微信登录待实现");
    }

    public Task<LoginResponse> AppleLoginAsync(AppleLoginRequest request, string? ip = null)
    {
        // 实现Apple ID登录
        throw new NotImplementedException("Apple登录待实现");
    }

    public async Task<LoginResponse> GuestLoginAsync(GuestLoginRequest request, string? ip = null)
    {
        var user = new User
        {
            Nickname = $"游客{DateTime.Now:HHmmss}",
            Status = "active"
        };
        await _userRepo.InsertAsync(user);
        return await BuildLoginResponseAsync(user, request.DeviceId);
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        // 解析refresh_token
        var principal = ValidateToken(refreshToken);
        if (principal == null)
            throw new BizException("refresh_token无效", 2001);

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId))
            throw new BizException("refresh_token无效", 2001);

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null || user.Status == "disabled")
            throw new BizException("用户不存在或已禁用", 4001);

        return await BuildLoginResponseAsync(user, null);
    }

    public async Task LogoutAsync(string userId, string? deviceId = null)
    {
        // 清除Token
        var tokens = await _tokenRepo.GetListAsync(t => t.UserId == userId);
        foreach (var t in tokens)
        {
            await _tokenRepo.DeleteAsync(t.Id);
        }
    }

    public async Task<UserInfo> GetCurrentUserAsync(string userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", 4001);
        return new UserInfo
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Avatar = user.Avatar,
            Gender = user.Gender,
            IsMember = user.IsMember,
            MemberExpireTime = user.MemberExpireTime
        };
    }

    #region 私有方法
    private async Task<LoginResponse> BuildLoginResponseAsync(User user, string? deviceId)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user);
        var expireTime = DateTime.Now.AddMinutes(_jwtOptions.AccessTokenExpireMinutes);

        // 存储Token
        await _tokenRepo.InsertAsync(new Token
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            DeviceId = deviceId ?? string.Empty,
            ExpiresAt = expireTime
        });

        return new LoginResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpireTime = expireTime,
            User = new UserInfo
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Avatar = user.Avatar,
                Gender = user.Gender,
                IsMember = user.IsMember,
                MemberExpireTime = user.MemberExpireTime
            }
        };
    }

    private string GenerateAccessToken(User user)
    {
        return GenerateToken(user.Id, _jwtOptions.AccessTokenExpireMinutes, "access");
    }

    private string GenerateRefreshToken(User user)
    {
        return GenerateToken(user.Id, _jwtOptions.RefreshTokenExpireMinutes, "refresh");
    }

    private string GenerateToken(string userId, int expireMinutes, string tokenType)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("token_type", tokenType)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireMinutes),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromSeconds(30)
            }, out SecurityToken validatedToken);
            return new ClaimsPrincipal((JwtSecurityToken)validatedToken);
        }
        catch
        {
            return null;
        }
    }
    #endregion
}
