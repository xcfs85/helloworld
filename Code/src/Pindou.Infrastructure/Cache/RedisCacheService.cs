using StackExchange.Redis;
using System.Text.Json;
using Pindou.Infrastructure.Options;

namespace Pindou.Infrastructure.Cache;

/// <summary>
/// Redis缓存服务接口
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<string?> GetStringAsync(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
    Task<bool> RemoveAsync(string key);
    Task<bool> RemoveByPrefixAsync(string prefix);
    Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiry = null);
    Task<long> DecrementAsync(string key, long value = 1);
    Task<bool> ExistsAsync(string key);
    Task<bool> SetNxAsync(string key, string value, TimeSpan expiry);
    Task<HashEntry[]> HashGetAllAsync(string key);
    Task<T?> HashGetAsync<T>(string key, string field);
    Task HashSetAsync(string key, string field, object value);
    Task<long> HashIncrementAsync(string key, string field, long value = 1);
}

/// <summary>
/// Redis缓存服务实现
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly RedisOptions _options;
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis, RedisOptions options)
    {
        _redis = redis;
        _options = options;
        _db = redis.GetDatabase(options.Database);
    }

    private string Key(string key) => $"{_options.Prefix}{key}";

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(Key(key));
        if (value.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task<string?> GetStringAsync(string key)
    {
        var value = await _db.StringGetAsync(Key(key));
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(Key(key), json, expiry);
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
    {
        await _db.StringSetAsync(Key(key), value, expiry);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _db.KeyDeleteAsync(Key(key));
    }

    public async Task<bool> RemoveByPrefixAsync(string prefix)
    {
        var endpoints = _redis.GetEndPoints();
        var deleted = 0L;
        foreach (var ep in endpoints)
        {
            var server = _redis.GetServer(ep);
            if (!server.IsConnected) continue;
            await foreach (var k in server.KeysAsync(pattern: Key(prefix) + "*"))
            {
                if (await _db.KeyDeleteAsync(k)) deleted++;
            }
        }
        return deleted > 0;
    }

    public async Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiry = null)
    {
        var result = await _db.StringIncrementAsync(Key(key), value);
        if (expiry.HasValue) await _db.KeyExpireAsync(Key(key), expiry);
        return result;
    }

    public async Task<long> DecrementAsync(string key, long value = 1)
    {
        return await _db.StringDecrementAsync(Key(key), value);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(Key(key));
    }

    public async Task<bool> SetNxAsync(string key, string value, TimeSpan expiry)
    {
        return await _db.StringSetAsync(Key(key), value, expiry, When.NotExists);
    }

    public async Task<HashEntry[]> HashGetAllAsync(string key)
    {
        return await _db.HashGetAllAsync(Key(key));
    }

    public async Task<T?> HashGetAsync<T>(string key, string field)
    {
        var value = await _db.HashGetAsync(Key(key), field);
        if (value.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task HashSetAsync(string key, string field, object value)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.HashSetAsync(Key(key), field, json);
    }

    public async Task<long> HashIncrementAsync(string key, string field, long value = 1)
    {
        return await _db.HashIncrementAsync(Key(key), field, value);
    }
}
