using Pindou.Application.Common;
using Pindou.Application.DTOs.User;

namespace Pindou.Application.Interfaces.User;

public interface IUserService
{
    Task<DTOs.Auth.UserInfo> GetUserInfoAsync(string userId);
    Task<DTOs.Auth.UserInfo> UpdateUserInfoAsync(string userId, UpdateUserRequest request);
    Task<PagedResult<UserListDto>> GetListAsync(UserListQuery query);
    Task<bool> DisableUserAsync(string userId, string reason, long operatorId);
    Task<bool> EnableUserAsync(string userId, long operatorId);
    Task<int> GetGenerationQuotaAsync(string userId);
    Task ConsumeGenerationQuotaAsync(string userId);
}
