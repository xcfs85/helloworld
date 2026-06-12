using Pindou.Application.Common;
using Pindou.Application.DTOs.User;
using Pindou.Application.Interfaces.User;
using Pindou.Domain.Entities.User;
using Pindou.Domain.Entities.Creation;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.Member;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Repositories;
using SqlSugar;
using UserEntity = Pindou.Domain.Entities.User.User;
using ICacheService = Pindou.Infrastructure.Cache.ICacheService;
using MemberEntity = Pindou.Domain.Entities.Member.Member;
using OrderEntity = Pindou.Domain.Entities.Member.Order;

namespace Pindou.Infrastructure.Services.User;

public class UserService : IUserService
{
    private readonly IRepository<UserEntity> _userRepo;
    private readonly IRepository<Diagram> _diagramRepo;
    private readonly IRepository<Post> _postRepo;
    private readonly IRepository<MemberEntity> _memberRepo;
    private readonly IRepository<OrderEntity> _orderRepo;
    private readonly IRepository<MemberProduct> _productRepo;
    private readonly ICacheService _cache;

    public UserService(
        IRepository<UserEntity> userRepo,
        IRepository<Diagram> diagramRepo,
        IRepository<Post> postRepo,
        IRepository<MemberEntity> memberRepo,
        IRepository<OrderEntity> orderRepo,
        IRepository<MemberProduct> productRepo,
        ICacheService cache)
    {
        _userRepo = userRepo;
        _diagramRepo = diagramRepo;
        _postRepo = postRepo;
        _memberRepo = memberRepo;
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _cache = cache;
    }

    public async Task<Pindou.Application.DTOs.Auth.UserInfo> GetUserInfoAsync(string userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);

        return new Pindou.Application.DTOs.Auth.UserInfo
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

    public async Task<Pindou.Application.DTOs.Auth.UserInfo> UpdateUserInfoAsync(string userId, UpdateUserRequest request)
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

        return new Pindou.Application.DTOs.Auth.UserInfo
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
        var exp = Expressionable.Create<UserEntity>();
        var now = DateTime.Now;

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

        // 到期状态过滤
        if (!string.IsNullOrWhiteSpace(query.Expire))
        {
            switch (query.Expire)
            {
                case "7d":
                    exp.And(u => u.MemberExpireTime != null && u.MemberExpireTime > now && u.MemberExpireTime <= now.AddDays(7));
                    break;
                case "30d":
                    exp.And(u => u.MemberExpireTime != null && u.MemberExpireTime > now && u.MemberExpireTime <= now.AddDays(30));
                    break;
                case "expired":
                    exp.And(u => u.MemberExpireTime != null && u.MemberExpireTime <= now);
                    break;
                case "long":
                    exp.And(u => u.MemberExpireTime != null && u.MemberExpireTime > now.AddYears(1));
                    break;
            }
        }

        // 是否需要按会员等级或支付渠道过滤（这些字段依赖订单/会员记录，无法在 SQL 中直接判断，需要先全量拉取再在内存中过滤）
        var needLevelFilter = !string.IsNullOrWhiteSpace(query.Level) && query.Level != "all";
        var needPayChannelFilter = !string.IsNullOrWhiteSpace(query.PayChannel) && query.PayChannel != "all";
        var needMemoryFilter = needLevelFilter || needPayChannelFilter;

        List<UserEntity> pageUsers;
        int total;

        if (needMemoryFilter)
        {
            // 拉取所有候选用户，在内存中按等级/支付渠道过滤后再分页
            var allCandidates = await _userRepo.GetListAsync(exp.ToExpression());

            if (needLevelFilter)
            {
                var userLevelMap = await ComputeUserLevelsAsync(allCandidates);
                allCandidates = allCandidates.Where(u => string.Equals(userLevelMap.GetValueOrDefault(u.Id), query.Level, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (needPayChannelFilter)
            {
                var userChannelMap = await ComputeUserPayChannelsAsync(allCandidates);
                allCandidates = allCandidates.Where(u => string.Equals(userChannelMap.GetValueOrDefault(u.Id), query.PayChannel, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            total = allCandidates.Count;
            pageUsers = allCandidates.Skip((query.Page - 1) * query.Size).Take(query.Size).ToList();
        }
        else
        {
            var (list, count) = await _userRepo.GetPagedAsync(
                exp.ToExpression(),
                query.Page,
                query.Size,
                u => u.CreateTime,
                true);
            pageUsers = list;
            total = count;
        }

        var result = new PagedResult<UserListDto>
        {
            Page = query.Page,
            Size = query.Size,
            Total = total,
            List = new List<UserListDto>()
        };

        foreach (var user in pageUsers)
        {
            var diagramCount = await _diagramRepo.CountAsync(d => d.UserId == user.Id);
            var postCount = await _postRepo.CountAsync(p => p.UserId == user.Id && p.Status == "active");

            // 获取会员相关信息
            var memberInfo = await GetMemberInfoAsync(user.Id);

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
                MemberLevel = memberInfo?.MemberLevel,
                AutoRenew = memberInfo?.AutoRenew ?? false,
                TotalPaid = memberInfo?.TotalPaid ?? 0,
                PayChannel = memberInfo?.PayChannel,
                Status = user.Status,
                CreateTime = user.CreateTime,
                LastLoginTime = user.LastLoginTime,
                DiagramCount = diagramCount,
                PostCount = postCount
            });
        }

        return result;
    }

    /// <summary>
    /// 批量计算会员等级（userId -> level），逻辑与单用户 GetMemberInfoAsync 保持一致。
    /// </summary>
    private async Task<Dictionary<string, string>> ComputeUserLevelsAsync(List<UserEntity> users)
    {
        var result = new Dictionary<string, string>();
        if (users == null || users.Count == 0) return result;

        var userIds = users.Select(u => u.Id).ToList();

        // 一次性拉取这些用户的所有已支付会员订单
        var paidOrders = await _orderRepo.GetListAsync(
            o => userIds.Contains(o.UserId) && o.Status == "paid" && o.ProductType == "member");
        var userLatestOrder = paidOrders
            .GroupBy(o => o.UserId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(o => o.PayTime ?? o.CreateTime).First());

        // 加载产品信息
        var productIds = userLatestOrder.Values.Select(o => o.ProductId).Distinct().ToList();
        var products = productIds.Count == 0
            ? new List<MemberProduct>()
            : await _productRepo.GetListAsync(p => productIds.Contains(p.ProductId));
        var productMap = products.ToDictionary(p => p.ProductId, p => p);

        // 拉取这些用户的所有会员记录（兜底用）
        var memberRecords = await _memberRepo.GetListAsync(m => userIds.Contains(m.UserId));
        var userLatestRecord = memberRecords
            .GroupBy(m => m.UserId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(m => m.ExpireTime).First());

        foreach (var user in users)
        {
            string? level = null;

            if (userLatestOrder.TryGetValue(user.Id, out var order) &&
                productMap.TryGetValue(order.ProductId, out var product) &&
                !string.IsNullOrWhiteSpace(product.Grade))
            {
                level = product.Grade;
            }
            else if (userLatestRecord.TryGetValue(user.Id, out var record))
            {
                var days = (record.ExpireTime - record.StartTime).TotalDays;
                if (days >= 365 * 10) level = "SVIP";
                else if (days >= 365) level = "VIP3";
                else if (days >= 90) level = "VIP2";
                else if (days > 0) level = "VIP1";
            }

            if (string.IsNullOrEmpty(level)) level = "VIP1";
            result[user.Id] = level;
        }

        return result;
    }

    /// <summary>
    /// 批量计算用户支付渠道（userId -> payChannel），逻辑与单用户 GetMemberInfoAsync 保持一致。
    /// </summary>
    private async Task<Dictionary<string, string>> ComputeUserPayChannelsAsync(List<UserEntity> users)
    {
        var result = new Dictionary<string, string>();
        if (users == null || users.Count == 0) return result;

        var userIds = users.Select(u => u.Id).ToList();

        // 拉取这些用户的所有已支付会员订单
        var paidOrders = await _orderRepo.GetListAsync(
            o => userIds.Contains(o.UserId) && o.Status == "paid" && o.ProductType == "member");

        // 取每个用户最近一笔已支付订单的支付方式
        var userLatestOrder = paidOrders
            .GroupBy(o => o.UserId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(o => o.PayTime ?? o.CreateTime).First());

        // 有已支付订单的用户集合
        var paidUserIds = new HashSet<string>(userLatestOrder.Keys);

        foreach (var user in users)
        {
            if (userLatestOrder.TryGetValue(user.Id, out var order) && !string.IsNullOrEmpty(order.PayMethod))
            {
                result[user.Id] = order.PayMethod;
            }
            else if (paidUserIds.Contains(user.Id))
            {
                // 有订单但无支付方式，标记为 backend
                result[user.Id] = "backend";
            }
            else
            {
                // 无已支付订单，也标记为 backend（后台开通）
                result[user.Id] = "backend";
            }
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

    public async Task< bool> EnableUserAsync(string userId, long operatorId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);

        user.Status = "active";
        user.UpdateTime = DateTime.Now;
        return await _userRepo.UpdateAsync(user);
    }

    public async Task<bool> MuteUserAsync(string userId, int days, string reason, long operatorId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);

        user.Status = "muted";
        user.MuteExpireTime = DateTime.Now.AddDays(days);
        user.MuteReason = reason;
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

    public async Task<UserMemberInfoDto?> GetMemberInfoAsync(string userId)
    {
        // 获取会员开通记录（使用足够大的size获取全部）
        var memberResult = await _memberRepo.GetPagedAsync(
            m => m.UserId == userId,
            1,
            1_000,
            m => m.CreateTime,
            true);
        var memberRecords = memberResult.list;

        // 获取用户的订单（已支付）
        var orderResult = await _orderRepo.GetPagedAsync(
            o => o.UserId == userId && o.Status == "paid" && o.ProductType == "member",
            1,
            1_000,
            o => o.CreateTime,
            true);
        var paidOrders = orderResult.list;

        if (!paidOrders.Any())
            return null;

        var memberProductRepo = _memberRepo; // 需要注入 MemberProduct
        var productIds = paidOrders.Select(o => o.ProductId).Distinct().ToList();

        // 获取会员产品信息
        var memberProduct = memberRecords.FirstOrDefault();
        string? memberLevel = null;

        // 根据会员时长判断等级
        if (memberProduct != null)
        {
            var days = (memberProduct.ExpireTime - memberProduct.StartTime).TotalDays;
            if (days >= 365 * 10) memberLevel = "SVIP";
            else if (days >= 365) memberLevel = "VIP3";
            else if (days >= 90) memberLevel = "VIP2";
            else memberLevel = "VIP1";
        }

        // 获取最后一次支付的渠道
        var lastPaidOrder = paidOrders.OrderByDescending(o => o.PayTime).FirstOrDefault();

        // 累计付费
        var totalPaid = paidOrders.Sum(o => o.Amount);

        // 自动续费（根据最近订单判断，如果有未过期的会员记录则视为开启）
        var currentMember = memberRecords.FirstOrDefault(m => m.ExpireTime > DateTime.Now);
        var autoRenew = currentMember != null; // 简化逻辑：有未过期会员即视为开启自动续费

        return new UserMemberInfoDto
        {
            MemberLevel = memberLevel,
            AutoRenew = autoRenew,
            TotalPaid = totalPaid,
            PayChannel = lastPaidOrder?.PayMethod,
            FirstOpenTime = memberRecords.LastOrDefault()?.CreateTime
        };
    }

    public async Task<MemberLevelStatsDto> GetMemberLevelStatsAsync()
    {
        var now = DateTime.Now;

        // 当前有效会员：IsMember = true 且未过期
        var allMembers = await _userRepo.GetListAsync(
            u => u.IsMember && u.Status == "active" && (u.MemberExpireTime == null || u.MemberExpireTime > now));

        // 初始化四个等级的计数
        var levelCounts = new List<MemberLevelCount>
        {
            new() { Level = "VIP1", Count = 0 },
            new() { Level = "VIP2", Count = 0 },
            new() { Level = "VIP3", Count = 0 },
            new() { Level = "SVIP", Count = 0 }
        };

        if (allMembers.Count == 0)
        {
            return new MemberLevelStatsDto
            {
                Total = 0,
                LevelCounts = levelCounts
            };
        }

        // 取每个用户的最新一笔已支付会员订单
        var memberUserIds = allMembers.Select(u => u.Id).ToList();
        var paidOrders = await _orderRepo.GetListAsync(
            o => memberUserIds.Contains(o.UserId) && o.Status == "paid" && o.ProductType == "member");

        var userLatestOrder = paidOrders
            .GroupBy(o => o.UserId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(o => o.PayTime ?? o.CreateTime).First());

        // 加载产品信息
        var productIds = userLatestOrder.Values.Select(o => o.ProductId).Distinct().ToList();
        var products = productIds.Count == 0
            ? new List<MemberProduct>()
            : await _productRepo.GetListAsync(p => productIds.Contains(p.ProductId));
        var productMap = products.ToDictionary(p => p.ProductId, p => p);

        foreach (var user in allMembers)
        {
            string? level = null;

            if (userLatestOrder.TryGetValue(user.Id, out var order) &&
                productMap.TryGetValue(order.ProductId, out var product) &&
                !string.IsNullOrWhiteSpace(product.Grade))
            {
                level = product.Grade;
            }
            else
            {
                // 兜底：按会员记录时长推断等级
                var record = (await _memberRepo.GetListAsync(m => m.UserId == user.Id))
                    .OrderByDescending(m => m.ExpireTime)
                    .FirstOrDefault();
                if (record != null)
                {
                    var days = (record.ExpireTime - record.StartTime).TotalDays;
                    if (days >= 365 * 10) level = "SVIP";
                    else if (days >= 365) level = "VIP3";
                    else if (days >= 90) level = "VIP2";
                    else if (days > 0) level = "VIP1";
                }
            }

            if (string.IsNullOrEmpty(level)) level = "VIP1";

            var item = levelCounts.FirstOrDefault(l => l.Level == level);
            if (item != null) item.Count++;
        }

        return new MemberLevelStatsDto
        {
            Total = allMembers.Count,
            LevelCounts = levelCounts
        };
    }

    public async Task<MemberStatsDto> GetMemberStatsAsync()
    {
        var now = DateTime.Now;
        var sevenDaysLater = now.AddDays(7);
        var thirtyDaysLater = now.AddDays(30);
        var oneYearLater = now.AddYears(1);

        // 获取所有会员用户
        var allMembers = await _userRepo.GetListAsync(u => u.IsMember && u.Status == "active");

        // 统计总数
        var total = allMembers.Count;

        // 获取所有会员记录来获取等级信息
        var memberUserIds = allMembers.Select(u => u.Id).ToList();
        var memberRecords = await _memberRepo.GetListAsync(m => memberUserIds.Contains(m.UserId));

        // 按等级统计
        var levelCounts = new List<MemberLevelCount>
        {
            new() { Level = "all", Count = total },
            new() { Level = "VIP1", Count = 0 },
            new() { Level = "VIP2", Count = 0 },
            new() { Level = "VIP3", Count = 0 },
            new() { Level = "SVIP", Count = 0 }
        };

        // 按用户分组获取最新的会员记录
        var latestMemberRecords = memberRecords
            .GroupBy(m => m.UserId)
            .Select(g => g.OrderByDescending(m => m.ExpireTime).First())
            .ToList();

        foreach (var record in latestMemberRecords)
        {
            var days = (record.ExpireTime - record.StartTime).TotalDays;
            string level;
            if (days >= 365 * 10) level = "SVIP";
            else if (days >= 365) level = "VIP3";
            else if (days >= 90) level = "VIP2";
            else level = "VIP1";

            var levelCount = levelCounts.FirstOrDefault(l => l.Level == level);
            if (levelCount != null)
                levelCount.Count++;
        }

        // 获取各渠道会员数量（从订单表）
        var paidOrders = await _orderRepo.GetListAsync(o => o.Status == "paid" && o.ProductType == "member");
        var userChannels = new Dictionary<string, HashSet<string>>(); // channel -> userIds

        foreach (var order in paidOrders)
        {
            if (!userChannels.ContainsKey(order.UserId))
                userChannels[order.UserId] = new HashSet<string>();
            if (!string.IsNullOrEmpty(order.PayMethod))
            {
                if (!userChannels.ContainsKey(order.PayMethod))
                    userChannels[order.PayMethod] = new HashSet<string>();
                userChannels[order.PayMethod].Add(order.UserId);
            }
        }

        var channelCounts = new List<MemberChannelCount>
        {
            new() { Channel = "wechat", Count = userChannels.GetValueOrDefault("wechat", new HashSet<string>()).Count },
            new() { Channel = "alipay", Count = userChannels.GetValueOrDefault("alipay", new HashSet<string>()).Count },
            new() { Channel = "appstore", Count = userChannels.GetValueOrDefault("appstore", new HashSet<string>()).Count },
            new() { Channel = "backend", Count = total - userChannels.Values.SelectMany(s => s).Distinct().Count() }
        };

        // 到期相关统计
        var expiringSoonCount = allMembers.Count(u => u.MemberExpireTime.HasValue && u.MemberExpireTime > now && u.MemberExpireTime <= sevenDaysLater);
        var expiring30dCount = allMembers.Count(u => u.MemberExpireTime.HasValue && u.MemberExpireTime > now && u.MemberExpireTime <= thirtyDaysLater);
        var longTermCount = allMembers.Count(u => u.MemberExpireTime.HasValue && u.MemberExpireTime > oneYearLater);
        var expiredCount = allMembers.Count(u => u.MemberExpireTime.HasValue && u.MemberExpireTime <= now);

        return new MemberStatsDto
        {
            Total = total,
            LevelCounts = levelCounts,
            ChannelCounts = channelCounts,
            ExpiringSoonCount = expiringSoonCount,
            Expiring30dCount = expiring30dCount,
            LongTermCount = longTermCount,
            ExpiredCount = expiredCount
        };
    }

    public async Task<UserStatsDto> GetUserStatsAsync()
    {
        var now = DateTime.Now;

        // 获取所有用户
        var allUsers = await _userRepo.GetListAsync(u => true);

        // 统计各类用户数量
        var activeCount = allUsers.Count(u => u.Status == "active" || u.Status == "normal");
        var mutedCount = allUsers.Count(u => u.Status == "muted");
        var disabledCount = allUsers.Count(u => u.Status == "disabled");

        // 会员状态
        var memberCount = allUsers.Count(u => u.IsMember && u.MemberExpireTime > now);
        var nonMemberCount = allUsers.Count - memberCount;

        // 注册方式统计
        var platformCounts = new List<PlatformCount>
        {
            new() { Platform = "phone", Count = allUsers.Count(u => !string.IsNullOrEmpty(u.Phone)) },
            new() { Platform = "wechat", Count = allUsers.Count(u => !string.IsNullOrEmpty(u.UnionId)) },
            new() { Platform = "apple", Count = allUsers.Count(u => !string.IsNullOrEmpty(u.AppleUserId)) },
            new() { Platform = "guest", Count = allUsers.Count(u => string.IsNullOrEmpty(u.Phone) && string.IsNullOrEmpty(u.UnionId) && string.IsNullOrEmpty(u.AppleUserId)) }
        };

        return new UserStatsDto
        {
            Total = allUsers.Count,
            ActiveCount = activeCount,
            MutedCount = mutedCount,
            DisabledCount = disabledCount,
            MemberCount = memberCount,
            NonMemberCount = nonMemberCount,
            PlatformCounts = platformCounts
        };
    }

    public async Task<UserListDto> CreateUserAsync(CreateUserRequest request, long operatorId)
    {
        if (string.IsNullOrWhiteSpace(request.Nickname))
            throw new BizException("昵称不能为空", ErrorCodes.ParamError);

        // 检查手机号是否已存在
        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            var existing = await _userRepo.FirstOrDefaultAsync(u => u.Phone == request.Phone);
            if (existing != null)
                throw new BizException("该手机号已注册", ErrorCodes.ParamError);
        }

        var user = new UserEntity
        {
            Nickname = request.Nickname,
            Phone = request.Phone,
            Gender = request.Gender ?? "unknown",
            City = request.City,
            Status = "active"
        };
        await _userRepo.InsertAsync(user);

        return new UserListDto
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Avatar = user.Avatar,
            Phone = user.Phone,
            Gender = user.Gender,
            City = user.City,
            IsMember = user.IsMember,
            Status = user.Status,
            CreateTime = user.CreateTime,
            LastLoginTime = user.LastLoginTime,
            DiagramCount = 0,
            PostCount = 0
        };
    }

    public async Task<ImportUserResult> ImportUsersAsync(List<CreateUserRequest> users, long operatorId)
    {
        var result = new ImportUserResult();

        for (var i = 0; i < users.Count; i++)
        {
            var req = users[i];
            try
            {
                if (string.IsNullOrWhiteSpace(req.Nickname))
                {
                    result.FailCount++;
                    result.FailDetails.Add(new ImportFailDetail { Row = i + 2, Reason = "昵称不能为空" });
                    continue;
                }

                // 检查手机号是否已存在
                if (!string.IsNullOrWhiteSpace(req.Phone))
                {
                    var existing = await _userRepo.FirstOrDefaultAsync(u => u.Phone == req.Phone);
                    if (existing != null)
                    {
                        result.FailCount++;
                        result.FailDetails.Add(new ImportFailDetail { Row = i + 2, Reason = $"手机号 {req.Phone} 已注册" });
                        continue;
                    }
                }

                var user = new UserEntity
                {
                    Nickname = req.Nickname,
                    Phone = req.Phone,
                    Gender = req.Gender ?? "unknown",
                    City = req.City,
                    Status = "active"
                };
                await _userRepo.InsertAsync(user);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailCount++;
                result.FailDetails.Add(new ImportFailDetail { Row = i + 2, Reason = ex.Message });
            }
        }

        return result;
    }
}
