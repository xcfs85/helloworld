using System.Text.Json;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Member;
using Pindou.Application.Interfaces.Member;
using Pindou.Domain.Entities.Member;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;
using SqlSugar;

namespace Pindou.Infrastructure.Services.Member;

public class MemberService : IMemberService
{
    private readonly IRepository<Member> _memberRepo;
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<MemberProduct> _productRepo;
    private readonly IRepository<User> _userRepo;

    public MemberService(
        IRepository<Member> memberRepo,
        IRepository<Order> orderRepo,
        IRepository<MemberProduct> productRepo,
        IRepository<User> userRepo)
    {
        _memberRepo = memberRepo;
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _userRepo = userRepo;
    }

    public async Task<List<MemberProductDto>> GetProductsAsync()
    {
        var products = await _productRepo.GetListAsync(p => p.Status == 1);
        return products.Select(p => new MemberProductDto
        {
            Id = p.Id.ToString(),
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            Grade = p.Grade,
            DurationDays = p.DurationDays,
            Price = p.Price,
            OriginalPrice = p.OriginalPrice,
            DailyGenerations = p.DailyGenerations,
            Features = ParseFeatures(p.Features)
        }).ToList();
    }

    public async Task<string> CreateOrderAsync(string userId, CreateOrderRequest request)
    {
        var product = await _productRepo.FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
        if (product == null) throw new BizException("产品不存在", ErrorCodes.NotFound);

        var order = new Order
        {
            UserId = userId,
            OrderNo = GenerateOrderNo(),
            ProductType = request.ProductType,
            ProductId = request.ProductId,
            Amount = product.Price,
            Status = "pending",
            PayMethod = request.PayMethod
        };
        await _orderRepo.InsertAsync(order);
        return order.Id;
    }

    public async Task<OrderDto> GetOrderAsync(string userId, string orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null || order.UserId != userId) throw new BizException("订单不存在", ErrorCodes.NotFound);

        var product = await _productRepo.FirstOrDefaultAsync(p => p.ProductId == order.ProductId);
        return new OrderDto
        {
            Id = order.Id,
            OrderNo = order.OrderNo,
            ProductType = order.ProductType,
            ProductId = order.ProductId,
            ProductName = product?.ProductName,
            Amount = order.Amount,
            Status = order.Status,
            PayMethod = order.PayMethod,
            PayTime = order.PayTime,
            CreateTime = order.CreateTime
        };
    }

    public async Task<bool> PayOrderAsync(string userId, string orderId, string payMethod)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null || order.UserId != userId) throw new BizException("订单不存在", ErrorCodes.NotFound);
        if (order.Status != "pending") throw new BizException("订单状态不可支付", ErrorCodes.ParamError);

        order.Status = "paid";
        order.PayMethod = payMethod;
        order.PayTime = DateTime.Now;
        order.UpdateTime = DateTime.Now;
        await _orderRepo.UpdateAsync(order);

        // 开通会员
        var product = await _productRepo.FirstOrDefaultAsync(p => p.ProductId == order.ProductId);
        if (product != null)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user != null)
            {
                var now = DateTime.Now;
                var startTime = user.IsMember && user.MemberExpireTime > now ? user.MemberExpireTime.Value : now;
                var expireTime = startTime.AddDays(product.DurationDays);

                await _memberRepo.InsertAsync(new Member
                {
                    UserId = userId,
                    MemberType = product.Grade,
                    StartTime = startTime,
                    ExpireTime = expireTime
                });

                user.IsMember = true;
                user.MemberExpireTime = expireTime;
                await _userRepo.UpdateAsync(user);
            }
        }

        return true;
    }

    public async Task<bool> CancelOrderAsync(string userId, string orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null || order.UserId != userId) throw new BizException("订单不存在", ErrorCodes.NotFound);
        if (order.Status != "pending") throw new BizException("订单状态不可取消", ErrorCodes.ParamError);

        order.Status = "canceled";
        order.UpdateTime = DateTime.Now;
        return await _orderRepo.UpdateAsync(order);
    }

    public async Task<bool> OpenMemberAsync(string userId, OpenMemberRequest request, long operatorId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);

        var now = DateTime.Now;
        var startTime = user.IsMember && user.MemberExpireTime > now ? user.MemberExpireTime.Value : now;
        var expireTime = startTime.AddDays(request.DurationDays);

        await _memberRepo.InsertAsync(new Member
        {
            UserId = userId,
            MemberType = request.MemberType,
            StartTime = startTime,
            ExpireTime = expireTime
        });

        user.IsMember = true;
        user.MemberExpireTime = expireTime;
        await _userRepo.UpdateAsync(user);

        return true;
    }

    public async Task<PagedResult<OrderDto>> GetOrdersAsync(string userId, PageRequest request)
    {
        var (list, total) = await _orderRepo.GetPagedAsync(
            o => o.UserId == userId,
            request.Page,
            request.Size,
            o => o.CreateTime,
            true);

        var result = new PagedResult<OrderDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<OrderDto>()
        };

        foreach (var order in list)
        {
            var product = await _productRepo.FirstOrDefaultAsync(p => p.ProductId == order.ProductId);
            result.List.Add(new OrderDto
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                ProductType = order.ProductType,
                ProductId = order.ProductId,
                ProductName = product?.ProductName,
                Amount = order.Amount,
                Status = order.Status,
                PayMethod = order.PayMethod,
                PayTime = order.PayTime,
                CreateTime = order.CreateTime
            });
        }

        return result;
    }

    public async Task<PagedResult<OrderDto>> AdminListOrdersAsync(OrderQuery query)
    {
        var exp = Expressionable.Create<Order>();
        if (!string.IsNullOrWhiteSpace(query.ProductType))
            exp.And(o => o.ProductType == query.ProductType);
        if (!string.IsNullOrWhiteSpace(query.PayMethod))
            exp.And(o => o.PayMethod == query.PayMethod);

        var (list, total) = await _orderRepo.GetPagedAsync(
            exp.ToExpression(),
            query.Page,
            query.Size,
            o => o.CreateTime,
            true);

        var result = new PagedResult<OrderDto>
        {
            Page = query.Page,
            Size = query.Size,
            Total = total,
            List = new List<OrderDto>()
        };

        foreach (var order in list)
        {
            var product = await _productRepo.FirstOrDefaultAsync(p => p.ProductId == order.ProductId);
            result.List.Add(new OrderDto
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                ProductType = order.ProductType,
                ProductId = order.ProductId,
                ProductName = product?.ProductName,
                Amount = order.Amount,
                Status = order.Status,
                PayMethod = order.PayMethod,
                PayTime = order.PayTime,
                CreateTime = order.CreateTime
            });
        }

        return result;
    }

    private static string GenerateOrderNo()
    {
        return $"{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }

    private static List<string>? ParseFeatures(string? features)
    {
        if (string.IsNullOrWhiteSpace(features)) return null;
        try { return JsonSerializer.Deserialize<List<string>>(features); }
        catch { return null; }
    }
}

public class UserMemberService : IUserMemberService
{
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Member> _memberRepo;
    private readonly IRepository<MemberProduct> _productRepo;

    public UserMemberService(
        IRepository<User> userRepo,
        IRepository<Member> memberRepo,
        IRepository<MemberProduct> productRepo)
    {
        _userRepo = userRepo;
        _memberRepo = memberRepo;
        _productRepo = productRepo;
    }

    public async Task<MemberStatusDto> GetMemberStatusAsync(string userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);

        var now = DateTime.Now;
        var remainingDays = 0;
        if (user.IsMember && user.MemberExpireTime > now)
        {
            remainingDays = (int)(user.MemberExpireTime.Value - now).TotalDays;
        }

        var product = await _productRepo.FirstOrDefaultAsync(p => p.Status == 1);
        var dailyGenerations = product?.DailyGenerations ?? 3;

        return new MemberStatusDto
        {
            IsMember = user.IsMember && user.MemberExpireTime > now,
            ExpireTime = user.MemberExpireTime,
            RemainingDays = remainingDays,
            DailyGenerations = dailyGenerations,
            UsedToday = 0,
            RemainingToday = dailyGenerations
        };
    }

    public async Task<List<MemberRecordDto>> GetMemberRecordsAsync(string userId)
    {
        var records = await _memberRepo.GetListAsync(
            m => m.UserId == userId,
            nameof(Member.CreateTime),
            true);

        return records.Select(r => new MemberRecordDto
        {
            Id = r.Id,
            MemberType = r.MemberType,
            StartTime = r.StartTime,
            ExpireTime = r.ExpireTime,
            CreateTime = r.CreateTime
        }).ToList();
    }
}