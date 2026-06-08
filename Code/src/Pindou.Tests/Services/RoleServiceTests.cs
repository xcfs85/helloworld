using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Admin;
using Pindou.Domain.Entities.Admin;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Admin;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class RoleServiceTests
{
    private readonly Mock<IRepository<Role>> _roleRepoMock;
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        _roleRepoMock = new Mock<IRepository<Role>>();
        _roleService = new RoleService(_roleRepoMock.Object);
    }

    #region GetListAsync Tests

    [Fact]
    public async Task GetListAsync_ShouldReturnPagedRoles()
    {
        var roles = new List<Role>
        {
            new Role { Id = 1, Name = "超级管理员", Code = "super_admin", Description = "全部权限", Permissions = "[\"*\"]" }
        };
        _roleRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Role, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Role, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((roles, 1));

        var result = await _roleService.GetListAsync(new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
        Assert.Equal("超级管理员", result.List[0].Name);
    }

    [Fact]
    public async Task GetListAsync_ShouldDeserializePermissionsJson()
    {
        var roles = new List<Role>
        {
            new Role { Id = 1, Name = "编辑", Code = "editor", Permissions = "[\"post:edit\",\"post:list\"]" }
        };
        _roleRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Role, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Role, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((roles, 1));

        var result = await _roleService.GetListAsync(new PageRequest { Page = 1, Size = 10 });

        Assert.Equal(2, result.List[0].Permissions.Count);
        Assert.Contains("post:edit", result.List[0].Permissions);
    }

    [Fact]
    public async Task GetListAsync_ShouldHandleInvalidPermissionsJson()
    {
        var roles = new List<Role>
        {
            new Role { Id = 1, Name = "角色", Code = "test", Permissions = "invalid-json" }
        };
        _roleRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Role, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Role, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((roles, 1));

        var result = await _roleService.GetListAsync(new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Empty(result.List[0].Permissions);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRoles()
    {
        var roles = new List<Role>
        {
            new Role { Id = 1, Name = "超级管理员", Code = "super_admin" },
            new Role { Id = 2, Name = "编辑", Code = "editor" }
        };
        _roleRepoMock.Setup(r => r.GetListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>())).ReturnsAsync(roles);

        var result = await _roleService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region GetDetailAsync Tests

    [Fact]
    public async Task GetDetailAsync_ShouldReturnRoleDetail()
    {
        var role = new Role { Id = 1, Name = "超级管理员", Code = "super_admin", Permissions = "[\"user:list\"]" };
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);

        var result = await _roleService.GetDetailAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("超级管理员", result.Name);
        Assert.Single(result.Permissions);
    }

    [Fact]
    public async Task GetDetailAsync_ShouldThrow_WhenNotFound()
    {
        _roleRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((Role?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _roleService.GetDetailAsync(999));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldCreateRole()
    {
        _roleRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Role, bool>>>())).ReturnsAsync(false);
        _roleRepoMock.Setup(r => r.InsertAsync(It.IsAny<Role>())).ReturnsAsync(1L);

        var request = new CreateRoleRequest
        {
            Name = "新角色",
            Code = "new_role",
            Description = "测试",
            Permissions = new List<string> { "p1", "p2" }
        };
        var result = await _roleService.CreateAsync(request);

        Assert.Equal(1L, result);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCodeExists()
    {
        _roleRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Role, bool>>>())).ReturnsAsync(true);

        var request = new CreateRoleRequest { Name = "重复", Code = "duplicate_code" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _roleService.CreateAsync(request));
        Assert.Contains("已存在", ex.Message);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateRole()
    {
        var role = new Role { Id = 1, Name = "old", Code = "old", Permissions = "[]" };
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);
        _roleRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Role>())).ReturnsAsync(true);

        var request = new CreateRoleRequest
        {
            Name = "new",
            Code = "new_code",
            Permissions = new List<string> { "x" }
        };
        var result = await _roleService.UpdateAsync(1, request);

        Assert.True(result);
        Assert.Equal("new", role.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenNotFound()
    {
        _roleRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((Role?)null);

        var request = new CreateRoleRequest { Name = "x", Code = "x" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _roleService.UpdateAsync(999, request));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldDeleteRole()
    {
        var role = new Role { Id = 1, Name = "x", Code = "x" };
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);
        _roleRepoMock.Setup(r => r.DeleteAsync(1L)).ReturnsAsync(true);

        var result = await _roleService.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        _roleRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((Role?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _roleService.DeleteAsync(999));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetPermissionsAsync Tests

    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnPermissions()
    {
        var role = new Role { Id = 1, Name = "x", Code = "x", Permissions = "[\"a\",\"b\",\"c\"]" };
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);

        var result = await _roleService.GetPermissionsAsync(1);

        Assert.Equal(3, result.Count);
        Assert.Contains("a", result);
    }

    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnEmpty_WhenRoleNotFound()
    {
        _roleRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((Role?)null);

        var result = await _roleService.GetPermissionsAsync(999);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnEmpty_WhenPermissionsIsNull()
    {
        var role = new Role { Id = 1, Name = "x", Code = "x", Permissions = "" };
        _roleRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(role);

        var result = await _roleService.GetPermissionsAsync(1);

        Assert.Empty(result);
    }

    #endregion
}
