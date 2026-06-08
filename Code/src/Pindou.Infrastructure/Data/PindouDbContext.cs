using SqlSugar;
using Pindou.Domain.Common;
using Pindou.Domain.Entities.Admin;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.Creation;
using Pindou.Domain.Entities.Member;
using Pindou.Domain.Entities.Messaging;
using Pindou.Domain.Entities.Operation;
using Pindou.Domain.Entities.Statistics;
using Pindou.Domain.Entities.System;
using Pindou.Domain.Entities.Template;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Options;

namespace Pindou.Infrastructure.Data;

/// <summary>
/// SqlSugar 数据库上下文
/// </summary>
public class PindouDbContext
{
    public ISqlSugarClient Db { get; }

    public PindouDbContext(DatabaseOptions options)
    {
        Db = new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = options.ConnectionString,
            DbType = options.DbType,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute,
            ConfigureExternalServices = new ConfigureExternalServices
            {
                EntityService = (c, p) =>
                {
                    // 统一处理
                    if (typeof(ISoftDelete).IsAssignableFrom(c.PropertyType))
                    {
                        // 软删除字段
                    }
                }
            }
        });

        Db.Aop.OnLogExecuting = (sql, pars) =>
        {
            // 由 LoggerFactory 接管，避免硬编码
        };
    }

    /// <summary>
    /// 初始化建表
    /// </summary>
    public void InitTables()
    {
        // 用户中心
        Db.CodeFirst.InitTables(typeof(User), typeof(Token), typeof(Device));
        // 创作中心
        Db.CodeFirst.InitTables(typeof(Diagram), typeof(ColorInfo), typeof(DiagramTask));
        // 社区
        Db.CodeFirst.InitTables(typeof(Post), typeof(Comment), typeof(Like), typeof(Favorite), typeof(Follow), typeof(Topic));
        // 模板
        Db.CodeFirst.InitTables(typeof(Template), typeof(TemplateCategory), typeof(TemplateFavorite), typeof(TemplateTag), typeof(TemplateReviewLog));
        // 会员
        Db.CodeFirst.InitTables(typeof(Member), typeof(Order), typeof(MemberProduct));
        // 消息
        Db.CodeFirst.InitTables(typeof(Message), typeof(MessageSetting));
        // 运营
        Db.CodeFirst.InitTables(typeof(Banner), typeof(OperationTopic), typeof(SpecialTopic), typeof(PushRecord));
        // 统计
        Db.CodeFirst.InitTables(typeof(DailyStats));
        // 系统
        Db.CodeFirst.InitTables(typeof(SystemConfig), typeof(MardColor), typeof(BeadKit), typeof(SensitiveWord), typeof(Report), typeof(PostReviewLog));
        // 后台
        Db.CodeFirst.InitTables(typeof(AdminUser), typeof(Role), typeof(OperationLog));
    }

    public void Dispose()
    {
        Db.Dispose();
    }

    /// <summary>
    /// 确保数据库本身存在（PostgreSQL 需要先建库才能建表）
    /// </summary>
    public void EnsureDatabase()
    {
        // 1) 优先尝试直接连接目标库
        try
        {
            Db.Ado.Connection.Open();
            Db.Ado.Connection.Close();
            return;
        }
        catch
        {
            // 目标库不存在，继续尝试建库
        }

        // 2) 退化为：解析连接串，连到默认库（postgres）后建库
        try
        {
            var original = Db.Ado.Connection.ConnectionString;
            var connString = new Npgsql.NpgsqlConnectionStringBuilder(original) { Database = "postgres" }.ToString();
            using var bootstrap = new Npgsql.NpgsqlConnection(connString);
            bootstrap.Open();
            using var cmd = bootstrap.CreateCommand();
            var targetDb = new Npgsql.NpgsqlConnectionStringBuilder(original).Database;
            if (!string.IsNullOrWhiteSpace(targetDb))
            {
                // 库名加引号，避免关键字冲突
                cmd.CommandText = $"CREATE DATABASE \"{targetDb}\"";
                cmd.ExecuteNonQuery();
            }
        }
        catch
        {
            // 若库已存在或建库失败，交给后续 InitTables 抛错提示
        }
    }
}
