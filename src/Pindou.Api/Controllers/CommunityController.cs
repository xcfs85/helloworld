using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.DTOs.User;
using Pindou.Application.Interfaces.Community;

namespace Pindou.Api.Controllers;

[ApiController]
[Route("api/v1/post")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    public PostController(IPostService postService) { _postService = postService; }

    /// <summary>发布帖子</summary>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreatePostRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var id = await _postService.CreatePostAsync(userId, request);
        return ApiResponse<string>.Ok(id);
    }

    /// <summary>信息流</summary>
    [HttpGet("feed")]
    public async Task<ApiResponse<PagedResult<PostDto>>> Feed([FromQuery] FeedQuery query)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _postService.GetFeedAsync(userId, query);
        return ApiResponse<PagedResult<PostDto>>.Ok(data);
    }

    /// <summary>用户帖子</summary>
    [HttpGet("user/{userId}")]
    public async Task<ApiResponse<PagedResult<PostDto>>> GetUserPosts(string userId, [FromQuery] PageRequest request)
    {
        var data = await _postService.GetUserPostsAsync(userId, request);
        return ApiResponse<PagedResult<PostDto>>.Ok(data);
    }

    /// <summary>帖子详情</summary>
    [HttpGet("{postId}")]
    public async Task<ApiResponse<PostDetailDto>> Detail(string postId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _postService.GetPostDetailAsync(userId, postId);
        return ApiResponse<PostDetailDto>.Ok(data);
    }

    /// <summary>删除帖子</summary>
    [HttpDelete("{postId}")]
    public async Task<ApiResponse> Delete(string postId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _postService.DeletePostAsync(userId, postId);
        return ApiResponse.Ok();
    }

    /// <summary>点赞</summary>
    [HttpPost("{postId}/like")]
    public async Task<ApiResponse> Like(string postId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _postService.LikeAsync(userId, postId, "post");
        return ApiResponse.Ok();
    }

    /// <summary>取消点赞</summary>
    [HttpDelete("{postId}/like")]
    public async Task<ApiResponse> Unlike(string postId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _postService.UnlikeAsync(userId, postId, "post");
        return ApiResponse.Ok();
    }

    /// <summary>收藏</summary>
    [HttpPost("{postId}/favorite")]
    public async Task<ApiResponse> Favorite(string postId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _postService.FavoriteAsync(userId, postId, "post");
        return ApiResponse.Ok();
    }

    /// <summary>取消收藏</summary>
    [HttpDelete("{postId}/favorite")]
    public async Task<ApiResponse> Unfavorite(string postId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _postService.UnfavoriteAsync(userId, postId, "post");
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/v1/comment")]
public class CommentController : ControllerBase
{
    private readonly IPostService _postService;
    public CommentController(IPostService postService) { _postService = postService; }

    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateCommentRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var id = await _postService.CreateCommentAsync(userId, request);
        return ApiResponse<string>.Ok(id);
    }

    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<CommentDto>>> List([FromQuery] string postId, [FromQuery] PageRequest request)
    {
        var data = await _postService.GetCommentsAsync(postId, request);
        return ApiResponse<PagedResult<CommentDto>>.Ok(data);
    }

    [HttpDelete("{commentId}")]
    public async Task<ApiResponse> Delete(string commentId)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _postService.DeleteCommentAsync(userId, commentId);
        return ApiResponse.Ok();
    }
}

[ApiController]
[Route("api/v1/follow")]
public class FollowController : ControllerBase
{
    private readonly IFollowService _followService;
    public FollowController(IFollowService followService) { _followService = followService; }

    [HttpPost("{userId}")]
    public async Task<ApiResponse> Follow(string userId)
    {
        var me = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _followService.FollowAsync(me, userId);
        return ApiResponse.Ok();
    }

    [HttpDelete("{userId}")]
    public async Task<ApiResponse> Unfollow(string userId)
    {
        var me = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        await _followService.UnfollowAsync(me, userId);
        return ApiResponse.Ok();
    }

    [HttpGet("following")]
    public async Task<ApiResponse<PagedResult<UserListDto>>> Following([FromQuery] PageRequest request)
    {
        var me = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _followService.GetFollowListAsync(me, request);
        return ApiResponse<PagedResult<UserListDto>>.Ok(data);
    }

    [HttpGet("fans")]
    public async Task<ApiResponse<PagedResult<UserListDto>>> Fans([FromQuery] PageRequest request)
    {
        var me = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _followService.GetFansListAsync(me, request);
        return ApiResponse<PagedResult<UserListDto>>.Ok(data);
    }
}

[ApiController]
[Route("api/v1/topic")]
public class TopicController : ControllerBase
{
    private readonly ITopicService _topicService;
    public TopicController(ITopicService topicService) { _topicService = topicService; }

    [HttpGet("hot")]
    public async Task<ApiResponse<PagedResult<TopicDto>>> Hot([FromQuery] PageRequest request)
    {
        var data = await _topicService.GetHotTopicsAsync(request);
        return ApiResponse<PagedResult<TopicDto>>.Ok(data);
    }

    [HttpGet("{topicId}")]
    public async Task<ApiResponse<TopicDto>> Get(string topicId)
    {
        var data = await _topicService.GetTopicAsync(topicId);
        return ApiResponse<TopicDto>.Ok(data);
    }

    [HttpGet("{topicId}/posts")]
    public async Task<ApiResponse<PagedResult<PostDto>>> Posts(string topicId, [FromQuery] PageRequest request)
    {
        var data = await _topicService.GetTopicPostsAsync(topicId, request);
        return ApiResponse<PagedResult<PostDto>>.Ok(data);
    }
}
