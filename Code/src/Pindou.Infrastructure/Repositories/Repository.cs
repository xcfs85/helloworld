using Pindou.Domain.Common;
using SqlSugar;
using System.Transactions;
using System.Linq.Expressions;

namespace Pindou.Infrastructure.Repositories;

/// <summary>
/// 通用仓储接口
/// </summary>
public interface IRepository<TE> where TE : class, new()
{
    ISqlSugarClient Db { get; }

    #region 查询
    Task<TE?> GetByIdAsync(object id);
    Task<List<TE>> GetListAsync(Expression<Func<TE, bool>>? where = null);
    Task<List<TE>> GetListAsync(Expression<Func<TE, bool>> where, string orderByProperty, bool isDesc = true);
    Task<TE?> FirstOrDefaultAsync(Expression<Func<TE, bool>> where);
    Task<int> CountAsync(Expression<Func<TE, bool>>? where = null);
    Task<bool> AnyAsync(Expression<Func<TE, bool>> where);
    #endregion

    #region 分页
    Task<(List<TE> list, int total)> GetPagedAsync(
        Expression<Func<TE, bool>>? where,
        int page,
        int size,
        Expression<Func<TE, object>>? orderBy = null,
        bool isDesc = true);
    #endregion

    #region 写入
    Task<object> InsertAsync(TE entity);
    Task<List<object>> InsertRangeAsync(List<TE> entities);
    Task<bool> UpdateAsync(TE entity);
    Task<bool> UpdateRangeAsync(List<TE> entities);
    Task<bool> DeleteAsync(object id);
    Task<bool> DeleteAsync(Expression<Func<TE, bool>> where);
    Task<bool> DeleteRangeAsync(List<TE> entities);
    #endregion
}

/// <summary>
/// 通用仓储实现
/// </summary>
public class Repository<TE> : IRepository<TE> where TE : class, new()
{
    public ISqlSugarClient Db { get; }

    public Repository(Pindou.Infrastructure.Data.PindouDbContext dbContext)
    {
        Db = dbContext.Db;
    }

    public async Task<TE?> GetByIdAsync(object id)
    {
        return await Db.Queryable<TE>().InSingleAsync(id);
    }

    public async Task<List<TE>> GetListAsync(Expression<Func<TE, bool>>? where = null)
    {
        return await Db.Queryable<TE>().Where(where ?? (Expression<Func<TE, bool>>)(e => true)).ToListAsync();
    }

    /// <summary>
    /// 使用表达式树构建 OrderBy，SqlSugar 才能正确映射列名
    /// </summary>
    public async Task<List<TE>> GetListAsync(Expression<Func<TE, bool>> where, string orderByProperty, bool isDesc = true)
    {
        // 使用表达式树构建 OrderBy，避免字符串拼接
        var parameter = Expression.Parameter(typeof(TE), "x");
        var property = Expression.Property(parameter, orderByProperty);
        var converted = Expression.Convert(property, typeof(object));
        var lambda = Expression.Lambda<Func<TE, object>>(converted, parameter);

        var query = Db.Queryable<TE>().Where(where);
        query = query.OrderBy(lambda, isDesc ? OrderByType.Desc : OrderByType.Asc);
        return await query.ToListAsync();
    }

    public async Task<TE?> FirstOrDefaultAsync(Expression<Func<TE, bool>> where)
    {
        return await Db.Queryable<TE>().FirstAsync(where);
    }

    public async Task<int> CountAsync(Expression<Func<TE, bool>>? where = null)
    {
        return await Db.Queryable<TE>().CountAsync(where ?? (Expression<Func<TE, bool>>)(e => true));
    }

    public async Task<bool> AnyAsync(Expression<Func<TE, bool>> where)
    {
        return await Db.Queryable<TE>().AnyAsync(where);
    }

    public async Task<(List<TE> list, int total)> GetPagedAsync(
        Expression<Func<TE, bool>>? where,
        int page,
        int size,
        Expression<Func<TE, object>>? orderBy = null,
        bool isDesc = true)
    {
        var query = Db.Queryable<TE>().Where(where ?? (Expression<Func<TE, bool>>)(e => true));
        if (orderBy != null)
        {
            query = query.OrderBy(orderBy, isDesc ? OrderByType.Desc : OrderByType.Asc);
        }
        var total = await query.CountAsync();
        var list = await query.Skip((page - 1) * size).Take(size).ToListAsync();
        return (list, total);
    }

    public async Task<object> InsertAsync(TE entity)
    {
        // 兼容 long/string 主键
        var id = await Db.Insertable(entity).ExecuteReturnIdentityAsync();
        if (id > 0)
        {
            ((dynamic)entity).Id = id;
            return id;
        }
        // 当主键为 string（UUID）或非自增时，尝试使用 ExecuteReturnEntityAsync
        try
        {
            var inserted = await Db.Insertable(entity).ExecuteReturnEntityAsync();
            return ((dynamic)inserted).Id!;
        }
        catch
        {
            await Db.Insertable(entity).ExecuteCommandAsync();
            return ((dynamic)entity).Id!;
        }
    }

    public async Task<List<object>> InsertRangeAsync(List<TE> entities)
    {
        // 兼容 long/string 主键：使用 ExecuteCommandAsync 让库自行处理主键生成
        var count = await Db.Insertable(entities).ExecuteCommandAsync();
        return entities.Select(e => (object)((dynamic)e).Id!).ToList();
    }

    public async Task<bool> UpdateAsync(TE entity)
    {
        return await Db.Updateable(entity).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> UpdateRangeAsync(List<TE> entities)
    {
        return await Db.Updateable(entities).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> DeleteAsync(object id)
    {
        return await Db.Deleteable<TE>().In(id).ExecuteCommandAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Expression<Func<TE, bool>> where)
    {
        return await Db.Deleteable<TE>().Where(where).ExecuteCommandAsync() > 0;
    }

    public async Task<bool> DeleteRangeAsync(List<TE> entities)
    {
        return await Db.Deleteable(entities).ExecuteCommandAsync() > 0;
    }
}