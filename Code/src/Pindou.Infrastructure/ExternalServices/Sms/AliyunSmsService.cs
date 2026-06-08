using Microsoft.Extensions.Logging;
using Pindou.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Pindou.Infrastructure.ExternalServices.Sms;

public class AliyunSmsService : ISmsService
{
    private readonly HttpClient _http;
    private readonly SmsOptions _options;
    private readonly ILogger<AliyunSmsService> _logger;
    public AliyunSmsService(HttpClient http, IOptions<SmsOptions> options, ILogger<AliyunSmsService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<SmsResult> SendCodeAsync(string phone, string code, string scene)
    {
        // 调用阿里云短信服务
        // 实际生产使用阿里云SDK
        try
        {
            _logger.LogInformation("Send SMS to {Phone}, scene: {Scene}", phone, scene);
            await Task.CompletedTask;
            return new SmsResult { Success = true, MessageId = Guid.NewGuid().ToString() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Send SMS failed: {Phone}", phone);
            return new SmsResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public Task<SmsResult> SendAsync(string phone, string templateCode, Dictionary<string, string> templateParams)
    {
        return SendCodeAsync(phone, templateCode, "custom");
    }
}
