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

namespace Pindou.Tests.Services;

public class AdminAuthServiceTests
{
    private readonly Mock<IRepository<AdminUser>> _adminUserRepoMock;
    private readonly Mock<IRepository<AdminToken>> _adminTokenRepoMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly JwtOptions _jwtOptions;
    private readonly Mock<ILogger<AdminAuthService>> _loggerMock;
    private readonly AdminAuthService _adminAuthService;

    public AdminAuthServiceTests()
    {
        _adminUserRepoMock = new Mock<IRepository<AdminUser>>();
        _adminTokenRepoMock = new Mock<IRepository<AdminToken>>();
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
            _adminUserRepoMock.Object, _adminTokenRepoMock.Object,
            _cacheMock.Object, Options.Create(_jwtOptions), _loggerMock.Object);
    }

    #region GenerateCaptchaAsync Tests

    [Fact]
    public async Task GenerateCaptchaAsync_ShouldGenerateCaptcha()
    {
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        var result = await _adminAuthService.GenerateCaptchaAsync();

        Assert.NotNull(result);
        Assert.NotNull(result.CaptchaId);
        Assert.NotNull(result.CaptchaImage);
        Assert.False(string.IsNullOrEmpty(result.CaptchaId));
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_ShouldLogin_WhenCredentialsValid()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminUser = new AdminUser
        {
            Id = "a1", Username = "admin", PasswordHash = passwordHash,
            Status = 1, RoleId = "r1", IsAdmin = true
        };
        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync("ABCD");
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync(adminUser);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);
        _adminTokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<AdminToken>())).ReturnsAsync("t1");

        var request = new AdminLoginRequest
        {
            Username = "admin",
            Password = "password123",
            CaptchaId = "cap1",
            CaptchaCode = "ABCD"
        };
        var result = await _adminAuthService.LoginAsync(request);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.Equal("a1", result.User.Id);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenCaptchaWrong()
    {
        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync("ABCD");

        var request = new AdminLoginRequest
        {
            Username = "admin",
            Password = "password123",
            CaptchaId = "cap1",
            CaptchaCode = "WRONG"
        };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.LoginAsync(request));
        Assert.Contains("验证码", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
    {
        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync("ABCD");
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync((AdminUser?)null);

        var request = new AdminLoginRequest
        {
            Username = "unknown",
            Password = "password123",
            CaptchaId = "cap1",
            CaptchaCode = "ABCD"
        };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.LoginAsync(request));
        Assert.Contains("用户名或密码错误", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordWrong()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminUser = new AdminUser { Id = "a1", Username = "admin", PasswordHash = passwordHash, Status = 1 };
        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync("ABCD");
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync(adminUser);

        var request = new AdminLoginRequest
        {
            Username = "admin",
            Password = "wrongpassword",
            CaptchaId = "cap1",
            CaptchaCode = "ABCD"
        };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.LoginAsync(request));
        Assert.Contains("用户名或密码错误", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserDisabled()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminUser = new AdminUser { Id = "a1", Username = "admin", PasswordHash = passwordHash, Status = 0 };
        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync("ABCD");
        _adminUserRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdminUser, bool>>>()))
            .ReturnsAsync(adminUser);

        var request = new AdminLoginRequest
        {
            Username = "admin",
            Password = "password123",
            CaptchaId = "cap1",
            CaptchaCode = "ABCD"
        };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.LoginAsync(request));
        Assert.Contains("禁用", ex.Message);
    }

    #endregion

    #region LogoutAsync Tests

    [Fact]
    public async Task LogoutAsync_ShouldClearTokens()
    {
        var tokens = new List<AdminToken>
        {
            new AdminToken { Id = "t1", AdminUserId = "a1", AccessToken = "at1", RefreshToken = "rt1", ExpiresAt = DateTime.Now.AddDays(1) }
        };
        _adminTokenRepoMock.Setup(r => r.GetListAsync(
                It.IsAny<Expression<Func<AdminToken, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .ReturnsAsync(tokens);
        _adminTokenRepoMock.Setup(r => r.DeleteAsync(It.IsAny<object>())).ReturnsAsync(true);

        await _adminAuthService.LogoutAsync("a1");

        _adminTokenRepoMock.Verify(r => r.DeleteAsync(It.IsAny<object>()), Times.Once);
    }

    #endregion

    #region RefreshTokenAsync Tests

    [Fact]
    public async Task RefreshTokenAsync_ShouldRefresh_WhenTokenValid()
    {
        var adminUser = new AdminUser { Id = "a1", Username = "admin", Status = 1, RoleId = "r1", IsAdmin = true };
        var refreshToken = GenerateTestRefreshToken("a1");

        _adminUserRepoMock.Setup(r => r.GetByIdAsync("a1")).ReturnsAsync(adminUser);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);
        _adminTokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<AdminToken>())).ReturnsAsync("t_new");

        var result = await _adminAuthService.RefreshTokenAsync(refreshToken);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.Equal("a1", result.User.Id);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenTokenInvalid()
    {
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminAuthService.RefreshTokenAsync("invalid_token"));
        Assert.Contains("无效", ex.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenUserDisabled()
    {
        var adminUser = new AdminUser { Id = "a1", Username = "admin", Status = 0 };
        var refreshToken = GenerateTestRefreshToken("a1");

        _adminUserRepoMock.Setup(r => r.GetByIdAsync("a1")).ReturnsAsync(adminUser);

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
            Id = "a1", Username = "admin", Nickname = "管理员", Status = 1,
            RoleId = "r1", IsAdmin = true, LastLoginTime = DateTime.Now
        };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("a1")).ReturnsAsync(adminUser);

        var result = await _adminAuthService.GetCurrentUserAsync("a1");

        Assert.NotNull(result);
        Assert.Equal("a1", result.Id);
        Assert.Equal("admin", result.Username);
        Assert.True(result.IsAdmin);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldThrow_WhenUserNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _adminAuthService.GetCurrentUserAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region Helper Methods

    private string GenerateTestRefreshToken(string adminUserId)
    {
        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.JwtRegisteredClaimNames.Sub, adminUserId),
            new System.Security.Claims.Claim(System.Security.Claims.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new System.Security.Claims.Claim("token_type", "admin_refresh")
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