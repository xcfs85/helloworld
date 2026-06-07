using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.User;
using Pindou.Domain.Entities.User;
using Pindou.Domain.Entities.Creation;
using Pindou.Domain.Entities.Community;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.User;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly Mock<IRepository<Diagram>> _diagramRepoMock;
    private readonly Mock<IRepository<Post>> _postRepoMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IRepository<User>>();
        _diagramRepoMock = new Mock<IRepository<Diagram>>();
        _postRepoMock = new Mock<IRepository<Post>>();
        _cacheMock = new Mock<ICacheService>();
        _userService = new UserService(_userRepoMock.Object, _diagramRepoMock.Object, _postRepoMock.Object, _cacheMock.Object);
    }

    #region GetUserInfoAsync Tests

    [Fact]
    public async Task GetUserInfoAsync_ShouldReturnUserInfo_WhenUserExists()
    {
        var user = new User
        {
            Id = "u1", Nickname = "test", Avatar = "avatar.png", Phone = "13800138000",
            Gender = "male", IsMember = true, MemberExpireTime = DateTime.Now.AddDays(30)
        };
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);

        var result = await _userService.GetUserInfoAsync("u1");

        Assert.NotNull(result);
        Assert.Equal("u1", result.Id);
        Assert.Equal("test", result.Nickname);
        Assert.True(result.IsMember);
    }

    [Fact]
    public async Task GetUserInfoAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _userService.GetUserInfoAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region UpdateUserInfoAsync Tests

    [Fact]
    public async Task UpdateUserInfoAsync_ShouldUpdate_WhenUserExists()
    {
        var user = new User { Id = "u1", Nickname = "old", Avatar = null, Gender = "unknown" };
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

        var request = new UpdateUserRequest { Nickname = "new", Avatar = "new.png", Gender = "male" };
        var result = await _userService.UpdateUserInfoAsync("u1", request);

        Assert.NotNull(result);
        Assert.Equal("new", result.Nickname);
        Assert.Equal("new.png", result.Avatar);
        Assert.Equal("male", result.Gender);
    }

    [Fact]
    public async Task UpdateUserInfoAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _userService.UpdateUserInfoAsync("nonexistent", new UpdateUserRequest()));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetListAsync Tests

    [Fact]
    public async Task GetListAsync_ShouldReturnPagedResult()
    {
        var users = new List<User>
        {
            new User { Id = "u1", Nickname = "user1", Status = "active", CreateTime = DateTime.Now },
            new User { Id = "u2", Nickname = "user2", Status = "active", CreateTime = DateTime.Now }
        };
        _userRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<User, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((users, 2));
        _diagramRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Diagram, bool>>>())).ReturnsAsync(1);
        _postRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Post, bool>>>())).ReturnsAsync(2);

        var query = new UserListQuery { Page = 1, Size = 10 };
        var result = await _userService.GetListAsync(query);

        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.List.Count);
    }

    #endregion

    #region DisableUserAsync Tests

    [Fact]
    public async Task DisableUserAsync_ShouldDisableUser()
    {
        var user = new User { Id = "u1", Status = "active" };
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

        var result = await _userService.DisableUserAsync("u1", "违规", 1);

        Assert.True(result);
        Assert.Equal("disabled", user.Status);
    }

    [Fact]
    public async Task DisableUserAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _userService.DisableUserAsync("nonexistent", "reason", 1));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region EnableUserAsync Tests

    [Fact]
    public async Task EnableUserAsync_ShouldEnableUser()
    {
        var user = new User { Id = "u1", Status = "disabled" };
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

        var result = await _userService.EnableUserAsync("u1", 1);

        Assert.True(result);
        Assert.Equal("active", user.Status);
    }

    [Fact]
    public async Task EnableUserAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _userService.EnableUserAsync("nonexistent", 1));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetGenerationQuotaAsync Tests

    [Fact]
    public async Task GetGenerationQuotaAsync_ShouldReturnDefault_WhenNotMember()
    {
        var user = new User { Id = "u1", IsMember = false };
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _cacheMock.Setup(c => c.GetAsync<int>(It.IsAny<string>())).ReturnsAsync(0);

        var result = await _userService.GetGenerationQuotaAsync("u1");

        Assert.Equal(3, result);
    }

    [Fact]
    public async Task GetGenerationQuotaAsync_ShouldReturnUnlimited_WhenLifetimeMember()
    {
        var user = new User { Id = "u1", IsMember = true, MemberExpireTime = DateTime.Now.AddYears(9) };
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _cacheMock.Setup(c => c.GetAsync<int>(It.IsAny<string>())).ReturnsAsync(0);

        var result = await _userService.GetGenerationQuotaAsync("u1");

        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task GetGenerationQuotaAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _userService.GetGenerationQuotaAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region ConsumeGenerationQuotaAsync Tests

    [Fact]
    public async Task ConsumeGenerationQuotaAsync_ShouldIncrement()
    {
        _cacheMock.Setup(c => c.IncrementAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<TimeSpan?>()))
            .ReturnsAsync(1L);

        await _userService.ConsumeGenerationQuotaAsync("u1");

        _cacheMock.Verify(c => c.IncrementAsync(It.IsAny<string>(), 1, It.IsAny<TimeSpan?>()), Times.Once);
    }

    #endregion
}