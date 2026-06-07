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
using SqlSugar;
using ICacheService = Pindou.Infrastructure.Cache.ICacheService;

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
        return key;
    }

    public async Task<AdminLoginResponse> LoginAsync(AdminLoginRequest request, string ip)
    {
        var admin = await _adminRepo.FirstOrDefaultAsync(a => a.Username == request.Username);
        if (admin == null) throw new BizException("用户名或密码错误", 2010);
        if (admin.Status == 0) throw new BizException("账号已禁用", 2011);

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

        if (!BCrypt.Net.BCrypt.Verify(request.Password, admin.Password))
        {
            await _cache.IncrementAsync(errKey, 1, TimeSpan.FromMinutes(15));
            throw new BizException("用户名或密码错误", 2010);
        }

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
        await _cache.SetStringAsync($"admin:token:revoked:{adminId}", "1", TimeSpan.FromDays(30));
    }

    public async Task<AdminLoginResponse> RefreshTokenAsync(string refreshToken)
    {
        var principal = ValidateToken(refreshToken);
        if (principal == null)
            throw new BizException("refresh_token无效", 2001);

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var adminId))
            throw new BizException("refresh_token无效", 2001);

        var admin = await _adminRepo.GetByIdAsync(adminId);
        if (admin == null || admin.Status == 0)
            throw new BizException("用户不存在或已禁用", 4001);

        var role = await _roleRepo.GetByIdAsync(admin.RoleId);
        var permissions = string.IsNullOrEmpty(role?.Permissions)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(role.Permissions);

        var token = GenerateToken(admin.Id, admin.Username, admin.RoleId, "admin", 60 * 24 * 7);
        var newRefreshToken = GenerateToken(admin.Id, admin.Username, admin.RoleId, "admin_refresh", 60 * 24 * 30);

        return new AdminLoginResponse
        {
            Token = token,
            RefreshToken = newRefreshToken,
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

    public async Task<AdminUserInfo> GetCurrentUserAsync(long adminId)
    {
        var admin = await _adminRepo.GetByIdAsync(adminId);
        if (admin == null) throw new BizException("用户不存在", 4001);

        var role = await _roleRepo.GetByIdAsync(admin.RoleId);
        var permissions = string.IsNullOrEmpty(role?.Permissions)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(role.Permissions);

        return new AdminUserInfo
        {
            Id = admin.Id,
            Username = admin.Username,
            Nickname = admin.Nickname,
            RoleId = admin.RoleId,
            RoleName = role?.Name,
            Permissions = permissions ?? new(),
            LastLoginTime = admin.LastLoginTime,
            LastLoginIp = admin.LastLoginIp
        };
    }

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

    private ClaimsPrincipal? ValidateToken(string token)
    {
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
            return new ClaimsPrincipal(new ClaimsIdentity(((JwtSecurityToken)validatedToken).Claims));
        }
        catch
        {
            return null;
        }
    }
}

public class AdminUserService : IAdminUserService
{
    private readonly IRepository<AdminUser> _adminRepo;
    private readonly IRepository<Role> _roleRepo;

    public AdminUserService(IRepository<AdminUser> adminRepo, IRepository<Role> roleRepo)
    {
        _adminRepo = adminRepo;
        _roleRepo = roleRepo;
    }

    public async Task<PagedResult<AdminUserListDto>> GetListAsync(AdminUserQuery query)
    {
        var exp = Expressionable.Create<AdminUser>();
        if (query.RoleId.HasValue)
            exp.And(a => a.RoleId == query.RoleId.Value);
        if (query.Status.HasValue)
            exp.And(a => a.Status == query.Status.Value);

        var (list, total) = await _adminRepo.GetPagedAsync(
            exp.ToExpression(),
            query.Page,
            query.Size,
            a => a.CreateTime,
            true);

        var result = new PagedResult<AdminUserListDto>
        {
            Page = query.Page,
            Size = query.Size,
            Total = total,
            List = new List<AdminUserListDto>()
        };

        foreach (var admin in list)
        {
            var role = await _roleRepo.GetByIdAsync(admin.RoleId);
            result.List.Add(new AdminUserListDto
            {
                Id = admin.Id,
                Username = admin.Username,
                Nickname = admin.Nickname,
                RoleId = admin.RoleId,
                RoleName = role?.Name,
                Status = admin.Status,
                LastLoginTime = admin.LastLoginTime,
                LastLoginIp = admin.LastLoginIp,
                CreateTime = admin.CreateTime
            });
        }

        return result;
    }

    public async Task<AdminUserDetailDto> GetDetailAsync(long id)
    {
        var admin = await _adminRepo.GetByIdAsync(id);
        if (admin == null) throw new BizException("管理员不存在", ErrorCodes.NotFound);

        var role = await _roleRepo.GetByIdAsync(admin.RoleId);
        return new AdminUserDetailDto
        {
            Id = admin.Id,
            Username = admin.Username,
            Nickname = admin.Nickname,
            RoleId = admin.RoleId,
            RoleName = role?.Name,
            Status = admin.Status,
            LastLoginTime = admin.LastLoginTime,
            LastLoginIp = admin.LastLoginIp,
            CreateTime = admin.CreateTime,
            UpdateTime = admin.UpdateTime
        };
    }

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

    public async Task<bool> UpdateAsync(long id, UpdateAdminUserRequest request)
    {
        var admin = await _adminRepo.GetByIdAsync(id);
        if (admin == null) throw new BizException("管理员不存在", ErrorCodes.NotFound);

        if (request.Nickname != null) admin.Nickname = request.Nickname;
        if (request.RoleId.HasValue) admin.RoleId = request.RoleId.Value;
        if (request.Status.HasValue) admin.Status = request.Status.Value;
        admin.UpdateTime = DateTime.Now;
        return await _adminRepo.UpdateAsync(admin);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var admin = await _adminRepo.GetByIdAsync(id);
        if (admin == null) throw new BizException("管理员不存在", ErrorCodes.NotFound);

        return await _adminRepo.DeleteAsync(id);
    }

    public async Task<bool> ResetPasswordAsync(long id, string newPassword)
    {
        var admin = await _adminRepo.GetByIdAsync(id);
        if (admin == null) throw new BizException("管理员不存在", ErrorCodes.NotFound);

        admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        admin.UpdateTime = DateTime.Now;
        return await _adminRepo.UpdateAsync(admin);
    }

    public async Task<bool> ChangePasswordAsync(long id, string oldPassword, string newPassword)
    {
        var admin = await _adminRepo.GetByIdAsync(id);
        if (admin == null) throw new BizException("管理员不存在", ErrorCodes.NotFound);

        if (!BCrypt.Net.BCrypt.Verify(oldPassword, admin.Password))
            throw new BizException("原密码错误", ErrorCodes.ParamError);

        admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        admin.UpdateTime = DateTime.Now;
        return await _adminRepo.UpdateAsync(admin);
    }

    public async Task<bool> UpdateStatusAsync(long id, int status)
    {
        var admin = await _adminRepo.GetByIdAsync(id);
        if (admin == null) throw new BizException("管理员不存在", ErrorCodes.NotFound);

        admin.Status = status;
        admin.UpdateTime = DateTime.Now;
        return await _adminRepo.UpdateAsync(admin);
    }
}

public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roleRepo;

    public RoleService(IRepository<Role> roleRepo)
    {
        _roleRepo = roleRepo;
    }

    public async Task<PagedResult<RoleDto>> GetListAsync(PageRequest request)
    {
        var (list, total) = await _roleRepo.GetPagedAsync(
            null,
            request.Page,
            request.Size,
            r => r.Id,
            true);

        var result = new PagedResult<RoleDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<RoleDto>()
        };

        foreach (var role in list)
        {
            var perms = new List<string>();
            if (!string.IsNullOrEmpty(role.Permissions))
            {
                try { perms = JsonSerializer.Deserialize<List<string>>(role.Permissions) ?? new(); }
                catch { }
            }

            result.List.Add(new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Code = role.Code,
                Description = role.Description,
                Permissions = perms,
                CreateTime = role.CreateTime
            });
        }

        return result;
    }

    public async Task<List<RoleDto>> GetAllAsync()
    {
        var roles = await _roleRepo.GetListAsync();
        return roles.Select(role =>
        {
            var perms = new List<string>();
            if (!string.IsNullOrEmpty(role.Permissions))
            {
                try { perms = JsonSerializer.Deserialize<List<string>>(role.Permissions) ?? new(); }
                catch { }
            }

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Code = role.Code,
                Description = role.Description,
                Permissions = perms,
                CreateTime = role.CreateTime
            };
        }).ToList();
    }

    public async Task<RoleDto> GetDetailAsync(long id)
    {
        var role = await _roleRepo.GetByIdAsync(id);
        if (role == null) throw new BizException("角色不存在", ErrorCodes.NotFound);

        var perms = new List<string>();
        if (!string.IsNullOrEmpty(role.Permissions))
        {
            try { perms = JsonSerializer.Deserialize<List<string>>(role.Permissions) ?? new(); }
            catch { }
        }

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Code = role.Code,
            Description = role.Description,
            Permissions = perms,
            CreateTime = role.CreateTime
        };
    }

    public async Task<long> CreateAsync(CreateRoleRequest request)
    {
        if (await _roleRepo.AnyAsync(r => r.Code == request.Code))
            throw new BizException("角色编码已存在", 3001);

        var role = new Role
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Permissions = JsonSerializer.Serialize(request.Permissions)
        };
        await _roleRepo.InsertAsync(role);
        return role.Id;
    }

    public async Task<bool> UpdateAsync(long id, CreateRoleRequest request)
    {
        var role = await _roleRepo.GetByIdAsync(id);
        if (role == null) throw new BizException("角色不存在", ErrorCodes.NotFound);

        role.Name = request.Name;
        role.Code = request.Code;
        role.Description = request.Description;
        role.Permissions = JsonSerializer.Serialize(request.Permissions);
        role.UpdateTime = DateTime.Now;
        return await _roleRepo.UpdateAsync(role);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var role = await _roleRepo.GetByIdAsync(id);
        if (role == null) throw new BizException("角色不存在", ErrorCodes.NotFound);

        return await _roleRepo.DeleteAsync(id);
    }

    public async Task<List<string>> GetPermissionsAsync(long roleId)
    {
        var role = await _roleRepo.GetByIdAsync(roleId);
        if (role == null) return new List<string>();

        if (string.IsNullOrEmpty(role.Permissions)) return new List<string>();
        try { return JsonSerializer.Deserialize<List<string>>(role.Permissions) ?? new(); }
        catch { return new List<string>(); }
    }
}

public class OperationLogService : IOperationLogService
{
    private readonly IRepository<OperationLog> _logRepo;

    public OperationLogService(IRepository<OperationLog> logRepo)
    {
        _logRepo = logRepo;
    }

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

    public async Task<PagedResult<OperationLogDto>> GetListAsync(OperationLogQuery query)
    {
        var exp = Expressionable.Create<OperationLog>();
        if (query.UserId.HasValue)
            exp.And(l => l.UserId == query.UserId.Value);
        if (!string.IsNullOrWhiteSpace(query.Operation))
            exp.And(l => l.Operation == query.Operation);
        if (query.StartTime.HasValue)
            exp.And(l => l.CreateTime >= query.StartTime.Value);
        if (query.EndTime.HasValue)
            exp.And(l => l.CreateTime <= query.EndTime.Value);

        var (list, total) = await _logRepo.GetPagedAsync(
            exp.ToExpression(),
            query.Page,
            query.Size,
            l => l.CreateTime,
            true);

        var result = new PagedResult<OperationLogDto>
        {
            Page = query.Page,
            Size = query.Size,
            Total = total,
            List = new List<OperationLogDto>()
        };

        foreach (var log in list)
        {
            result.List.Add(new OperationLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Username = log.Username,
                Nickname = log.Nickname,
                Operation = log.Operation,
                Content = log.Content,
                Method = log.Method,
                Params = log.Params,
                Ip = log.Ip,
                CreateTime = log.CreateTime
            });
        }

        return result;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var log = await _logRepo.GetByIdAsync(id);
        if (log == null) throw new BizException("日志不存在", ErrorCodes.NotFound);

        return await _logRepo.DeleteAsync(id);
    }

    public async Task<bool> ClearAsync(DateTime? beforeTime)
    {
        var cutoff = beforeTime ?? DateTime.Now.AddDays(-30);
        return await _logRepo.DeleteAsync(l => l.CreateTime < cutoff);
    }
}