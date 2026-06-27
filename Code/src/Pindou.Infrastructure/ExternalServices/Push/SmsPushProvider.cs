using Microsoft.Extensions.Logging;
using Pindou.Application.Interfaces.System;
using Pindou.Infrastructure.ExternalServices.Sms;

/**
 *@Author: 西成峰
 *@CreateTime: 2026-06-27
 *@Description: 短信推送渠道提供者，支持阿里云/腾讯云切换
 */

namespace Pindou.Infrastructure.ExternalServices.Push;

/// <summary>
/// 短信推送渠道提供者
/// </summary>
public class SmsPushProvider : IPushChannelProvider
{
    public string Channel => "sms";

    private readonly ISystemConfigService _configService;
    private readonly ISmsService _smsService;
    private readonly ILogger<SmsPushProvider> _logger;

    public SmsPushProvider(
        ISystemConfigService configService,
        ISmsService smsService,
        ILogger<SmsPushProvider> logger)
    {
        _configService = configService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task<PushChannelResult> SendAsync(PushChannelMessage message)
    {
        // 读取短信配置
        var provider = await _configService.GetAsync("push_sms_provider") ?? "aliyun";
        var signName = await _configService.GetAsync("push_sms_sign_name");
        var templateCode = await _configService.GetAsync("push_sms_template_code");

        if (string.IsNullOrEmpty(signName) || string.IsNullOrEmpty(templateCode))
        {
            _logger.LogWarning("短信推送配置未设置，跳过短信推送");
            return new PushChannelResult
            {
                Success = false,
                ErrorMessage = "短信推送配置未设置，请在系统配置中填写签名和模板编号"
            };
        }

        var phones = message.PhoneNumbers ?? new List<string>();
        if (phones.Count == 0)
        {
            _logger.LogWarning("短信推送无目标手机号");
            return new PushChannelResult
            {
                Success = false,
                ErrorMessage = "无目标手机号"
            };
        }

        int success = 0, fail = 0;
        // 构建模板参数（标题和内容）
        var templateParams = new Dictionary<string, string>
        {
            ["title"] = message.Title,
            ["content"] = message.Content
        };

        foreach (var phone in phones)
        {
            try
            {
                var result = await _smsService.SendAsync(phone, templateCode, templateParams);
                if (result.Success)
                {
                    success++;
                    _logger.LogDebug("短信推送成功: {Phone}, Provider: {Provider}", phone, provider);
                }
                else
                {
                    fail++;
                    _logger.LogWarning("短信推送失败: {Phone}, Error: {Error}", phone, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                fail++;
                _logger.LogError(ex, "短信推送异常: {Phone}", phone);
            }
        }

        return new PushChannelResult
        {
            Success = fail == 0,
            TotalCount = phones.Count,
            SuccessCount = success,
            FailCount = fail
        };
    }
}
