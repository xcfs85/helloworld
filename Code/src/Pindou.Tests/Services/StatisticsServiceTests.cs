using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Statistics;
using Pindou.Domain.Entities.Statistics;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.Creation;
using Pindou.Domain.Entities.Member;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Statistics;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class StatisticsServiceTests
{
    private readonly Mock<IRepository<DailyStats>> _statsRepoMock;
    private readonly Mock<IRepository<Diagram>> _diagramRepoMock;
    private readonly Mock<IRepository<Post>> _postRepoMock;
    private readonly Mock<IRepository<Comment>> _commentRepoMock;
    private readonly Mock<IRepository<Like>> _likeRepoMock;
    private readonly Mock<IRepository<Order>> _orderRepoMock;
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly StatisticsService _statisticsService;

    public StatisticsServiceTests()
    {
        _statsRepoMock = new Mock<IRepository<DailyStats>>();
        _diagramRepoMock = new Mock<IRepository<Diagram>>();
        _postRepoMock = new Mock<IRepository<Post>>();
        _commentRepoMock = new Mock<IRepository<Comment>>();
        _likeRepoMock = new Mock<IRepository<Like>>();
        _orderRepoMock = new Mock<IRepository<Order>>();
        _userRepoMock = new Mock<IRepository<User>>();
        _statisticsService = new StatisticsService(
            _statsRepoMock.Object, _diagramRepoMock.Object, _postRepoMock.Object,
            _commentRepoMock.Object, _likeRepoMock.Object, _orderRepoMock.Object, _userRepoMock.Object);
    }

    #region RecordGenerationAsync Tests

    [Fact]
    public async Task RecordGenerationAsync_ShouldCreateNewStats_WhenFirstToday()
    {
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync((DailyStats?)null);
        _statsRepoMock.Setup(r => r.InsertAsync(It.IsAny<DailyStats>())).ReturnsAsync(1L);

        await _statisticsService.RecordGenerationAsync("u1", 100, 5);

        _statsRepoMock.Verify(r => r.InsertAsync(It.IsAny<DailyStats>()), Times.Once);
    }

    [Fact]
    public async Task RecordGenerationAsync_ShouldUpdateExistingStats()
    {
        var stats = new DailyStats { Id = 1, StatDate = DateTime.Now.Date, GenerationCount = 5, AvgBeadCount = 80, AvgColorCount = 4 };
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync(stats);
        _statsRepoMock.Setup(r => r.UpdateAsync(It.IsAny<DailyStats>())).ReturnsAsync(true);

        await _statisticsService.RecordGenerationAsync("u1", 100, 5);

        _statsRepoMock.Verify(r => r.UpdateAsync(It.IsAny<DailyStats>()), Times.Once);
    }

    #endregion

    #region RecordExportAsync Tests

    [Fact]
    public async Task RecordExportAsync_ShouldIncrementExportCount()
    {
        var stats = new DailyStats { Id = 1, StatDate = DateTime.Now.Date, ExportCount = 0 };
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync(stats);
        _statsRepoMock.Setup(r => r.UpdateAsync(It.IsAny<DailyStats>())).ReturnsAsync(true);

        await _statisticsService.RecordExportAsync("u1");

        _statsRepoMock.Verify(r => r.UpdateAsync(It.IsAny<DailyStats>()), Times.Once);
    }

    #endregion

    #region RecordPostAsync Tests

    [Fact]
    public async Task RecordPostAsync_ShouldRecordPost()
    {
        var stats = new DailyStats { Id = 1, StatDate = DateTime.Now.Date, PostCount = 0, WorkCount = 0 };
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync(stats);
        _statsRepoMock.Setup(r => r.UpdateAsync(It.IsAny<DailyStats>())).ReturnsAsync(true);

        await _statisticsService.RecordPostAsync("u1", "work");

        _statsRepoMock.Verify(r => r.UpdateAsync(It.IsAny<DailyStats>()), Times.Once);
    }

    #endregion

    #region RecordCommentAsync Tests

    [Fact]
    public async Task RecordCommentAsync_ShouldIncrementCommentCount()
    {
        var stats = new DailyStats { Id = 1, StatDate = DateTime.Now.Date, CommentCount = 0 };
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync(stats);
        _statsRepoMock.Setup(r => r.UpdateAsync(It.IsAny<DailyStats>())).ReturnsAsync(true);

        await _statisticsService.RecordCommentAsync("u1", "p1");

        _statsRepoMock.Verify(r => r.UpdateAsync(It.IsAny<DailyStats>()), Times.Once);
    }

    #endregion

    #region RecordLikeAsync Tests

    [Fact]
    public async Task RecordLikeAsync_ShouldIncrementLikeCount()
    {
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync((DailyStats?)null);
        _statsRepoMock.Setup(r => r.InsertAsync(It.IsAny<DailyStats>())).ReturnsAsync(1L);

        await _statisticsService.RecordLikeAsync("u1", "post");

        _statsRepoMock.Verify(r => r.InsertAsync(It.IsAny<DailyStats>()), Times.Once);
    }

    #endregion

    #region RecordShareAsync Tests

    [Fact]
    public async Task RecordShareAsync_ShouldIncrementShareCount()
    {
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync((DailyStats?)null);
        _statsRepoMock.Setup(r => r.InsertAsync(It.IsAny<DailyStats>())).ReturnsAsync(1L);

        await _statisticsService.RecordShareAsync("u1", "post");

        _statsRepoMock.Verify(r => r.InsertAsync(It.IsAny<DailyStats>()), Times.Once);
    }

    #endregion

    #region RecordFavoriteAsync Tests

    [Fact]
    public async Task RecordFavoriteAsync_ShouldIncrementFavoriteCount()
    {
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync((DailyStats?)null);
        _statsRepoMock.Setup(r => r.InsertAsync(It.IsAny<DailyStats>())).ReturnsAsync(1L);

        await _statisticsService.RecordFavoriteAsync("u1", "post");

        _statsRepoMock.Verify(r => r.InsertAsync(It.IsAny<DailyStats>()), Times.Once);
    }

    #endregion

    #region RecordMemberOrderAsync Tests

    [Fact]
    public async Task RecordMemberOrderAsync_ShouldRecord()
    {
        var stats = new DailyStats { Id = 1, StatDate = DateTime.Now.Date, MemberOrderCount = 0, MemberRevenue = 0 };
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync(stats);
        _statsRepoMock.Setup(r => r.UpdateAsync(It.IsAny<DailyStats>())).ReturnsAsync(true);

        await _statisticsService.RecordMemberOrderAsync(29.9m);

        _statsRepoMock.Verify(r => r.UpdateAsync(It.IsAny<DailyStats>()), Times.Once);
    }

    #endregion

    #region GetDailyStatsAsync Tests

    [Fact]
    public async Task GetDailyStatsAsync_ShouldReturnStats_WhenExists()
    {
        var stats = new DailyStats
        {
            Id = 1, StatDate = DateTime.Now.Date, Dau = 100, NewUserCount = 10,
            GenerationCount = 50, AvgBeadCount = 200, AvgColorCount = 5
        };
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync(stats);

        var result = await _statisticsService.GetDailyStatsAsync(DateTime.Now);

        Assert.NotNull(result);
        Assert.Equal(100, result.Dau);
        Assert.Equal(10, result.NewUserCount);
        Assert.Equal(50, result.GenerationCount);
    }

    [Fact]
    public async Task GetDailyStatsAsync_ShouldReturnEmpty_WhenNotExists()
    {
        _statsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<DailyStats, bool>>>()))
            .ReturnsAsync((DailyStats?)null);

        var result = await _statisticsService.GetDailyStatsAsync(DateTime.Now);

        Assert.NotNull(result);
        Assert.Equal(0, result.Dau);
    }

    #endregion

    #region GetOverviewAsync Tests

    [Fact]
    public async Task GetOverviewAsync_ShouldReturnOverview()
    {
        _userRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(100);
        _diagramRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Diagram, bool>>>())).ReturnsAsync(500);
        _postRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Post, bool>>>())).ReturnsAsync(200);
        _orderRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<Order> { new Order { Amount = 29.9m }, new Order { Amount = 59.9m } });

        var result = await _statisticsService.GetOverviewAsync(null, null);

        Assert.NotNull(result);
        Assert.Equal(100, result.TotalUsers);
        Assert.Equal(500, result.TotalDiagrams);
        Assert.Equal(200, result.TotalPosts);
        Assert.Equal(89.8m, result.TotalRevenue);
    }

    #endregion
}