using Pindou.Application.Common;
using Pindou.Application.DTOs.Member;

namespace Pindou.Application.Interfaces.Member;

public interface IMemberService
{
    Task<List<MemberProductDto>> GetProductsAsync();
    Task<string> CreateOrderAsync(string userId, CreateOrderRequest request);
    Task<OrderDto> GetOrderAsync(string userId, string orderId);
    Task<bool> PayOrderAsync(string userId, string orderId, string payMethod);
    Task<bool> CancelOrderAsync(string userId, string orderId);
    Task<bool> OpenMemberAsync(string userId, OpenMemberRequest request, long operatorId);
    Task<PagedResult<OrderDto>> GetOrdersAsync(string userId, PageRequest request);
    Task<PagedResult<OrderDto>> AdminListOrdersAsync(OrderQuery query);
}

public interface IUserMemberService
{
    Task<MemberStatusDto> GetMemberStatusAsync(string userId);
    Task<List<MemberRecordDto>> GetMemberRecordsAsync(string userId);
}
