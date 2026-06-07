using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.Interfaces.Community;
using Pindou.Domain.Entities.Community;
using Pindou.Infrastructure.Repositories;

namespace Pindou.Infrastructure.Services.Community;

public class PostService : IPostService
{
    private readonly IRepository<Post> _postRepo;
    public PostService(IRepository<Post> postRepo) { _postRepo = postRepo; }

    public Task<string> CreatePostAsync(string userId, CreatePostRequest request) { throw new NotImplementedException(); }
    public Task<PagedResult<PostDto>> GetFeedAsync(string userId, FeedQuery query) { throw new NotImplementedException(); }
    public Task<PagedResult<PostDto>> GetUserPostsAsync(string userId, PageRequest request) { throw new NotImplementedException(); }
    public Task<PostDetailDto> GetPostDetailAsync(string userId, string postId) { throw new NotImplementedException(); }
    public Task<bool> DeletePostAsync(string userId, string postId) { throw new NotImplementedException(); }
    public Task<bool> LikeAsync(string userId, string targetId, string targetType) { throw new NotImplementedException(); }
    public Task<bool> UnlikeAsync(string userId, string targetId, string targetType) { throw new NotImplementedException(); }
    public Task<bool> FavoriteAsync(string userId, string targetId, string targetType) { throw new NotImplementedException(); }
    public Task<bool> UnfavoriteAsync(string userId, string targetId, string targetType) { throw new NotImplementedException(); }
    public Task<string> CreateCommentAsync(string userId, CreateCommentRequest request) { throw new NotImplementedException(); }
    public Task<PagedResult<CommentDto>> GetCommentsAsync(string postId, PageRequest request) { throw new NotImplementedException(); }
    public Task<bool> DeleteCommentAsync(string userId, string commentId) { throw new NotImplementedException(); }
}

public class FollowService : IFollowService
{
    public Task<bool> FollowAsync(string userId, string followUserId) { throw new NotImplementedException(); }
    public Task<bool> UnfollowAsync(string userId, string followUserId) { throw new NotImplementedException(); }
    public Task<PagedResult<DTOs.User.UserListDto>> GetFollowListAsync(string userId, PageRequest request) { throw new NotImplementedException(); }
    public Task<PagedResult<DTOs.User.UserListDto>> GetFansListAsync(string userId, PageRequest request) { throw new NotImplementedException(); }
}

public class TopicService : ITopicService
{
    public Task<PagedResult<TopicDto>> GetHotTopicsAsync(PageRequest request) { throw new NotImplementedException(); }
    public Task<TopicDto> GetTopicAsync(string topicId) { throw new NotImplementedException(); }
    public Task<PagedResult<PostDto>> GetTopicPostsAsync(string topicId, PageRequest request) { throw new NotImplementedException(); }
}
