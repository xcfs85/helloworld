using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;

namespace Pindou.Application.Interfaces.Community;

public interface IPostService
{
    Task<string> CreatePostAsync(string userId, CreatePostRequest request);
    Task<PagedResult<PostDto>> GetFeedAsync(string userId, FeedQuery query);
    Task<PagedResult<PostDto>> GetUserPostsAsync(string userId, PageRequest request);
    Task<PostDetailDto> GetPostDetailAsync(string userId, string postId);
    Task<bool> DeletePostAsync(string userId, string postId);
    Task<bool> LikeAsync(string userId, string targetId, string targetType);
    Task<bool> UnlikeAsync(string userId, string targetId, string targetType);
    Task<bool> FavoriteAsync(string userId, string targetId, string targetType);
    Task<bool> UnfavoriteAsync(string userId, string targetId, string targetType);
    Task<string> CreateCommentAsync(string userId, CreateCommentRequest request);
    Task<PagedResult<CommentDto>> GetCommentsAsync(string postId, PageRequest request);
    Task<bool> DeleteCommentAsync(string userId, string commentId);
}

public interface IFollowService
{
    Task<bool> FollowAsync(string userId, string followUserId);
    Task<bool> UnfollowAsync(string userId, string followUserId);
    Task<PagedResult<DTOs.User.UserListDto>> GetFollowListAsync(string userId, PageRequest request);
    Task<PagedResult<DTOs.User.UserListDto>> GetFansListAsync(string userId, PageRequest request);
}

public interface ITopicService
{
    Task<PagedResult<DTOs.Community.TopicDto>> GetHotTopicsAsync(PageRequest request);
    Task<DTOs.Community.TopicDto> GetTopicAsync(string topicId);
    Task<PagedResult<PostDto>> GetTopicPostsAsync(string topicId, PageRequest request);
}
