using Pindou.Application.Common;
using Pindou.Application.DTOs.Member;
using Pindou.Application.Interfaces.Member;
using Pindou.Domain.Entities.Member;
using Pindou.Infrastructure.Repositories;

namespace Pindou.Infrastructure.Services.Member;

public class MemberService : IMemberService
{
    private readonly IRepository<Member> _memberRepo;
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<MemberProduct> _productRepo;
    public MemberService(
        IRepository<Member> memberRepo,
        IRepository<Order> orderRepo,
        IRepository<MemberProduct> productRepo)
    {
        _memberRepo = memberRepo;
        _orderRepo = orderRepo;
        _productRepo = productRepo;
    }

    public Task<List<MemberProductDto>> GetProductsAsync() { throw new NotImplementedException(); }
    public Task<string> CreateOrderAsync(string userId, CreateOrderRequest request) { throw new NotImplementedException(); }
    public Task<OrderDto> GetOrderAsync(string userId, string orderId) { throw new NotImplementedException(); }
    public Task<bool> PayOrderAsync(string userId, string orderId, string payMethod) { throw new NotImplementedException(); }
    public Task<bool> CancelOrderAsync(string userId, string orderId) { throw new NotImplementedException(); }
    public Task<bool> OpenMemberAsync(string userId, OpenMemberRequest request, long operatorId) { throw new NotImplementedException(); }
    public Task<PagedResult<OrderDto>> GetOrdersAsync(string userId, PageRequest request) { throw new NotImplementedException(); }
    public Task<PagedResult<OrderDto>> AdminListOrdersAsync(OrderQuery query) { throw new NotImplementedException(); }
}

public class UserMemberService : IUserMemberService
{
    public Task<MemberStatusDto> GetMemberStatusAsync(string userId) { throw new NotImplementedException(); }
    public Task<List<MemberRecordDto>> GetMemberRecordsAsync(string userId) { throw new NotImplementedException(); }
}
