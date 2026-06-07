using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.DTOs.System;
using Pindou.Application.Interfaces.Community;
using Pindou.Application.Interfaces.System;
using Pindou.Shared.Attributes;

namespace Pindou.Admin.Api.Controllers;

[ApiController]
[Route("api/admin/v1/content")]
[Permission("content:view")]
public class ContentController : ControllerBase
{
    private readonly IContentReviewService _reviewService;
    private readonly IPostService _postService;

    public ContentController(IContentReviewService reviewService, IPostService postService)
    {
        _reviewService = reviewService;
        _postService = postService;
    }

    /// <summary>待审核帖子列表</summary>
    [HttpGet("posts/pending")]
    public async Task<ApiResponse<PagedResult<PostDto>>> PendingPosts([FromQuery] PageRequest request)
    {
        var data = await _reviewService.GetPendingPostsAsync(request);
        return ApiResponse<PagedResult<PostDto>>.Ok(data);
    }

    /// <summary>帖子详情</summary>
    [HttpGet("posts/{id}")]
    public async Task<ApiResponse<PostDetailDto>> PostDetail(string id)
    {
        var data = await _postService.GetPostDetailAsync(string.Empty, id);
        return ApiResponse<PostDetailDto>.Ok(data);
    }

    /// <summary>审核通过帖子</summary>
    [HttpPost("posts/{id}/approve")]
    [Permission("content:approve")]
    [OperationLog("审核通过帖子")]
    public async Task<ApiResponse> ApprovePost(string id)
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        await _reviewService.ReviewPostAsync(id, adminId.ToString(), true);
        return ApiResponse.Ok();
    }

    /// <summary>驳回帖子</summary>
    [HttpPost("posts/{id}/reject")]
    [Permission("content:reject")]
    [OperationLog("驳回帖子")]
    public async Task<ApiResponse> RejectPost(string id, [FromBody] RejectPostRequest request)
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        await _reviewService.ReviewPostAsync(id, adminId.ToString(), false, request.Reason);
        return ApiResponse.Ok();
    }

    /// <summary>批量审核通过</summary>
    [HttpPost("posts/batch-approve")]
    [Permission("content:approve")]
    [OperationLog("批量审核通过帖子")]
    public async Task<ApiResponse> BatchApprove([FromBody] BatchReviewRequest request)
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        foreach (var postId in request.PostIds)
        {
            await _reviewService.ReviewPostAsync(postId, adminId.ToString(), true);
        }
        return ApiResponse.Ok();
    }

    /// <summary>批量驳回</summary>
    [HttpPost("posts/batch-reject")]
    [Permission("content:reject")]
    [OperationLog("批量驳回帖子")]
    public async Task<ApiResponse> BatchReject([FromBody] BatchReviewRequest request)
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        foreach (var postId in request.PostIds)
        {
            await _reviewService.ReviewPostAsync(postId, adminId.ToString(), false, request.Reason);
        }
        return ApiResponse.Ok();
    }

    /// <summary>评论列表</summary>
    [HttpGet("comments")]
    public async Task<ApiResponse<PagedResult<CommentDto>>> Comments([FromQuery] CommentAdminQuery query)
    {
        var data = await _postService.GetCommentsAsync(query.PostId ?? string.Empty, query);
        return ApiResponse<PagedResult<CommentDto>>.Ok(data);
    }

    /// <summary>隐藏评论</summary>
    [HttpPost("comments/{id}/hide")]
    [Permission("content:hide-comment")]
    [OperationLog("隐藏评论")]
    public async Task<ApiResponse> HideComment(string id)
    {
        await _postService.DeleteCommentAsync(string.Empty, id);
        return ApiResponse.Ok();
    }

    /// <summary>敏感词列表</summary>
    [HttpGet("sensitive-words")]
    public async Task<ApiResponse<List<SensitiveWordDto>>> SensitiveWords([FromQuery] string? type)
    {
        var data = await _reviewService.GetSensitiveWordsAsync(type);
        return ApiResponse<List<SensitiveWordDto>>.Ok(data);
    }

    /// <summary>添加敏感词</summary>
    [HttpPost("sensitive-words")]
    [Permission("content:sensitive-word")]
    [OperationLog("添加敏感词", SaveParams = true)]
    public async Task<ApiResponse<string>> AddSensitiveWord([FromBody] AddSensitiveWordRequest request)
    {
        var id = await _reviewService.AddSensitiveWordAsync(request);
        return ApiResponse<string>.Ok(id);
    }

    /// <summary>批量添加敏感词</summary>
    [HttpPost("sensitive-words/batch-add")]
    [Permission("content:sensitive-word")]
    [OperationLog("批量添加敏感词")]
    public async Task<ApiResponse<List<string>>> BatchAddSensitiveWords([FromBody] List<AddSensitiveWordRequest> requests)
    {
        var ids = new List<string>();
        foreach (var req in requests)
        {
            var id = await _reviewService.AddSensitiveWordAsync(req);
            ids.Add(id);
        }
        return ApiResponse<List<string>>.Ok(ids);
    }

    /// <summary>更新敏感词</summary>
    [HttpPut("sensitive-words/{id}")]
    [Permission("content:sensitive-word")]
    [OperationLog("更新敏感词", SaveParams = true)]
    public async Task<ApiResponse> UpdateSensitiveWord(string id, [FromBody] AddSensitiveWordRequest request)
    {
        await _reviewService.UpdateSensitiveWordAsync(id, request);
        return ApiResponse.Ok();
    }

    /// <summary>删除敏感词</summary>
    [HttpDelete("sensitive-words/{id}")]
    [Permission("content:sensitive-word")]
    [OperationLog("删除敏感词")]
    public async Task<ApiResponse> DeleteSensitiveWord(string id)
    {
        await _reviewService.DeleteSensitiveWordAsync(id);
        return ApiResponse.Ok();
    }

    /// <summary>举报列表</summary>
    [HttpGet("reports")]
    public async Task<ApiResponse<PagedResult<ReportDto>>> Reports([FromQuery] ReportQuery query)
    {
        var data = await _reviewService.GetReportsAsync(query);
        return ApiResponse<PagedResult<ReportDto>>.Ok(data);
    }

    /// <summary>处理举报</summary>
    [HttpPost("reports/{id}/handle")]
    [Permission("content:handle-report")]
    [OperationLog("处理举报")]
    public async Task<ApiResponse> HandleReport(string id, [FromBody] HandleReportRequest request)
    {
        var adminId = long.Parse(HttpContext.Items["AdminId"]?.ToString() ?? "0");
        await _reviewService.HandleReportAsync(id, adminId.ToString(), request.Action, request.Result);
        return ApiResponse.Ok();
    }
}

public class RejectPostRequest
{
    public string? Reason { get; set; }
}

public class BatchReviewRequest
{
    public List<string> PostIds { get; set; } = new();
    public string? Reason { get; set; }
}

public class CommentAdminQuery : PageRequest
{
    public string? PostId { get; set; }
}

public class HandleReportRequest
{
    public string Action { get; set; } = "processed";
    public string? Result { get; set; }
}