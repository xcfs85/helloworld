using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Pindou.Application.Interfaces.System;

/**
 *@Author: 西成峰
 *@CreateTime: 2026-06-27
 *@Description: 邮件推送渠道提供者，使用原生SMTP发送邮件
 */

namespace Pindou.Infrastructure.ExternalServices.Push;

/// <summary>
/// 邮件推送渠道提供者
/// </summary>
public class EmailPushProvider : IPushChannelProvider
{
    public string Channel => "email";

    private readonly ISystemConfigService _configService;
    private readonly ILogger<EmailPushProvider> _logger;

    public EmailPushProvider(ISystemConfigService configService, ILogger<EmailPushProvider> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    public async Task<PushChannelResult> SendAsync(PushChannelMessage message)
    {
        // 读取邮件SMTP配置
        var host = await _configService.GetAsync("push_email_smtp_host");
        var portStr = await _configService.GetAsync("push_email_smtp_port");
        var username = await _configService.GetAsync("push_email_username");
        var password = await _configService.GetAsync("push_email_password");
        var from = await _configService.GetAsync("push_email_from");
        var sslStr = await _configService.GetAsync("push_email_ssl");

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(from))
        {
            _logger.LogWarning("邮件推送配置未设置，跳过邮件推送");
            return new PushChannelResult
            {
                Success = false,
                ErrorMessage = "邮件推送配置未设置，请在系统配置中填写SMTP信息"
            };
        }

        var emails = message.EmailAddresses ?? new List<string>();
        if (emails.Count == 0)
        {
            _logger.LogWarning("邮件推送无目标邮箱");
            return new PushChannelResult
            {
                Success = false,
                ErrorMessage = "无目标邮箱地址"
            };
        }

        int port = int.TryParse(portStr, out var p) ? p : 465;
        bool enableSsl = sslStr != "false";

        int success = 0, fail = 0;

        try
        {
            using var client = new SmtpClient(host, port);
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = enableSsl;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            foreach (var email in emails)
            {
                try
                {
                    using var mail = new MailMessage();
                    mail.From = new MailAddress(from);
                    mail.To.Add(email);
                    mail.Subject = message.Title;
                    mail.Body = message.Content;
                    mail.IsBodyHtml = false;

                    await client.SendMailAsync(mail);
                    success++;
                    _logger.LogDebug("邮件推送成功: {Email}", email);
                }
                catch (Exception ex)
                {
                    fail++;
                    _logger.LogError(ex, "邮件推送失败: {Email}", email);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "邮件SMTP连接失败: {Host}:{Port}", host, port);
            return new PushChannelResult
            {
                Success = false,
                TotalCount = emails.Count,
                SuccessCount = 0,
                FailCount = emails.Count,
                ErrorMessage = $"SMTP连接失败: {ex.Message}"
            };
        }

        return new PushChannelResult
        {
            Success = fail == 0,
            TotalCount = emails.Count,
            SuccessCount = success,
            FailCount = fail
        };
    }
}
