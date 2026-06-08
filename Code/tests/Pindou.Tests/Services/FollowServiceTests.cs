using Moq;
using Pindou.Application.Common;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Community;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class FollowServiceTests
{
    private readonly Mock<IRepository<Follow>> _followRepoMock;
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly FollowService _followService;

    public FollowServiceTests()
    {
        _followRepoMock = new Mock<IRepository<Follow>>();
        _userRepoMock = new Mock<IRepository<User>>();
        _followService = new FollowService(_followRepoMock.Object, _userRepoMock.Object);
    }

    #region FollowAsync Tests

    [Fact]
    public async Task FollowAsync_ShouldFollow_WhenValid()
    {
        _followRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Follow, bool>>>())).ReturnsAsync(false);
        _followRepoMock.Setup(r => r.InsertAsync(It.IsAny<Follow>())).ReturnsAsync("f1");

        var result = await _followService.FollowAsync("u1", "u2");

        Assert.True(result);
    }

    [Fact]
    public async Task FollowAsync_ShouldThrow_WhenFollowSelf()
    {
        var ex = await Assert.ThrowsAsync<BizException>(() => _followService.FollowAsync("u1", "u1"));
        Assert.Contains("自己", ex.Message);
    }

    [Fact]
    public async Task FollowAsync_ShouldReturnTrue_WhenAlreadyFollowing()
    {
        _followRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Follow, bool>>>())).ReturnsAsync(true);

        var result = await _followService.FollowAsync("u1", "u2");

        Assert.True(result);
    }

    #endregion

    #region UnfollowAsync Tests

    [Fact]
    public async Task UnfollowAsync_ShouldUnfollow_WhenFollowing()
    {
        var follow = new Follow { Id = "f1", UserId = "u1", FollowUserId = "u2" };
        _followRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Follow, bool>>>())).ReturnsAsync(follow);
        _followRepoMock.Setup(r => r.DeleteAsync(It.IsAny<object>())).ReturnsAsync(true);

        var result = await _followService.UnfollowAsync("u1", "u2");

        Assert.True(result);
    }

    [Fact]
    public async Task UnfollowAsync_ShouldReturnTrue_WhenNotFollowing()
    {
        _followRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Follow, bool>>>())).ReturnsAsync((Follow?)null);

        var result = await _followService.UnfollowAsync("u1", "u2");

        Assert.True(result);
    }

    #endregion

    #region GetFollowListAsync Tests

    [Fact]
    public async Task GetFollowListAsync_ShouldReturnFollowList()
    {
        var follows = new List<Follow>
        {
            new Follow { Id = "f1", UserId = "u1", FollowUserId = "u2", CreateTime = DateTime.Now }
        };
        var user = new User { Id = "u2", Nickname = "user2", Gender = "male", IsMember = false, Status = "active", CreateTime = DateTime.Now };

        _followRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Follow, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Follow, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((follows, 1));
        _userRepoMock.Setup(r => r.GetByIdAsync("u2")).ReturnsAsync(user);

        var result = await _followService.GetFollowListAsync("u1", new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
    }

    #endregion

    #region GetFansListAsync Tests

    [Fact]
    public async Task GetFansListAsync_ShouldReturnFans()
    {
        var follows = new List<Follow>
        {
            new Follow { Id = "f1", UserId = "u2", FollowUserId = "u1", CreateTime = DateTime.Now }
        };
        var user = new User { Id = "u2", Nickname = "fan1", Gender = "male", IsMember = false, Status = "active", CreateTime = DateTime.Now };

        _followRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Follow, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Follow, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((follows, 1));
        _userRepoMock.Setup(r => r.GetByIdAsync("u2")).ReturnsAsync(user);

        var result = await _followService.GetFansListAsync("u1", new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
    }

    #endregion
}