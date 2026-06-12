using System.Text.Json.Serialization;
using Pindou.Application.Common;

namespace Pindou.Application.DTOs.User;

public class UserListQuery : QueryRequest
{
    public bool? IsMember { get; set; }
    public string? Platform { get; set; }
    public DateTime? RegisterStartTime { get; set; }
    public DateTime? RegisterEndTime { get; set; }
    /// <summary>会员等级:VIP1/VIP2/VIP3/SVIP</summary>
    public string? Level { get; set; }
    /// <summary>到期状态:7d-7天内到期 / 30d-30天内到期 / expired-已过期 / long-长期有效</summary>
    public string? Expire { get; set; }
    /// <summary>支付渠道:wechat/alipay/appstore/backend</summary>
    public string? PayChannel { get; set; }
}

public class UserListDto
{
    public string Id { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Phone { get; set; }
    public string Gender { get; set; } = "unknown";
    public string? City { get; set; }
    public bool IsMember { get; set; }
    public DateTime? MemberExpireTime { get; set; }
    /// <summary>会员等级:VIP1/VIP2/VIP3/SVIP</summary>
    public string? MemberLevel { get; set; }
    /// <summary>自动续费</summary>
    public bool AutoRenew { get; set; }
    /// <summary>累计付费金额</summary>
    public decimal TotalPaid { get; set; }
    /// <summary>支付渠道:wechat/alipay/appstore/backend</summary>
    public string? PayChannel { get; set; }
    public string Status { get; set; } = "active";
    public DateTime CreateTime { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public int DiagramCount { get; set; }
    public int PostCount { get; set; }
}

public class UpdateUserRequest
{
    public string? Nickname { get; set; }
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Gender { get; set; }
}

/// <summary>
/// 用户会员信息（包含会员等级、累计付费等）
/// </summary>
public class UserMemberInfoDto
{
    /// <summary>会员等级:VIP1/VIP2/VIP3/SVIP</summary>
    public string? MemberLevel { get; set; }
    /// <summary>自动续费</summary>
    public bool AutoRenew { get; set; }
    /// <summary>累计付费金额</summary>
    public decimal TotalPaid { get; set; }
    /// <summary>支付渠道:wechat/alipay/appstore/backend</summary>
    public string? PayChannel { get; set; }
    /// <summary>首次开通时间</summary>
    public DateTime? FirstOpenTime { get; set; }
}

/// <summary>
/// 会员统计数据
/// </summary>
public class MemberStatsDto
{
    /// <summary>会员总数</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
    /// <summary>各等级会员数量</summary>
    [JsonPropertyName("level_counts")]
    public List<MemberLevelCount> LevelCounts { get; set; } = new();
    /// <summary>各支付渠道会员数量</summary>
    [JsonPropertyName("channel_counts")]
    public List<MemberChannelCount> ChannelCounts { get; set; } = new();
    /// <summary>即将到期会员数量(7天内)</summary>
    [JsonPropertyName("expiring_soon_count")]
    public int ExpiringSoonCount { get; set; }
    /// <summary>30天内到期会员数量</summary>
    [JsonPropertyName("expiring_30d_count")]
    public int Expiring30dCount { get; set; }
    /// <summary>长期有效会员数量(过期时间>1年)</summary>
    [JsonPropertyName("long_term_count")]
    public int LongTermCount { get; set; }
    /// <summary>已过期会员数量</summary>
    [JsonPropertyName("expired_count")]
    public int ExpiredCount { get; set; }
}

public class MemberLevelCount
{
    [JsonPropertyName("level")]
    public string Level { get; set; } = string.Empty;
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class MemberChannelCount
{
    [JsonPropertyName("channel")]
    public string Channel { get; set; } = string.Empty;
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

/// <summary>
/// 会员等级分布（专门用于侧边栏分类计数）
/// </summary>
public class MemberLevelStatsDto
{
    /// <summary>会员总数</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
    /// <summary>各等级会员数量</summary>
    [JsonPropertyName("level_counts")]
    public List<MemberLevelCount> LevelCounts { get; set; } = new();
}

/// <summary>
/// 用户统计数据（用于侧边栏分类计数）
/// </summary>
public class UserStatsDto
{
    /// <summary>全部用户数量</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
    /// <summary>正常用户数量</summary>
    [JsonPropertyName("active_count")]
    public int ActiveCount { get; set; }
    /// <summary>禁言中用户数量</summary>
    [JsonPropertyName("muted_count")]
    public int MutedCount { get; set; }
    /// <summary>已禁用用户数量</summary>
    [JsonPropertyName("disabled_count")]
    public int DisabledCount { get; set; }
    /// <summary>会员用户数量</summary>
    [JsonPropertyName("member_count")]
    public int MemberCount { get; set; }
    /// <summary>非会员用户数量</summary>
    [JsonPropertyName("non_member_count")]
    public int NonMemberCount { get; set; }
    /// <summary>各注册方式用户数量</summary>
    [JsonPropertyName("platform_counts")]
    public List<PlatformCount> PlatformCounts { get; set; } = new();
}

public class PlatformCount
{
    [JsonPropertyName("platform")]
    public string Platform { get; set; } = string.Empty;
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

/// <summary>
/// 后台创建用户请求
/// </summary>
public class CreateUserRequest
{
    /// <summary>昵称</summary>
    public string Nickname { get; set; } = string.Empty;
    /// <summary>手机号</summary>
    public string? Phone { get; set; }
    /// <summary>性别: male/female/unknown</summary>
    public string Gender { get; set; } = "unknown";
    /// <summary>城市</summary>
    public string? City { get; set; }
}

/// <summary>
/// 批量导入用户结果
/// </summary>
public class ImportUserResult
{
    /// <summary>成功数量</summary>
    [JsonPropertyName("success_count")]
    public int SuccessCount { get; set; }
    /// <summary>失败数量</summary>
    [JsonPropertyName("fail_count")]
    public int FailCount { get; set; }
    /// <summary>失败详情</summary>
    [JsonPropertyName("fail_details")]
    public List<ImportFailDetail> FailDetails { get; set; } = new();
}

/// <summary>
/// 导入失败详情
/// </summary>
public class ImportFailDetail
{
    /// <summary>行号</summary>
    [JsonPropertyName("row")]
    public int Row { get; set; }
    /// <summary>原因</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;
}
