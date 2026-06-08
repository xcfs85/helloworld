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
    private readonly Mock<IRepository<AdminRole>> _roleRepoMock;
    private readonly Mock<IRepository<AdminPermission>> _permissionRepoMock;
    private readonly Mock<IRepository<AdminRolePermission>> _rolePermissionRepoMock;
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        _roleRepoMock = new Mock<IRepository<AdminRole>>();
        _permissionRepoMock = new Mock<IRepository<AdminPermission>>();
        _rolePermissionRepoMock = new Mock<IRepository<AdminRolePermission>>();
        _roleService = new RoleService(_roleRepoMock.Object, _permissionRepoMock.Object, _rolePermissionRepoMock.Object);
    }

    #region GetListAsync Tests

    [Fact]
    public async Task GetListAsync_ShouldReturnPagedRoles()
    {
        var roles = new List<AdminRole>
        {
            new AdminRole { Id = "r1", Name = "超级管理员", Status = 1, CreateTime = DateTime.Now }
        };
        _roleRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<AdminRole, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<AdminRole, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((roles, 1));
        _rolePermissionRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<AdminRolePermission, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<AdminRolePermission>());

        var query = new RoleQuery { Page = 1, Size = 10 };
        var result = await _roleService.GetListAsync(query);

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRoles()
    {
        var roles = new List<AdminRole>
        {
            new AdminRole { Id = "r1", Name = "管理员", Status = 1 },
            new AdminRole { Id = "r2", Name = "编辑", Status = 1 }
        };
        _roleRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<AdminRole, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(roles);

        var result = await _roleService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region GetDetailAsync Tests

    [Fact]
    public async Task GetDetailAsync_ShouldReturnRoleDetail()
    {
        var role = new AdminRole { Id = "r1", Name = "超级管理员", Status = 1 };
        _roleRepoMock.Setup(r => r.GetByIdAsync("r1")).ReturnsAsync(role);
        _rolePermissionRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<AdminRolePermission, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<AdminRolePermission>
            {
                new AdminRolePermission { Id = "rp1", RoleId = "r1", PermissionId = "p1" }
            });
        _permissionRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<AdminPermission, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<AdminPermission>
            {
                new AdminPermission { Id = "p1", PermissionCode = "user:list", PermissionName = "用户列表", Type = "menu", ParentId = "0" }
            });

        var result = await _roleService.GetDetailAsync("r1");

        Assert.NotNull(result);
        Assert.Equal("r1", result.Id);
        Assert.Single(result.PermissionIds);
    }

    [Fact]
    public async Task GetDetailAsync_ShouldThrow_WhenNotFound()
    {
        _roleRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((AdminRole?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _roleService.GetDetailAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldCreateRole()
    {
        _roleRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<AdminRole, bool>>>())).ReturnsAsync(false);
        _roleRepoMock.Setup(r => r.InsertAsync(It.IsAny<AdminRole>())).ReturnsAsync("r1");
        _rolePermissionRepoMock.Setup(r => r.InsertRangeAsync(It.IsAny<List<AdminRolePermission>>()))
            .ReturnsAsync(new List<object> { "rp1" });

        var request = new CreateRoleRequest
        {
            Name = "新角色", Status = 1,
            PermissionIds = new List<string> { "p1", "p2" }
        };
        var result = await _roleService.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("r1", result);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenNameExists()
    {
        _roleRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<AdminRole, bool>>>())).ReturnsAsync(true);

        var request = new CreateRoleRequest { Name = "重复角色", Status = 1 };
        var ex = await Assert.ThrowsAsync<BizException>(() => _roleService.CreateAsync(request));
        Assert.Contains("已存在", ex.Message);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateRole()
    {
        var role = new AdminRole { Id = "r1", Name = "old", Status = 1 };
        _roleRepoMock.Setup(r => r.GetByIdAsync("r1")).ReturnsAsync(role);
        _roleRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AdminRole>())).ReturnsAsync(true);
        _rolePermissionRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<AdminRolePermission, bool>>>()))
            .ReturnsAsync(true);
        _rolePermissionRepoMock.Setup(r => r.InsertRangeAsync(It.IsAny<List<AdminRolePermission>>()))
            .ReturnsAsync(new List<object> { "rp1" });

        var request = new CreateRoleRequest
        {
            Name = "new", Status = 1, PermissionIds = new List<string> { "p1" }
        };
        var result = await _roleService.UpdateAsync("r1", request);

        Assert.True(result);
        Assert.Equal("new", role.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenNotFound()
    {
        _roleRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((AdminRole?)null);

        var request = new CreateRoleRequest { Name = "new", Status = 1 };
        var ex = await Assert.ThrowsAsync<BizException>(() => _roleService.UpdateAsync("nonexistent", request));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldDeleteRole()
    {
        _roleRepoMock.Setup(r => r.DeleteAsync(It.IsAny<object>())).ReturnsAsync(true);
        _rolePermissionRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<AdminRolePermission, bool>>>()))
            .ReturnsAsync(true);

        var result = await _roleService.DeleteAsync("r1");

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        _roleRepoMock.Setup(r => r.DeleteAsync(It.IsAny<object>())).ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<BizException>(() => _roleService.DeleteAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetPermissionsAsync Tests

    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnAllPermissions()
    {
        _permissionRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<AdminPermission, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<AdminPermission>
            {
                new AdminPermission { Id = "p1", PermissionCode = "user:list", PermissionName = "用户列表", Type = "menu", ParentId = "0", Sort = 1 },
                new AdminPermission { Id = "p2", PermissionCode = "user:create", PermissionName = "创建用户", Type = "button", ParentId = "p1", Sort = 1 }
            });

        var result = await _roleService.GetPermissionsAsync();

        Assert.NotNull(result);
        // Should be organized as a tree
        Assert.NotEmpty(result);
    }

    #endregion
}