using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Pindou.Application.Common;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Pindou.Api.Filters;

/// <summary>
/// 全局异常过滤器
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        _logger.LogError(exception, "请求异常: {Path}", context.HttpContext.Request.Path);

        var (code, message) = exception switch
        {
            BizException biz => (biz.Code, biz.Message),
            _ => (ErrorCodes.ServerError, "系统错误")
        };

        var response = ApiResponse<object>.Fail(message, code);
        context.Result = new ObjectResult(response)
        {
            StatusCode = code == 2001 || code == 2002 ? StatusCodes.Status401Unauthorized : StatusCodes.Status200OK
        };
        context.ExceptionHandled = true;
    }
}
