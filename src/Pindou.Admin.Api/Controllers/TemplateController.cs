using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Template;
using Pindou.Application.Interfaces.Template;
using Pindou.Shared.Attributes;

namespace Pindou.Admin.Api.Controllers;

[ApiController]
[Route("api/admin/v1/template")]
[Permission("template:view")]
public class TemplateController : ControllerBase
{
    private readonly ITemplateService _templateService;

    public TemplateController(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    /// <summary>模板列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<TemplateDto>>> List([FromQuery] TemplateQuery query)
    {
        var data = await _templateService.GetTemplatesAsync(query);
        return ApiResponse<PagedResult<TemplateDto>>.Ok(data);
    }

    /// <summary>待审核模板</summary>
    [HttpGet("pending")]
    public async Task<ApiResponse<PagedResult<TemplateDto>>> Pending([FromQuery] TemplateQuery query)
    {
        var data = await _templateService.GetTemplatesAsync(query);
        return ApiResponse<PagedResult<TemplateDto>>.Ok(data);
    }

    /// <summary>模板详情</summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<TemplateDetailDto>> Detail(string id)
    {
        var data = await _templateService.GetTemplateDetailAsync(id);
        return ApiResponse<TemplateDetailDto>.Ok(data);
    }

    /// <summary>审核通过模板</summary>
    [HttpPost("{id}/approve")]
    [Permission("template:approve")]
    [OperationLog("审核通过模板")]
    public async Task<ApiResponse> Approve(string id)
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        var data = await _templateService.GetTemplateDetailAsync(id);
        return ApiResponse.Ok();
    }

    /// <summary>驳回模板</summary>
    [HttpPost("{id}/reject")]
    [Permission("template:reject")]
    [OperationLog("驳回模板")]
    public async Task<ApiResponse> Reject(string id, [FromBody] RejectTemplateRequest request)
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        var data = await _templateService.GetTemplateDetailAsync(id);
        return ApiResponse.Ok();
    }

    /// <summary>发布模板</summary>
    [HttpPost("{id}/publish")]
    [Permission("template:publish")]
    [OperationLog("发布模板")]
    public async Task<ApiResponse> Publish(string id)
    {
        var data = await _templateService.GetTemplateDetailAsync(id);
        return ApiResponse.Ok();
    }

    /// <summary>下架模板</summary>
    [HttpPost("{id}/unpublish")]
    [Permission("template:unpublish")]
    [OperationLog("下架模板")]
    public async Task<ApiResponse> Unpublish(string id)
    {
        var data = await _templateService.GetTemplateDetailAsync(id);
        return ApiResponse.Ok();
    }

    /// <summary>设为精选</summary>
    [HttpPost("{id}/feature")]
    [Permission("template:feature")]
    [OperationLog("设为精选模板")]
    public async Task<ApiResponse> Feature(string id)
    {
        var data = await _templateService.GetTemplateDetailAsync(id);
        return ApiResponse.Ok();
    }

    /// <summary>取消精选</summary>
    [HttpPost("{id}/unfeature")]
    [Permission("template:unfeature")]
    [OperationLog("取消精选模板")]
    public async Task<ApiResponse> Unfeature(string id)
    {
        var data = await _templateService.GetTemplateDetailAsync(id);
        return ApiResponse.Ok();
    }

    /// <summary>创建模板</summary>
    [HttpPost]
    [Permission("template:add")]
    [OperationLog("创建模板", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateTemplateRequest request)
    {
        var data = await _templateService.GetTemplateDetailAsync(request.Name);
        return ApiResponse<string>.Ok(string.Empty);
    }
}

[ApiController]
[Route("api/admin/v1/template-category")]
[Permission("template-category:view")]
public class TemplateCategoryController : ControllerBase
{
    private readonly ITemplateService _templateService;

    public TemplateCategoryController(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    /// <summary>分类列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<TemplateCategoryDto>>> List()
    {
        var data = await _templateService.GetCategoriesAsync();
        return ApiResponse<PagedResult<TemplateCategoryDto>>.Ok(data);
    }

    /// <summary>创建分类</summary>
    [HttpPost]
    [Permission("template-category:add")]
    [OperationLog("创建模板分类", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateTemplateCategoryRequest request)
    {
        var data = await _templateService.GetCategoriesAsync();
        return ApiResponse<string>.Ok(string.Empty);
    }

    /// <summary>更新分类</summary>
    [HttpPut("{id}")]
    [Permission("template-category:edit")]
    [OperationLog("更新模板分类", SaveParams = true)]
    public async Task<ApiResponse> Update(string id, [FromBody] CreateTemplateCategoryRequest request)
    {
        var data = await _templateService.GetCategoriesAsync();
        return ApiResponse.Ok();
    }

    /// <summary>删除分类</summary>
    [HttpDelete("{id}")]
    [Permission("template-category:delete")]
    [OperationLog("删除模板分类")]
    public async Task<ApiResponse> Delete(string id)
    {
        var data = await _templateService.GetCategoriesAsync();
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/admin/v1/template-tag")]
[Permission("template-tag:view")]
public class TemplateTagController : ControllerBase
{
    private readonly ITemplateService _templateService;

    public TemplateTagController(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    /// <summary>标签列表</summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<TemplateTagDto>>> List([FromQuery] PageRequest request, [FromQuery] string? type)
    {
        var data = await _templateService.GetTagsAsync(type, request);
        return ApiResponse<PagedResult<TemplateTagDto>>.Ok(data);
    }

    /// <summary>创建标签</summary>
    [HttpPost]
    [Permission("template-tag:add")]
    [OperationLog("创建模板标签", SaveParams = true)]
    public async Task<ApiResponse<string>> Create([FromBody] CreateTemplateTagRequest request)
    {
        var data = await _templateService.GetTagsAsync(null, new PageRequest());
        return ApiResponse<string>.Ok(string.Empty);
    }

    /// <summary>更新标签</summary>
    [HttpPut("{id}")]
    [Permission("template-tag:edit")]
    [OperationLog("更新模板标签", SaveParams = true)]
    public async Task<ApiResponse> Update(string id, [FromBody] CreateTemplateTagRequest request)
    {
        var data = await _templateService.GetTagsAsync(null, new PageRequest());
        return ApiResponse.Ok();
    }

    /// <summary>删除标签</summary>
    [HttpDelete("{id}")]
    [Permission("template-tag:delete")]
    [OperationLog("删除模板标签")]
    public async Task<ApiResponse> Delete(string id)
    {
        var data = await _templateService.GetTagsAsync(null, new PageRequest());
        return ApiResponse.Ok();
    }
}

public class RejectTemplateRequest
{
    public string? Reason { get; set; }
}

public class CreateTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public string BoardSize { get; set; } = string.Empty;
    public int BeadCount { get; set; }
    public string Difficulty { get; set; } = "easy";
    public int TotalColors { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? PreviewUrls { get; set; }
}

public class CreateTemplateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int Sort { get; set; }
}

public class CreateTemplateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Type { get; set; }
}