using Microsoft.Extensions.Logging;
using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Creation;
using Pindou.Domain.Entities.Creation;
using Pindou.Infrastructure.ExternalServices.AI;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Creation;

namespace Pindou.Tests.Services;

public class DiagramServiceTests
{
    private readonly Mock<IRepository<Diagram>> _diagramRepoMock;
    private readonly Mock<IRepository<ColorInfo>> _colorInfoRepoMock;
    private readonly Mock<IRepository<DiagramTask>> _taskRepoMock;
    private readonly Mock<IAiGenerationService> _aiServiceMock;
    private readonly Mock<ILogger<DiagramService>> _loggerMock;
    private readonly DiagramService _diagramService;

    public DiagramServiceTests()
    {
        _diagramRepoMock = new Mock<IRepository<Diagram>>();
        _colorInfoRepoMock = new Mock<IRepository<ColorInfo>>();
        _taskRepoMock = new Mock<IRepository<DiagramTask>>();
        _aiServiceMock = new Mock<IAiGenerationService>();
        _loggerMock = new Mock<ILogger<DiagramService>>();
        _diagramService = new DiagramService(
            _diagramRepoMock.Object, _colorInfoRepoMock.Object, _taskRepoMock.Object,
            _aiServiceMock.Object, _loggerMock.Object);
    }

    #region CreateGenerationTaskAsync Tests

    [Fact]
    public async Task CreateGenerationTaskAsync_ShouldCreateTask()
    {
        _taskRepoMock.Setup(r => r.InsertAsync(It.IsAny<DiagramTask>()))
            .Callback<DiagramTask>(t => t.Id = "task1")
            .ReturnsAsync("task1");

        var request = new CreateDiagramRequest
        {
            SourceImageUrl = "image.png", BoardSize = "29x29", Difficulty = "easy",
            Style = "pixel", IsSync = false
        };
        var result = await _diagramService.CreateGenerationTaskAsync("u1", request);

        Assert.NotNull(result);
        Assert.Equal("task1", result);
    }

    [Fact]
    public async Task CreateGenerationTaskAsync_ShouldProcessSyncTask()
    {
        var task = new DiagramTask
        {
            Id = "task1", UserId = "u1", Status = "pending",
            SourceImageUrl = "image.png", Params = "{\"BoardSize\":\"29x29\",\"Difficulty\":\"easy\",\"Style\":\"pixel\"}",
            IsSync = true
        };
        _taskRepoMock.Setup(r => r.InsertAsync(It.IsAny<DiagramTask>()))
            .Callback<DiagramTask>(t => t.Id = "task1")
            .ReturnsAsync("task1");
        _taskRepoMock.Setup(r => r.GetByIdAsync("task1")).ReturnsAsync(task);
        _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<DiagramTask>())).ReturnsAsync(true);
        _aiServiceMock.Setup(a => a.GenerateSyncAsync(It.IsAny<AiGenerationRequest>()))
            .ReturnsAsync(new AiGenerationResult
            {
                Success = true, DiagramId = "d1", PreviewUrl = "preview.png",
                BeadCount = 100, ColorCount = 5, ColorInfos = new List<ColorMapping>
                {
                    new ColorMapping { ColorIndex = 1, ColorCode = "M01", ColorName = "Red", Rgb = "FF0000", BeadCount = 50, Percentage = 50 }
                }
            });
        _diagramRepoMock.Setup(r => r.InsertAsync(It.IsAny<Diagram>()))
            .Callback<Diagram>(d => d.Id = "d1")
            .ReturnsAsync("d1");
        _colorInfoRepoMock.Setup(r => r.InsertRangeAsync(It.IsAny<List<ColorInfo>>()))
            .ReturnsAsync(new List<object> { "c1" });

        var request = new CreateDiagramRequest
        {
            SourceImageUrl = "image.png", BoardSize = "29x29", Difficulty = "easy",
            Style = "pixel", IsSync = true
        };
        var result = await _diagramService.CreateGenerationTaskAsync("u1", request);

        Assert.Equal("task1", result);
    }

    #endregion

    #region GetTaskStatusAsync Tests

    [Fact]
    public async Task GetTaskStatusAsync_ShouldReturnStatus()
    {
        var task = new DiagramTask
        {
            Id = "task1", UserId = "u1", Status = "processing", Progress = 50,
            CurrentStage = "图像处理", DiagramId = "d1", ErrorMessage = null
        };
        _taskRepoMock.Setup(r => r.GetByIdAsync("task1")).ReturnsAsync(task);

        var result = await _diagramService.GetTaskStatusAsync("u1", "task1");

        Assert.NotNull(result);
        Assert.Equal("task1", result.TaskId);
        Assert.Equal("processing", result.Status);
        Assert.Equal(50, result.Progress);
    }

    [Fact]
    public async Task GetTaskStatusAsync_ShouldThrow_WhenTaskNotFound()
    {
        _taskRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((DiagramTask?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _diagramService.GetTaskStatusAsync("u1", "nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    [Fact]
    public async Task GetTaskStatusAsync_ShouldThrow_WhenNotOwner()
    {
        var task = new DiagramTask { Id = "task1", UserId = "u2", Status = "pending" };
        _taskRepoMock.Setup(r => r.GetByIdAsync("task1")).ReturnsAsync(task);

        var ex = await Assert.ThrowsAsync<BizException>(() => _diagramService.GetTaskStatusAsync("u1", "task1"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GenerateSyncAsync Tests

    [Fact]
    public async Task GenerateSyncAsync_ShouldGenerateAndReturnDiagramId()
    {
        var task = new DiagramTask
        {
            Id = "task1", UserId = "u1", Status = "pending",
            SourceImageUrl = "image.png", Params = "{\"BoardSize\":\"29x29\",\"Difficulty\":\"easy\",\"Style\":\"pixel\"}",
            IsSync = true
        };
        _taskRepoMock.Setup(r => r.InsertAsync(It.IsAny<DiagramTask>()))
            .Callback<DiagramTask>(t => t.Id = "task1")
            .ReturnsAsync("task1");
        _taskRepoMock.Setup(r => r.GetByIdAsync("task1")).ReturnsAsync(task);
        _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<DiagramTask>())).ReturnsAsync(true);
        _aiServiceMock.Setup(a => a.GenerateSyncAsync(It.IsAny<AiGenerationRequest>()))
            .ReturnsAsync(new AiGenerationResult
            {
                Success = true, DiagramId = "d1", PreviewUrl = "preview.png",
                BeadCount = 100, ColorCount = 5
            });
        _diagramRepoMock.Setup(r => r.InsertAsync(It.IsAny<Diagram>()))
            .Callback<Diagram>(d => d.Id = "d1")
            .ReturnsAsync("d1");

        var request = new CreateDiagramRequest
        {
            SourceImageUrl = "image.png", BoardSize = "29x29", Difficulty = "easy", Style = "pixel"
        };
        var result = await _diagramService.GenerateSyncAsync("u1", request);

        Assert.Equal("d1", result);
    }

    [Fact]
    public async Task GenerateSyncAsync_ShouldThrow_WhenGenerationFails()
    {
        var task = new DiagramTask
        {
            Id = "task1", UserId = "u1", Status = "failed",
            SourceImageUrl = "image.png", Params = "{\"BoardSize\":\"29x29\",\"Difficulty\":\"easy\",\"Style\":\"pixel\"}",
            ErrorMessage = "AI生成失败", IsSync = true
        };
        _taskRepoMock.Setup(r => r.InsertAsync(It.IsAny<DiagramTask>()))
            .Callback<DiagramTask>(t => t.Id = "task1")
            .ReturnsAsync("task1");
        _taskRepoMock.SetupSequence(r => r.GetByIdAsync("task1"))
            .ReturnsAsync((DiagramTask?)null) // first call in ProcessTaskAsync
            .ReturnsAsync(task); // second call after ProcessTaskAsync
        _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<DiagramTask>())).ReturnsAsync(true);
        _aiServiceMock.Setup(a => a.GenerateSyncAsync(It.IsAny<AiGenerationRequest>()))
            .ReturnsAsync(new AiGenerationResult { Success = false, ErrorMessage = "AI生成失败" });

        var request = new CreateDiagramRequest
        {
            SourceImageUrl = "image.png", BoardSize = "29x29", Difficulty = "easy", Style = "pixel"
        };
        var ex = await Assert.ThrowsAsync<BizException>(() => _diagramService.GenerateSyncAsync("u1", request));
        Assert.Contains("生成失败", ex.Message);
    }

    #endregion
}