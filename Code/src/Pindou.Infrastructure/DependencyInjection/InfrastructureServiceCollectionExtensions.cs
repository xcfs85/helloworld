using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pindou.Application.Interfaces.Admin;
using Pindou.Application.Interfaces.Auth;
using Pindou.Application.Interfaces.Community;
using Pindou.Application.Interfaces.Creation;
using Pindou.Application.Interfaces.Member;
using Pindou.Application.Interfaces.Messaging;
using Pindou.Application.Interfaces.Operation;
using Pindou.Application.Interfaces.Statistics;
using Pindou.Application.Interfaces.System;
using Pindou.Application.Interfaces.Template;
using Pindou.Application.Interfaces.User;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Data;
using Pindou.Infrastructure.Data.SeedData;
using Pindou.Infrastructure.ExternalServices.AI;
using Pindou.Infrastructure.ExternalServices.Push;
using Pindou.Infrastructure.ExternalServices.Sms;
using Pindou.Infrastructure.ExternalServices.Storage;
using Pindou.Infrastructure.Options;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Admin;
using Pindou.Infrastructure.Services.Auth;
using Pindou.Infrastructure.Services.Community;
using Pindou.Infrastructure.Services.Creation;
using Pindou.Infrastructure.Services.Member;
using Pindou.Infrastructure.Services.Messaging;
using Pindou.Infrastructure.Services.Operation;
using Pindou.Infrastructure.Services.Statistics;
using Pindou.Infrastructure.Services.System;
using Pindou.Infrastructure.Services.Template;
using Pindou.Infrastructure.Services.User;
using StackExchange.Redis;

namespace Pindou.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SmsOptions>(configuration.GetSection(SmsOptions.SectionName));
        services.Configure<WechatOptions>(configuration.GetSection(WechatOptions.SectionName));
        services.Configure<PushOptions>(configuration.GetSection(PushOptions.SectionName));
        services.Configure<AiOptions>(configuration.GetSection(AiOptions.SectionName));
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));

        // 数据库
        services.AddScoped(sp =>
        {
            var options = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();
            return new PindouDbContext(options);
        });
        services.AddScoped<DataSeeder>();

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>() ?? new RedisOptions();
            try
            {
                var configOptions = ConfigurationOptions.Parse(options.ConnectionString);
                configOptions.AbortOnConnectFail = false;
                configOptions.ConnectTimeout = 2000;
                return ConnectionMultiplexer.Connect(configOptions);
            }
            catch
            {
                // Redis 不可用时返回 null (调用方会回退到内存缓存)
                return null!;
            }
        });
        services.AddSingleton<ICacheService, FallbackCacheService>();

        // 仓储
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // 外部服务
        services.AddHttpClient<IAiGenerationService, AliyunAiGenerationService>();
        services.AddHttpClient<ISmsService, AliyunSmsService>();
        services.AddSingleton<IStorageService, LocalStorageService>();

        // 推送渠道提供者
        services.AddHttpClient<IPushChannelProvider, JPushProvider>();
        services.AddScoped<IPushChannelProvider, SmsPushProvider>();
        services.AddScoped<IPushChannelProvider, EmailPushProvider>();

        // 业务服务
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDiagramService, DiagramService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IFollowService, FollowService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IUserMemberService, UserMemberService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IOperationService, OperationService>();
        services.AddScoped<IPushService, PushService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<ISystemConfigService, SystemConfigService>();
        services.AddScoped<IContentReviewService, ContentReviewService>();

        // 后台
        services.AddScoped<IAdminAuthService, AdminAuthService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IOperationLogService, OperationLogService>();

        return services;
    }
}
