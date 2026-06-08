using Pindou.Domain.Common;
using SqlSugar;
using System.Linq.Expressions;

namespace Pindou.Infrastructure.Repositories;

/// <summary>
/// 通用仓储接口
/// </summary>
public interface IRepository<TEntity> where TEntity : class, new()
{
    ISqlSugarClient Db { get; }

    #region 查询
    Task<TEntity?> GetByIdAsync(object id);
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? where = null);
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> where, string orderBy, bool isDesc = true);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? where = null);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where);
    #endregion

    #region 分页
    Task<(List<TEntity> list, int total)> GetPagedAsync(
        Expression<Func<TEntity, bool>>? where,
        int page,
        int size,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool isDesc = true);
    #endregion

    #region 写入
    Task<object> InsertAsync(TEntity entity);
    Task<List<object>> InsertRangeAsync(List<TEntity> entities);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> UpdateRangeAsync(List<TEntity> entities);
    Task<bool> DeleteAsync(object id);
    Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> where);
    Task<bool> DeleteRangeAsync(List<TEntity> entities);
    #endregion
}

/// <summary>
/// 通用仓储实现
/// </summary>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
{
    public ISqlSugarClient Db { get; }

    public Repository(Pindou.Infrastructure.Data.PindouDbContext dbContext)
    {
        Db = dbContext.Db;
    }

    public async Task<TEntity?> GetByIdAsync(object id)
    {
        return await Db.Queryable<TEntity>().InSingleAsync(id);
    }

    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? where = null)
    {
        return await Db.Queryable<TEntity>().Where(where ?? (Expression<Func<TEntity, bool>>)(e => true)).ToListAsync();
    }

    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> where, string orderBy, bool isDesc = true)
    {
        var query = Db.Queryable<TEntity>().Where(where);
        query = query.OrderBy(orderBy + (isDesc ? " desc" : " asc"));
        return await query.ToListAsync();
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where)
    {
        return await Db.Queryable<TEntity>().FirstAsync(where);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? where = null)
    {
        return await Db.Queryable<TEntity>().CountAsync(where ?? (Expression<Func<TEntity, bool>>)(e => true));
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where)
    {
        return await Db.Queryable<TEntity>().AnyAsync(where);
    }

    public async Task<(List<TEntity> list, int total)> GetPagedAsync(
        Expression<Func<TEntity, bool>>? where,
        int page,
        int size,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool isDesc = true)
    {
        var query = Db.Queryable<TEntity>().Where(where ?? (Expression<Func<TEntity, bool>>)(e => true));
        if (orderBy != null)
        {
            query = query.OrderBy(orderBy, isDesc ? OrderByType.Desc : OrderByType.Asc);
        }
        var total = await query.CountAsync();
        var list = await query.Skip((page - 1) * size).Take(size).ToListAsync();
        return (list, total);
    }

    public async Task<object> InsertAsync(TEntity entity)
    {
        return await Db.Insertable(entity).ExecuteReturnSnowflakeIdAsync();
    }

    public async Task<List<object>> InsertRangeAsync(List<TEntity> entities)
    {
        return (await Db.Insertable(entities).ExecuteReturnSnowflakeIdListAsync()).Select(id => (object)id).ToList();
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        return await Db.Updateable(entity).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> UpdateRangeAsync(List<TEntity> entities)
    {
        return await Db.Updateable(entities).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> DeleteAsync(object id)
    {
        return await Db.Deleteable<TEntity>().In(id).ExecuteCommandAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> where)
    {
        return await Db.Deleteable<TEntity>().Where(where).ExecuteCommandAsync() > 0;
    }

    public async Task<bool> DeleteRangeAsync(List<TEntity> entities)
    {
        return await Db.Deleteable(entities).ExecuteCommandAsync() > 0;
    }
}
