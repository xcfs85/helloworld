using Pindou.Application.Common;
using Pindou.Application.DTOs.Admin;

namespace Pindou.Application.Interfaces.Admin;

public interface IAdminAuthService
{
    Task<string> GenerateCaptchaAsync();
    Task<AdminLoginResponse> LoginAsync(AdminLoginRequest request, string ip);
    Task LogoutAsync(long adminId);
    Task<AdminLoginResponse> RefreshTokenAsync(string refreshToken);
    Task<AdminUserInfo> GetCurrentUserAsync(long adminId);
}

public interface IAdminUserService
{
    Task<PagedResult<AdminUserListDto>> GetListAsync(AdminUserQuery query);
    Task<AdminUserDetailDto> GetDetailAsync(long id);
    Task<long> CreateAsync(CreateAdminUserRequest request);
    Task<bool> UpdateAsync(long id, UpdateAdminUserRequest request);
    Task<bool> DeleteAsync(long id);
    Task<bool> ResetPasswordAsync(long id, string newPassword);
    Task<bool> ChangePasswordAsync(long id, string oldPassword, string newPassword);
    Task<bool> UpdateStatusAsync(long id, int status);
}

public interface IRoleService
{
    Task<PagedResult<RoleDto>> GetListAsync(PageRequest request);
    Task<List<RoleDto>> GetAllAsync();
    Task<RoleDto> GetDetailAsync(long id);
    Task<long> CreateAsync(CreateRoleRequest request);
    Task<bool> UpdateAsync(long id, CreateRoleRequest request);
    Task<bool> DeleteAsync(long id);
    Task<List<string>> GetPermissionsAsync(long roleId);
}

public interface IOperationLogService
{
    Task RecordAsync(long userId, string username, string? nickname, string operation, string? content, string? method, string? @params, string? ip, string? userAgent);
    Task<PagedResult<OperationLogDto>> GetListAsync(OperationLogQuery query);
    Task<bool> DeleteAsync(long id);
    Task<bool> ClearAsync(DateTime? beforeTime);
}
