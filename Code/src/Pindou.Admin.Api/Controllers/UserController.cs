using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.DTOs.Member;
using Pindou.Application.DTOs.User;
using Pindou.Application.Interfaces.Community;
using Pindou.Application.Interfaces.Member;
using Pindou.Application.Interfaces.User;
using Pindou.Shared.Attributes;

namespace Pindou.Admin.Api.Controllers;

[ApiController]
[Route("api/admin/v1/user")]
[Permission("user:view")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPostService _postService;

    public UserController(IUserService userService, IPostService postService)
    {
        _userService = userService;
        _postService = postService;
    }

    /// <summary>用户列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<UserListDto>>> List([FromQuery] UserListQuery query)
    {
        var data = await _userService.GetListAsync(query);
        return ApiResponse<PagedResult<UserListDto>>.Ok(data);
    }

    /// <summary>用户详情</summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<UserListDto>> Detail(string id)
    {
        // UserService doesn't have GetDetail, use list with single query
        var query = new UserListQuery { Page = 1, Size = 1 };
        var data = await _userService.GetListAsync(query);
        var user = data.List.FirstOrDefault(u => u.Id == id);
        if (user == null) throw new BizException("用户不存在", ErrorCodes.NotFound);
        return ApiResponse<UserListDto>.Ok(user);
    }

    /// <summary>禁用用户</summary>
    [HttpPost("{id}/disable")]
    [Permission("user:disable")]
    [OperationLog("禁用用户")]
    public async Task<ApiResponse> Disable(string id, [FromBody] DisableUserRequest request)
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        await _userService.DisableUserAsync(id, request.Reason ?? string.Empty, adminId);
        return ApiResponse.Ok();
    }

    /// <summary>启用用户</summary>
    [HttpPost("{id}/enable")]
    [Permission("user:enable")]
    [OperationLog("启用用户")]
    public async Task<ApiResponse> Enable(string id)
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        await _userService.EnableUserAsync(id, adminId);
        return ApiResponse.Ok();
    }

    /// <summary>用户帖子</summary>
    [HttpGet("{id}/posts")]
    public async Task<ApiResponse<PagedResult<PostDto>>> Posts(string id, [FromQuery] PageRequest request)
    {
        var data = await _postService.GetUserPostsAsync(id, request);
        return ApiResponse<PagedResult<PostDto>>.Ok(data);
    }
}

[ApiController]
[Route("api/admin/v1/member")]
[Permission("member:view")]
public class MemberController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly IUserService _userService;

    public MemberController(IMemberService memberService, IUserService userService)
    {
        _memberService = memberService;
        _userService = userService;
    }

    /// <summary>会员列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<UserListDto>>> List([FromQuery] UserListQuery query)
    {
        query.IsMember = true;
        var data = await _userService.GetListAsync(query);
        return ApiResponse<PagedResult<UserListDto>>.Ok(data);
    }

    /// <summary>开通会员</summary>
    [HttpPost("open")]
    [Permission("member:open")]
    [OperationLog("开通会员")]
    public async Task<ApiResponse> Open([FromBody] OpenMemberRequest request)
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        await _memberService.OpenMemberAsync(request.UserId, request, adminId);
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/admin/v1/order")]
[Permission("order:view")]
public class OrderController : ControllerBase
{
    private readonly IMemberService _memberService;

    public OrderController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>订单列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<OrderDto>>> List([FromQuery] OrderQuery query)
    {
        var data = await _memberService.AdminListOrdersAsync(query);
        return ApiResponse<PagedResult<OrderDto>>.Ok(data);
    }

    /// <summary>订单详情</summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<OrderDto>> Detail(string id)
    {
        var data = await _memberService.GetOrderAsync(string.Empty, id);
        return ApiResponse<OrderDto>.Ok(data);
    }
}

public class DisableUserRequest
{
    public string? Reason { get; set; }
}