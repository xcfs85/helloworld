using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Template;
using Pindou.Application.Interfaces.Template;

namespace Pindou.Api.Controllers;

[ApiController]
[Route("api/v1/template")]
public class TemplateController : ControllerBase
{
    private readonly ITemplateService _templateService;
    public TemplateController(ITemplateService templateService) { _templateService = templateService; }

    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<TemplateDto>>> List([FromQuery] TemplateQuery query)
    {
        var data = await _templateService.GetTemplatesAsync(query);
        return ApiResponse<PagedResult<TemplateDto>>.Ok(data);
    }

    [HttpGet("{templateId}")]
    public async Task<ApiResponse<TemplateDetailDto>> Detail(string templateId)
    {
        var data = await _templateService.GetTemplateDetailAsync(templateId);
        return ApiResponse<TemplateDetailDto>.Ok(data);
    }

    [HttpGet("categories")]
    public async Task<ApiResponse<PagedResult<TemplateCategoryDto>>> Categories()
    {
        var data = await _templateService.GetCategoriesAsync();
        return ApiResponse<PagedResult<TemplateCategoryDto>>.Ok(data);
    }

    [HttpGet("tags")]
    public async Task<ApiResponse<PagedResult<TemplateTagDto>>> Tags([FromQuery] string? type, [FromQuery] PageRequest request)
    {
        var data = await _templateService.GetTagsAsync(type, request);
        return ApiResponse<PagedResult<TemplateTagDto>>.Ok(data);
    }

    [HttpPost("{templateId}/favorite")]
    public async Task<ApiResponse> Favorite(string templateId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _templateService.FavoriteAsync(userId, templateId);
        return ApiResponse.Ok();
    }

    [HttpDelete("{templateId}/favorite")]
    public async Task<ApiResponse> Unfavorite(string templateId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _templateService.UnfavoriteAsync(userId, templateId);
        return ApiResponse.Ok();
    }

    [HttpGet("favorites")]
    public async Task<ApiResponse<PagedResult<TemplateDto>>> Favorites([FromQuery] PageRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _templateService.GetFavoritesAsync(userId, request);
        return ApiResponse<PagedResult<TemplateDto>>.Ok(data);
    }

    [HttpPost("{templateId}/use")]
    public async Task<ApiResponse<string>> Use(string templateId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var diagramId = await _templateService.UseTemplateAsync(userId, templateId);
        return ApiResponse<string>.Ok(diagramId);
    }
}
