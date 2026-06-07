using Pindou.Application.Common;
using Pindou.Application.DTOs.System;
using Pindou.Application.Interfaces.System;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.System;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Repositories;

namespace Pindou.Infrastructure.Services.System;

public class SystemConfigService : ISystemConfigService
{
    private readonly IRepository<SystemConfig> _configRepo;
    private readonly IRepository<MardColor> _mardRepo;
    private readonly IRepository<BeadKit> _kitRepo;
    private readonly ICacheService _cache;

    public SystemConfigService(
        IRepository<SystemConfig> configRepo,
        IRepository<MardColor> mardRepo,
        IRepository<BeadKit> kitRepo,
        ICacheService cache)
    {
        _configRepo = configRepo;
        _mardRepo = mardRepo;
        _kitRepo = kitRepo;
        _cache = cache;
    }

    public async Task<string?> GetAsync(string key)
    {
        var cacheKey = $"sys:config:{key}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null) return cached;
        var cfg = await _configRepo.FirstOrDefaultAsync(c => c.ConfigKey == key);
        var value = cfg?.ConfigValue;
        if (value != null) await _cache.SetStringAsync(cacheKey, value, TimeSpan.FromMinutes(30));
        return value;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await GetAsync(key);
        if (value == null) return default;
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }
        catch
        {
            return default;
        }
    }

    public async Task SetAsync(string key, string value, string? type = null, string? description = null)
    {
        var cfg = await _configRepo.FirstOrDefaultAsync(c => c.ConfigKey == key);
        if (cfg == null)
        {
            cfg = new SystemConfig { ConfigKey = key, ConfigValue = value, ConfigType = type, Description = description };
            await _configRepo.InsertAsync(cfg);
        }
        else
        {
            cfg.ConfigValue = value;
            if (type != null) cfg.ConfigType = type;
            if (description != null) cfg.Description = description;
            cfg.UpdateTime = DateTime.Now;
            await _configRepo.UpdateAsync(cfg);
        }
        await _cache.RemoveAsync($"sys:config:{key}");
    }

    public Task<List<SystemConfigDto>> GetAllAsync() { throw new NotImplementedException(); }
    public Task<string?> GetMardColorAsync(string colorNo) { throw new NotImplementedException(); }
    public Task<List<MardColorDto>> GetAllMardColorsAsync() { throw new NotImplementedException(); }
    public Task<List<BeadKitDto>> GetAllBeadKitsAsync(int? colorCount = null) { throw new NotImplementedException(); }
    public Task<bool> RecommendKitAsync(int requiredColors) { throw new NotImplementedException(); }
}

public class ContentReviewService : IContentReviewService
{
    private readonly IRepository<SensitiveWord> _sensitiveRepo;
    private readonly IRepository<Report> _reportRepo;
    private readonly IRepository<PostReviewLog> _reviewLogRepo;
    private readonly ICacheService _cache;

    public ContentReviewService(
        IRepository<SensitiveWord> sensitiveRepo,
        IRepository<Report> reportRepo,
        IRepository<PostReviewLog> reviewLogRepo,
        ICacheService cache)
    {
        _sensitiveRepo = sensitiveRepo;
        _reportRepo = reportRepo;
        _reviewLogRepo = reviewLogRepo;
        _cache = cache;
    }

    public async Task<(bool Passed, string Reason, string? ReplacedContent)> CheckAsync(string content)
    {
        var words = await _sensitiveRepo.GetListAsync(w => w.Status == 1);
        var replaced = content;
        foreach (var w in words)
        {
            if (content.Contains(w.Word))
            {
                if (w.Level == 3) // 拦截
                    return (false, $"内容含敏感词: {w.Word}", null);
                if (w.Level == 2 && !string.IsNullOrEmpty(w.ReplaceWord)) // 替换
                    replaced = replaced.Replace(w.Word, w.ReplaceWord);
                // 警告：放行
            }
        }
        return (true, string.Empty, replaced != content ? replaced : null);
    }

    public Task<bool> ReviewPostAsync(string postId, string reviewerId, bool approved, string? reason = null) { throw new NotImplementedException(); }
    public Task<PagedResult<DTOs.Community.PostDto>> GetPendingPostsAsync(PageRequest request) { throw new NotImplementedException(); }
    public Task<bool> HandleReportAsync(string reportId, string handlerId, string action, string? result = null) { throw new NotImplementedException(); }
    public Task<PagedResult<ReportDto>> GetReportsAsync(ReportQuery query) { throw new NotImplementedException(); }
    public Task<List<SensitiveWordDto>> GetSensitiveWordsAsync(string? type = null) { throw new NotImplementedException(); }
    public Task<string> AddSensitiveWordAsync(AddSensitiveWordRequest request) { throw new NotImplementedException(); }
    public Task<bool> UpdateSensitiveWordAsync(string id, AddSensitiveWordRequest request) { throw new NotImplementedException(); }
    public Task<bool> DeleteSensitiveWordAsync(string id) { throw new NotImplementedException(); }
}
