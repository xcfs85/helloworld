using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Member;
using Pindou.Domain.Entities.Member;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Member;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class MemberServiceTests
{
    private readonly Mock<IRepository<Member>> _memberRepoMock;
    private readonly Mock<IRepository<Order>> _orderRepoMock;
    private readonly Mock<IRepository<MemberProduct>> _productRepoMock;
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly MemberService _memberService;

    public MemberServiceTests()
    {
        _memberRepoMock = new Mock<IRepository<Member>>();
        _orderRepoMock = new Mock<IRepository<Order>>();
        _productRepoMock = new Mock<IRepository<MemberProduct>>();
        _userRepoMock = new Mock<IRepository<User>>();
        _memberService = new MemberService(_memberRepoMock.Object, _orderRepoMock.Object, _productRepoMock.Object, _userRepoMock.Object);
    }

    #region GetProductsAsync Tests

    [Fact]
    public async Task GetProductsAsync_ShouldReturnProducts()
    {
        var products = new List<MemberProduct>
        {
            new MemberProduct { Id = "p1", ProductId = "prod_month", ProductName = "月度会员", Grade = "month", DurationDays = 30, Price = 29.9m, DailyGenerations = 10, Status = 1 }
        };
        _productRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<MemberProduct, bool>>>()))
            .ReturnsAsync(products);

        var result = await _memberService.GetProductsAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("月度会员", result[0].ProductName);
        Assert.Equal(29.9m, result[0].Price);
    }

    #endregion

    #region CreateOrderAsync Tests

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrder()
    {
        var product = new MemberProduct { Id = "p1", ProductId = "prod_month", ProductName = "月度会员", Price = 29.9m, Status = 1 };
        _productRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MemberProduct, bool>>>()))
            .ReturnsAsync(product);
        _orderRepoMock.Setup(r => r.InsertAsync(It.IsAny<Order>())).ReturnsAsync("o1");

        var request = new CreateOrderRequest { ProductId = "prod_month", ProductType = "member" };
        var result = await _memberService.CreateOrderAsync("u1", request);

        Assert.NotNull(result);
        Assert.Equal("o1", result);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrow_WhenProductNotFound()
    {
        _productRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MemberProduct, bool>>>()))
            .ReturnsAsync((MemberProduct?)null);

        var request = new CreateOrderRequest { ProductId = "invalid", ProductType = "member" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _memberService.CreateOrderAsync("u1", request));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetOrderAsync Tests

    [Fact]
    public async Task GetOrderAsync_ShouldReturnOrder()
    {
        var order = new Order
        {
            Id = "o1", UserId = "u1", OrderNo = "20260101000001", ProductType = "member",
            ProductId = "prod_month", Amount = 29.9m, Status = "pending", CreateTime = DateTime.Now
        };
        var product = new MemberProduct { Id = "p1", ProductId = "prod_month", ProductName = "月度会员" };
        _orderRepoMock.Setup(r => r.GetByIdAsync("o1")).ReturnsAsync(order);
        _productRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MemberProduct, bool>>>()))
            .ReturnsAsync(product);

        var result = await _memberService.GetOrderAsync("u1", "o1");

        Assert.NotNull(result);
        Assert.Equal("o1", result.Id);
        Assert.Equal("月度会员", result.ProductName);
    }

    [Fact]
    public async Task GetOrderAsync_ShouldThrow_WhenNotOwner()
    {
        var order = new Order { Id = "o1", UserId = "u2", OrderNo = "NO1", ProductType = "member", ProductId = "p1", Amount = 29.9m, Status = "pending" };
        _orderRepoMock.Setup(r => r.GetByIdAsync("o1")).ReturnsAsync(order);

        var ex = await Assert.ThrowsAsync<BizException>(() => _memberService.GetOrderAsync("u1", "o1"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region PayOrderAsync Tests

    [Fact]
    public async Task PayOrderAsync_ShouldPayAndOpenMember()
    {
        var order = new Order { Id = "o1", UserId = "u1", OrderNo = "NO1", ProductType = "member", ProductId = "prod_month", Amount = 29.9m, Status = "pending" };
        var product = new MemberProduct { Id = "p1", ProductId = "prod_month", ProductName = "月度会员", Grade = "month", DurationDays = 30, Price = 29.9m };
        var user = new User { Id = "u1", IsMember = false, Nickname = "test" };

        _orderRepoMock.Setup(r => r.GetByIdAsync("o1")).ReturnsAsync(order);
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(true);
        _productRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MemberProduct, bool>>>()))
            .ReturnsAsync(product);
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _memberRepoMock.Setup(r => r.InsertAsync(It.IsAny<Member>())).ReturnsAsync("m1");
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

        var result = await _memberService.PayOrderAsync("u1", "o1", "wechat");

        Assert.True(result);
        Assert.Equal("paid", order.Status);
        Assert.True(user.IsMember);
    }

    [Fact]
    public async Task PayOrderAsync_ShouldThrow_WhenOrderNotPending()
    {
        var order = new Order { Id = "o1", UserId = "u1", OrderNo = "NO1", ProductType = "member", ProductId = "p1", Amount = 29.9m, Status = "paid" };
        _orderRepoMock.Setup(r => r.GetByIdAsync("o1")).ReturnsAsync(order);

        var ex = await Assert.ThrowsAsync<BizException>(() => _memberService.PayOrderAsync("u1", "o1", "wechat"));
        Assert.Contains("不可支付", ex.Message);
    }

    #endregion

    #region CancelOrderAsync Tests

    [Fact]
    public async Task CancelOrderAsync_ShouldCancelOrder()
    {
        var order = new Order { Id = "o1", UserId = "u1", OrderNo = "NO1", ProductType = "member", ProductId = "p1", Amount = 29.9m, Status = "pending" };
        _orderRepoMock.Setup(r => r.GetByIdAsync("o1")).ReturnsAsync(order);
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(true);

        var result = await _memberService.CancelOrderAsync("u1", "o1");

        Assert.True(result);
        Assert.Equal("canceled", order.Status);
    }

    [Fact]
    public async Task CancelOrderAsync_ShouldThrow_WhenOrderNotPending()
    {
        var order = new Order { Id = "o1", UserId = "u1", OrderNo = "NO1", ProductType = "member", ProductId = "p1", Amount = 29.9m, Status = "paid" };
        _orderRepoMock.Setup(r => r.GetByIdAsync("o1")).ReturnsAsync(order);

        var ex = await Assert.ThrowsAsync<BizException>(() => _memberService.CancelOrderAsync("u1", "o1"));
        Assert.Contains("不可取消", ex.Message);
    }

    #endregion

    #region OpenMemberAsync Tests

    [Fact]
    public async Task OpenMemberAsync_ShouldOpenMember()
    {
        var user = new User { Id = "u1", IsMember = false, Nickname = "test" };
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _memberRepoMock.Setup(r => r.InsertAsync(It.IsAny<Member>())).ReturnsAsync("m1");
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

        var request = new OpenMemberRequest { UserId = "u1", MemberType = "month", DurationDays = 30 };
        var result = await _memberService.OpenMemberAsync("u1", request, 1);

        Assert.True(result);
        Assert.True(user.IsMember);
    }

    [Fact]
    public async Task OpenMemberAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var request = new OpenMemberRequest { UserId = "nonexistent", MemberType = "month", DurationDays = 30 };
        var ex = await Assert.ThrowsAsync<BizException>(() => _memberService.OpenMemberAsync("nonexistent", request, 1));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetOrdersAsync Tests

    [Fact]
    public async Task GetOrdersAsync_ShouldReturnOrders()
    {
        var orders = new List<Order>
        {
            new Order { Id = "o1", UserId = "u1", OrderNo = "NO1", ProductType = "member", ProductId = "p1", Amount = 29.9m, Status = "paid", CreateTime = DateTime.Now }
        };
        var product = new MemberProduct { Id = "p1", ProductId = "p1", ProductName = "月度会员" };
        _orderRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Order, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((orders, 1));
        _productRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MemberProduct, bool>>>()))
            .ReturnsAsync(product);

        var result = await _memberService.GetOrdersAsync("u1", new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
    }

    #endregion

    #region AdminListOrdersAsync Tests

    [Fact]
    public async Task AdminListOrdersAsync_ShouldReturnOrders()
    {
        var orders = new List<Order>
        {
            new Order { Id = "o1", UserId = "u1", OrderNo = "NO1", ProductType = "member", ProductId = "p1", Amount = 29.9m, Status = "paid", CreateTime = DateTime.Now }
        };
        var product = new MemberProduct { Id = "p1", ProductId = "p1", ProductName = "月度会员" };
        _orderRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Order, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((orders, 1));
        _productRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MemberProduct, bool>>>()))
            .ReturnsAsync(product);

        var query = new OrderQuery { Page = 1, Size = 10 };
        var result = await _memberService.AdminListOrdersAsync(query);

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
    }

    #endregion
}