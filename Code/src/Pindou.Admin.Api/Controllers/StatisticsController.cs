using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Statistics;
using Pindou.Application.Interfaces.Statistics;
using Pindou.Shared.Attributes;

namespace Pindou.Admin.Api.Controllers;

[ApiController]
[Route("api/admin/v1/statistics")]
[Permission("statistics:view")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    /// <summary>概览统计</summary>
    [HttpGet("overview")]
    public async Task<ApiResponse<OverviewDto>> Overview([FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var data = await _statisticsService.GetOverviewAsync(start, end);
        return ApiResponse<OverviewDto>.Ok(data);
    }

    /// <summary>每日统计</summary>
    [HttpGet("daily")]
    public async Task<ApiResponse<DailyStatsDto>> Daily([FromQuery] DateTime? date)
    {
        var targetDate = date ?? DateTime.Now.Date;
        var data = await _statisticsService.GetDailyStatsAsync(targetDate);
        return ApiResponse<DailyStatsDto>.Ok(data);
    }

    /// <summary>范围统计</summary>
    [HttpGet("range")]
    public async Task<ApiResponse<List<DailyStatsDto>>> Range([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var data = await _statisticsService.GetRangeStatsAsync(start, end);
        return ApiResponse<List<DailyStatsDto>>.Ok(data);
    }

    /// <summary>趋势数据</summary>
    [HttpGet("trends")]
    public async Task<ApiResponse<List<DailyStatsDto>>> Trends([FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var startDate = start ?? DateTime.Now.AddDays(-30).Date;
        var endDate = end ?? DateTime.Now.Date;
        var data = await _statisticsService.GetRangeStatsAsync(startDate, endDate);
        return ApiResponse<List<DailyStatsDto>>.Ok(data);
    }

    /// <summary>导出报表</summary>
    [HttpGet("export")]
    [Permission("statistics:export")]
    [OperationLog("导出统计报表")]
    public async Task<ApiResponse<OverviewDto>> Export([FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var data = await _statisticsService.GetOverviewAsync(start, end);
        return ApiResponse<OverviewDto>.Ok(data);
    }
}