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
    private readonly Mock<IRepository<Role>> _roleRepoMock;
    private readonly AdminUserService _adminUserService;

    public AdminUserServiceTests()
    {
        _adminUserRepoMock = new Mock<IRepository<AdminUser>>();
        _roleRepoMock = new Mock<IRepository<Role>>();
        _adminUserService = new AdminUserService(_adminUserRepoMock.Object, _roleRepoMock.Object);
    }

    #region GetListAsync Tests

    [Fact]
    public async Task GetListAsync_ShouldReturnPagedResult()
    {
        var users = new List<AdminUser>
        {
            new AdminUser { Id = 1, Username = "admin", Nickname = "管理员", Status = 1, RoleId = 1, CreateTime = DateTime.Now }
        };
        var role = new Role { Id = 1, Name = "超级管理员" };
        _adminUserRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<AdminUser, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<AdminUser, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((users, 1));
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);

        var query = new AdminUserQuery { page = 1, page_size = 10 };
        var result = await _adminUserService.GetListAsync(query);

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
        Assert.Equal("超级管理员", result.List[0].RoleName);
    }

    [Fact]
    public async Task GetListAsync_ShouldFilterByRoleId()
    {
        Expression<Func<AdminUser, bool>>? capturedExpr = null;
        _adminUserRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<AdminUser, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<AdminUser, object>>>(),
                It.IsAny<bool>()))
            .Callback<Expression<Func<AdminUser, bool>>, int, int, Expression<Func<AdminUser, object>>, bool>(
                (expr, _, _, _, _) => capturedExpr = expr)
            .ReturnsAsync((new List<AdminUser>(), 0));
        _roleRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Role?)null);

        var query = new AdminUserQuery { page = 1, page_size = 10, role_id = 2 };
        var result = await _adminUserService.GetListAsync(query);

        Assert.Equal(0, result.Total);
        Assert.NotNull(capturedExpr);
    }

    [Fact]
    public async Task GetListAsync_ShouldFilterByStatus()
    {
        _adminUserRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<AdminUser, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<AdminUser, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((new List<AdminUser>(), 0));

        var query = new AdminUserQuery { page = 1, page_size = 10, status = "0" };
        var result = await _adminUserService.GetListAsync(query);

        Assert.Equal(0, result.Total);
    }

    #endregion

    #region GetDetailAsync Tests

    [Fact]
    public async Task GetDetailAsync_ShouldReturnDetail()
    {
        var user = new AdminUser
        {
            Id = 1, Username = "admin", Nickname = "管理员", Status = 1,
            RoleId = 1, CreateTime = DateTime.Now
        };
        var role = new Role { Id = 1, Name = "超级管理员" };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(user);
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);

        var result = await _adminUserService.GetDetailAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("admin", result.Username);
        Assert.Equal("超级管理员", result.RoleName);
    }

    [Fact]
    public async Task GetDetailAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _adminUserService.GetDetailAsync(999));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldCreateUser()
    {
        _adminUserRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<AdminUser, bool>>>())).ReturnsAsync(false);
        _adminUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AdminUser>()))
            .Callback<AdminUser>(u => u.Id = 1L)
            .ReturnsAsync(1L);

        var request = new CreateAdminUserRequest
        {
            Username = "newadmin",
            Password = "password123",
            Nickname = "新管理员",
            RoleId = 1,
            Status = 1
        };
        var result = await _adminUserService.CreateAsync(request);

        Assert.Equal(1L, result);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUsernameExists()
    {
        _adminUserRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<AdminUser, bool>>>())).ReturnsAsync(true);

        var request = new CreateAdminUserRequest
        {
            Username = "admin", Password = "password123", Nickname = "重复", RoleId = 1
        };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminUserService.CreateAsync(request));
        Assert.Contains("已存在", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldHashPassword()
    {
        _adminUserRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<AdminUser, bool>>>())).ReturnsAsync(false);
        AdminUser? captured = null;
        _adminUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AdminUser>()))
            .Callback<AdminUser>(u => captured = u)
            .ReturnsAsync(1L);

        var request = new CreateAdminUserRequest
        {
            Username = "newadmin", Password = "plain_password", RoleId = 1
        };
        await _adminUserService.CreateAsync(request);

        Assert.NotNull(captured);
        Assert.NotEqual("plain_password", captured.Password);
        Assert.True(BCrypt.Net.BCrypt.Verify("plain_password", captured.Password));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        var user = new AdminUser { Id = 1, Username = "admin", Nickname = "old", RoleId = 1, Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);

        var request = new UpdateAdminUserRequest { Nickname = "new", RoleId = 2, Status = 0 };
        var result = await _adminUserService.UpdateAsync(1, request);

        Assert.True(result);
        Assert.Equal("new", user.Nickname);
        Assert.Equal(2, user.RoleId);
        Assert.Equal(0, user.Status);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((AdminUser?)null);

        var request = new UpdateAdminUserRequest { Nickname = "new" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _adminUserService.UpdateAsync(999, request));
        Assert.Contains("不存在", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldKeepOriginalValues_WhenFieldsAreNull()
    {
        var user = new AdminUser { Id = 1, Username = "admin", Nickname = "old", RoleId = 1, Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);

        var request = new UpdateAdminUserRequest(); // 全部为 null
        await _adminUserService.UpdateAsync(1, request);

        Assert.Equal("old", user.Nickname);
        Assert.Equal(1, user.RoleId);
        Assert.Equal(1, user.Status);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser()
    {
        var user = new AdminUser { Id = 1, Username = "admin", Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.DeleteAsync(1L)).ReturnsAsync(true);

        var result = await _adminUserService.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _adminUserService.DeleteAsync(999));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region ResetPasswordAsync Tests

    [Fact]
    public async Task ResetPasswordAsync_ShouldResetPassword()
    {
        var user = new AdminUser { Id = 1, Username = "admin", Password = "old_hash", Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);

        var result = await _adminUserService.ResetPasswordAsync(1, "newpassword");

        Assert.True(result);
        Assert.NotEqual("old_hash", user.Password);
        Assert.True(BCrypt.Net.BCrypt.Verify("newpassword", user.Password));
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _adminUserService.ResetPasswordAsync(999, "newpassword"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region ChangePasswordAsync Tests

    [Fact]
    public async Task ChangePasswordAsync_ShouldChangePassword()
    {
        var oldHash = BCrypt.Net.BCrypt.HashPassword("oldpassword");
        var user = new AdminUser { Id = 1, Username = "admin", Password = oldHash, Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);

        var result = await _adminUserService.ChangePasswordAsync(1, "oldpassword", "newpassword");

        Assert.True(result);
        Assert.NotEqual(oldHash, user.Password);
        Assert.True(BCrypt.Net.BCrypt.Verify("newpassword", user.Password));
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldThrow_WhenOldPasswordWrong()
    {
        var oldHash = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new AdminUser { Id = 1, Username = "admin", Password = oldHash, Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _adminUserService.ChangePasswordAsync(1, "wrongpassword", "newpassword"));
        Assert.Contains("密码错误", ex.Message);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _adminUserService.ChangePasswordAsync(999, "old", "new"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region UpdateStatusAsync Tests

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus()
    {
        var user = new AdminUser { Id = 1, Username = "admin", Status = 1 };
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(user);
        _adminUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminUser>())).ReturnsAsync(true);

        var result = await _adminUserService.UpdateStatusAsync(1, 0);

        Assert.True(result);
        Assert.Equal(0, user.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrow_WhenNotFound()
    {
        _adminUserRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((AdminUser?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _adminUserService.UpdateStatusAsync(999, 0));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion
}
