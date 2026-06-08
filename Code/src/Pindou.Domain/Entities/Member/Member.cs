using SqlSugar;
using Pindou.Domain.Common;

namespace Pindou.Domain.Entities.Member;

/// <summary>
/// 会员表
/// </summary>
[SugarTable("members")]
public class Member : UuidEntity
{
    /// <summary>用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>会员类型:month/quarter/year/lifetime</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string MemberType { get; set; } = string.Empty;

    /// <summary>开始时间</summary>
    [SugarColumn(IsNullable = false)]
    public DateTime StartTime { get; set; } = DateTime.Now;

    /// <summary>过期时间</summary>
    [SugarColumn(IsNullable = false)]
    public DateTime ExpireTime { get; set; }
}

/// <summary>
/// 订单表
/// </summary>
[SugarTable("orders")]
public class Order : UuidEntity
{
    /// <summary>用户ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>订单号</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>商品类型:member/coupon</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string ProductType { get; set; } = string.Empty;

    /// <summary>商品ID</summary>
    [SugarColumn(IsNullable = false, Length = 36)]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>金额</summary>
    [SugarColumn(Length = 10, IsNullable = false)]
    public decimal Amount { get; set; }

    /// <summary>状态:pending/paid/canceled/refunded</summary>
    [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "'pending'")]
    public string Status { get; set; } = "pending";

    /// <summary>支付方式</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? PayMethod { get; set; }

    /// <summary>支付时间</summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? PayTime { get; set; }
}

/// <summary>
/// 会员产品表
/// </summary>
[SugarTable("member_products")]
public class MemberProduct : UuidEntity
{
    /// <summary>产品标识</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>产品名称</summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>会员等级</summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Grade { get; set; } = string.Empty;

    /// <summary>时长(天)</summary>
    [SugarColumn(IsNullable = false)]
    public int DurationDays { get; set; }

    /// <summary>价格</summary>
    [SugarColumn(Length = 10, IsNullable = false)]
    public decimal Price { get; set; }

    /// <summary>原价</summary>
    [SugarColumn(Length = 10, IsNullable = true)]
    public decimal? OriginalPrice { get; set; }

    /// <summary>每日生成次数(-1无限)</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "-1")]
    public int DailyGenerations { get; set; } = -1;

    /// <summary>权益列表(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Features { get; set; }

    /// <summary>状态:0-禁用 1-启用</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public int Status { get; set; }
}
