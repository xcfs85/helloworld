using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Admin;
using Pindou.Domain.Entities.Admin;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Admin;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class AdminUserServiceTests
{
    private readonly Mock<IRepository<AdminUser>> _adminUserRepoMock;
    private readonly Mock<IRepository<AdminRole>> _roleRepoMock;
    private readonly AdminUserService _adminUserService;

    public AdminUserServiceTests()
    {
        _adminUserRepoMock = new Mock<IRepository<AdminUser>>();
        _roleRepoMock = new Mock<IRepository<AdminRole>>();
        _adminUserService = new AdminUserService(_adminUserRepoMock.Object, _roleRepoMock.Object);
    }

    #region GetListAsync Tests

    [Fact]
    public async Task GetListAsync_ShouldReturnPagedResult()
    {
        var users = new List<AdminUser>
        {
            new AdminUser { Id = "a1", Username = "admin", Nickname = "管理员", Status = 1, RoleId = "r1", IsAdmin = true, CreateTime = DateTime.Now }
        };
        var role = new AdminRole { Id = "r1", Name = "超级管理员" };
        _adminUserRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<AdminUser, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<AdminUser, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((users, 1));
        _roleRepoMock.Setup(r => r.GetByIdAsync("r1")).ReturnsAsync(role);

        var query = new AdminUserQuery { Page = 1, Size = 10 };
        var result = await _adminUserService.GetListAsync(query);

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
        Assert.Equal("超级管理员", result.List[0].RoleName);
    }

    #endregion

    #region GetDetailAsync Tests

    [Fact]
    public async Task GetDetailAsync_ShouldReturnDetail()
    {
        var user = new AdminUser
        {
            Id = "a1", Username = "admin", Nickname = "管理员", Status = 1,
            RoleId = "r1", IsAdmin = true, CreateTime = DateTime.Now
        };
        var role = new AdminRole { Id = "r1", Name = "超级管理员" };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("a1")).ReturnsAsync(user);
        _roleRepoMock.Setup(r => r.GetByIdAsync("r1")).ReturnsAsync(role);

        var result = await _adminUserService.GetDetailAsync("a1");

        Assert.NotNull(result);
        Assert.Equal("a1", result.Id);
        Assert.Equal("admin", result.Username);
        Assert.Equal("超级管理员", result.RoleName);
    }

    [Fact]
    public async Task GetDetailAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _adminUserService.GetDetailAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldCreateUser()
    {
        _adminUserRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<AdminUser, bool>>>())).ReturnsAsync(false);
        _adminUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AdminUser>())).ReturnsAsync("a1");

        var request = new CreateAdminUserRequest
        {
            Username = "newadmin", Password = "password123", Nickname = "新管理员",
            RoleId = "r1", IsAdmin = false
        };
        var result = await _adminUserService.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("a1", result);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUsernameExists()
    {
        _adminUserRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<AdminUser, bool>>>())).ReturnsAsync(true);

        var request = new CreateAdminUserRequest
        {
            Username = "admin", Password = "password123", Nickname = "重复", RoleId = "r1"
        };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminUserService.CreateAsync(request));
        Assert.Contains("已存在", ex.Message);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        var user = new AdminUser { Id = "a1", Username = "admin", Nickname = "old", RoleId = "r1", Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("a1")).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);

        var request = new UpdateAdminUserRequest { Nickname = "new", RoleId = "r2", Status = 1 };
        var result = await _adminUserService.UpdateAsync("a1", request);

        Assert.True(result);
        Assert.Equal("new", user.Nickname);
        Assert.Equal("r2", user.RoleId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((AdminUser?)null);

        var request = new UpdateAdminUserRequest { Nickname = "new" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminUserService.UpdateAsync("nonexistent", request));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldSoftDelete()
    {
        var user = new AdminUser { Id = "a1", Username = "admin", Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("a1")).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);

        var result = await _adminUserService.DeleteAsync("a1");

        Assert.True(result);
        Assert.Equal(0, user.Status);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _adminUserService.DeleteAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region ResetPasswordAsync Tests

    [Fact]
    public async Task ResetPasswordAsync_ShouldResetPassword()
    {
        var user = new AdminUser { Id = "a1", Username = "admin", PasswordHash = "old_hash", Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("a1")).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);

        var result = await _adminUserService.ResetPasswordAsync("a1", "newpassword", "r1");

        Assert.True(result);
        Assert.NotEqual("old_hash", user.PasswordHash);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _adminUserService.ResetPasswordAsync("nonexistent", "newpassword", "r1"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region ChangePasswordAsync Tests

    [Fact]
    public async Task ChangePasswordAsync_ShouldChangePassword()
    {
        var oldHash = BCrypt.Net.BCrypt.HashPassword("oldpassword");
        var user = new AdminUser { Id = "a1", Username = "admin", PasswordHash = oldHash, Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("a1")).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);

        var result = await _adminUserService.ChangePasswordAsync("a1", "oldpassword", "newpassword");

        Assert.True(result);
        Assert.NotEqual(oldHash, user.PasswordHash);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldThrow_WhenOldPasswordWrong()
    {
        var oldHash = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new AdminUser { Id = "a1", Username = "admin", PasswordHash = oldHash, Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("a1")).ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _adminUserService.ChangePasswordAsync("a1", "wrongpassword", "newpassword"));
        Assert.Contains("密码错误", ex.Message);
    }

    #endregion

    #region UpdateStatusAsync Tests

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus()
    {
        var user = new AdminUser { Id = "a1", Username = "admin", Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("a1")).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);

        var result = await _adminUserService.UpdateStatusAsync("a1", 0, "r1");

        Assert.True(result);
        Assert.Equal(0, user.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _adminUserService.UpdateStatusAsync("nonexistent", 0, "r1"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion
}