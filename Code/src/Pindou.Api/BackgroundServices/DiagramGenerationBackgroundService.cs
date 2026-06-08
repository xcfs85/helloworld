using System.Threading.Channels;
using Pindou.Application.Interfaces.Creation;

namespace Pindou.Api.BackgroundServices;

/// <summary>
/// 图纸生成后台服务 - 处理异步生成任务
/// </summary>
public class DiagramGenerationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DiagramGenerationBackgroundService> _logger;
    private readonly Channel<string> _queue;

    public DiagramGenerationBackgroundService(IServiceProvider serviceProvider, ILogger<DiagramGenerationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _queue = Channel.CreateBounded<string>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    /// <summary>添加任务到队列</summary>
    public bool TryEnqueue(string taskId, out string? error)
    {
        error = null;
        if (_queue.Writer.TryWrite(taskId)) return true;
        error = "队列已满";
        return false;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DiagramGenerationBackgroundService started.");

        // 启动多个消费者
        var consumerCount = Environment.ProcessorCount;
        var tasks = new Task[consumerCount];
        for (var i = 0; i < consumerCount; i++)
        {
            tasks[i] = Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
        }

        await Task.WhenAll(tasks);
    }

    private async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        await foreach (var taskId in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Processing task: {TaskId}", taskId);
                using var scope = _serviceProvider.CreateScope();
                var diagramService = scope.ServiceProvider.GetRequiredService<IDiagramService>();
                // 实际调用处理逻辑
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Process task failed: {TaskId}", taskId);
            }
        }
    }
}
