using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Member;
using Pindou.Domain.Entities.Member;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Member;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class UserMemberServiceTests
{
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly Mock<IRepository<Member>> _memberRepoMock;
    private readonly Mock<IRepository<MemberProduct>> _productRepoMock;
    private readonly UserMemberService _userMemberService;

    public UserMemberServiceTests()
    {
        _userRepoMock = new Mock<IRepository<User>>();
        _memberRepoMock = new Mock<IRepository<Member>>();
        _productRepoMock = new Mock<IRepository<MemberProduct>>();
        _userMemberService = new UserMemberService(_userRepoMock.Object, _memberRepoMock.Object, _productRepoMock.Object);
    }

    #region GetMemberStatusAsync Tests

    [Fact]
    public async Task GetMemberStatusAsync_ShouldReturnStatus_WhenMember()
    {
        var user = new User { Id = "u1", IsMember = true, MemberExpireTime = DateTime.Now.AddDays(15) };
        var product = new MemberProduct { Id = "p1", ProductId = "prod_month", DailyGenerations = 10, Status = 1 };

        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _productRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MemberProduct, bool>>>()))
            .ReturnsAsync(product);

        var result = await _userMemberService.GetMemberStatusAsync("u1");

        Assert.NotNull(result);
        Assert.True(result.IsMember);
        Assert.Equal(10, result.DailyGenerations);
    }

    [Fact]
    public async Task GetMemberStatusAsync_ShouldReturnNotMember_WhenExpired()
    {
        var user = new User { Id = "u1", IsMember = true, MemberExpireTime = DateTime.Now.AddDays(-1) };
        var product = new MemberProduct { Id = "p1", ProductId = "prod_month", DailyGenerations = 3, Status = 1 };

        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _productRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MemberProduct, bool>>>()))
            .ReturnsAsync(product);

        var result = await _userMemberService.GetMemberStatusAsync("u1");

        Assert.NotNull(result);
        Assert.False(result.IsMember);
        Assert.Equal(0, result.RemainingDays);
    }

    [Fact]
    public async Task GetMemberStatusAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _userMemberService.GetMemberStatusAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetMemberRecordsAsync Tests

    [Fact]
    public async Task GetMemberRecordsAsync_ShouldReturnRecords()
    {
        var records = new List<Member>
        {
            new Member
            {
                Id = "m1", UserId = "u1", MemberType = "month",
                StartTime = DateTime.Now.AddDays(-10), ExpireTime = DateTime.Now.AddDays(20),
                CreateTime = DateTime.Now.AddDays(-10)
            }
        };
        _memberRepoMock.Setup(r => r.GetListAsync(
                It.IsAny<Expression<Func<Member, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .ReturnsAsync(records);

        var result = await _userMemberService.GetMemberRecordsAsync("u1");

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("month", result[0].MemberType);
    }

    [Fact]
    public async Task GetMemberRecordsAsync_ShouldReturnEmpty_WhenNoRecords()
    {
        _memberRepoMock.Setup(r => r.GetListAsync(
                It.IsAny<Expression<Func<Member, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .ReturnsAsync(new List<Member>());

        var result = await _userMemberService.GetMemberRecordsAsync("u1");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion
}