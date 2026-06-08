using System.Collections.Concurrent;
using StackExchange.Redis;

namespace Pindou.Infrastructure.Cache;

/// <summary>
/// 带回退的缓存服务: Redis 可用时使用 Redis, 不可用时回退到进程内内存。
/// 适用于本地开发或 Redis 临时不可用的场景。
/// </summary>
public class FallbackCacheService : ICacheService
{
    private readonly IConnectionMultiplexer? _redis;
    private readonly InMemoryCache _fallback = new();

    public FallbackCacheService(IConnectionMultiplexer? redis = null)
    {
        _redis = redis;
    }

    private IDatabase? Db => _redis?.IsConnected == true ? _redis.GetDatabase() : null;

    private static string Key(string key) => key;

    public async Task<T?> GetAsync<T>(string key)
    {
        var db = Db;
        if (db == null) return _fallback.Get<T>(key);
        var v = await db.StringGetAsync(Key(key));
        if (v.IsNullOrEmpty) return default;
        return System.Text.Json.JsonSerializer.Deserialize<T>(v!);
    }

    public async Task<string?> GetStringAsync(string key)
    {
        var db = Db;
        if (db == null) return _fallback.GetString(key);
        var v = await db.StringGetAsync(Key(key));
        return v.IsNullOrEmpty ? null : v.ToString();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var db = Db;
        if (db == null) { _fallback.Set(key, value, expiry); return; }
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        await db.StringSetAsync(Key(key), json, expiry);
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
    {
        var db = Db;
        if (db == null) { _fallback.SetString(key, value, expiry); return; }
        await db.StringSetAsync(Key(key), value, expiry);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        var db = Db;
        if (db == null) return _fallback.Remove(key);
        return await db.KeyDeleteAsync(Key(key));
    }

    public async Task<bool> RemoveByPrefixAsync(string prefix)
    {
        var db = Db;
        if (db == null) return _fallback.RemoveByPrefix(prefix);
        if (_redis == null) return false;
        var endpoints = _redis.GetEndPoints();
        var deleted = 0L;
        foreach (var ep in endpoints)
        {
            var server = _redis.GetServer(ep);
            if (!server.IsConnected) continue;
            await foreach (var k in server.KeysAsync(pattern: Key(prefix) + "*"))
            {
                if (await db.KeyDeleteAsync(k)) deleted++;
            }
        }
        return deleted > 0;
    }

    public async Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiry = null)
    {
        var db = Db;
        if (db == null) return _fallback.Increment(key, value, expiry);
        var result = await db.StringIncrementAsync(Key(key), value);
        if (expiry.HasValue) await db.KeyExpireAsync(Key(key), expiry);
        return result;
    }

    public async Task<long> DecrementAsync(string key, long value = 1)
    {
        var db = Db;
        if (db == null) return _fallback.Decrement(key, value);
        return await db.StringDecrementAsync(Key(key), value);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var db = Db;
        if (db == null) return _fallback.Exists(key);
        return await db.KeyExistsAsync(Key(key));
    }

    public async Task<bool> SetNxAsync(string key, string value, TimeSpan expiry)
    {
        var db = Db;
        if (db == null) return _fallback.SetNx(key, value, expiry);
        return await db.StringSetAsync(Key(key), value, expiry, When.NotExists);
    }

    public Task<HashEntry[]> HashGetAllAsync(string key)
    {
        var db = Db;
        if (db == null) return Task.FromResult(Array.Empty<HashEntry>());
        return db.HashGetAllAsync(Key(key));
    }

    public async Task<T?> HashGetAsync<T>(string key, string field)
    {
        var db = Db;
        if (db == null) return default;
        var v = await db.HashGetAsync(Key(key), field);
        if (v.IsNullOrEmpty) return default;
        return System.Text.Json.JsonSerializer.Deserialize<T>(v!);
    }

    public async Task HashSetAsync(string key, string field, object value)
    {
        var db = Db;
        if (db == null) return;
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        await db.HashSetAsync(Key(key), field, json);
    }

    public async Task<long> HashIncrementAsync(string key, string field, long value = 1)
    {
        var db = Db;
        if (db == null) return 0;
        return await db.HashIncrementAsync(Key(key), field, value);
    }
}

internal class InMemoryCache
{
    private readonly ConcurrentDictionary<string, CacheEntry> _store = new();

    private class CacheEntry
    {
        public object? Value { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public T? Get<T>(string key)
    {
        if (!_store.TryGetValue(key, out var entry)) return default;
        if (entry.ExpiresAt.HasValue && entry.ExpiresAt.Value < DateTime.UtcNow)
        {
            _store.TryRemove(key, out _);
            return default;
        }
        if (entry.Value is T t) return t;
        if (entry.Value == null) return default;
        return System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(entry.Value));
    }

    public string? GetString(string key)
    {
        if (!_store.TryGetValue(key, out var entry)) return null;
        if (entry.ExpiresAt.HasValue && entry.ExpiresAt.Value < DateTime.UtcNow)
        {
            _store.TryRemove(key, out _);
            return null;
        }
        return entry.Value?.ToString();
    }

    public void Set<T>(string key, T value, TimeSpan? expiry)
    {
        _store[key] = new CacheEntry { Value = value, ExpiresAt = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : null };
    }

    public void SetString(string key, string value, TimeSpan? expiry)
    {
        _store[key] = new CacheEntry { Value = value, ExpiresAt = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : null };
    }

    public bool Remove(string key) => _store.TryRemove(key, out _);

    public bool RemoveByPrefix(string prefix)
    {
        var removed = 0;
        foreach (var k in _store.Keys.Where(k => k.StartsWith(prefix)).ToList())
            if (_store.TryRemove(k, out _)) removed++;
        return removed > 0;
    }

    public long Increment(string key, long value, TimeSpan? expiry)
    {
        var current = Get<long>(key);
        var newVal = current + value;
        Set(key, newVal, expiry);
        return newVal;
    }

    public long Decrement(string key, long value)
    {
        var current = Get<long>(key);
        var newVal = current - value;
        Set(key, newVal, null);
        return newVal;
    }

    public bool Exists(string key)
    {
        if (!_store.TryGetValue(key, out var entry)) return false;
        if (entry.ExpiresAt.HasValue && entry.ExpiresAt.Value < DateTime.UtcNow)
        {
            _store.TryRemove(key, out _);
            return false;
        }
        return true;
    }

    public bool SetNx(string key, string value, TimeSpan expiry)
    {
        if (Exists(key)) return false;
        SetString(key, value, expiry);
        return true;
    }
}
