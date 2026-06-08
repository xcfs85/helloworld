using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.DTOs.Operation;
using Pindou.Application.Interfaces.Messaging;
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
    public async Task<ApiResponse<List<BannerDto>>> List([FromQuery] string? position)
    {
        var data = await _operationService.GetActiveBannersAsync(position ?? "home");
        return ApiResponse<List<BannerDto>>.Ok(data);
    }

    /// <summary>创建Banner</summary>
    [HttpPost]
    [Permission("banner:add")]
    [OperationLog("创建Banner", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateBannerRequest request)
    {
        return ApiResponse<string>.Ok(string.Empty);
    }

    /// <summary>更新Banner</summary>
    [HttpPut("{id}")]
    [Permission("banner:edit")]
    [OperationLog("更新Banner", SaveParams = true)]
    public async Task<ApiResponse> Update(string id, [FromBody] CreateBannerRequest request)
    {
        return ApiResponse.Ok();
    }

    /// <summary>删除Banner</summary>
    [HttpDelete("{id}")]
    [Permission("banner:delete")]
    [OperationLog("删除Banner")]
    public async Task<ApiResponse> Delete(string id)
    {
        return ApiResponse.Ok();
    }

    /// <summary>更新排序</summary>
    [HttpPut("{id}/sort")]
    [Permission("banner:sort")]
    [OperationLog("更新Banner排序")]
    public async Task<ApiResponse> UpdateSort(string id, [FromBody] UpdateSortRequest request)
    {
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
    public async Task<ApiResponse<List<TopicDto>>> List()
    {
        var data = await _operationService.GetOfficialTopicsAsync();
        return ApiResponse<List<TopicDto>>.Ok(data);
    }

    /// <summary>创建话题</summary>
    [HttpPost]
    [Permission("topic:add")]
    [OperationLog("创建话题", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateTopicRequest request)
    {
        return ApiResponse<string>.Ok(string.Empty);
    }

    /// <summary>更新话题</summary>
    [HttpPut("{id}")]
    [Permission("topic:edit")]
    [OperationLog("更新话题", SaveParams = true)]
    public async Task<ApiResponse> Update(string id, [FromBody] CreateTopicRequest request)
    {
        return ApiResponse.Ok();
    }

    /// <summary>关闭话题</summary>
    [HttpPost("{id}/close")]
    [Permission("topic:close")]
    [OperationLog("关闭话题")]
    public async Task<ApiResponse> Close(string id)
    {
        return ApiResponse.Ok();
    }

    /// <summary>开启话题</summary>
    [HttpPost("{id}/open")]
    [Permission("topic:open")]
    [OperationLog("开启话题")]
    public async Task<ApiResponse> Open(string id)
    {
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
    public async Task<ApiResponse<PagedResult<SpecialTopicDto>>> List([FromQuery] PageRequest request)
    {
        var data = await _operationService.GetActiveSpecialTopicsAsync(request);
        return ApiResponse<PagedResult<SpecialTopicDto>>.Ok(data);
    }

    /// <summary>创建专题</summary>
    [HttpPost]
    [Permission("special-topic:add")]
    [OperationLog("创建专题", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateSpecialTopicRequest request)
    {
        return ApiResponse<string>.Ok(string.Empty);
    }

    /// <summary>更新专题</summary>
    [HttpPut("{id}")]
    [Permission("special-topic:edit")]
    [OperationLog("更新专题", SaveParams = true)]
    public async Task<ApiResponse> Update(string id, [FromBody] CreateSpecialTopicRequest request)
    {
        return ApiResponse.Ok();
    }

    /// <summary>删除专题</summary>
    [HttpDelete("{id}")]
    [Permission("special-topic:delete")]
    [OperationLog("删除专题")]
    public async Task<ApiResponse> Delete(string id)
    {
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/admin/v1/push")]
[Permission("push:view")]
public class PushController : ControllerBase
{
    private readonly IOperationService _operationService;

    public PushController(IOperationService operationService)
    {
        _operationService = operationService;
    }

    /// <summary>推送列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<int>> List()
    {
        var count = await _operationService.GetActivePushCountAsync();
        return ApiResponse<int>.Ok(count);
    }

    /// <summary>发送推送</summary>
    [HttpPost]
    [Permission("push:send")]
    [OperationLog("发送推送", SaveParams = true)]
    public async Task<ApiResponse> Send([FromBody] SendPushRequest request)
    {
        return ApiResponse.Ok();
    }

    /// <summary>定时推送</summary>
    [HttpPost("schedule")]
    [Permission("push:schedule")]
    [OperationLog("定时推送", SaveParams = true)]
    public async Task<ApiResponse> Schedule([FromBody] SchedulePushRequest request)
    {
        return ApiResponse.Ok();
    }

    /// <summary>取消定时推送</summary>
    [HttpPost("{id}/cancel")]
    [Permission("push:cancel")]
    [OperationLog("取消定时推送")]
    public async Task<ApiResponse> Cancel(string id)
    {
        return ApiResponse.Ok();
    }
}

public class CreateBannerRequest
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string LinkType { get; set; } = "none";
    public string? LinkValue { get; set; }
    public string Position { get; set; } = "home";
    public int Sort { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class UpdateSortRequest
{
    public int Sort { get; set; }
}

public class CreateTopicRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
}

public class CreateSpecialTopicRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CoverUrl { get; set; } = string.Empty;
    public List<string>? TemplateIds { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class SendPushRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? TargetType { get; set; }
    public List<string>? TargetIds { get; set; }
}

public class SchedulePushRequest : SendPushRequest
{
    public DateTime ScheduleTime { get; set; }
}