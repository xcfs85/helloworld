namespace Pindou.Application.Common;

/// <summary>
/// 分页请求
/// </summary>
public class PageRequest
{
    /// <summary>页码(从1开始)</summary>
    public int Page { get; set; } = 1;

    /// <summary>每页条数</summary>
    public int Size { get; set; } = 20;

    /// <summary>排序字段</summary>
    public string? OrderBy { get; set; }

    /// <summary>排序方向 asc/desc</summary>
    public string? OrderDir { get; set; } = "desc";
}

/// <summary>
/// 分页结果
/// </summary>
public class PagedResult<T>
{
    public List<T> List { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
}

/// <summary>
/// 通用查询条件基类
/// </summary>
public abstract class QueryRequest : PageRequest
{
    /// <summary>关键字</summary>
    public string? Keyword { get; set; }
    /// <summary>开始时间</summary>
    public DateTime? StartTime { get; set; }
    /// <summary>结束时间</summary>
    public DateTime? EndTime { get; set; }
    /// <summary>状态</summary>
    public string? Status { get; set; }
}
