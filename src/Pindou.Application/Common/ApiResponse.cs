namespace Pindou.Application.Common;

/// <summary>
/// 统一API响应
/// </summary>
public class ApiResponse<T>
{
    /// <summary>状态码:0成功 其它失败</summary>
    public int Code { get; set; } = 0;

    /// <summary>提示信息</summary>
    public string Message { get; set; } = "success";

    /// <summary>业务数据</summary>
    public T? Data { get; set; }

    /// <summary>时间戳(秒)</summary>
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static ApiResponse<T> Ok(T data, string message = "success")
    {
        return new ApiResponse<T> { Code = 0, Message = message, Data = data };
    }

    public static ApiResponse<T> Fail(string message, int code = -1)
    {
        return new ApiResponse<T> { Code = code, Message = message, Data = default };
    }
}

/// <summary>
/// 无数据的统一响应
/// </summary>
public class ApiResponse
{
    public int Code { get; set; } = 0;
    public string Message { get; set; } = "success";
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static ApiResponse Ok(string message = "success") => new() { Code = 0, Message = message };
    public static ApiResponse Fail(string message, int code = -1) => new() { Code = code, Message = message };
}
