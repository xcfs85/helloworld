using SqlSugar;

namespace Pindou.Domain.Common;

/// <summary>
/// 软删除标记
/// </summary>
public interface ISoftDelete
{
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    bool IsDeleted { get; set; }
}

/// <summary>
/// 实体删除接口
/// </summary>
public interface IEntity
{
}

/// <summary>
/// 多租户接口（预留扩展）
/// </summary>
public interface IMultiTenant
{
    long TenantId { get; set; }
}
