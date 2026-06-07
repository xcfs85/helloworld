using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Community;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class TopicServiceTests
{
    private readonly Mock<IRepository<Topic>> _topicRepoMock;
    private readonly Mock<IRepository<Post>> _postRepoMock;
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly Mock<IRepository<Like>> _likeRepoMock;
    private readonly Mock<IRepository<Favorite>> _favoriteRepoMock;
    private readonly TopicService _topicService;

    public TopicServiceTests()
    {
        _topicRepoMock = new Mock<IRepository<Topic>>();
        _postRepoMock = new Mock<IRepository<Post>>();
        _userRepoMock = new Mock<IRepository<User>>();
        _likeRepoMock = new Mock<IRepository<Like>>();
        _favoriteRepoMock = new Mock<IRepository<Favorite>>();
        _topicService = new TopicService(_topicRepoMock.Object, _postRepoMock.Object, _userRepoMock.Object, _likeRepoMock.Object, _favoriteRepoMock.Object);
    }

    #region GetHotTopicsAsync Tests

    [Fact]
    public async Task GetHotTopicsAsync_ShouldReturnHotTopics()
    {
        var topics = new List<Topic>
        {
            new Topic { Id = "t1", Name = "热门话题", PostCount = 100, IsHot = true, Status = "active" }
        };
        _topicRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Topic, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Topic, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((topics, 1));

        var result = await _topicService.GetHotTopicsAsync(new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
        Assert.True(result.List[0].IsHot);
    }

    #endregion

    #region GetTopicAsync Tests

    [Fact]
    public async Task GetTopicAsync_ShouldReturnTopic_WhenExists()
    {
        var topic = new Topic { Id = "t1", Name = "话题1", Description = "描述", CoverUrl = "cover.png", PostCount = 50, IsHot = true };
        _topicRepoMock.Setup(r => r.GetByIdAsync("t1")).ReturnsAsync(topic);

        var result = await _topicService.GetTopicAsync("t1");

        Assert.NotNull(result);
        Assert.Equal("t1", result.Id);
        Assert.Equal("话题1", result.Name);
        Assert.Equal(50, result.PostCount);
    }

    [Fact]
    public async Task GetTopicAsync_ShouldThrow_WhenNotFound()
    {
        _topicRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((Topic?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _topicService.GetTopicAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetTopicPostsAsync Tests

    [Fact]
    public async Task GetTopicPostsAsync_ShouldReturnPosts()
    {
        var posts = new List<Post>
        {
            new Post
            {
                Id = "p1", UserId = "u1", Type = "work", Title = "作品", Content = "内容",
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now,
                TopicIds = "[\"t1\"]"
            }
        };
        var user = new User { Id = "u1", Nickname = "作者", IsMember = false };

        _postRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Post, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Post, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((posts, 1));
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);

        var result = await _topicService.GetTopicPostsAsync("t1", new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
    }

    #endregion
}