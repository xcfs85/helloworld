using Pindou.Application.Common;
using Pindou.Application.DTOs.System;

namespace Pindou.Application.Interfaces.System;

public interface ISystemConfigService
{
    Task<string?> GetAsync(string key);
    Task<T?> GetAsync<T>(string key);
    Task SetAsync(string key, string value, string? type = null, string? description = null);
    Task<List<SystemConfigDto>> GetAllAsync();
    Task<string?> GetMardColorAsync(string colorNo);
    Task<List<MardColorDto>> GetAllMardColorsAsync();
    Task<List<BeadKitDto>> GetAllBeadKitsAsync(int? colorCount = null);
    Task<bool> RecommendKitAsync(int requiredColors);
}

public interface IContentReviewService
{
    Task<(bool Passed, string Reason, string? ReplacedContent)> CheckAsync(string content);
    Task<bool> ReviewPostAsync(string postId, string reviewerId, bool approved, string? reason = null);
    Task<PagedResult<DTOs.Community.PostDto>> GetPendingPostsAsync(PageRequest request);
    Task<bool> HandleReportAsync(string reportId, string handlerId, string action, string? result = null);
    Task<PagedResult<ReportDto>> GetReportsAsync(ReportQuery query);
    Task<List<SensitiveWordDto>> GetSensitiveWordsAsync(string? type = null);
    Task<string> AddSensitiveWordAsync(AddSensitiveWordRequest request);
    Task<bool> UpdateSensitiveWordAsync(string id, AddSensitiveWordRequest request);
    Task<bool> DeleteSensitiveWordAsync(string id);
}
