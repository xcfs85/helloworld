using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Member;
using Pindou.Application.Interfaces.Member;

namespace Pindou.Api.Controllers;

[ApiController]
[Route("api/v1/member")]
public class MemberController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly IUserMemberService _userMemberService;
    public MemberController(IMemberService memberService, IUserMemberService userMemberService)
    {
        _memberService = memberService;
        _userMemberService = userMemberService;
    }

    [HttpGet("products")]
    public async Task<ApiResponse<List<MemberProductDto>>> Products()
    {
        var data = await _memberService.GetProductsAsync();
        return ApiResponse<List<MemberProductDto>>.Ok(data);
    }

    [HttpPost("order")]
    public async Task<ApiResponse<string>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var orderId = await _memberService.CreateOrderAsync(userId, request);
        return ApiResponse<string>.Ok(orderId);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ApiResponse<OrderDto>> GetOrder(string orderId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _memberService.GetOrderAsync(userId, orderId);
        return ApiResponse<OrderDto>.Ok(data);
    }

    [HttpPost("order/{orderId}/pay")]
    public async Task<ApiResponse> PayOrder(string orderId, [FromBody] PayOrderRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _memberService.PayOrderAsync(userId, orderId, request.PayMethod);
        return ApiResponse.Ok();
    }

    [HttpPost("order/{orderId}/cancel")]
    public async Task<ApiResponse> CancelOrder(string orderId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _memberService.CancelOrderAsync(userId, orderId);
        return ApiResponse.Ok();
    }

    [HttpGet("orders")]
    public async Task<ApiResponse<PagedResult<OrderDto>>> GetOrders([FromQuery] PageRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _memberService.GetOrdersAsync(userId, request);
        return ApiResponse<PagedResult<OrderDto>>.Ok(data);
    }

    [HttpGet("status")]
    public async Task<ApiResponse<MemberStatusDto>> Status()
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _userMemberService.GetMemberStatusAsync(userId);
        return ApiResponse<MemberStatusDto>.Ok(data);
    }

    [HttpGet("records")]
    public async Task<ApiResponse<List<MemberRecordDto>>> Records()
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _userMemberService.GetMemberRecordsAsync(userId);
        return ApiResponse<List<MemberRecordDto>>.Ok(data);
    }
}

public class PayOrderRequest
{
    public string PayMethod { get; set; } = "wechat";
}
