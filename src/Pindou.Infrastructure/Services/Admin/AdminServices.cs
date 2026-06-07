using Microsoft.Extensions.Logging;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Admin;
using Pindou.Application.Interfaces.Admin;
using Pindou.Domain.Entities.Admin;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Options;
using Pindou.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Pindou.Infrastructure.Services.Admin;

public class AdminAuthService : IAdminAuthService
{
    private readonly IRepository<AdminUser> _adminRepo;
    private readonly IRepository<Role> _roleRepo;
    private readonly ICacheService _cache;
    private readonly JwtOptions _jwt;
    private readonly ILogger<AdminAuthService> _logger;
    public AdminAuthService(
        IRepository<AdminUser> adminRepo,
        IRepository<Role> roleRepo,
        ICacheService cache,
        IOptions<JwtOptions> jwt,
        ILogger<AdminAuthService> logger)
    {
        _adminRepo = adminRepo;
        _roleRepo = roleRepo;
        _cache = cache;
        _jwt = jwt.Value;
        _logger = logger;
    }

    public async Task<string> GenerateCaptchaAsync()
    {
        var code = Random.Shared.Next(100000, 999999).ToString();
        var key = Guid.NewGuid().ToString("N");
        await _cache.SetStringAsync($"admin:captcha:{key}", code, TimeSpan.FromMinutes(5));
        // 简化：返回key，验证码图片由前端渲染或单独API生成
        return key;
    }

    public async Task<AdminLoginResponse> LoginAsync(AdminLoginRequest request, string ip)
    {
        var admin = await _adminRepo.FirstOrDefaultAsync(a => a.Username == request.Username);
        if (admin == null) throw new BizException("用户名或密码错误", 2010);
        if (admin.Status == 0) throw new BizException("账号已禁用", 2011);

        // 错误次数
        var errKey = $"admin:login:err:{admin.Id}";
        var errCount = await _cache.GetAsync<int>(errKey);
        if (errCount >= 3)
        {
            if (string.IsNullOrEmpty(request.CaptchaKey) || string.IsNullOrEmpty(request.Captcha))
                throw new BizException("请输入验证码", 2013);
            var captcha = await _cache.GetStringAsync($"admin:captcha:{request.CaptchaKey}");
            if (string.IsNullOrEmpty(captcha) || captcha != request.Captcha)
                throw new BizException("验证码错误", 2013);
        }

        // 密码校验 - BCrypt
        if (!BCrypt.Net.BCrypt.Verify(request.Password, admin.Password))
        {
            await _cache.IncrementAsync(errKey, 1, TimeSpan.FromMinutes(15));
            throw new BizException("用户名或密码错误", 2010);
        }

        // 清理错误次数
        await _cache.RemoveAsync(errKey);

        admin.LastLoginTime = DateTime.Now;
        admin.LastLoginIp = ip;
        await _adminRepo.UpdateAsync(admin);

        var role = await _roleRepo.GetByIdAsync(admin.RoleId);
        var permissions = string.IsNullOrEmpty(role?.Permissions)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(role.Permissions);

        var token = GenerateToken(admin.Id, admin.Username, admin.RoleId, "admin", 60 * 24 * 7);
        var refreshToken = GenerateToken(admin.Id, admin.Username, admin.RoleId, "admin_refresh", 60 * 24 * 30);

        return new AdminLoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpireTime = DateTime.Now.AddMinutes(60 * 24 * 7),
            User = new AdminUserInfo
            {
                Id = admin.Id,
                Username = admin.Username,
                Nickname = admin.Nickname,
                RoleId = admin.RoleId,
                RoleName = role?.Name,
                Permissions = permissions ?? new(),
                LastLoginTime = admin.LastLoginTime,
                LastLoginIp = admin.LastLoginIp
            }
        };
    }

    public async Task LogoutAsync(long adminId)
    {
        // Token撤销（黑名单）
        await _cache.SetStringAsync($"admin:token:revoked:{adminId}", "1", TimeSpan.FromDays(30));
    }

    public Task<AdminLoginResponse> RefreshTokenAsync(string refreshToken) { throw new NotImplementedException(); }
    public Task<AdminUserInfo> GetCurrentUserAsync(long adminId) { throw new NotImplementedException(); }

    private string GenerateToken(long userId, string username, long roleId, string tokenType, int expireMinutes)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim("role_id", roleId.ToString()),
            new Claim("token_type", tokenType),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireMinutes),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class AdminUserService : IAdminUserService
{
    private readonly IRepository<AdminUser> _adminRepo;
    public AdminUserService(IRepository<AdminUser> adminRepo) { _adminRepo = adminRepo; }

    public Task<PagedResult<AdminUserListDto>> GetListAsync(AdminUserQuery query) { throw new NotImplementedException(); }
    public Task<AdminUserDetailDto> GetDetailAsync(long id) { throw new NotImplementedException(); }
    public async Task<long> CreateAsync(CreateAdminUserRequest request)
    {
        if (await _adminRepo.AnyAsync(a => a.Username == request.Username))
            throw new BizException("用户名已存在", 3001);
        var admin = new AdminUser
        {
            Username = request.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Nickname = request.Nickname,
            RoleId = request.RoleId,
            Status = request.Status
        };
        await _adminRepo.InsertAsync(admin);
        return admin.Id;
    }
    public Task<bool> UpdateAsync(long id, UpdateAdminUserRequest request) { throw new NotImplementedException(); }
    public Task<bool> DeleteAsync(long id) { throw new NotImplementedException(); }
    public Task<bool> ResetPasswordAsync(long id, string newPassword)
    {
        throw new NotImplementedException();
    }
    public Task<bool> ChangePasswordAsync(long id, string oldPassword, string newPassword) { throw new NotImplementedException(); }
    public Task<bool> UpdateStatusAsync(long id, int status) { throw new NotImplementedException(); }
}

public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roleRepo;
    public RoleService(IRepository<Role> roleRepo) { _roleRepo = roleRepo; }

    public Task<PagedResult<RoleDto>> GetListAsync(PageRequest request) { throw new NotImplementedException(); }
    public Task<List<RoleDto>> GetAllAsync() { throw new NotImplementedException(); }
    public Task<RoleDto> GetDetailAsync(long id) { throw new NotImplementedException(); }
    public Task<long> CreateAsync(CreateRoleRequest request) { throw new NotImplementedException(); }
    public Task<bool> UpdateAsync(long id, CreateRoleRequest request) { throw new NotImplementedException(); }
    public Task<bool> DeleteAsync(long id) { throw new NotImplementedException(); }
    public Task<List<string>> GetPermissionsAsync(long roleId) { throw new NotImplementedException(); }
}

public class OperationLogService : IOperationLogService
{
    private readonly IRepository<OperationLog> _logRepo;
    public OperationLogService(IRepository<OperationLog> logRepo) { _logRepo = logRepo; }

    public async Task RecordAsync(long userId, string username, string? nickname, string operation, string? content, string? method, string? @params, string? ip, string? userAgent)
    {
        await _logRepo.InsertAsync(new OperationLog
        {
            UserId = userId,
            Username = username,
            Nickname = nickname,
            Operation = operation,
            Content = content,
            Method = method,
            Params = @params,
            Ip = ip,
            UserAgent = userAgent
        });
    }
    public Task<PagedResult<OperationLogDto>> GetListAsync(OperationLogQuery query) { throw new NotImplementedException(); }
    public Task<bool> DeleteAsync(long id) { throw new NotImplementedException(); }
    public Task<bool> ClearAsync(DateTime? beforeTime) { throw new NotImplementedException(); }
}
