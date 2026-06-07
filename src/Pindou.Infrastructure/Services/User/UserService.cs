using Pindou.Application.Common;
using Pindou.Application.DTOs.User;
using Pindou.Application.Interfaces.User;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;

namespace Pindou.Infrastructure.Services.User;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepo;
    public UserService(IRepository<User> userRepo) { _userRepo = userRepo; }

    public Task<DTOs.Auth.UserInfo> GetUserInfoAsync(string userId) { throw new NotImplementedException(); }
    public Task<DTOs.Auth.UserInfo> UpdateUserInfoAsync(string userId, UpdateUserRequest request) { throw new NotImplementedException(); }
    public Task<PagedResult<UserListDto>> GetListAsync(UserListQuery query) { throw new NotImplementedException(); }
    public Task<bool> DisableUserAsync(string userId, string reason, long operatorId) { throw new NotImplementedException(); }
    public Task<bool> EnableUserAsync(string userId, long operatorId) { throw new NotImplementedException(); }
    public Task<int> GetGenerationQuotaAsync(string userId) { throw new NotImplementedException(); }
    public Task ConsumeGenerationQuotaAsync(string userId) { throw new NotImplementedException(); }
}
