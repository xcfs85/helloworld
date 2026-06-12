using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Auth;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.ExternalServices.Sms;
using Pindou.Infrastructure.Options;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Auth;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Pindou.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly Mock<IRepository<Token>> _tokenRepoMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly JwtOptions _jwtOptions;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly Mock<ISmsService> _smsServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IRepository<User>>();
        _tokenRepoMock = new Mock<IRepository<Token>>();
        _cacheMock = new Mock<ICacheService>();
        _jwtOptions = new JwtOptions
        {
            Secret = "PindouTestSecretKey2026_AtLeast32Chars!",
            Issuer = "Pindou",
            Audience = "Pindou",
            AccessTokenExpireMinutes = 1440,
            RefreshTokenExpireMinutes = 43200
        };
        _loggerMock = new Mock<ILogger<AuthService>>();
        _smsServiceMock = new Mock<ISmsService>();

        _authService = new AuthService(
            _userRepoMock.Object,
            _tokenRepoMock.Object,
            _cacheMock.Object,
            Options.Create(_jwtOptions),
            _loggerMock.Object,
            _smsServiceMock.Object);
    }

    #region SendSmsAsync Tests

    [Fact]
    public async Task SendSmsAsync_ShouldSendSms_WhenPhoneIsValid()
    {
        var request = new SmsSendRequest { Phone = "13800138000", Scene = "login" };
        _cacheMock.Setup(c => c.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _cacheMock.Setup(c => c.GetAsync<long>(It.IsAny<string>())).ReturnsAsync(0L);
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>())).Returns(Task.CompletedTask);
        _cacheMock.Setup(c => c.IncrementAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<TimeSpan?>())).ReturnsAsync(1L);
        _smsServiceMock.Setup(s => s.SendCodeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new SmsResult { Success = true });

        var result = await _authService.SendSmsAsync(request);

        Assert.NotNull(result);
        Assert.Equal(300, result.ExpireSeconds);
        Assert.True(result.RemainingTimes >= 0);
    }

    [Fact]
    public async Task SendSmsAsync_ShouldThrow_WhenRateLimited()
    {
        var request = new SmsSendRequest { Phone = "13800138000", Scene = "login" };
        _cacheMock.Setup(c => c.ExistsAsync("sms:rate:13800138000")).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<BizException>(() => _authService.SendSmsAsync(request));
        Assert.Contains("频繁", ex.Message);
    }

    [Fact]
    public async Task SendSmsAsync_ShouldThrow_WhenDailyLimitReached()
    {
        var request = new SmsSendRequest { Phone = "13800138000", Scene = "login" };
        _cacheMock.Setup(c => c.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _cacheMock.Setup(c => c.GetAsync<long>(It.IsAny<string>())).ReturnsAsync(10L);

        var ex = await Assert.ThrowsAsync<BizException>(() => _authService.SendSmsAsync(request));
        Assert.Contains("上限", ex.Message);
    }

    #endregion

    #region PhoneLoginAsync Tests

    [Fact]
    public async Task PhoneLoginAsync_ShouldLogin_WhenCodeIsValid()
    {
        var request = new PhoneLoginRequest { Phone = "13800138000", Code = "123456" };
        var user = new User { Id = "u1", Phone = "13800138000", Nickname = "test", Status = "active" };

        _cacheMock.Setup(c => c.GetStringAsync("sms:code:13800138000:login")).ReturnsAsync("123456");
        _cacheMock.Setup(c => c.RemoveAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);
        _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<Token>())).ReturnsAsync("t1");

        var result = await _authService.PhoneLoginAsync(request);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.Equal("u1", result.User.Id);
    }

    [Fact]
    public async Task PhoneLoginAsync_ShouldThrow_WhenCodeExpired()
    {
        var request = new PhoneLoginRequest { Phone = "13800138000", Code = "123456" };
        _cacheMock.Setup(c => c.GetStringAsync("sms:code:13800138000:login")).ReturnsAsync((string?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _authService.PhoneLoginAsync(request));
        Assert.Contains("过期", ex.Message);
    }

    [Fact]
    public async Task PhoneLoginAsync_ShouldThrow_WhenCodeWrong()
    {
        var request = new PhoneLoginRequest { Phone = "13800138000", Code = "wrong" };
        _cacheMock.Setup(c => c.GetStringAsync("sms:code:13800138000:login")).ReturnsAsync("123456");

        var ex = await Assert.ThrowsAsync<BizException>(() => _authService.PhoneLoginAsync(request));
        Assert.Contains("错误", ex.Message);
    }

    [Fact]
    public async Task PhoneLoginAsync_ShouldThrow_WhenUserDisabled()
    {
        var request = new PhoneLoginRequest { Phone = "13800138000", Code = "123456" };
        var user = new User { Id = "u1", Phone = "13800138000", Nickname = "test", Status = "disabled" };

        _cacheMock.Setup(c => c.GetStringAsync("sms:code:13800138000:login")).ReturnsAsync("123456");
        _cacheMock.Setup(c => c.RemoveAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<BizException>(() => _authService.PhoneLoginAsync(request));
        Assert.Contains("禁用", ex.Message);
    }

    [Fact]
    public async Task PhoneLoginAsync_ShouldAutoRegister_WhenUserNotExists()
    {
        var request = new PhoneLoginRequest { Phone = "13800138000", Code = "123456" };

        _cacheMock.Setup(c => c.GetStringAsync("sms:code:13800138000:login")).ReturnsAsync("123456");
        _cacheMock.Setup(c => c.RemoveAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.InsertAsync(It.IsAny<User>())).ReturnsAsync("u_new");
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);
        _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<Token>())).ReturnsAsync("t_new");

        var result = await _authService.PhoneLoginAsync(request);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
    }

    #endregion

    #region GuestLoginAsync Tests

    [Fact]
    public async Task GuestLoginAsync_ShouldCreateUserAndLogin()
    {
        var request = new GuestLoginRequest { DeviceId = "device1" };
        _userRepoMock.Setup(r => r.InsertAsync(It.IsAny<User>())).ReturnsAsync("u_guest");
        _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<Token>())).ReturnsAsync("t_guest");

        var result = await _authService.GuestLoginAsync(request);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.Contains("游客", result.User.Nickname);
    }

    #endregion

    #region RefreshTokenAsync Tests

    [Fact]
    public async Task RefreshTokenAsync_ShouldRefresh_WhenTokenValid()
    {
        var user = new User { Id = "u1", Nickname = "test", Status = "active" };
        // Generate a valid refresh token
        var refreshToken = GenerateTestRefreshToken("u1");

        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);
        _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<Token>())).ReturnsAsync("t_new");

        var result = await _authService.RefreshTokenAsync(refreshToken);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.Equal("u1", result.User.Id);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenTokenInvalid()
    {
        var ex = await Assert.ThrowsAsync<BizException>(() => _authService.RefreshTokenAsync("invalid_token"));
        Assert.Contains("无效", ex.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenUserDisabled()
    {
        var user = new User { Id = "u1", Nickname = "test", Status = "disabled" };
        var refreshToken = GenerateTestRefreshToken("u1");

        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<BizException>(() => _authService.RefreshTokenAsync(refreshToken));
        Assert.Contains("禁用", ex.Message);
    }

    #endregion

    #region LogoutAsync Tests

    [Fact]
    public async Task LogoutAsync_ShouldClearTokens()
    {
        var tokens = new List<Token>
        {
            new Token { Id = "t1", UserId = "u1", AccessToken = "at1", RefreshToken = "rt1", DeviceId = "d1", ExpiresAt = DateTime.Now.AddDays(1) },
            new Token { Id = "t2", UserId = "u1", AccessToken = "at2", RefreshToken = "rt2", DeviceId = "d2", ExpiresAt = DateTime.Now.AddDays(1) }
        };
        _tokenRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Token, bool>>>()))
            .ReturnsAsync(tokens);
        _tokenRepoMock.Setup(r => r.DeleteAsync(It.IsAny<object>())).ReturnsAsync(true);

        await _authService.LogoutAsync("u1");

        _tokenRepoMock.Verify(r => r.DeleteAsync(It.IsAny<object>()), Times.Exactly(2));
    }

    #endregion

    #region GetCurrentUserAsync Tests

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnUserInfo()
    {
        var user = new User
        {
            Id = "u1",
            Nickname = "test",
            Avatar = "avatar.png",
            Phone = "13800138000",
            Gender = "male",
            IsMember = true,
            MemberExpireTime = DateTime.Now.AddDays(30)
        };
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);

        var result = await _authService.GetCurrentUserAsync("u1");

        Assert.NotNull(result);
        Assert.Equal("u1", result.Id);
        Assert.Equal("test", result.Nickname);
        Assert.Equal("avatar.png", result.Avatar);
        Assert.True(result.IsMember);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _authService.GetCurrentUserAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region Helper Methods

    private string GenerateTestRefreshToken(string userId)
    {
        var claims = new[]
        {
            new Claim("sub", userId),
            new Claim("jti", Guid.NewGuid().ToString()),
            new System.Security.Claims.Claim("token_type", "refresh")
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