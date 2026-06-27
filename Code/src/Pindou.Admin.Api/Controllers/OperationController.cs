using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.DTOs.Operation;
using Pindou.Application.Interfaces.Messaging;
using Pindou.Application.Interfaces.Operation;
using Pindou.Shared.Attributes;

namespace Pindou.Admin.Api.Controllers;

[ApiController]
[Route("api/admin/v1/banner")]
[Permission("banner:view")]
public class BannerController : ControllerBase
{
    private readonly IOperationService _operationService;

    public BannerController(IOperationService operationService)
    {
        _operationService = operationService;
    }

    /// <summary>Banner列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<BannerAdminDto>>> List([FromQuery] BannerListQuery query)
    {
        var data = await _operationService.GetBannersAsync(query);
        return ApiResponse<PagedResult<BannerAdminDto>>.Ok(data);
    }

    /// <summary>创建Banner</summary>
    [HttpPost]
    [Permission("banner:add")]
    [OperationLog("创建Banner", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateBannerRequest request)
    {
        var id = await _operationService.CreateBannerAsync(request);
        return ApiResponse<string>.Ok(id);
    }

    /// <summary>更新Banner</summary>
    [HttpPut("{id}")]
    [Permission("banner:edit")]
    [OperationLog("更新Banner", SaveParams = true)]
    public async Task<ApiResponse> Update(string id, [FromBody] CreateBannerRequest request)
    {
        await _operationService.UpdateBannerAsync(id, request);
        return ApiResponse.Ok();
    }

    /// <summary>删除Banner</summary>
    [HttpDelete("{id}")]
    [Permission("banner:delete")]
    [OperationLog("删除Banner")]
    public async Task<ApiResponse> Delete(string id)
    {
        await _operationService.DeleteBannerAsync(id);
        return ApiResponse.Ok();
    }

    /// <summary>更新排序</summary>
    [HttpPut("{id}/sort")]
    [Permission("banner:sort")]
    [OperationLog("更新Banner排序")]
    public async Task<ApiResponse> UpdateSort(string id, [FromBody] UpdateSortRequest request)
    {
        await _operationService.UpdateBannerSortAsync(id, request.Sort);
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/admin/v1/topic")]
[Permission("topic:view")]
public class TopicController : ControllerBase
{
    private readonly IOperationService _operationService;

    public TopicController(IOperationService operationService)
    {
        _operationService = operationService;
    }

    /// <summary>话题列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<TopicAdminDto>>> List([FromQuery] TopicListQuery query)
    {
        var data = await _operationService.GetTopicsAsync(query);
        return ApiResponse<PagedResult<TopicAdminDto>>.Ok(data);
    }

    /// <summary>话题详情</summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<TopicAdminDto>> Detail(string id)
    {
        var data = await _operationService.GetTopicAsync(id);
        return ApiResponse<TopicAdminDto>.Ok(data);
    }

    /// <summary>创建话题</summary>
    [HttpPost]
    [Permission("topic:add")]
    [OperationLog("创建话题", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateTopicAdminRequest request)
    {
        var id = await _operationService.CreateTopicAsync(request);
        return ApiResponse<string>.Ok(id);
    }

    /// <summary>更新话题</summary>
    [HttpPut("{id}")]
    [Permission("topic:edit")]
    [OperationLog("更新话题", SaveParams = true)]
    public async Task<ApiResponse> Update(string id, [FromBody] UpdateTopicAdminRequest request)
    {
        await _operationService.UpdateTopicAsync(id, request);
        return ApiResponse.Ok();
    }

    /// <summary>关闭话题</summary>
    [HttpPost("{id}/close")]
    [Permission("topic:close")]
    [OperationLog("关闭话题")]
    public async Task<ApiResponse> Close(string id)
    {
        await _operationService.CloseTopicAsync(id);
        return ApiResponse.Ok();
    }

    /// <summary>开启话题</summary>
    [HttpPost("{id}/open")]
    [Permission("topic:open")]
    [OperationLog("开启话题")]
    public async Task<ApiResponse> Open(string id)
    {
        await _operationService.OpenTopicAsync(id);
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/admin/v1/special-topic")]
[Permission("special-topic:view")]
public class SpecialTopicController : ControllerBase
{
    private readonly IOperationService _operationService;

    public SpecialTopicController(IOperationService operationService)
    {
        _operationService = operationService;
    }

    /// <summary>专题列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<SpecialTopicDto>>> List([FromQuery] SpecialTopicListQuery query)
    {
        var data = await _operationService.GetSpecialTopicsAsync(query);
        return ApiResponse<PagedResult<SpecialTopicDto>>.Ok(data);
    }

    /// <summary>专题详情</summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SpecialTopicDto>> Detail(string id)
    {
        var data = await _operationService.GetSpecialTopicAsync(id);
        return ApiResponse<SpecialTopicDto>.Ok(data);
    }

    /// <summary>创建专题</summary>
    [HttpPost]
    [Permission("special-topic:add")]
    [OperationLog("创建专题", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateSpecialTopicRequest request)
    {
        var id = await _operationService.CreateSpecialTopicAsync(request);
        return ApiResponse<string>.Ok(id);
    }

    /// <summary>更新专题</summary>
    [HttpPut("{id}")]
    [Permission("special-topic:edit")]
    [OperationLog("更新专题", SaveParams = true)]
    public async Task<ApiResponse> Update(string id, [FromBody] UpdateSpecialTopicRequest request)
    {
        await _operationService.UpdateSpecialTopicAsync(id, request);
        return ApiResponse.Ok();
    }

    /// <summary>删除专题</summary>
    [HttpDelete("{id}")]
    [Permission("special-topic:delete")]
    [OperationLog("删除专题")]
    public async Task<ApiResponse> Delete(string id)
    {
        await _operationService.DeleteSpecialTopicAsync(id);
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/admin/v1/push")]
[Permission("push:view")]
public class PushController : ControllerBase
{
    private readonly IPushService _pushService;

    public PushController(IPushService pushService)
    {
        _pushService = pushService;
    }

    /// <summary>推送列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<PushRecordDto>>> List([FromQuery] PushListQuery query)
    {
        var data = await _pushService.GetPushListAsync(query);
        return ApiResponse<PagedResult<PushRecordDto>>.Ok(data);
    }

    /// <summary>推送详情</summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<PushRecordDto>> Detail(string id)
    {
        var data = await _pushService.GetPushAsync(id);
        return ApiResponse<PushRecordDto>.Ok(data);
    }

    /// <summary>创建推送(草稿)</summary>
    [HttpPost]
    [Permission("push:add")]
    [OperationLog("创建推送", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] SendPushRequest request)
    {
        var id = await _pushService.CreatePushAsync(request);
        return ApiResponse<string>.Ok(id);
    }

    /// <summary>发送推送</summary>
    [HttpPost("send")]
    [Permission("push:send")]
    [OperationLog("发送推送", SaveParams = true)]
    public async Task<ApiResponse<string>> Send([FromBody] SendPushRequest request)
    {
        var id = await _pushService.SendPushAsync(request);
        return ApiResponse<string>.Ok(id);
    }

    /// <summary>定时推送</summary>
    [HttpPost("schedule")]
    [Permission("push:schedule")]
    [OperationLog("定时推送", SaveParams = true)]
    public async Task<ApiResponse<string>> Schedule([FromBody] SchedulePushRequest request)
    {
        var id = await _pushService.SchedulePushAsync(request);
        return ApiResponse<string>.Ok(id);
    }

    /// <summary>取消定时推送</summary>
    [HttpPost("{id}/cancel")]
    [Permission("push:cancel")]
    [OperationLog("取消定时推送")]
    public async Task<ApiResponse> Cancel(string id)
    {
        await _pushService.CancelPushAsync(id);
        return ApiResponse.Ok();
    }

    /// <summary>重试推送</summary>
    [HttpPost("{id}/retry")]
    [Permission("push:send")]
    [OperationLog("重试推送")]
    public async Task<ApiResponse> Retry(string id)
    {
        await _pushService.RetryPushAsync(id);
        return ApiResponse.Ok();
    }
}

public class UpdateSortRequest
{
    public int Sort { get; set; }
}