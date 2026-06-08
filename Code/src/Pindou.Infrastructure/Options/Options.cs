using SqlSugar;

namespace Pindou.Infrastructure.Options;

/// <summary>
/// 数据库配置
/// </summary>
public class DatabaseOptions
{
    public const string SectionName = "Database";

    public string ConnectionString { get; set; } = "Data Source=pindou.db";
    public DbType DbType { get; set; } = DbType.Sqlite;
    public bool EnableLog { get; set; } = false;
}

/// <summary>
/// Redis配置
/// </summary>
public class RedisOptions
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; set; } = "localhost:6379";
    public int Database { get; set; } = 0;
    public string Prefix { get; set; } = "pindou:";
}

/// <summary>
/// JWT配置
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = "PindouAppSecretKey2026PindouAppSecretKey2026";
    public string Issuer { get; set; } = "Pindou";
    public string Audience { get; set; } = "Pindou";
    public int AccessTokenExpireMinutes { get; set; } = 60 * 24; // 1天
    public int RefreshTokenExpireMinutes { get; set; } = 60 * 24 * 30; // 30天
}

/// <summary>
/// 短信配置
/// </summary>
public class SmsOptions
{
    public const string SectionName = "Sms";

    public string Provider { get; set; } = "aliyun";
    public string AccessKey { get; set; } = string.Empty;
    public string AccessSecret { get; set; } = string.Empty;
    public string SignName { get; set; } = string.Empty;
    public string TemplateCode { get; set; } = string.Empty;
}

/// <summary>
/// 微信配置
/// </summary>
public class WechatOptions
{
    public const string SectionName = "Wechat";

    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
}

/// <summary>
/// 推送配置
/// </summary>
public class PushOptions
{
    public const string SectionName = "Push";
    public string Provider { get; set; } = "jpush";
    public string AppKey { get; set; } = string.Empty;
    public string MasterSecret { get; set; } = string.Empty;
}

/// <summary>
/// AI生图服务配置
/// </summary>
public class AiOptions
{
    public const string SectionName = "Ai";

    public string Provider { get; set; } = "aliyun";
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public int Timeout { get; set; } = 60;
    /// <summary>同步阈值(颗数)，小于此值同步处理</summary>
    public int SyncThreshold { get; set; } = 5000;
}

/// <summary>
/// 文件存储配置
/// </summary>
public class StorageOptions
{
    public const string SectionName = "Storage";

    public string Provider { get; set; } = "local";
    public string LocalPath { get; set; } = "./uploads";
    public string PublicBaseUrl { get; set; } = "https://api.pindou.com";
    public string? QiniuAccessKey { get; set; }
    public string? QiniuSecretKey { get; set; }
    public string? QiniuBucket { get; set; }
    public string? QiniuDomain { get; set; }
}
