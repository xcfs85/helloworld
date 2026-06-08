using SqlSugar;
using Pindou.Domain.Common;

namespace Pindou.Domain.Entities.Statistics;

/// <summary>
/// 日统计表
/// </summary>
[SugarTable("daily_stats")]
public class DailyStats : BaseEntity
{
    /// <summary>统计日期</summary>
    [SugarColumn(IsNullable = false)]
    public DateTime StatDate { get; set; }

    /// <summary>活跃用户数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int Dau { get; set; }

    /// <summary>新增用户</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int NewUserCount { get; set; }

    /// <summary>次日留存率</summary>
    [SugarColumn(Length = 5, IsNullable = true)]
    public decimal? Retention1d { get; set; }

    /// <summary>7日留存率</summary>
    [SugarColumn(Length = 5, IsNullable = true)]
    public decimal? Retention7d { get; set; }

    /// <summary>30日留存率</summary>
    [SugarColumn(Length = 5, IsNullable = true)]
    public decimal? Retention30d { get; set; }

    /// <summary>生成次数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int GenerationCount { get; set; }

    /// <summary>平均粒数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int AvgBeadCount { get; set; }

    /// <summary>平均色号数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int AvgColorCount { get; set; }

    /// <summary>导出次数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int ExportCount { get; set; }

    /// <summary>发帖数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int PostCount { get; set; }

    /// <summary>作品数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int WorkCount { get; set; }

    /// <summary>教程数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int TutorialCount { get; set; }

    /// <summary>评论数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int CommentCount { get; set; }

    /// <summary>点赞数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int LikeCount { get; set; }

    /// <summary>分享数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int ShareCount { get; set; }

    /// <summary>收藏数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int FavoriteCount { get; set; }

    /// <summary>会员订单数</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int MemberOrderCount { get; set; }

    /// <summary>会员收入</summary>
    [SugarColumn(Length = 10, IsNullable = false, DefaultValue = "0")]
    public decimal MemberRevenue { get; set; }
}
