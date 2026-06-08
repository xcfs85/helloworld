using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.System;
using Pindou.Domain.Entities.System;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.System;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class SystemConfigServiceTests
{
    private readonly Mock<IRepository<SystemConfig>> _configRepoMock;
    private readonly Mock<IRepository<MardColor>> _mardRepoMock;
    private readonly Mock<IRepository<BeadKit>> _kitRepoMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly SystemConfigService _systemConfigService;

    public SystemConfigServiceTests()
    {
        _configRepoMock = new Mock<IRepository<SystemConfig>>();
        _mardRepoMock = new Mock<IRepository<MardColor>>();
        _kitRepoMock = new Mock<IRepository<BeadKit>>();
        _cacheMock = new Mock<ICacheService>();
        _systemConfigService = new SystemConfigService(
            _configRepoMock.Object, _mardRepoMock.Object, _kitRepoMock.Object, _cacheMock.Object);
    }

    #region GetAsync(string key) Tests

    [Fact]
    public async Task GetAsync_ShouldReturnValue_FromCache()
    {
        _cacheMock.Setup(c => c.GetStringAsync("sys:config:key1")).ReturnsAsync("value1");

        var result = await _systemConfigService.GetAsync("key1");

        Assert.Equal("value1", result);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnValue_FromDb_WhenNotInCache()
    {
        _cacheMock.Setup(c => c.GetStringAsync("sys:config:key1")).ReturnsAsync((string?)null);
        _configRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<SystemConfig, bool>>>()))
            .ReturnsAsync(new SystemConfig { ConfigKey = "key1", ConfigValue = "dbValue" });
        _cacheMock.Setup(c => c.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        var result = await _systemConfigService.GetAsync("key1");

        Assert.Equal("dbValue", result);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenNotFound()
    {
        _cacheMock.Setup(c => c.GetStringAsync("sys:config:missing")).ReturnsAsync((string?)null);
        _configRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<SystemConfig, bool>>>()))
            .ReturnsAsync((SystemConfig?)null);

        var result = await _systemConfigService.GetAsync("missing");

        Assert.Null(result);
    }

    #endregion

    #region GetAsync<T>(string key) Tests

    [Fact]
    public async Task GetAsyncT_ShouldDeserializeValue()
    {
        _cacheMock.Setup(c => c.GetStringAsync("sys:config:num")).ReturnsAsync("42");

        var result = await _systemConfigService.GetAsync<int>("num");

        Assert.Equal(42, result);
    }

    [Fact]
    public async Task GetAsyncT_ShouldReturnDefault_WhenNotFound()
    {
        _cacheMock.Setup(c => c.GetStringAsync("sys:config:missing")).ReturnsAsync((string?)null);
        _configRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<SystemConfig, bool>>>()))
            .ReturnsAsync((SystemConfig?)null);

        var result = await _systemConfigService.GetAsync<int>("missing");

        Assert.Equal(0, result);
    }

    #endregion

    #region SetAsync Tests

    [Fact]
    public async Task SetAsync_ShouldCreateNewConfig_WhenNotExists()
    {
        _configRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<SystemConfig, bool>>>()))
            .ReturnsAsync((SystemConfig?)null);
        _configRepoMock.Setup(r => r.InsertAsync(It.IsAny<SystemConfig>())).ReturnsAsync("c1");
        _cacheMock.Setup(c => c.RemoveAsync(It.IsAny<string>())).ReturnsAsync(true);

        await _systemConfigService.SetAsync("key1", "value1", "string", "description");

        _configRepoMock.Verify(r => r.InsertAsync(It.IsAny<SystemConfig>()), Times.Once);
    }

    [Fact]
    public async Task SetAsync_ShouldUpdateExistingConfig()
    {
        var config = new SystemConfig { Id = "c1", ConfigKey = "key1", ConfigValue = "old" };
        _configRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<SystemConfig, bool>>>()))
            .ReturnsAsync(config);
        _configRepoMock.Setup(r => r.UpdateAsync(It.IsAny<SystemConfig>())).ReturnsAsync(true);
        _cacheMock.Setup(c => c.RemoveAsync(It.IsAny<string>())).ReturnsAsync(true);

        await _systemConfigService.SetAsync("key1", "new");

        _configRepoMock.Verify(r => r.UpdateAsync(It.IsAny<SystemConfig>()), Times.Once);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllConfigs()
    {
        var configs = new List<SystemConfig>
        {
            new SystemConfig { Id = "c1", ConfigKey = "key1", ConfigValue = "val1", ConfigType = "string", Status = 1, CreateTime = DateTime.Now }
        };
        _configRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<SystemConfig, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(configs);

        var result = await _systemConfigService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("key1", result[0].ConfigKey);
    }

    #endregion

    #region GetMardColorAsync Tests

    [Fact]
    public async Task GetMardColorAsync_ShouldReturnColor_FromCache()
    {
        _cacheMock.Setup(c => c.GetStringAsync("sys:mard:M01")).ReturnsAsync("{\"Id\":\"c1\",\"ColorNo\":\"M01\",\"ColorName\":\"Red\"}");

        var result = await _systemConfigService.GetMardColorAsync("M01");

        Assert.NotNull(result);
        Assert.Contains("M01", result);
    }

    [Fact]
    public async Task GetMardColorAsync_ShouldReturnColor_FromDb()
    {
        _cacheMock.Setup(c => c.GetStringAsync("sys:mard:M01")).ReturnsAsync((string?)null);
        _mardRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MardColor, bool>>>()))
            .ReturnsAsync(new MardColor { Id = "c1", ColorNo = "M01", ColorName = "Red", Rgb = "FF0000", IsCommon = 1, Status = 1 });
        _cacheMock.Setup(c => c.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        var result = await _systemConfigService.GetMardColorAsync("M01");

        Assert.NotNull(result);
        Assert.Contains("M01", result);
    }

    [Fact]
    public async Task GetMardColorAsync_ShouldReturnNull_WhenNotFound()
    {
        _cacheMock.Setup(c => c.GetStringAsync("sys:mard:unknown")).ReturnsAsync((string?)null);
        _mardRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MardColor, bool>>>()))
            .ReturnsAsync((MardColor?)null);

        var result = await _systemConfigService.GetMardColorAsync("unknown");

        Assert.Null(result);
    }

    #endregion

    #region GetAllMardColorsAsync Tests

    [Fact]
    public async Task GetAllMardColorsAsync_ShouldReturnColors()
    {
        _cacheMock.Setup(c => c.GetAsync<List<MardColorDto>>("sys:mard:all")).ReturnsAsync((List<MardColorDto>?)null);
        _mardRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<MardColor, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<MardColor>
            {
                new MardColor { Id = "c1", ColorNo = "M01", ColorName = "Red", Rgb = "FF0000", IsCommon = 1, Status = 1 }
            });
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        var result = await _systemConfigService.GetAllMardColorsAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("M01", result[0].ColorNo);
    }

    [Fact]
    public async Task GetAllMardColorsAsync_ShouldReturnFromCache()
    {
        var cached = new List<MardColorDto>
        {
            new MardColorDto { Id = "c1", ColorNo = "M01", ColorName = "Red", Rgb = "FF0000" }
        };
        _cacheMock.Setup(c => c.GetAsync<List<MardColorDto>>("sys:mard:all")).ReturnsAsync(cached);

        var result = await _systemConfigService.GetAllMardColorsAsync();

        Assert.NotNull(result);
        Assert.Single(result);
    }

    #endregion

    #region GetAllBeadKitsAsync Tests

    [Fact]
    public async Task GetAllBeadKitsAsync_ShouldReturnKits()
    {
        _kitRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<BeadKit, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<BeadKit>
            {
                new BeadKit { Id = "k1", KitId = "kit1", KitName = "套装1", Brand = "MARD", ColorCount = 24, BeadCount = 1000, Price = 99.9m, Status = 1 }
            });

        var result = await _systemConfigService.GetAllBeadKitsAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("kit1", result[0].KitId);
    }

    [Fact]
    public async Task GetAllBeadKitsAsync_ShouldFilterByColorCount()
    {
        _kitRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<BeadKit, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<BeadKit>());

        var result = await _systemConfigService.GetAllBeadKitsAsync(48);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion
}