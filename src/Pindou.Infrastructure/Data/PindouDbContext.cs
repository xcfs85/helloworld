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
                    if (typeof(ISoftDelete).IsAssignableFrom(c))
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
}
