using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.DTOs.System;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.System;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.System;
using System.Linq.Expressions;
using UserEntity = Pindou.Domain.Entities.User.User;

namespace Pindou.Tests.Services;

public class ContentReviewServiceTests
{
    private readonly Mock<IRepository<SensitiveWord>> _sensitiveRepoMock;
    private readonly Mock<IRepository<Report>> _reportRepoMock;
    private readonly Mock<IRepository<PostReviewLog>> _reviewLogRepoMock;
    private readonly Mock<IRepository<Post>> _postRepoMock;
    private readonly Mock<IRepository<UserEntity>> _userRepoMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly ContentReviewService _contentReviewService;

    public ContentReviewServiceTests()
    {
        _sensitiveRepoMock = new Mock<IRepository<SensitiveWord>>();
        _reportRepoMock = new Mock<IRepository<Report>>();
        _reviewLogRepoMock = new Mock<IRepository<PostReviewLog>>();
        _postRepoMock = new Mock<IRepository<Post>>();
        _userRepoMock = new Mock<IRepository<UserEntity>>();
        _cacheMock = new Mock<ICacheService>();
        _contentReviewService = new ContentReviewService(
            _sensitiveRepoMock.Object, _reportRepoMock.Object, _reviewLogRepoMock.Object,
            _postRepoMock.Object, _userRepoMock.Object, _cacheMock.Object);
    }

    #region CheckAsync Tests

    [Fact]
    public async Task CheckAsync_ShouldPass_WhenNoSensitiveWords()
    {
        _sensitiveRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<SensitiveWord, bool>>>()))
            .ReturnsAsync(new List<SensitiveWord>());

        var (passed, reason, replaced) = await _contentReviewService.CheckAsync("正常内容");

        Assert.True(passed);
        Assert.Empty(reason);
    }

    [Fact]
    public async Task CheckAsync_ShouldBlock_WhenLevel3Word()
    {
        _sensitiveRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<SensitiveWord, bool>>>()))
            .ReturnsAsync(new List<SensitiveWord>
            {
                new SensitiveWord { Word = "badword", Level = 3, Type = "other", Status = 1 }
            });

        var (passed, reason, replaced) = await _contentReviewService.CheckAsync("包含badword的内容");

        Assert.False(passed);
        Assert.Contains("badword", reason);
    }

    [Fact]
    public async Task CheckAsync_ShouldReplace_WhenLevel2Word()
    {
        _sensitiveRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<SensitiveWord, bool>>>()))
            .ReturnsAsync(new List<SensitiveWord>
            {
                new SensitiveWord { Word = "badword", Level = 2, ReplaceWord = "***", Type = "other", Status = 1 }
            });

        var (passed, reason, replaced) = await _contentReviewService.CheckAsync("包含badword的内容");

        Assert.True(passed);
        Assert.NotNull(replaced);
        Assert.Contains("***", replaced);
    }

    [Fact]
    public async Task CheckAsync_ShouldPass_WhenLevel1Word()
    {
        _sensitiveRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<SensitiveWord, bool>>>()))
            .ReturnsAsync(new List<SensitiveWord>
            {
                new SensitiveWord { Word = "warning", Level = 1, Type = "other", Status = 1 }
            });

        var (passed, reason, replaced) = await _contentReviewService.CheckAsync("包含warning的内容");

        Assert.True(passed);
    }

    #endregion

    #region ReviewPostAsync Tests

    [Fact]
    public async Task ReviewPostAsync_ShouldApprove()
    {
        var post = new Post { Id = "p1", ReviewStatus = "pending", Status = "active" };
        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(post);
        _postRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Post>())).ReturnsAsync(true);
        _reviewLogRepoMock.Setup(r => r.InsertAsync(It.IsAny<PostReviewLog>())).ReturnsAsync("l1");

        var result = await _contentReviewService.ReviewPostAsync("p1", "r1", true);

        Assert.True(result);
        Assert.Equal("approved", post.ReviewStatus);
    }

    [Fact]
    public async Task ReviewPostAsync_ShouldReject()
    {
        var post = new Post { Id = "p1", ReviewStatus = "pending", Status = "active" };
        _postRepoMock.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(post);
        _postRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Post>())).ReturnsAsync(true);
        _reviewLogRepoMock.Setup(r => r.InsertAsync(It.IsAny<PostReviewLog>())).ReturnsAsync("l1");

        var result = await _contentReviewService.ReviewPostAsync("p1", "r1", false, "内容违规");

        Assert.True(result);
        Assert.Equal("rejected", post.ReviewStatus);
        Assert.Equal("内容违规", post.ReviewReason);
    }

    [Fact]
    public async Task ReviewPostAsync_ShouldThrow_WhenPostNotFound()
    {
        _postRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((Post?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _contentReviewService.ReviewPostAsync("nonexistent", "r1", true));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetAdminPostsAsync Tests

    [Fact]
    public async Task GetAdminPostsAsync_ShouldReturnPendingPosts()
    {
        var posts = new List<Post>
        {
            new Post { Id = "p1", UserId = "u1", Type = "work", Title = "待审核", Content = "内容", Status = "active", ReviewStatus = "pending", RiskLevel = "none", PublishTime = DateTime.Now }
        };
        _postRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Post, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Post, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((posts, 1));
        _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync((UserEntity?)null);

        var result = await _contentReviewService.GetAdminPostsAsync(new PostAdminQuery { ReviewStatus = "pending", Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
        Assert.Equal("pending", result.List[0].ReviewStatus);
    }

    #endregion

    #region HandleReportAsync Tests

    [Fact]
    public async Task HandleReportAsync_ShouldHandleReport()
    {
        var report = new Report { Id = "r1", ReportId = "RP001", Status = "pending" };
        _reportRepoMock.Setup(r => r.GetByIdAsync("r1")).ReturnsAsync(report);
        _reportRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Report>())).ReturnsAsync(true);

        var result = await _contentReviewService.HandleReportAsync("r1", "h1", "warned", "已警告");

        Assert.True(result);
        Assert.Equal("warned", report.Status);
    }

    [Fact]
    public async Task HandleReportAsync_ShouldThrow_WhenNotFound()
    {
        _reportRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((Report?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _contentReviewService.HandleReportAsync("nonexistent", "h1", "warned"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetReportsAsync Tests

    [Fact]
    public async Task GetReportsAsync_ShouldReturnReports()
    {
        var reports = new List<Report>
        {
            new Report
            {
                Id = "r1", ReportId = "RP001", ReporterId = "u1", TargetType = "post",
                TargetId = "p1", TargetUserId = "u2", Reason = "违规", Status = "pending",
                CreateTime = DateTime.Now
            }
        };
        _reportRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Report, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Report, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((reports, 1));

        var query = new ReportQuery { Page = 1, Size = 10 };
        var result = await _contentReviewService.GetReportsAsync(query);

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
    }

    #endregion

    #region GetSensitiveWordsAsync Tests

    [Fact]
    public async Task GetSensitiveWordsAsync_ShouldReturnWords()
    {
        _sensitiveRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<SensitiveWord, bool>>>()))
            .ReturnsAsync(new List<SensitiveWord>
            {
                new SensitiveWord { Id = "w1", Word = "badword", Level = 2, Type = "other", ReplaceWord = "***", Status = 1 }
            });

        var result = await _contentReviewService.GetSensitiveWordsAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("badword", result[0].Word);
    }

    [Fact]
    public async Task GetSensitiveWordsAsync_ShouldFilterByType()
    {
        _sensitiveRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<SensitiveWord, bool>>>()))
            .ReturnsAsync(new List<SensitiveWord>());

        var result = await _contentReviewService.GetSensitiveWordsAsync("politics");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region AddSensitiveWordAsync Tests

    [Fact]
    public async Task AddSensitiveWordAsync_ShouldAddWord()
    {
        _sensitiveRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<SensitiveWord, bool>>>())).ReturnsAsync(false);
        _sensitiveRepoMock.Setup(r => r.InsertAsync(It.IsAny<SensitiveWord>()))
            .Callback<SensitiveWord>(w => w.Id = "w1")
            .ReturnsAsync("w1");

        var request = new AddSensitiveWordRequest { Word = "badword", Level = 2, Type = "other" };
        var result = await _contentReviewService.AddSensitiveWordAsync(request);

        Assert.NotNull(result);
        Assert.Equal("w1", result);
    }

    [Fact]
    public async Task AddSensitiveWordAsync_ShouldThrow_WhenAlreadyExists()
    {
        _sensitiveRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<SensitiveWord, bool>>>())).ReturnsAsync(true);

        var request = new AddSensitiveWordRequest { Word = "badword", Level = 2, Type = "other" };
        var ex = await Assert.ThrowsAsync<BizException>(() => _contentReviewService.AddSensitiveWordAsync(request));
        Assert.Contains("已存在", ex.Message);
    }

    #endregion

    #region UpdateSensitiveWordAsync Tests

    [Fact]
    public async Task UpdateSensitiveWordAsync_ShouldUpdateWord()
    {
        var word = new SensitiveWord { Id = "w1", Word = "old", Level = 1, Type = "other", Status = 1 };
        _sensitiveRepoMock.Setup(r => r.GetByIdAsync("w1")).ReturnsAsync(word);
        _sensitiveRepoMock.Setup(r => r.UpdateAsync(It.IsAny<SensitiveWord>())).ReturnsAsync(true);

        var request = new AddSensitiveWordRequest { Word = "new", Level = 2, Type = "politics", Status = 1 };
        var result = await _contentReviewService.UpdateSensitiveWordAsync("w1", request);

        Assert.True(result);
        Assert.Equal("new", word.Word);
    }

    [Fact]
    public async Task UpdateSensitiveWordAsync_ShouldThrow_WhenNotFound()
    {
        _sensitiveRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((SensitiveWord?)null);

        var request = new AddSensitiveWordRequest { Word = "new", Level = 2, Type = "other" };
        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _contentReviewService.UpdateSensitiveWordAsync("nonexistent", request));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region DeleteSensitiveWordAsync Tests

    [Fact]
    public async Task DeleteSensitiveWordAsync_ShouldSoftDelete()
    {
        var word = new SensitiveWord { Id = "w1", Word = "badword", Status = 1 };
        _sensitiveRepoMock.Setup(r => r.GetByIdAsync("w1")).ReturnsAsync(word);
        _sensitiveRepoMock.Setup(r => r.UpdateAsync(It.IsAny<SensitiveWord>())).ReturnsAsync(true);

        var result = await _contentReviewService.DeleteSensitiveWordAsync("w1");

        Assert.True(result);
        Assert.Equal(0, word.Status);
    }

    [Fact]
    public async Task DeleteSensitiveWordAsync_ShouldThrow_WhenNotFound()
    {
        _sensitiveRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((SensitiveWord?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            _contentReviewService.DeleteSensitiveWordAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion
}