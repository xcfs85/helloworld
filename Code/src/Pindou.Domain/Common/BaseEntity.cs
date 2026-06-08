using SqlSugar;

namespace Pindou.Domain.Common;

/// <summary>
/// 实体基类
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public long Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? UpdateTime { get; set; }
}

/// <summary>
/// 带UUID主键的实体基类
/// </summary>
public abstract class UuidEntity
{
    /// <summary>
    /// UUID主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, Length = 36, IsNullable = false)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? UpdateTime { get; set; }
}
