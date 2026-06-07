namespace Pindou.Application.DTOs.Statistics;

public class DailyStatsDto
{
    public DateTime StatDate { get; set; }
    public int Dau { get; set; }
    public int NewUserCount { get; set; }
    public decimal? Retention1d { get; set; }
    public decimal? Retention7d { get; set; }
    public decimal? Retention30d { get; set; }
    public int GenerationCount { get; set; }
    public int AvgBeadCount { get; set; }
    public int AvgColorCount { get; set; }
    public int ExportCount { get; set; }
    public int PostCount { get; set; }
    public int WorkCount { get; set; }
    public int TutorialCount { get; set; }
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
    public int ShareCount { get; set; }
    public int FavoriteCount { get; set; }
    public int MemberOrderCount { get; set; }
    public decimal MemberRevenue { get; set; }
}

public class OverviewDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsers { get; set; }
    public int TotalDiagrams { get; set; }
    public int TotalPosts { get; set; }
    public int TotalTemplates { get; set; }
    public int MemberCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<DailyStatsDto> DailyStats { get; set; } = new();
}
