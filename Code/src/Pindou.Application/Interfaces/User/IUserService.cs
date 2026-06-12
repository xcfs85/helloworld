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
    Task<bool> MuteUserAsync(string userId, int days, string reason, long operatorId);
    Task<int> GetGenerationQuotaAsync(string userId);
    Task ConsumeGenerationQuotaAsync(string userId);
    Task<UserMemberInfoDto?> GetMemberInfoAsync(string userId);
    Task<MemberStatsDto> GetMemberStatsAsync();
    /// <summary>会员等级分布（专门用于侧边栏分类计数）</summary>
    Task<MemberLevelStatsDto> GetMemberLevelStatsAsync();

    /// <summary>用户统计（用于侧边栏分类计数）</summary>
    Task<UserStatsDto> GetUserStatsAsync();

    /// <summary>后台创建用户</summary>
    Task<UserListDto> CreateUserAsync(CreateUserRequest request, long operatorId);

    /// <summary>批量导入用户</summary>
    Task<ImportUserResult> ImportUsersAsync(List<CreateUserRequest> users, long operatorId);
}
