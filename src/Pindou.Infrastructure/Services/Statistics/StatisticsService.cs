using Pindou.Application.Common;
using Pindou.Application.DTOs.Statistics;
using Pindou.Application.Interfaces.Statistics;
using Pindou.Domain.Entities.Statistics;
using Pindou.Infrastructure.Repositories;

namespace Pindou.Infrastructure.Services.Statistics;

public class StatisticsService : IStatisticsService
{
    private readonly IRepository<DailyStats> _statsRepo;
    public StatisticsService(IRepository<DailyStats> statsRepo) { _statsRepo = statsRepo; }

    public Task RecordGenerationAsync(string userId, int beadCount, int colorCount) { throw new NotImplementedException(); }
    public Task RecordExportAsync(string userId) { throw new NotImplementedException(); }
    public Task RecordPostAsync(string userId, string type) { throw new NotImplementedException(); }
    public Task RecordCommentAsync(string userId, string postId) { throw new NotImplementedException(); }
    public Task RecordLikeAsync(string userId, string targetType) { throw new NotImplementedException(); }
    public Task RecordShareAsync(string userId, string targetType) { throw new NotImplementedException(); }
    public Task RecordFavoriteAsync(string userId, string targetType) { throw new NotImplementedException(); }
    public Task RecordMemberOrderAsync(decimal amount) { throw new NotImplementedException(); }
    public Task<DailyStatsDto> GetDailyStatsAsync(DateTime date) { throw new NotImplementedException(); }
    public Task<List<DailyStatsDto>> GetRangeStatsAsync(DateTime start, DateTime end) { throw new NotImplementedException(); }
    public Task<OverviewDto> GetOverviewAsync(DateTime? start, DateTime? end) { throw new NotImplementedException(); }
}
