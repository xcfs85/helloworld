using Microsoft.Extensions.Logging;
using Pindou.Infrastructure.Options;
using Microsoft.Extensions.Options;

/**
 *@Author: 西成峰
 *@CreateTime: 2026-06-27
 *@Description: 腾讯云短信服务实现（骨架），后续对接腾讯云SMS API
 */

namespace Pindou.Infrastructure.ExternalServices.Sms;

/// <summary>
/// 腾讯云短信服务实现（骨架）
/// </summary>
public class TencentSmsService : ISmsService
{
    private readonly HttpClient _http;
    private readonly SmsOptions _options;
    private readonly ILogger<TencentSmsService> _logger;

    public TencentSmsService(HttpClient http, IOptions<SmsOptions> options, ILogger<TencentSmsService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<SmsResult> SendCodeAsync(string phone, string code, string scene)
    {
        // TODO: 对接腾讯云短信服务 API
        try
        {
            _logger.LogInformation("腾讯云短信发送: Phone={Phone}, Scene={Scene}, Provider=tencent", phone, scene);
            await Task.CompletedTask;
            return new SmsResult { Success = true, MessageId = Guid.NewGuid().ToString() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "腾讯云短信发送失败: {Phone}", phone);
            return new SmsResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public Task<SmsResult> SendAsync(string phone, string templateCode, Dictionary<string, string> templateParams)
    {
        return SendCodeAsync(phone, templateCode, "custom");
    }
}
