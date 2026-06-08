using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.DTOs.Messaging;
using Pindou.Application.DTOs.Operation;
using Pindou.Application.Interfaces.Messaging;

namespace Pindou.Api.Controllers;

[ApiController]
[Route("api/v1/message")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    public MessageController(IMessageService messageService) { _messageService = messageService; }

    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<MessageDto>>> List([FromQuery] string? type, [FromQuery] PageRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _messageService.GetMessagesAsync(userId, type, request);
        return ApiResponse<PagedResult<MessageDto>>.Ok(data);
    }

    [HttpGet("unread-count")]
    public async Task<ApiResponse<int>> UnreadCount()
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _messageService.GetUnreadCountAsync(userId);
        return ApiResponse<int>.Ok(data);
    }

    [HttpPost("{messageId}/read")]
    public async Task<ApiResponse> Read(string messageId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _messageService.MarkReadAsync(userId, messageId);
        return ApiResponse.Ok();
    }

    [HttpPost("read-all")]
    public async Task<ApiResponse> ReadAll()
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _messageService.MarkAllReadAsync(userId);
        return ApiResponse.Ok();
    }

    [HttpGet("settings")]
    public async Task<ApiResponse<MessageSettingDto>> GetSettings()
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _messageService.GetSettingsAsync(userId);
        return ApiResponse<MessageSettingDto>.Ok(data);
    }

    [HttpPut("settings")]
    public async Task<ApiResponse<MessageSettingDto>> UpdateSettings([FromBody] UpdateMessageSettingRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _messageService.UpdateSettingsAsync(userId, request);
        return ApiResponse<MessageSettingDto>.Ok(data);
    }
}

[ApiController]
[Route("api/v1/operation")]
public class OperationController : ControllerBase
{
    private readonly IOperationService _operationService;
    public OperationController(IOperationService operationService) { _operationService = operationService; }

    [HttpGet("banners")]
    public async Task<ApiResponse<List<BannerDto>>> Banners([FromQuery] string position = "home_top")
    {
        var data = await _operationService.GetActiveBannersAsync(position);
        return ApiResponse<List<BannerDto>>.Ok(data);
    }

    [HttpGet("topics")]
    public async Task<ApiResponse<List<TopicDto>>> Topics()
    {
        var data = await _operationService.GetOfficialTopicsAsync();
        return ApiResponse<List<TopicDto>>.Ok(data);
    }

    [HttpGet("special-topics")]
    public async Task<ApiResponse<PagedResult<SpecialTopicDto>>> SpecialTopics([FromQuery] PageRequest request)
    {
        var data = await _operationService.GetActiveSpecialTopicsAsync(request);
        return ApiResponse<PagedResult<SpecialTopicDto>>.Ok(data);
    }

    [HttpGet("special-topics/{id}")]
    public async Task<ApiResponse<SpecialTopicDto>> GetSpecialTopic(string id)
    {
        var data = await _operationService.GetSpecialTopicAsync(id);
        return ApiResponse<SpecialTopicDto>.Ok(data);
    }
}
