using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Admin;
using Pindou.Application.DTOs.System;
using Pindou.Application.Interfaces.Admin;
using Pindou.Application.Interfaces.System;
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

[ApiController]
[Route("api/admin/v1/config")]
[Permission("config:view")]
public class ConfigController : ControllerBase
{
    private readonly ISystemConfigService _configService;

    public ConfigController(ISystemConfigService configService)
    {
        _configService = configService;
    }

    /// <summary>所有配置</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<List<SystemConfigDto>>> List()
    {
        var data = await _configService.GetAllAsync();
        return ApiResponse<List<SystemConfigDto>>.Ok(data);
    }

    /// <summary>获取配置</summary>
    [HttpGet("{key}")]
    public async Task<ApiResponse<string?>> Get(string key)
    {
        var data = await _configService.GetAsync(key);
        return ApiResponse<string?>.Ok(data);
    }

    /// <summary>设置配置</summary>
    [HttpPut("{key}")]
    [Permission("config:edit")]
    [OperationLog("修改系统配置", SaveParams = true)]
    public async Task<ApiResponse> Set(string key, [FromBody] SetConfigRequest request)
    {
        await _configService.SetAsync(key, request.Value, request.Type, request.Description);
        return ApiResponse.Ok();
    }

    /// <summary>批量设置配置</summary>
    [HttpPost("batch")]
    [Permission("config:edit")]
    [OperationLog("批量设置系统配置")]
    public async Task<ApiResponse> BatchSet([FromBody] List<SetConfigRequest> requests)
    {
        foreach (var req in requests)
        {
            await _configService.SetAsync(req.Key, req.Value, req.Type, req.Description);
        }
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/admin/v1/mard-color")]
[Permission("mard-color:view")]
public class MardColorController : ControllerBase
{
    private readonly ISystemConfigService _configService;

    public MardColorController(ISystemConfigService configService)
    {
        _configService = configService;
    }

    /// <summary>MARD颜色列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<List<MardColorDto>>> List()
    {
        var data = await _configService.GetAllMardColorsAsync();
        return ApiResponse<List<MardColorDto>>.Ok(data);
    }

    /// <summary>添加颜色</summary>
    [HttpPost]
    [Permission("mard-color:add")]
    [OperationLog("添加MARD颜色", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateMardColorRequest request)
    {
        return ApiResponse<string>.Ok(string.Empty);
    }

    /// <summary>批量导入</summary>
    [HttpPost("batch-import")]
    [Permission("mard-color:import")]
    [OperationLog("批量导入MARD颜色")]
    public async Task<ApiResponse> BatchImport([FromBody] List<CreateMardColorRequest> requests)
    {
        return ApiResponse.Ok();
    }

    /// <summary>更新颜色</summary>
    [HttpPut("{id}")]
    [Permission("mard-color:edit")]
    [OperationLog("更新MARD颜色", SaveParams = true)]
    public async Task<ApiResponse> Update(string id, [FromBody] CreateMardColorRequest request)
    {
        return ApiResponse.Ok();
    }

    /// <summary>删除颜色</summary>
    [HttpDelete("{id}")]
    [Permission("mard-color:delete")]
    [OperationLog("删除MARD颜色")]
    public async Task<ApiResponse> Delete(string id)
    {
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/admin/v1/bead-kit")]
[Permission("bead-kit:view")]
public class BeadKitController : ControllerBase
{
    private readonly ISystemConfigService _configService;

    public BeadKitController(ISystemConfigService configService)
    {
        _configService = configService;
    }

    /// <summary>珠套列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<List<BeadKitDto>>> List([FromQuery] int? colorCount)
    {
        var data = await _configService.GetAllBeadKitsAsync(colorCount);
        return ApiResponse<List<BeadKitDto>>.Ok(data);
    }

    /// <summary>创建珠套</summary>
    [HttpPost]
    [Permission("bead-kit:add")]
    [OperationLog("创建珠套", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateBeadKitRequest request)
    {
        return ApiResponse<string>.Ok(string.Empty);
    }

    /// <summary>更新珠套</summary>
    [HttpPut("{id}")]
    [Permission("bead-kit:edit")]
    [OperationLog("更新珠套", SaveParams = true)]
    public async Task<ApiResponse> Update(string id, [FromBody] CreateBeadKitRequest request)
    {
        return ApiResponse.Ok();
    }

    /// <summary>删除珠套</summary>
    [HttpDelete("{id}")]
    [Permission("bead-kit:delete")]
    [OperationLog("删除珠套")]
    public async Task<ApiResponse> Delete(string id)
    {
        return ApiResponse.Ok();
    }
}

public class SetConfigRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? Description { get; set; }
}

public class CreateMardColorRequest
{
    public string ColorNo { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string Rgb { get; set; } = string.Empty;
    public string? Lab { get; set; }
    public string? Category { get; set; }
    public bool IsCommon { get; set; }
}

public class CreateBeadKitRequest
{
    public string KitId { get; set; } = string.Empty;
    public string KitName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int ColorCount { get; set; }
    public int BeadCount { get; set; }
    public decimal Price { get; set; }
    public string? PurchaseUrl { get; set; }
}
