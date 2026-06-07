using Pindou.Application.Common;
using Pindou.Application.DTOs.Creation;

namespace Pindou.Application.Interfaces.Creation;

public interface IDiagramService
{
    Task<string> CreateGenerationTaskAsync(string userId, CreateDiagramRequest request);
    Task<GenerationStatusResponse> GetTaskStatusAsync(string userId, string taskId);
    Task<string> GenerateSyncAsync(string userId, CreateDiagramRequest request);
    Task<PagedResult<DiagramDto>> GetUserDiagramsAsync(string userId, PageRequest request);
    Task<DiagramDetailDto> GetDiagramDetailAsync(string userId, string diagramId);
    Task DeleteDiagramAsync(string userId, string diagramId);
    Task<List<ColorInfoDto>> GetColorInfosAsync(string diagramId);
    Task<string> ExportDiagramAsync(string userId, ExportDiagramRequest request);
    Task<string> ShareDiagramAsync(string userId, string diagramId);
}
