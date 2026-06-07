using SqlSugar;
using Pindou.Domain.Common;

namespace Pindou.Domain.Entities.User;

/// <summary>
/// 令牌表
/// </summary>
[SugarTable("tokens")]
public class Token : UuidEntity
{
    /// <summary>用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>访问令牌</summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>刷新令牌</summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>设备ID</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>过期时间</summary>
    [SugarColumn(IsNullable = false)]
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// 设备表
/// </summary>
[SugarTable("devices")]
public class Device : UuidEntity
{
    /// <summary>用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>设备ID</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>平台:ios/android</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Platform { get; set; } = string.Empty;

    /// <summary>推送令牌</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? PushToken { get; set; }

    /// <summary>APP版本</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? AppVersion { get; set; }

    /// <summary>最后活跃时间</summary>
    [SugarColumn(IsNullable = false)]
    public DateTime LastActiveTime { get; set; } = DateTime.Now;
}
