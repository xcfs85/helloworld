using SqlSugar;
using Pindou.Domain.Common;
using Pindou.Domain.Enums;

namespace Pindou.Domain.Entities.User;

/// <summary>
/// 用户表
/// </summary>
[SugarTable("users")]
public class User : UuidEntity
{
    /// <summary>昵称</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Nickname { get; set; } = string.Empty;

    /// <summary>头像URL</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Avatar { get; set; }

    /// <summary>手机号(加密)</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>微信unionid</summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? UnionId { get; set; }

    /// <summary>Apple用户ID</summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? AppleUserId { get; set; }

    /// <summary>性别</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'unknown'")]
    public string Gender { get; set; } = "unknown";

    /// <summary>城市</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? City { get; set; }

    /// <summary>个性签名</summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Bio { get; set; }

    /// <summary>是否会员</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public bool IsMember { get; set; }

    /// <summary>会员过期时间</summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? MemberExpireTime { get; set; }

    /// <summary>状态:active/disabled</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'active'")]
    public string Status { get; set; } = "active";

    /// <summary>最后登录时间</summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>最后登录IP</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LastLoginIp { get; set; }
}
