using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Admin;
using Pindou.Domain.Entities.Admin;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Options;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Admin;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Pindou.Tests.Services;

public class AdminAuthServiceTests
{
    private readonly Mock<IRepository<AdminUser>> _adminUserRepoMock;
    private readonly Mock<IRepository<Role>> _roleRepoMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly JwtOptions _jwtOptions;
    private readonly Mock<ILogger<AdminAuthService>> _loggerMock;
    private readonly AdminAuthService _adminAuthService;

    public AdminAuthServiceTests()
    {
        _adminUserRepoMock = new Mock<IRepository<AdminUser>>();
        _roleRepoMock = new Mock<IRepository<Role>>();
        _cacheMock = new Mock<ICacheService>();
        _jwtOptions = new JwtOptions
        {
            Secret = "PindouAdminTestSecretKey2026_AtLeast32Chars!!",
            Issuer = "PindouAdmin",
            Audience = "PindouAdmin",
            AccessTokenExpireMinutes = 120,
            RefreshTokenExpireMinutes = 1440
        };
        _loggerMock = new Mock<ILogger<AdminAuthService>>();
        _adminAuthService = new AdminAuthService(
            _adminUserRepoMock.Object,
            _roleRepoMock.Object,
            _cacheMock.Object,
            Options.Create(_jwtOptions),
            _loggerMock.Object);
    }

    #region GenerateCaptchaAsync Tests

    [Fact]
    public async Task GenerateCaptchaAsync_ShouldReturnCaptchaKey()
    {
        _cacheMock.Setup(c => c.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        var key = await _adminAuthService.GenerateCaptchaAsync();

        Assert.False(string.IsNullOrEmpty(key));
        _cacheMock.Verify(c => c.SetStringAsync(It.Is<string>(k => k.StartsWith("admin:captcha:")), It.IsAny<string>(), It.IsAny<TimeSpan?>()), Times.Once);
    }

    [Fact]
    public async Task GenerateCaptchaAsync_ShouldGenerateUniqueKeys()
    {
        _cacheMock.Setup(c => c.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        var keys = new HashSet<string>();
        for (var i = 0; i < 50; i++) keys.Add(await _adminAuthService.GenerateCaptchaAsync());

        Assert.Equal(50, keys.Count); // 全部唯一
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_ShouldReturnTokens_WhenCredentialsValid()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminUser = new AdminUser
        {
            Id = 1, Username = "admin", Password = passwordHash,
            Status = 1, RoleId = 1
        };
        var role = new Role { Id = 1, Name = "超级管理员", Code = "super_admin", Permissions = "[\"user:list\"]" };
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync(adminUser);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);

        var request = new AdminLoginRequest { Username = "admin", Password = "password123" };
        var result = await _adminAuthService.LoginAsync(request, "127.0.0.1");

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.NotNull(result.User);
        Assert.Equal("admin", result.User.Username);
        Assert.Contains("user:list", result.User.Permissions);
        Assert.Equal("127.0.0.1", adminUser.LastLoginIp);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
    {
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync((AdminUser?)null);

        var request = new AdminLoginRequest { Username = "unknown", Password = "password123" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.LoginAsync(request, "127.0.0.1"));
        Assert.Contains("用户名或密码错误", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserDisabled()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminUser = new AdminUser { Id = 1, Username = "admin", Password = passwordHash, Status = 0 };
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync(adminUser);

        var request = new AdminLoginRequest { Username = "admin", Password = "password123" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.LoginAsync(request, "127.0.0.1"));
        Assert.Contains("禁用", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordWrong()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminUser = new AdminUser { Id = 1, Username = "admin", Password = passwordHash, Status = 1 };
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync(adminUser);
        _cacheMock.Setup(c => c.IncrementAsync(It.IsAny<string>(), 1L, It.IsAny<TimeSpan?>())).ReturnsAsync(1L);

        var request = new AdminLoginRequest { Username = "admin", Password = "wrongpassword" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.LoginAsync(request, "127.0.0.1"));
        Assert.Contains("用户名或密码错误", ex.Message);
        _cacheMock.Verify(c => c.IncrementAsync(It.IsAny<string>(), 1L, It.IsAny<TimeSpan?>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldRequireCaptcha_WhenErrorCountExceeds3()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminUser = new AdminUser { Id = 1, Username = "admin", Password = passwordHash, Status = 1 };
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync(adminUser);
        _cacheMock.Setup(c => c.GetAsync<int>(It.IsAny<string>())).ReturnsAsync(5);

        var request = new AdminLoginRequest { Username = "admin", Password = "password123" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.LoginAsync(request, "127.0.0.1"));
        Assert.Contains("请输入验证码", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenCaptchaWrong()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminUser = new AdminUser { Id = 1, Username = "admin", Password = passwordHash, Status = 1 };
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync(adminUser);
        _cacheMock.Setup(c => c.GetAsync<int>(It.IsAny<string>())).ReturnsAsync(5);
        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync("123456");

        var request = new AdminLoginRequest
        {
            Username = "admin",
            Password = "password123",
            CaptchaKey = "k1",
            Captcha = "654321"
        };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.LoginAsync(request, "127.0.0.1"));
        Assert.Contains("验证码错误", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldLogin_WhenCaptchaCorrect()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminUser = new AdminUser { Id = 1, Username = "admin", Password = passwordHash, Status = 1, RoleId = 1 };
        var role = new Role { Id = 1, Name = "Admin", Code = "admin", Permissions = "[]" };
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync(adminUser);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);
        _cacheMock.Setup(c => c.GetAsync<int>(It.IsAny<string>())).ReturnsAsync(5);
        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync("123456");

        var request = new AdminLoginRequest
        {
            Username = "admin",
            Password = "password123",
            CaptchaKey = "k1",
            Captcha = "123456"
        };
        var result = await _adminAuthService.LoginAsync(request, "127.0.0.1");

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
    }

    [Fact]
    public async Task LoginAsync_ShouldHandleRoleWithoutPermissions()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminUser = new AdminUser { Id = 1, Username = "admin", Password = passwordHash, Status = 1, RoleId = 99 };
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync(adminUser);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);
        _roleRepoMock.Setup(r => r.GetByIdAsync(99L)).ReturnsAsync((Role?)null);

        var request = new AdminLoginRequest { Username = "admin", Password = "password123" };
        var result = await _adminAuthService.LoginAsync(request, "127.0.0.1");

        Assert.NotNull(result);
        Assert.Empty(result.User.Permissions);
    }

    #endregion

    #region LogoutAsync Tests

    [Fact]
    public async Task LogoutAsync_ShouldRevokeTokenInCache()
    {
        await _adminAuthService.LogoutAsync(1);

        _cacheMock.Verify(c => c.SetStringAsync(
            "admin:token:revoked:1",
            "1",
            It.Is<TimeSpan>(t => t.TotalDays == 30)),
            Times.Once);
    }

    #endregion

    #region RefreshTokenAsync Tests

    [Fact]
    public async Task RefreshTokenAsync_ShouldRefresh_WhenTokenValid()
    {
        var adminUser = new AdminUser { Id = 1, Username = "admin", Status = 1, RoleId = 1 };
        var role = new Role { Id = 1, Name = "Admin", Code = "admin", Permissions = "[]" };
        var refreshToken = GenerateTestRefreshToken(1);

        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(adminUser);
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);

        var result = await _adminAuthService.RefreshTokenAsync(refreshToken);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.Equal("admin", result.User.Username);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenTokenInvalid()
    {
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.RefreshTokenAsync("invalid_token"));
        Assert.Contains("无效", ex.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenUserNotFound()
    {
        var refreshToken = GenerateTestRefreshToken(999);
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.RefreshTokenAsync(refreshToken));
        Assert.Contains("用户不存在", ex.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenUserDisabled()
    {
        var adminUser = new AdminUser { Id = 1, Username = "admin", Status = 0 };
        var refreshToken = GenerateTestRefreshToken(1);
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(adminUser);

        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.RefreshTokenAsync(refreshToken));
        Assert.Contains("禁用", ex.Message);
    }

    #endregion

    #region GetCurrentUserAsync Tests

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnUserInfo()
    {
        var adminUser = new AdminUser
        {
            Id = 1, Username = "admin", Nickname = "管理员", Status = 1,
            RoleId = 1, LastLoginTime = DateTime.Now, LastLoginIp = "127.0.0.1"
        };
        var role = new Role { Id = 1, Name = "Admin", Code = "admin", Permissions = "[\"a\",\"b\"]" };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(adminUser);
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);

        var result = await _adminAuthService.GetCurrentUserAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("admin", result.Username);
        Assert.Equal("Admin", result.RoleName);
        Assert.Equal(2, result.Permissions.Count);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldThrow_WhenUserNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.GetCurrentUserAsync(999));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region Helper Methods

    private string GenerateTestRefreshToken(long adminUserId)
    {
        var claims = new[]
        {
            new Claim("sub", adminUserId.ToString()),
            new Claim("token_type", "admin_refresh"),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtOptions.RefreshTokenExpireMinutes),
            signingCredentials: creds);
        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }

    #endregion
}
