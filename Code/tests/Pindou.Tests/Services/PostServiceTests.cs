using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.Interfaces.System;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Community;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IRepository<Post>> _postRepoMock;
    private readonly Mock<IRepository<Comment>> _commentRepoMock;
    private readonly Mock<IRepository<Like>> _likeRepoMock;
    private readonly Mock<IRepository<Favorite>> _favoriteRepoMock;
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly Mock<IContentReviewService> _contentReviewMock;
    private readonly PostService _postService;

    public PostServiceTests()
    {
        _postRepoMock = new Mock<IRepository<Post>>();
        _commentRepoMock = new Mock<IRepository<Comment>>();
        _likeRepoMock = new Mock<IRepository<Like>>();
        _favoriteRepoMock = new Mock<IRepository<Favorite>>();
        _userRepoMock = new Mock<IRepository<User>>();
        _contentReviewMock = new Mock<IContentReviewService>();
        _postService = new PostService(
            _postRepoMock.Object, _commentRepoMock.Object, _likeRepoMock.Object,
            _favoriteRepoMock.Object, _userRepoMock.Object, _contentReviewMock.Object);
    }

    #region CreatePostAsync Tests

    [Fact]
    public async Task CreatePostAsync_ShouldCreatePost_WhenContentPassesReview()
    {
        _contentReviewMock.Setup(c => c.CheckAsync(It.IsAny<string>()))
            .ReturnsAsync((true, "", (string?)null));
        _postRepoMock.Setup(r => r.InsertAsync(It.IsAny<Post>())).ReturnsAsync("post1");

        var request = new CreatePostRequest { Type = "work", Title = "test", Content = "hello" };
        var result = await _postService.CreatePostAsync("u1", request);

        Assert.NotNull(result);
        Assert.Equal("post1", result);
    }

    [Fact]
    public async Task CreatePostAsync_ShouldThrow_WhenContentFailsReview()
    {
        _contentReviewMock.Setup(c => c.CheckAsync(It.IsAny<string>()))
            .ReturnsAsync((false, "含敏感词", (string?)null));

        var request = new CreatePostRequest { Type = "work", Title = "test", Content = "bad" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _postService.CreatePostAsync("u1", request));
        Assert.Contains("含敏感词", ex.Message);
    }

    #endregion

    #region GetFeedAsync Tests

    [Fact]
    public async Task GetFeedAsync_ShouldReturnPagedResult()
    {
        var posts = new List<Post>
        {
            new Post { Id = "p1", UserId = "u1", Type = "work", Title = "Post 1", Content = "content", Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now }
        };
        var user = new User { Id = "u1", Nickname = "author", IsMember = false };

        _postRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Post, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Post, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((posts, 1));
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _likeRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Like, bool>>>())).ReturnsAsync(false);
        _favoriteRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Favorite, bool>>>())).ReturnsAsync(false);

        var query = new FeedQuery { Page = 1, Size = 10 };
        var result = await _postService.GetFeedAsync("u1", query);

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
    }

    #endregion

    #region GetUserPostsAsync Tests

    [Fact]
    public async Task GetUserPostsAsync_ShouldReturnUserPosts()
    {
        var posts = new List<Post>
        {
            new Post { Id = "p1", UserId = "u1", Type = "work", Title = "Post 1", Content = "content", Status = "active", PublishTime = DateTime.Now }
        };
        var user = new User { Id = "u1", Nickname = "author" };

        _postRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Post, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Post, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((posts, 1));
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _likeRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Like, bool>>>())).ReturnsAsync(false);
        _favoriteRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Favorite, bool>>>())).ReturnsAsync(false);

        var result = await _postService.GetUserPostsAsync("u1", new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
    }

    #endregion

    #region GetPostDetailAsync Tests

    [Fact]
    public async Task GetPostDetailAsync_ShouldReturnPostDetail()
    {
        var post = new Post
        {
            Id = "p1", UserId = "u1", Type = "work", Title = "Post 1", Content = "content",
            Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now,
            TopicIds = "[\"t1\"]", BeadParams = "{\"size\":\"29x29\"}"
        };
        var user = new User { Id = "u1", Nickname = "author", IsMember = false };

        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(post);
        _postRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Post>())).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _likeRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Like, bool>>>())).ReturnsAsync(false);
        _favoriteRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Favorite, bool>>>())).ReturnsAsync(false);

        var result = await _postService.GetPostDetailAsync("u1", "p1");

        Assert.NotNull(result);
        Assert.Equal("p1", result.Id);
        Assert.Single(result.TopicIds);
    }

    [Fact]
    public async Task GetPostDetailAsync_ShouldThrow_WhenPostNotFound()
    {
        _postRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((Post?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _postService.GetPostDetailAsync("u1", "nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region DeletePostAsync Tests

    [Fact]
    public async Task DeletePostAsync_ShouldDeletePost_WhenOwner()
    {
        var post = new Post { Id = "p1", UserId = "u1", Status = "active" };
        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(post);
        _postRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Post>())).ReturnsAsync(true);

        var result = await _postService.DeletePostAsync("u1", "p1");

        Assert.True(result);
        Assert.Equal("deleted", post.Status);
    }

    [Fact]
    public async Task DeletePostAsync_ShouldThrow_WhenNotOwner()
    {
        var post = new Post { Id = "p1", UserId = "u2", Status = "active" };
        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(post);

        var ex = await Assert.ThrowsAsync<BizException>(() => _postService.DeletePostAsync("u1", "p1"));
        Assert.Contains("无权", ex.Message);
    }

    #endregion

    #region LikeAsync Tests

    [Fact]
    public async Task LikeAsync_ShouldLikePost()
    {
        _likeRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Like, bool>>>())).ReturnsAsync(false);
        _likeRepoMock.Setup(r => r.InsertAsync(It.IsAny<Like>())).ReturnsAsync("l1");
        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(new Post { Id = "p1", LikeCount = 0 });
        _postRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Post>())).ReturnsAsync(true);

        var result = await _postService.LikeAsync("u1", "p1", "post");

        Assert.True(result);
    }

    [Fact]
    public async Task LikeAsync_ShouldReturnTrue_WhenAlreadyLiked()
    {
        _likeRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Like, bool>>>())).ReturnsAsync(true);

        var result = await _postService.LikeAsync("u1", "p1", "post");

        Assert.True(result);
    }

    #endregion

    #region UnlikeAsync Tests

    [Fact]
    public async Task UnlikeAsync_ShouldUnlike()
    {
        var like = new Like { Id = "l1", UserId = "u1", TargetId = "p1", TargetType = "post" };
        _likeRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Like, bool>>>())).ReturnsAsync(like);
        _likeRepoMock.Setup(r => r.DeleteAsync(It.IsAny<object>())).ReturnsAsync(true);
        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(new Post { Id = "p1", LikeCount = 1 });
        _postRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Post>())).ReturnsAsync(true);

        var result = await _postService.UnlikeAsync("u1", "p1", "post");

        Assert.True(result);
    }

    [Fact]
    public async Task UnlikeAsync_ShouldReturnTrue_WhenNotLiked()
    {
        _likeRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Like, bool>>>())).ReturnsAsync((Like?)null);

        var result = await _postService.UnlikeAsync("u1", "p1", "post");

        Assert.True(result);
    }

    #endregion

    #region FavoriteAsync Tests

    [Fact]
    public async Task FavoriteAsync_ShouldFavorite()
    {
        _favoriteRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Favorite, bool>>>())).ReturnsAsync(false);
        _favoriteRepoMock.Setup(r => r.InsertAsync(It.IsAny<Favorite>())).ReturnsAsync("f1");
        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(new Post { Id = "p1", FavoriteCount = 0 });
        _postRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Post>())).ReturnsAsync(true);

        var result = await _postService.FavoriteAsync("u1", "p1", "post");

        Assert.True(result);
    }

    #endregion

    #region UnfavoriteAsync Tests

    [Fact]
    public async Task UnfavoriteAsync_ShouldUnfavorite()
    {
        var fav = new Favorite { Id = "f1", UserId = "u1", TargetId = "p1", TargetType = "post" };
        _favoriteRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Favorite, bool>>>())).ReturnsAsync(fav);
        _favoriteRepoMock.Setup(r => r.DeleteAsync(It.IsAny<object>())).ReturnsAsync(true);
        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(new Post { Id = "p1", FavoriteCount = 1 });
        _postRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Post>())).ReturnsAsync(true);

        var result = await _postService.UnfavoriteAsync("u1", "p1", "post");

        Assert.True(result);
    }

    #endregion

    #region CreateCommentAsync Tests

    [Fact]
    public async Task CreateCommentAsync_ShouldCreateComment()
    {
        var post = new Post { Id = "p1", Status = "active", CommentCount = 0 };
        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(post);
        _contentReviewMock.Setup(c => c.CheckAsync(It.IsAny<string>()))
            .ReturnsAsync((true, "", (string?)null));
        _commentRepoMock.Setup(r => r.InsertAsync(It.IsAny<Comment>())).ReturnsAsync("c1");
        _postRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Post>())).ReturnsAsync(true);

        var request = new CreateCommentRequest { PostId = "p1", Content = "nice" };
        var result = await _postService.CreateCommentAsync("u1", request);

        Assert.NotNull(result);
        Assert.Equal("c1", result);
    }

    [Fact]
    public async Task CreateCommentAsync_ShouldThrow_WhenPostNotFound()
    {
        _postRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((Post?)null);

        var request = new CreateCommentRequest { PostId = "nonexistent", Content = "nice" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _postService.CreateCommentAsync("u1", request));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetCommentsAsync Tests

    [Fact]
    public async Task GetCommentsAsync_ShouldReturnComments()
    {
        var comments = new List<Comment>
        {
            new Comment { Id = "c1", PostId = "p1", UserId = "u1", Content = "nice", Status = "active", CreateTime = DateTime.Now }
        };
        var user = new User { Id = "u1", Nickname = "user1" };

        _commentRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Comment, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Comment, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((comments, 1));
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
        _likeRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Like, bool>>>())).ReturnsAsync(false);

        var result = await _postService.GetCommentsAsync("p1", new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
    }

    #endregion

    #region DeleteCommentAsync Tests

    [Fact]
    public async Task DeleteCommentAsync_ShouldDeleteComment_WhenOwner()
    {
        var comment = new Comment { Id = "c1", UserId = "u1", PostId = "p1", Status = "active" };
        _commentRepoMock.Setup(r => r.GetByIdAsync("c1")).ReturnsAsync(comment);
        _commentRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Comment>())).ReturnsAsync(true);
        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(new Post { Id = "p1", CommentCount = 1 });
        _postRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Post>())).ReturnsAsync(true);

        var result = await _postService.DeleteCommentAsync("u1", "c1");

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCommentAsync_ShouldThrow_WhenNotOwner()
    {
        var comment = new Comment { Id = "c1", UserId = "u2", PostId = "p1", Status = "active" };
        _commentRepoMock.Setup(r => r.GetByIdAsync("c1")).ReturnsAsync(comment);

        var ex = await Assert.ThrowsAsync<BizException>(() => _postService.DeleteCommentAsync("u1", "c1"));
        Assert.Contains("无权", ex.Message);
    }

    #endregion
}