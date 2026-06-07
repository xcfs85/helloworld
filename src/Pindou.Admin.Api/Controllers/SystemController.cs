using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Admin;
using Pindou.Application.Interfaces.Admin;
using Pindou.Shared.Attributes;

namespace Pindou.Admin.Api.Controllers;

[ApiController]
[Route("api/admin/v1/admin")]
[Permission("admin:view")]
public class AdminController : ControllerBase
{
    private readonly IAdminUserService _adminService;
    public AdminController(IAdminUserService adminService) { _adminService = adminService; }

    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<AdminUserListDto>>> List([FromQuery] AdminUserQuery query)
    {
        var data = await _adminService.GetListAsync(query);
        return ApiResponse<PagedResult<AdminUserListDto>>.Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<AdminUserDetailDto>> Detail(long id)
    {
        var data = await _adminService.GetDetailAsync(id);
        return ApiResponse<AdminUserDetailDto>.Ok(data);
    }

    [HttpPost]
    [Permission("admin:add")]
    [OperationLog("新增管理员", SaveParams = true)]
    public async Task<ApiResponse<long>> Create([FromBody] CreateAdminUserRequest request)
    {
        var id = await _adminService.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    [HttpPut("{id}")]
    [Permission("admin:edit")]
    [OperationLog("修改管理员", SaveParams = true)]
    public async Task<ApiResponse> Update(long id, [FromBody] UpdateAdminUserRequest request)
    {
        await _adminService.UpdateAsync(id, request);
        return ApiResponse.Ok();
    }

    [HttpDelete("{id}")]
    [Permission("admin:delete")]
    [OperationLog("删除管理员")]
    public async Task<ApiResponse> Delete(long id)
    {
        await _adminService.DeleteAsync(id);
        return ApiResponse.Ok();
    }

    [HttpPost("{id}/reset-password")]
    [Permission("admin:reset-password")]
    [OperationLog("重置密码")]
    public async Task<ApiResponse> ResetPassword(long id, [FromBody] ResetPasswordRequest request)
    {
        await _adminService.ResetPasswordAsync(id, request.NewPassword);
        return ApiResponse.Ok();
    }

    [HttpPost("{id}/status")]
    [Permission("admin:status")]
    [OperationLog("修改状态")]
    public async Task<ApiResponse> UpdateStatus(long id, [FromBody] UpdateStatusRequest request)
    {
        await _adminService.UpdateStatusAsync(id, request.Status);
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/admin/v1/role")]
[Permission("role:view")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    public RoleController(IRoleService roleService) { _roleService = roleService; }

    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<RoleDto>>> List([FromQuery] PageRequest request)
    {
        var data = await _roleService.GetListAsync(request);
        return ApiResponse<PagedResult<RoleDto>>.Ok(data);
    }

    [HttpGet("all")]
    public async Task<ApiResponse<List<RoleDto>>> All()
    {
        var data = await _roleService.GetAllAsync();
        return ApiResponse<List<RoleDto>>.Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<RoleDto>> Detail(long id)
    {
        var data = await _roleService.GetDetailAsync(id);
        return ApiResponse<RoleDto>.Ok(data);
    }

    [HttpPost]
    [Permission("role:add")]
    [OperationLog("新增角色", SaveParams = true)]
    public async Task<ApiResponse<long>> Create([FromBody] CreateRoleRequest request)
    {
        var id = await _roleService.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    [HttpPut("{id}")]
    [Permission("role:edit")]
    [OperationLog("修改角色", SaveParams = true)]
    public async Task<ApiResponse> Update(long id, [FromBody] CreateRoleRequest request)
    {
        await _roleService.UpdateAsync(id, request);
        return ApiResponse.Ok();
    }

    [HttpDelete("{id}")]
    [Permission("role:delete")]
    [OperationLog("删除角色")]
    public async Task<ApiResponse> Delete(long id)
    {
        await _roleService.DeleteAsync(id);
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/admin/v1/log")]
[Permission("log:view")]
public class OperationLogController : ControllerBase
{
    private readonly IOperationLogService _logService;
    public OperationLogController(IOperationLogService logService) { _logService = logService; }

    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<OperationLogDto>>> List([FromQuery] OperationLogQuery query)
    {
        var data = await _logService.GetListAsync(query);
        return ApiResponse<PagedResult<OperationLogDto>>.Ok(data);
    }

    [HttpDelete("{id}")]
    [Permission("log:delete")]
    public async Task<ApiResponse> Delete(long id)
    {
        await _logService.DeleteAsync(id);
        return ApiResponse.Ok();
    }

    [HttpPost("clear")]
    [Permission("log:clear")]
    public async Task<ApiResponse> Clear([FromQuery] DateTime? beforeTime)
    {
        await _logService.ClearAsync(beforeTime);
        return ApiResponse.Ok();
    }
}

public class ResetPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}

public class UpdateStatusRequest
{
    public int Status { get; set; } = 1;
}
