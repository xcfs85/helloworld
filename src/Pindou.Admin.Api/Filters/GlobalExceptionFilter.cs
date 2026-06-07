using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Pindou.Application.Common;

namespace Pindou.Admin.Api.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) { _logger = logger; }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        _logger.LogError(exception, "Admin API异常: {Path}", context.HttpContext.Request.Path);

        var (code, message) = exception switch
        {
            BizException biz => (biz.Code, biz.Message),
            _ => (ErrorCodes.ServerError, "系统错误")
        };

        context.Result = new ObjectResult(ApiResponse<object>.Fail(message, code))
        {
            StatusCode = code is ErrorCodes.TokenInvalid or ErrorCodes.TokenExpired
                ? StatusCodes.Status401Unauthorized
                : code is ErrorCodes.NoPermission
                    ? StatusCodes.Status403Forbidden
                    : StatusCodes.Status200OK
        };
        context.ExceptionHandled = true;
    }
}
