using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Operation;
using Pindou.Domain.Entities.Operation;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Operation;
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
    public async Task RecordAsync_ShouldRecordLog()
    {
        _logRepoMock.Setup(r => r.InsertAsync(It.IsAny<OperationLog>())).ReturnsAsync(1L);

        await _operationLogService.RecordAsync("admin1", "admin", "用户管理", "删除用户", "删除用户 user1", "{\"userId\":\"user1\"}", "成功");

        _logRepoMock.Verify(r => r.InsertAsync(It.IsAny<OperationLog>()), Times.Once);
    }

    [Fact]
    public async Task RecordAsync_ShouldRecordFailedLog()
    {
        _logRepoMock.Setup(r => r.InsertAsync(It.IsAny<OperationLog>())).ReturnsAsync(1L);

        await _operationLogService.RecordAsync("admin1", "admin", "用户管理", "创建用户", "创建用户失败", "{\"username\":\"test\"}", "失败", "用户名已存在");

        _logRepoMock.Verify(r => r.InsertAsync(It.IsAny<OperationLog>()), Times.Once);
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
                Id = 1, AdminUserId = "admin1", AdminUsername = "admin",
                Module = "用户管理", Action = "删除用户", Description = "描述",
                RequestParams = "{}", Result = "成功", Ip = "127.0.0.1",
                CreateTime = DateTime.Now
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
        Assert.Equal("admin", result.List[0].AdminUsername);
    }

    [Fact]
    public async Task GetListAsync_ShouldFilterByModule()
    {
        _logRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<OperationLog, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<OperationLog, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((new List<OperationLog>(), 0));

        var query = new OperationLogQuery { Page = 1, Size = 10, Module = "用户管理" };
        var result = await _operationLogService.GetListAsync(query);

        Assert.NotNull(result);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task GetListAsync_ShouldFilterByAdminUserId()
    {
        _logRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<OperationLog, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<OperationLog, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((new List<OperationLog>(), 0));

        var query = new OperationLogQuery { Page = 1, Size = 10, AdminUserId = "admin1" };
        var result = await _operationLogService.GetListAsync(query);

        Assert.NotNull(result);
        Assert.Equal(0, result.Total);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldDeleteLog()
    {
        _logRepoMock.Setup(r => r.DeleteAsync(It.IsAny<object>())).ReturnsAsync(true);

        var result = await _operationLogService.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        _logRepoMock.Setup(r => r.DeleteAsync(It.IsAny<object>())).ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<BizException>(() => _operationLogService.DeleteAsync(999));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region ClearAsync Tests

    [Fact]
    public async Task ClearAsync_ShouldClearOldLogs()
    {
        _logRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<OperationLog, bool>>>())).ReturnsAsync(true);

        var result = await _operationLogService.ClearAsync(30);

        Assert.True(result);
    }

    [Fact]
    public async Task ClearAsync_ShouldClearAllLogs()
    {
        _logRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<OperationLog, bool>>>())).ReturnsAsync(true);

        var result = await _operationLogService.ClearAsync(null);

        Assert.True(result);
    }

    #endregion
}