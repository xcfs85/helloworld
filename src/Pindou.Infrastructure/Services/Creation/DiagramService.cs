using Microsoft.Extensions.Logging;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Creation;
using Pindou.Application.Interfaces.Creation;
using Pindou.Domain.Entities.Creation;
using Pindou.Infrastructure.ExternalServices.AI;
using Pindou.Infrastructure.Repositories;

namespace Pindou.Infrastructure.Services.Creation;

/// <summary>
/// 拼豆图纸服务实现
/// </summary>
public class DiagramService : IDiagramService
{
    private readonly IRepository<Diagram> _diagramRepo;
    private readonly IRepository<ColorInfo> _colorInfoRepo;
    private readonly IRepository<DiagramTask> _taskRepo;
    private readonly IAiGenerationService _aiService;
    private readonly ILogger<DiagramService> _logger;

    public DiagramService(
        IRepository<Diagram> diagramRepo,
        IRepository<ColorInfo> colorInfoRepo,
        IRepository<DiagramTask> taskRepo,
        IAiGenerationService aiService,
        ILogger<DiagramService> logger)
    {
        _diagramRepo = diagramRepo;
        _colorInfoRepo = colorInfoRepo;
        _taskRepo = taskRepo;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<string> CreateGenerationTaskAsync(string userId, CreateDiagramRequest request)
    {
        // 创建任务
        var task = new DiagramTask
        {
            UserId = userId,
            Status = "pending",
            SourceImageUrl = request.SourceImageUrl,
            Params = System.Text.Json.JsonSerializer.Serialize(request),
            IsSync = request.IsSync
        };
        await _taskRepo.InsertAsync(task);

        if (request.IsSync)
        {
            // 同步处理
            await ProcessTaskAsync(task.Id);
        }
        else
        {
            // 异步处理 - 由BackgroundService消费
            // 实际生产使用Channel/Quartz
        }

        return task.Id;
    }

    public async Task<GenerationStatusResponse> GetTaskStatusAsync(string userId, string taskId)
    {
        var task = await _taskRepo.GetByIdAsync(taskId);
        if (task == null || task.UserId != userId)
            throw new BizException("任务不存在", 4001);

        return new GenerationStatusResponse
        {
            TaskId = task.Id,
            Status = task.Status,
            Progress = task.Progress,
            CurrentStage = task.CurrentStage,
            DiagramId = task.DiagramId,
            ErrorMessage = task.ErrorMessage
        };
    }

    public async Task<string> GenerateSyncAsync(string userId, CreateDiagramRequest request)
    {
        var task = new DiagramTask
        {
            UserId = userId,
            Status = "pending",
            SourceImageUrl = request.SourceImageUrl,
            Params = System.Text.Json.JsonSerializer.Serialize(request),
            IsSync = true
        };
        await _taskRepo.InsertAsync(task);
        await ProcessTaskAsync(task.Id);

        var result = await _taskRepo.GetByIdAsync(task.Id);
        if (result?.Status == "completed" && !string.IsNullOrEmpty(result.DiagramId))
            return result.DiagramId;
        throw new BizException(result?.ErrorMessage ?? "生成失败", 5001);
    }

    public Task<PagedResult<DiagramDto>> GetUserDiagramsAsync(string userId, PageRequest request)
    {
        // 实现查询
        throw new NotImplementedException();
    }

    public Task<DiagramDetailDto> GetDiagramDetailAsync(string userId, string diagramId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDiagramAsync(string userId, string diagramId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ColorInfoDto>> GetColorInfosAsync(string diagramId)
    {
        throw new NotImplementedException();
    }

    public Task<string> ExportDiagramAsync(string userId, ExportDiagramRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<string> ShareDiagramAsync(string userId, string diagramId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 处理生成任务
    /// </summary>
    private async Task ProcessTaskAsync(string taskId)
    {
        var task = await _taskRepo.GetByIdAsync(taskId);
        if (task == null) return;

        try
        {
            task.Status = "processing";
            task.CurrentStage = "图像处理";
            task.Progress = 10;
            await _taskRepo.UpdateAsync(task);

            // 调用AI生图
            var request = System.Text.Json.JsonSerializer.Deserialize<CreateDiagramRequest>(task.Params);
            var result = await _aiService.GenerateSyncAsync(new AiGenerationRequest
            {
                UserId = task.UserId,
                TaskId = taskId,
                SourceImageUrl = task.SourceImageUrl,
                BoardSize = request?.BoardSize ?? "29x29",
                Difficulty = request?.Difficulty ?? "easy",
                Style = request?.Style ?? "pixel"
            });

            if (!result.Success)
            {
                task.Status = "failed";
                task.ErrorMessage = result.ErrorMessage;
                task.CompleteTime = DateTime.Now;
                await _taskRepo.UpdateAsync(task);
                return;
            }

            task.CurrentStage = "色号映射";
            task.Progress = 80;
            await _taskRepo.UpdateAsync(task);

            // 创建Diagram
            var diagram = new Diagram
            {
                UserId = task.UserId,
                Name = $"图纸{DateTime.Now:yyyyMMddHHmmss}",
                Status = "completed",
                SourceImageUrl = task.SourceImageUrl,
                PreviewUrl = result.PreviewUrl,
                PreviewNoGridUrl = result.PreviewNoGridUrl,
                BoardSize = request?.BoardSize ?? "29x29",
                BeadCount = result.BeadCount,
                Difficulty = request?.Difficulty ?? "easy",
                Style = request?.Style ?? "pixel",
                TotalColors = result.ColorCount,
                TotalBeads = result.BeadCount,
                Tags = request?.Tags != null ? System.Text.Json.JsonSerializer.Serialize(request.Tags) : null,
                SourceType = request?.TemplateId != null ? "template" : "create",
                TemplateId = request?.TemplateId
            };
            await _diagramRepo.InsertAsync(diagram);

            // 保存色号
            if (result.ColorInfos != null)
            {
                var colorInfos = result.ColorInfos.Select(c => new ColorInfo
                {
                    DiagramId = diagram.Id,
                    ColorIndex = c.ColorIndex,
                    ColorCode = c.ColorCode,
                    ColorName = c.ColorName,
                    Rgb = c.Rgb,
                    BeadCount = c.BeadCount,
                    Percentage = c.Percentage
                }).ToList();
                await _colorInfoRepo.InsertRangeAsync(colorInfos);
            }

            task.DiagramId = diagram.Id;
            task.Progress = 100;
            task.Status = "completed";
            task.CurrentStage = "完成";
            task.CompleteTime = DateTime.Now;
            await _taskRepo.UpdateAsync(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Process task failed: {TaskId}", taskId);
            task.Status = "failed";
            task.ErrorMessage = ex.Message;
            task.CompleteTime = DateTime.Now;
            await _taskRepo.UpdateAsync(task);
        }
    }
}
