using Pindou.Application.Common;
using Pindou.Application.DTOs.Statistics;

namespace Pindou.Application.Interfaces.Statistics;

public interface IStatisticsService
{
    Task RecordGenerationAsync(string userId, int beadCount, int colorCount);
    Task RecordExportAsync(string userId);
    Task RecordPostAsync(string userId, string type);
    Task RecordCommentAsync(string userId, string postId);
    Task RecordLikeAsync(string userId, string targetType);
    Task RecordShareAsync(string userId, string targetType);
    Task RecordFavoriteAsync(string userId, string targetType);
    Task RecordMemberOrderAsync(decimal amount);
    Task<DailyStatsDto> GetDailyStatsAsync(DateTime date);
    Task<List<DailyStatsDto>> GetRangeStatsAsync(DateTime start, DateTime end);
    Task<OverviewDto> GetOverviewAsync(DateTime? start, DateTime? end);
}
