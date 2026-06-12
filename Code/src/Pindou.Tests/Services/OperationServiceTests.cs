using Moq;
using Pindou.Application.Common;
using Pindou.Domain.Entities.Operation;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Messaging;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class OperationServiceTests
{
    private readonly Mock<IRepository<Banner>> _bannerRepoMock;
    private readonly Mock<IRepository<SpecialTopic>> _specialTopicRepoMock;
    private readonly Mock<IRepository<OperationTopic>> _topicRepoMock;
    private readonly Mock<IRepository<PushRecord>> _pushRepoMock;
    private readonly OperationService _operationService;

    public OperationServiceTests()
    {
        _bannerRepoMock = new Mock<IRepository<Banner>>();
        _specialTopicRepoMock = new Mock<IRepository<SpecialTopic>>();
        _topicRepoMock = new Mock<IRepository<OperationTopic>>();
        _pushRepoMock = new Mock<IRepository<PushRecord>>();
        _operationService = new OperationService(
            _bannerRepoMock.Object,
            _specialTopicRepoMock.Object,
            _topicRepoMock.Object,
            _pushRepoMock.Object);
    }

    [Fact]
    public async Task GetActiveBannersAsync_ShouldReturnActiveBanners()
    {
        var now = DateTime.Now;
        var banners = new List<Banner>
        {
            new Banner
            {
                Id = "b1", Title = "活动", ImageUrl = "a.png", LinkType = "url",
                LinkValue = "https://example.com", Position = "home_top", Sort = 1,
                Status = "active", StartTime = now.AddDays(-1), EndTime = now.AddDays(1)
            }
        };
        _bannerRepoMock.Setup(r => r.GetListAsync(
                It.IsAny<Expression<Func<Banner, bool>>>(),
                It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(banners);

        var result = await _operationService.GetActiveBannersAsync("home_top");

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("活动", result[0].Title);
    }

    [Fact]
    public async Task GetActiveBannersAsync_ShouldReturnEmpty_WhenNoActiveBanners()
    {
        _bannerRepoMock.Setup(r => r.GetListAsync(
                It.IsAny<Expression<Func<Banner, bool>>>(),
                It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<Banner>());

        var result = await _operationService.GetActiveBannersAsync("home_top");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetActiveSpecialTopicsAsync_ShouldReturnTopics()
    {
        var now = DateTime.Now;
        var topics = new List<SpecialTopic>
        {
            new SpecialTopic
            {
                Id = "t1", Name = "春节专题", Description = "春节活动",
                CoverUrl = "cover.png", TemplateIds = "[\"tpl1\",\"tpl2\"]",
                StartTime = now.AddDays(-1), EndTime = now.AddDays(7), Status = 1
            }
        };
        _specialTopicRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<SpecialTopic, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<SpecialTopic, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((topics, 1));

        var result = await _operationService.GetActiveSpecialTopicsAsync(new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
        Assert.Equal("春节专题", result.List[0].Name);
        Assert.Equal(2, result.List[0].TemplateIds.Count);
    }

    [Fact]
    public async Task GetSpecialTopicAsync_ShouldReturnTopic()
    {
        var topic = new SpecialTopic
        {
            Id = "t1", Name = "春节专题", Description = "春节活动",
            CoverUrl = "cover.png", TemplateIds = "[\"tpl1\"]",
            StartTime = DateTime.Now.AddDays(-1), EndTime = DateTime.Now.AddDays(7), Status = 1
        };
        _specialTopicRepoMock.Setup(r => r.GetByIdAsync("t1")).ReturnsAsync(topic);

        var result = await _operationService.GetSpecialTopicAsync("t1");

        Assert.NotNull(result);
        Assert.Equal("春节专题", result.Name);
        Assert.Single(result.TemplateIds);
    }

    [Fact]
    public async Task GetSpecialTopicAsync_ShouldThrow_WhenNotFound()
    {
        _specialTopicRepoMock.Setup(r => r.GetByIdAsync("t1")).ReturnsAsync((SpecialTopic?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _operationService.GetSpecialTopicAsync("t1"));
        Assert.Contains("专题不存在", ex.Message);
    }

    [Fact]
    public async Task GetOfficialTopicsAsync_ShouldReturnOfficialTopics()
    {
        var topics = new List<OperationTopic>
        {
            new OperationTopic
            {
                Id = "t1", TopicId = "official1", Name = "官方话题", Description = "官方",
                CoverUrl = "cover.png", IsOfficial = 1, Status = "active", PostCount = 10
            }
        };
        _topicRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<OperationTopic, bool>>>()))
            .ReturnsAsync(topics);

        var result = await _operationService.GetOfficialTopicsAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("官方话题", result[0].Name);
        Assert.Equal(10, result[0].PostCount);
    }

    [Fact]
    public async Task GetActivePushCountAsync_ShouldReturnCount()
    {
        _pushRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<PushRecord, bool>>>()))
            .ReturnsAsync(3);

        var result = await _operationService.GetActivePushCountAsync();

        Assert.Equal(3, result);
    }
}
