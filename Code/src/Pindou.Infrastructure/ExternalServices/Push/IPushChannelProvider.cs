/**
 *@Author: 西成峰
 *@CreateTime: 2026-06-27
 *@Description: 推送渠道提供者接口定义
 */

namespace Pindou.Infrastructure.ExternalServices.Push;

/// <summary>
/// 推送渠道提供者接口
/// </summary>
public interface IPushChannelProvider
{
    /// <summary>渠道标识: app/sms/email</summary>
    string Channel { get; }

    /// <summary>发送推送</summary>
    Task<PushChannelResult> SendAsync(PushChannelMessage message);
}

/// <summary>
/// 推送渠道消息
/// </summary>
public class PushChannelMessage
{
    /// <summary>标题</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>内容</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>目标类型: all/tag/user</summary>
    public string TargetType { get; set; } = "all";

    /// <summary>目标参数</summary>
    public string? TargetParam { get; set; }

    /// <summary>目标用户ID列表</summary>
    public List<string>? TargetUserIds { get; set; }

    /// <summary>推送注册ID列表(用于App推送)</summary>
    public List<string>? RegistrationIds { get; set; }

    /// <summary>手机号列表(用于短信推送)</summary>
    public List<string>? PhoneNumbers { get; set; }

    /// <summary>邮箱列表(用于邮件推送)</summary>
    public List<string>? EmailAddresses { get; set; }

    /// <summary>扩展参数(JSON)</summary>
    public Dictionary<string, object>? Extras { get; set; }
}

/// <summary>
/// 推送渠道发送结果
/// </summary>
public class PushChannelResult
{
    /// <summary>是否成功</summary>
    public bool Success { get; set; }

    /// <summary>总发送数</summary>
    public int TotalCount { get; set; }

    /// <summary>成功数</summary>
    public int SuccessCount { get; set; }

    /// <summary>失败数</summary>
    public int FailCount { get; set; }

    /// <summary>第三方返回的消息ID</summary>
    public string? MessageId { get; set; }

    /// <summary>错误信息</summary>
    public string? ErrorMessage { get; set; }
}
