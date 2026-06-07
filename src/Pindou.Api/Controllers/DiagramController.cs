using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Creation;
using Pindou.Application.Interfaces.Creation;

namespace Pindou.Api.Controllers;

[ApiController]
[Route("api/v1/diagram")]
public class DiagramController : ControllerBase
{
    private readonly IDiagramService _diagramService;

    public DiagramController(IDiagramService diagramService)
    {
        _diagramService = diagramService;
    }

    /// <summary>创建生成任务(异步)</summary>
    [HttpPost("generate")]
    public async Task<ApiResponse<string>> CreateTask([FromBody] CreateDiagramRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var taskId = await _diagramService.CreateGenerationTaskAsync(userId, request);
        return ApiResponse<string>.Ok(taskId);
    }

    /// <summary>同步生成(≤5000颗)</summary>
    [HttpPost("generate/sync")]
    public async Task<ApiResponse<string>> GenerateSync([FromBody] CreateDiagramRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var diagramId = await _diagramService.GenerateSyncAsync(userId, request);
        return ApiResponse<string>.Ok(diagramId);
    }

    /// <summary>查询任务状态</summary>
    [HttpGet("task/{taskId}")]
    public async Task<ApiResponse<GenerationStatusResponse>> GetTaskStatus(string taskId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _diagramService.GetTaskStatusAsync(userId, taskId);
        return ApiResponse<GenerationStatusResponse>.Ok(data);
    }

    /// <summary>用户图纸列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<DiagramDto>>> GetList([FromQuery] PageRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _diagramService.GetUserDiagramsAsync(userId, request);
        return ApiResponse<PagedResult<DiagramDto>>.Ok(data);
    }

    /// <summary>图纸详情</summary>
    [HttpGet("{diagramId}")]
    public async Task<ApiResponse<DiagramDetailDto>> GetDetail(string diagramId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _diagramService.GetDiagramDetailAsync(userId, diagramId);
        return ApiResponse<DiagramDetailDto>.Ok(data);
    }

    /// <summary>删除图纸</summary>
    [HttpDelete("{diagramId}")]
    public async Task<ApiResponse> Delete(string diagramId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _diagramService.DeleteDiagramAsync(userId, diagramId);
        return ApiResponse.Ok();
    }

    /// <summary>导出图纸</summary>
    [HttpPost("export")]
    public async Task<ApiResponse<string>> Export([FromBody] ExportDiagramRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var url = await _diagramService.ExportDiagramAsync(userId, request);
        return ApiResponse<string>.Ok(url);
    }

    /// <summary>分享图纸</summary>
    [HttpPost("{diagramId}/share")]
    public async Task<ApiResponse<string>> Share(string diagramId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var url = await _diagramService.ShareDiagramAsync(userId, diagramId);
        return ApiResponse<string>.Ok(url);
    }
}
