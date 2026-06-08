using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Admin;
using Pindou.Domain.Entities.Admin;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Admin;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class OperationLogServiceTests
{
    private readonly Mock<IRepository<OperationLog>> _logRepoMock;
    private readonly OperationLogService _operationLogService;

    public OperationLogServiceTests()
    {
        _logRepoMock = new Mock<IRepository<OperationLog>>();
        _operationLogService = new OperationLogService(_logRepoMock.Object);
    }

    #region RecordAsync Tests

    [Fact]
    public async Task RecordAsync_ShouldInsertLog()
    {
        _logRepoMock.Setup(r => r.InsertAsync(It.IsAny<OperationLog>())).ReturnsAsync(1L);

        await _operationLogService.RecordAsync(1, "admin", "管理员", "删除用户", "删除用户 user1", "UserService.Delete", "{\"userId\":\"user1\"}", "127.0.0.1", "Mozilla/5.0");

        _logRepoMock.Verify(r => r.InsertAsync(It.Is<OperationLog>(l =>
            l.UserId == 1 &&
            l.Username == "admin" &&
            l.Operation == "删除用户" &&
            l.Ip == "127.0.0.1")), Times.Once);
    }

    [Fact]
    public async Task RecordAsync_ShouldAcceptNullOptionalFields()
    {
        _logRepoMock.Setup(r => r.InsertAsync(It.IsAny<OperationLog>())).ReturnsAsync(1L);

        await _operationLogService.RecordAsync(1, "admin", null, "login", null, null, null, null, null);

        _logRepoMock.Verify(r => r.InsertAsync(It.Is<OperationLog>(l =>
            l.Nickname == null && l.Ip == null)), Times.Once);
    }

    #endregion

    #region GetListAsync Tests

    [Fact]
    public async Task GetListAsync_ShouldReturnPagedLogs()
    {
        var logs = new List<OperationLog>
        {
            new OperationLog
            {
                Id = 1, UserId = 1, Username = "admin", Nickname = "管理员",
                Operation = "删除用户", Content = "描述", Method = "UserService.Delete",
                Params = "{}", Ip = "127.0.0.1", CreateTime = DateTime.Now
            }
        };
        _logRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<OperationLog, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<OperationLog, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((logs, 1));

        var query = new OperationLogQuery { Page = 1, Size = 10 };
        var result = await _operationLogService.GetListAsync(query);

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
        Assert.Equal("admin", result.List[0].Username);
    }

    [Fact]
    public async Task GetListAsync_ShouldFilterByUserId()
    {
        Expression<Func<OperationLog, bool>>? capturedExpr = null;
        _logRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<OperationLog, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<OperationLog, object>>>(),
                It.IsAny<bool>()))
            .Callback<Expression<Func<OperationLog, bool>>, int, int, Expression<Func<OperationLog, object>>, bool>(
                (expr, _, _, _, _) => capturedExpr = expr)
            .ReturnsAsync((new List<OperationLog>(), 0));

        var query = new OperationLogQuery { Page = 1, Size = 10, UserId = 5 };
        var result = await _operationLogService.GetListAsync(query);

        Assert.NotNull(result);
        Assert.Equal(0, result.Total);
        Assert.NotNull(capturedExpr);
    }

    [Fact]
    public async Task GetListAsync_ShouldFilterByOperation()
    {
        _logRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<OperationLog, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<OperationLog, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((new List<OperationLog>(), 0));

        var query = new OperationLogQuery { Page = 1, Size = 10, Operation = "login" };
        var result = await _operationLogService.GetListAsync(query);

        Assert.NotNull(result);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task GetListAsync_ShouldFilterByTimeRange()
    {
        _logRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<OperationLog, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<OperationLog, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((new List<OperationLog>(), 0));

        var start = DateTime.Now.AddDays(-7);
        var end = DateTime.Now;
        var query = new OperationLogQuery { Page = 1, Size = 10, StartTime = start, EndTime = end };
        var result = await _operationLogService.GetListAsync(query);

        Assert.NotNull(result);
        Assert.Equal(0, result.Total);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldDeleteLog()
    {
        var log = new OperationLog { Id = 1, Operation = "login", UserId = 1, Username = "admin" };
        _logRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(log);
        _logRepoMock.Setup(r => r.DeleteAsync(1L)).ReturnsAsync(true);

        var result = await _operationLogService.DeleteAsync(1);

        Assert.True(result);
        _logRepoMock.Verify(r => r.DeleteAsync(1L), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        _logRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((OperationLog?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _operationLogService.DeleteAsync(999));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region ClearAsync Tests

    [Fact]
    public async Task ClearAsync_ShouldClearOldLogs_WithBeforeTime()
    {
        _logRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<OperationLog, bool>>>())).ReturnsAsync(true);

        var result = await _operationLogService.ClearAsync(DateTime.Now.AddDays(-30));

        Assert.True(result);
    }

    [Fact]
    public async Task ClearAsync_ShouldUseDefaultCutoff_WhenBeforeTimeIsNull()
    {
        _logRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<OperationLog, bool>>>())).ReturnsAsync(true);

        var result = await _operationLogService.ClearAsync(null);

        Assert.True(result);
    }

    #endregion
}
