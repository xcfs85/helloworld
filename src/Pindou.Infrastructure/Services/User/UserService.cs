using Pindou.Application.Common;
using Pindou.Application.DTOs.User;
using Pindou.Application.Interfaces.User;
using Pindou.Domain.Entities.User;
using Pindou.Domain.Entities.Creation;
using Pindou.Domain.Entities.Community;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Repositories;
using SqlSugar;

namespace Pindou.Infrastructure.Services.User;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Diagram> _diagramRepo;
    private readonly IRepository<Post> _postRepo;
    private readonly ICacheService _cache;

    public UserService(
        IRepository<User> userRepo,
        IRepository<Diagram> diagramRepo,
        IRepository<Post> postRepo,
        ICacheService cache)
    {
        _userRepo = userRepo;
        _diagramRepo = diagramRepo;
        _postRepo = postRepo;
        _cache = cache;
    }

    public async Task<DTOs.Auth.UserInfo> GetUserInfoAsync(string userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);

        return new DTOs.Auth.UserInfo
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Avatar = user.Avatar,
            Gender = user.Gender,
            Phone = user.Phone,
            IsMember = user.IsMember,
            MemberExpireTime = user.MemberExpireTime
        };
    }

    public async Task<DTOs.Auth.UserInfo> UpdateUserInfoAsync(string userId, UpdateUserRequest request)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);

        if (!string.IsNullOrWhiteSpace(request.Nickname))
            user.Nickname = request.Nickname;
        if (request.Avatar != null)
            user.Avatar = request.Avatar;
        if (request.Bio != null)
            user.Bio = request.Bio;
        if (request.City != null)
            user.City = request.City;
        if (!string.IsNullOrWhiteSpace(request.Gender))
            user.Gender = request.Gender;

        user.UpdateTime = DateTime.Now;
        await _userRepo.UpdateAsync(user);

        return new DTOs.Auth.UserInfo
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Avatar = user.Avatar,
            Gender = user.Gender,
            Phone = user.Phone,
            IsMember = user.IsMember,
            MemberExpireTime = user.MemberExpireTime
        };
    }

    public async Task<PagedResult<UserListDto>> GetListAsync(UserListQuery query)
    {
        var exp = Expressionable.Create<User>();

        if (query.IsMember.HasValue)
            exp.And(u => u.IsMember == query.IsMember.Value);
        if (!string.IsNullOrWhiteSpace(query.Status))
            exp.And(u => u.Status == query.Status);
        if (query.RegisterStartTime.HasValue)
            exp.And(u => u.CreateTime >= query.RegisterStartTime.Value);
        if (query.RegisterEndTime.HasValue)
            exp.And(u => u.CreateTime <= query.RegisterEndTime.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
            exp.And(u => u.Nickname.Contains(query.Keyword) || (u.Phone != null && u.Phone.Contains(query.Keyword)));

        var (list, total) = await _userRepo.GetPagedAsync(
            exp.ToExpression(),
            query.Page,
            query.Size,
            u => u.CreateTime,
            true);

        var result = new PagedResult<UserListDto>
        {
            Page = query.Page,
            Size = query.Size,
            Total = total,
            List = new List<UserListDto>()
        };

        foreach (var user in list)
        {
            var diagramCount = await _diagramRepo.CountAsync(d => d.UserId == user.Id);
            var postCount = await _postRepo.CountAsync(p => p.UserId == user.Id && p.Status == "active");

            result.List.Add(new UserListDto
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Avatar = user.Avatar,
                Phone = user.Phone,
                Gender = user.Gender,
                City = user.City,
                IsMember = user.IsMember,
                MemberExpireTime = user.MemberExpireTime,
                Status = user.Status,
                CreateTime = user.CreateTime,
                LastLoginTime = user.LastLoginTime,
                DiagramCount = diagramCount,
                PostCount = postCount
            });
        }

        return result;
    }

    public async Task<bool> DisableUserAsync(string userId, string reason, long operatorId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);

        user.Status = "disabled";
        user.UpdateTime = DateTime.Now;
        return await _userRepo.UpdateAsync(user);
    }

    public async Task<bool> EnableUserAsync(string userId, long operatorId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);

        user.Status = "active";
        user.UpdateTime = DateTime.Now;
        return await _userRepo.UpdateAsync(user);
    }

    public async Task<int> GetGenerationQuotaAsync(string userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);

        var cacheKey = $"gen:daily:{userId}:{DateTime.Now:yyyyMMdd}";
        var used = await _cache.GetAsync<int>(cacheKey);

        if (user.IsMember && user.MemberExpireTime > DateTime.Now)
        {
            if (user.MemberExpireTime.Value.AddYears(10) > DateTime.Now)
                return -1;
            if (user.MemberExpireTime.Value.AddMonths(12) > DateTime.Now)
                return 50 - used;
            if (user.MemberExpireTime.Value.AddMonths(3) > DateTime.Now)
                return 20 - used;
            if (user.MemberExpireTime.Value.AddMonths(1) > DateTime.Now)
                return 10 - used;
        }

        return 3 - used;
    }

    public async Task ConsumeGenerationQuotaAsync(string userId)
    {
        var cacheKey = $"gen:daily:{userId}:{DateTime.Now:yyyyMMdd}";
        await _cache.IncrementAsync(cacheKey, 1, TimeSpan.FromDays(1));
    }
}
