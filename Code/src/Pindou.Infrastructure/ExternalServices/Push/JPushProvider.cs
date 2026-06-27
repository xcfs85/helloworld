using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Pindou.Application.Interfaces.System;

/**
 *@Author: 西成峰
 *@CreateTime: 2026-06-27
 *@Description: 极光推送(App)渠道提供者，通过 JPush REST API v3 发送推送
 */

namespace Pindou.Infrastructure.ExternalServices.Push;

/// <summary>
/// 极光推送(App)渠道提供者
/// </summary>
public class JPushProvider : IPushChannelProvider
{
    public string Channel => "app";

    private readonly HttpClient _http;
    private readonly ISystemConfigService _configService;
    private readonly ILogger<JPushProvider> _logger;

    public JPushProvider(HttpClient http, ISystemConfigService configService, ILogger<JPushProvider> logger)
    {
        _http = http;
        _configService = configService;
        _logger = logger;
    }

    public async Task<PushChannelResult> SendAsync(PushChannelMessage message)
    {
        // 1. 从动态配置读取 AppKey/MasterSecret
        var appKey = await _configService.GetAsync("push_jpush_appkey");
        var masterSecret = await _configService.GetAsync("push_jpush_master_secret");

        if (string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(masterSecret))
        {
            _logger.LogWarning("JPush配置未设置，跳过App推送");
            return new PushChannelResult
            {
                Success = false,
                ErrorMessage = "JPush配置未设置，请在系统配置中填写AppKey和MasterSecret"
            };
        }

        // 2. 构建JPush推送请求体
        var payload = BuildPayload(message);
        var json = JsonSerializer.Serialize(payload);

        // 3. 调用JPush REST API
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.jpush.cn/v3/push");
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{appKey}:{masterSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _http.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("JPush推送成功: {Response}", responseBody);

                // 解析返回的 msgid
                var respData = JsonSerializer.Deserialize<JsonElement>(responseBody);
                var msgId = respData.TryGetProperty("msg_id", out var msgIdProp)
                    ? msgIdProp.ToString()
                    : null;

                return new PushChannelResult
                {
                    Success = true,
                    TotalCount = message.TargetType == "all" ? 1 : (message.RegistrationIds?.Count ?? 1),
                    SuccessCount = message.TargetType == "all" ? 1 : (message.RegistrationIds?.Count ?? 1),
                    FailCount = 0,
                    MessageId = msgId
                };
            }
            else
            {
                _logger.LogError("JPush推送失败: StatusCode={StatusCode}, Response={Response}",
                    response.StatusCode, responseBody);

                return new PushChannelResult
                {
                    Success = false,
                    TotalCount = message.TargetType == "all" ? 1 : (message.RegistrationIds?.Count ?? 1),
                    SuccessCount = 0,
                    FailCount = message.TargetType == "all" ? 1 : (message.RegistrationIds?.Count ?? 1),
                    ErrorMessage = $"JPush返回错误: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "JPush推送异常");
            return new PushChannelResult
            {
                Success = false,
                ErrorMessage = $"JPush推送异常: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 构建JPush推送请求体
    /// </summary>
    private static object BuildPayload(PushChannelMessage message)
    {
        // 构建 audience
        object audience = message.TargetType switch
        {
            "all" => "all",
            "tag" => new { tag = ParseTargetParam(message.TargetParam) },
            "user" => new { registration_id = message.RegistrationIds ?? new List<string>() },
            _ => "all"
        };

        return new
        {
            platform = "all",
            audience,
            notification = new
            {
                android = new
                {
                    alert = message.Content,
                    title = message.Title,
                    extras = message.Extras
                },
                ios = new
                {
                    alert = message.Content,
                    title = message.Title,
                    extras = message.Extras
                }
            },
            options = new
            {
                time_to_live = 86400
            }
        };
    }

    /// <summary>
    /// 解析目标参数为标签列表
    /// </summary>
    private static List<string> ParseTargetParam(string? targetParam)
    {
        if (string.IsNullOrEmpty(targetParam)) return new List<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(targetParam) ?? new List<string>();
        }
        catch
        {
            return new List<string> { targetParam };
        }
    }
}
