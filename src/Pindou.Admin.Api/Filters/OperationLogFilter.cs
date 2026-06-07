using Microsoft.AspNetCore.Mvc.Filters;
using Pindou.Application.Interfaces.Admin;
using Pindou.Shared.Attributes;
using System.Text.Json;

namespace Pindou.Admin.Api.Filters;

/// <summary>
/// 自动记录操作日志
/// </summary>
public class OperationLogFilter : IAsyncActionFilter
{
    private readonly IOperationLogService _logService;
    public OperationLogFilter(IOperationLogService logService) { _logService = logService; }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        var logAttr = endpoint?.Metadata.GetMetadata<OperationLogAttribute>();
        if (logAttr == null)
        {
            await next();
            return;
        }

        var adminIdStr = context.HttpContext.Items["AdminId"]?.ToString();
        if (string.IsNullOrEmpty(adminIdStr) || !long.TryParse(adminIdStr, out var adminId))
        {
            await next();
            return;
        }

        // 仅记录写操作的结果
        var result = await next();
        if (result.Exception != null) return;

        // 简化实现：从JWT获取username，从claims中拿nickname
        var username = context.HttpContext.User.Identity?.Name ?? string.Empty;
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.HttpContext.Request.Headers.UserAgent.ToString();

        string? paramJson = null;
        if (logAttr.SaveParams)
        {
            try
            {
                paramJson = JsonSerializer.Serialize(context.ActionArguments);
            }
            catch { }
        }

        await _logService.RecordAsync(
            adminId, username, null,
            logAttr.Name,
            logAttr.Content,
            context.HttpContext.Request.Method,
            paramJson,
            ip,
            userAgent);
    }
}
