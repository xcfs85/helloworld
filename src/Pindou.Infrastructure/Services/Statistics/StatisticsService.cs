using Pindou.Application.Common;
using Pindou.Application.DTOs.Statistics;
using Pindou.Application.Interfaces.Statistics;
using Pindou.Domain.Entities.Statistics;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.Creation;
using Pindou.Domain.Entities.Member;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;
using SqlSugar;

namespace Pindou.Infrastructure.Services.Statistics;

public class StatisticsService : IStatisticsService
{
    private readonly IRepository<DailyStats> _statsRepo;
    private readonly IRepository<Diagram> _diagramRepo;
    private readonly IRepository<Post> _postRepo;
    private readonly IRepository<Comment> _commentRepo;
    private readonly IRepository<Like> _likeRepo;
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<User> _userRepo;

    public StatisticsService(
        IRepository<DailyStats> statsRepo,
        IRepository<Diagram> diagramRepo,
        IRepository<Post> postRepo,
        IRepository<Comment> commentRepo,
        IRepository<Like> likeRepo,
        IRepository<Order> orderRepo,
        IRepository<User> userRepo)
    {
        _statsRepo = statsRepo;
        _diagramRepo = diagramRepo;
        _postRepo = postRepo;
        _commentRepo = commentRepo;
        _likeRepo = likeRepo;
        _orderRepo = orderRepo;
        _userRepo = userRepo;
    }

    public async Task RecordGenerationAsync(string userId, int beadCount, int colorCount)
    {
        await UpdateDailyStatsAsync(stats =>
        {
            stats.GenerationCount++;
            stats.AvgBeadCount = (stats.AvgBeadCount * (stats.GenerationCount - 1) + beadCount) / stats.GenerationCount;
            stats.AvgColorCount = (stats.AvgColorCount * (stats.GenerationCount - 1) + colorCount) / stats.GenerationCount;
        });
    }

    public async Task RecordExportAsync(string userId)
    {
        await UpdateDailyStatsAsync(stats => stats.ExportCount++);
    }

    public async Task RecordPostAsync(string userId, string type)
    {
        await UpdateDailyStatsAsync(stats =>
        {
            stats.PostCount++;
            if (type == "work") stats.WorkCount++;
            if (type == "tutorial") stats.TutorialCount++;
        });
    }

    public async Task RecordCommentAsync(string userId, string postId)
    {
        await UpdateDailyStatsAsync(stats => stats.CommentCount++);
    }

    public async Task RecordLikeAsync(string userId, string targetType)
    {
        await UpdateDailyStatsAsync(stats => stats.LikeCount++);
    }

    public async Task RecordShareAsync(string userId, string targetType)
    {
        await UpdateDailyStatsAsync(stats => stats.ShareCount++);
    }

    public async Task RecordFavoriteAsync(string userId, string targetType)
    {
        await UpdateDailyStatsAsync(stats => stats.FavoriteCount++);
    }

    public async Task RecordMemberOrderAsync(decimal amount)
    {
        await UpdateDailyStatsAsync(stats =>
        {
            stats.MemberOrderCount++;
            stats.MemberRevenue += amount;
        });
    }

    public async Task<DailyStatsDto> GetDailyStatsAsync(DateTime date)
    {
        var stats = await _statsRepo.FirstOrDefaultAsync(s => s.StatDate == date.Date);
        if (stats == null) return new DailyStatsDto { StatDate = date.Date };

        return MapToDto(stats);
    }

    public async Task<List<DailyStatsDto>> GetRangeStatsAsync(DateTime start, DateTime end)
    {
        var stats = await _statsRepo.GetListAsync(
            s => s.StatDate >= start.Date && s.StatDate <= end.Date,
            nameof(DailyStats.StatDate),
            false);

        return stats.Select(MapToDto).ToList();
    }

    public async Task<OverviewDto> GetOverviewAsync(DateTime? start, DateTime? end)
    {
        var totalUsers = await _userRepo.CountAsync();
        var totalDiagrams = await _diagramRepo.CountAsync();
        var totalPosts = await _postRepo.CountAsync(p => p.Status == "active");
        var totalTemplates = await _diagramRepo.CountAsync();
        var memberCount = await _userRepo.CountAsync(u => u.IsMember);
        var totalRevenue = await _orderRepo.CountAsync(o => false);

        // 活跃用户数（今日）
        var today = DateTime.Now.Date;
        var activeUsers = await _userRepo.CountAsync(u => u.LastLoginTime >= today);

        // 今日新增用户
        var newUsers = await _userRepo.CountAsync(u => u.CreateTime >= today);

        // 总收入
        var orders = await _orderRepo.GetListAsync(o => o.Status == "paid");
        var revenue = orders.Sum(o => o.Amount);

        var dailyStats = new List<DailyStatsDto>();
        if (start.HasValue && end.HasValue)
        {
            dailyStats = await GetRangeStatsAsync(start.Value, end.Value);
        }

        return new OverviewDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            NewUsers = newUsers,
            TotalDiagrams = totalDiagrams,
            TotalPosts = totalPosts,
            TotalTemplates = totalTemplates,
            MemberCount = memberCount,
            TotalRevenue = revenue,
            DailyStats = dailyStats
        };
    }

    private async Task UpdateDailyStatsAsync(Action<DailyStats> updateAction)
    {
        var today = DateTime.Now.Date;
        var stats = await _statsRepo.FirstOrDefaultAsync(s => s.StatDate == today);
        if (stats == null)
        {
            stats = new DailyStats { StatDate = today };
            updateAction(stats);
            await _statsRepo.InsertAsync(stats);
        }
        else
        {
            updateAction(stats);
            stats.UpdateTime = DateTime.Now;
            await _statsRepo.UpdateAsync(stats);
        }
    }

    private static DailyStatsDto MapToDto(DailyStats stats)
    {
        return new DailyStatsDto
        {
            StatDate = stats.StatDate,
            Dau = stats.Dau,
            NewUserCount = stats.NewUserCount,
            Retention1d = stats.Retention1d,
            Retention7d = stats.Retention7d,
            Retention30d = stats.Retention30d,
            GenerationCount = stats.GenerationCount,
            AvgBeadCount = stats.AvgBeadCount,
            AvgColorCount = stats.AvgColorCount,
            ExportCount = stats.ExportCount,
            PostCount = stats.PostCount,
            WorkCount = stats.WorkCount,
            TutorialCount = stats.TutorialCount,
            CommentCount = stats.CommentCount,
            LikeCount = stats.LikeCount,
            ShareCount = stats.ShareCount,
            FavoriteCount = stats.FavoriteCount,
            MemberOrderCount = stats.MemberOrderCount,
            MemberRevenue = stats.MemberRevenue
        };
    }
}