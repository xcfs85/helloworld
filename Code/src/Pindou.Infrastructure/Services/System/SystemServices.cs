using System.Text.Json;
using Pindou.Application.Common;
using Pindou.Application.DTOs.System;
using Pindou.Application.Interfaces.System;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.System;
using Pindou.Infrastructure.Cache;
using Pindou.Infrastructure.Repositories;
using SqlSugar;
using ICacheService = Pindou.Infrastructure.Cache.ICacheService;
using UserEntity = Pindou.Domain.Entities.User.User;

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
            return JsonSerializer.Deserialize<T>(value);
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

    public async Task<List<SystemConfigDto>> GetAllAsync()
    {
        var configs = await _configRepo.GetListAsync();
        return configs.Select(c => new SystemConfigDto
        {
            Id = c.Id,
            ConfigKey = c.ConfigKey,
            ConfigValue = c.ConfigValue,
            ConfigType = c.ConfigType,
            Description = c.Description,
            Status = c.Status,
            CreateTime = c.CreateTime,
            UpdateTime = c.UpdateTime
        }).ToList();
    }

    public async Task<string?> GetMardColorAsync(string colorNo)
    {
        var cacheKey = $"sys:mard:{colorNo}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null) return cached;

        var color = await _mardRepo.FirstOrDefaultAsync(c => c.ColorNo == colorNo && c.Status == 1);
        if (color == null) return null;

        var json = JsonSerializer.Serialize(new MardColorDto
        {
            Id = color.Id,
            ColorNo = color.ColorNo,
            ColorName = color.ColorName,
            Rgb = color.Rgb,
            Lab = color.Lab,
            Category = color.Category,
            IsCommon = color.IsCommon == 1
        });
        await _cache.SetStringAsync(cacheKey, json, TimeSpan.FromHours(1));
        return json;
    }

    public async Task<List<MardColorDto>> GetAllMardColorsAsync()
    {
        var cacheKey = "sys:mard:all";
        var cached = await _cache.GetAsync<List<MardColorDto>>(cacheKey);
        if (cached != null) return cached;

        var colors = await _mardRepo.GetListAsync(c => c.Status == 1);
        var result = colors.Select(c => new MardColorDto
        {
            Id = c.Id,
            ColorNo = c.ColorNo,
            ColorName = c.ColorName,
            Rgb = c.Rgb,
            Lab = c.Lab,
            Category = c.Category,
            IsCommon = c.IsCommon == 1
        }).ToList();

        await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(1));
        return result;
    }

    public async Task<List<BeadKitDto>> GetAllBeadKitsAsync(int? colorCount = null)
    {
        var exp = Expressionable.Create<BeadKit>().And(k => k.Status == 1);
        if (colorCount.HasValue)
            exp.And(k => k.ColorCount <= colorCount.Value);

        var kits = await _kitRepo.GetListAsync(exp.ToExpression());
        return kits.Select(k => new BeadKitDto
        {
            Id = k.Id,
            KitId = k.KitId,
            KitName = k.KitName,
            Brand = k.Brand,
            ColorCount = k.ColorCount,
            BeadCount = k.BeadCount,
            Price = k.Price,
            PurchaseUrl = k.PurchaseUrl
        }).ToList();
    }

    public async Task<bool> RecommendKitAsync(int requiredColors)
    {
        var kits = await _kitRepo.GetListAsync(k => k.Status == 1 && k.ColorCount >= requiredColors);
        return kits.Count > 0;
    }
}

public class ContentReviewService : IContentReviewService
{
    private readonly IRepository<SensitiveWord> _sensitiveRepo;
    private readonly IRepository<Report> _reportRepo;
    private readonly IRepository<PostReviewLog> _reviewLogRepo;
    private readonly IRepository<Post> _postRepo;
    private readonly IRepository<UserEntity> _userRepo;
    private readonly ICacheService _cache;

    public ContentReviewService(
        IRepository<SensitiveWord> sensitiveRepo,
        IRepository<Report> reportRepo,
        IRepository<PostReviewLog> reviewLogRepo,
        IRepository<Post> postRepo,
        IRepository<UserEntity> userRepo,
        ICacheService cache)
    {
        _sensitiveRepo = sensitiveRepo;
        _reportRepo = reportRepo;
        _reviewLogRepo = reviewLogRepo;
        _postRepo = postRepo;
        _userRepo = userRepo;
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

    public async Task<bool> ReviewPostAsync(string postId, string reviewerId, bool approved, string? reason = null)
    {
        var post = await _postRepo.GetByIdAsync(postId);
        if (post == null) throw new BizException("帖子不存在", ErrorCodes.NotFound);

        post.ReviewStatus = approved ? "approved" : "rejected";
        if (!approved) post.ReviewReason = reason;
        post.UpdateTime = DateTime.Now;
        await _postRepo.UpdateAsync(post);

        // 记录审核日志
        await _reviewLogRepo.InsertAsync(new PostReviewLog
        {
            PostId = postId,
            ReviewerId = reviewerId,
            Action = approved ? "approve" : "reject",
            Reason = reason
        });

        return true;
    }

    public async Task<PagedResult<Pindou.Application.DTOs.Community.AdminPostDto>> GetAdminPostsAsync(Pindou.Application.DTOs.Community.PostAdminQuery query)
    {
        var exp = Expressionable.Create<Post>().And(p => p.Status == "active");

        // 按审核状态筛选
        if (!string.IsNullOrWhiteSpace(query.ReviewStatus))
            exp.And(p => p.ReviewStatus == query.ReviewStatus);

        // 按内容类型筛选
        if (!string.IsNullOrWhiteSpace(query.Type))
            exp.And(p => p.Type == query.Type);

        // 按AI风险等级筛选
        if (!string.IsNullOrWhiteSpace(query.RiskLevel))
            exp.And(p => p.RiskLevel == query.RiskLevel);

        // 关键词搜索（标题或内容）
        if (!string.IsNullOrWhiteSpace(query.Keyword))
            exp.And(p => p.Title.Contains(query.Keyword) || p.Content.Contains(query.Keyword));

        // 时间范围筛选
        if (query.StartTime.HasValue)
            exp.And(p => p.PublishTime >= query.StartTime.Value);
        if (query.EndTime.HasValue)
            exp.And(p => p.PublishTime <= query.EndTime.Value);

        var (list, total) = await _postRepo.GetPagedAsync(
            exp.ToExpression(),
            query.Page,
            query.Size,
            p => p.PublishTime,
            true);

        var result = new PagedResult<Pindou.Application.DTOs.Community.AdminPostDto>
        {
            Page = query.Page,
            Size = query.Size,
            Total = total,
            List = new List<Pindou.Application.DTOs.Community.AdminPostDto>()
        };

        foreach (var post in list)
        {
            // 填充作者信息
            var author = await _userRepo.GetByIdAsync(post.UserId);

            // 反序列化媒体
            var media = DeserializeMedia(post.Media);

            // 反序列化风险标签
            var riskTags = new List<string>();
            if (!string.IsNullOrWhiteSpace(post.RiskTags))
            {
                try { riskTags = JsonSerializer.Deserialize<List<string>>(post.RiskTags) ?? new(); }
                catch { /* 忽略反序列化异常 */ }
            }

            result.List.Add(new Pindou.Application.DTOs.Community.AdminPostDto
            {
                Id = post.Id,
                Type = post.Type,
                Title = post.Title,
                Content = post.Content,
                Media = media,
                DiagramId = post.DiagramId,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                FavoriteCount = post.FavoriteCount,
                ViewCount = post.ViewCount,
                Status = post.Status,
                ReviewStatus = post.ReviewStatus,
                ReviewReason = post.ReviewReason,
                RiskLevel = post.RiskLevel,
                RiskTags = riskTags,
                PublishTime = post.PublishTime,
                CreateTime = post.CreateTime,
                Author = new Pindou.Application.DTOs.Community.AuthorBrief
                {
                    Id = author?.Id ?? string.Empty,
                    Nickname = author?.Nickname ?? string.Empty,
                    Avatar = author?.Avatar,
                    IsMember = author?.IsMember ?? false
                }
            });
        }

        return result;
    }

    private static List<Pindou.Application.DTOs.Community.MediaItem> DeserializeMedia(string media)
    {
        try { return JsonSerializer.Deserialize<List<Pindou.Application.DTOs.Community.MediaItem>>(media) ?? new(); }
        catch { return new(); }
    }

    public async Task<bool> HandleReportAsync(string reportId, string handlerId, string action, string? result = null)
    {
        var report = await _reportRepo.GetByIdAsync(reportId);
        if (report == null) throw new BizException("举报不存在", ErrorCodes.NotFound);

        report.Status = action;
        report.HandleResult = result;
        report.HandlerId = handlerId;
        report.HandleTime = DateTime.Now;
        report.UpdateTime = DateTime.Now;
        await _reportRepo.UpdateAsync(report);

        return true;
    }

    public async Task<PagedResult<ReportDto>> GetReportsAsync(ReportQuery query)
    {
        var exp = Expressionable.Create<Report>();
        if (!string.IsNullOrWhiteSpace(query.TargetType))
            exp.And(r => r.TargetType == query.TargetType);
        if (!string.IsNullOrWhiteSpace(query.Status))
            exp.And(r => r.Status == query.Status);
        if (!string.IsNullOrWhiteSpace(query.Reason))
            exp.And(r => r.Reason == query.Reason);

        var (list, total) = await _reportRepo.GetPagedAsync(
            exp.ToExpression(),
            query.Page,
            query.Size,
            r => r.CreateTime,
            true);

        var result = new PagedResult<ReportDto>
        {
            Page = query.Page,
            Size = query.Size,
            Total = total,
            List = new List<ReportDto>()
        };

        foreach (var report in list)
        {
            var images = new List<string>();
            if (!string.IsNullOrWhiteSpace(report.Images))
            {
                try { images = JsonSerializer.Deserialize<List<string>>(report.Images) ?? new(); }
                catch { }
            }

            result.List.Add(new ReportDto
            {
                Id = report.Id,
                ReportId = report.ReportId,
                ReporterId = report.ReporterId,
                ReporterName = string.Empty,
                TargetType = report.TargetType,
                TargetId = report.TargetId,
                TargetUserId = report.TargetUserId,
                TargetUserName = string.Empty,
                Reason = report.Reason,
                Content = report.Content,
                Images = images,
                Status = report.Status,
                HandleResult = report.HandleResult,
                CreateTime = report.CreateTime,
                HandleTime = report.HandleTime
            });
        }

        return result;
    }

    public async Task<List<SensitiveWordDto>> GetSensitiveWordsAsync(string? type = null)
    {
        var exp = Expressionable.Create<SensitiveWord>();
        if (!string.IsNullOrWhiteSpace(type))
            exp.And(w => w.Type == type);

        var words = await _sensitiveRepo.GetListAsync(exp.ToExpression());
        return words.Select(w => new SensitiveWordDto
        {
            Id = w.Id,
            Word = w.Word,
            Level = w.Level,
            Type = w.Type,
            ReplaceWord = w.ReplaceWord,
            Status = w.Status
        }).ToList();
    }

    public async Task<string> AddSensitiveWordAsync(AddSensitiveWordRequest request)
    {
        var exists = await _sensitiveRepo.AnyAsync(w => w.Word == request.Word && w.Status == 1);
        if (exists) throw new BizException("敏感词已存在", ErrorCodes.AlreadyExists);

        var word = new SensitiveWord
        {
            Word = request.Word,
            Level = request.Level,
            Type = request.Type,
            ReplaceWord = request.ReplaceWord,
            Status = request.Status
        };
        await _sensitiveRepo.InsertAsync(word);
        return word.Id;
    }

    public async Task<bool> UpdateSensitiveWordAsync(string id, AddSensitiveWordRequest request)
    {
        var word = await _sensitiveRepo.GetByIdAsync(id);
        if (word == null) throw new BizException("敏感词不存在", ErrorCodes.NotFound);

        word.Word = request.Word;
        word.Level = request.Level;
        word.Type = request.Type;
        word.ReplaceWord = request.ReplaceWord;
        word.Status = request.Status;
        word.UpdateTime = DateTime.Now;
        return await _sensitiveRepo.UpdateAsync(word);
    }

    public async Task<bool> DeleteSensitiveWordAsync(string id)
    {
        var word = await _sensitiveRepo.GetByIdAsync(id);
        if (word == null) throw new BizException("敏感词不存在", ErrorCodes.NotFound);

        word.Status = 0;
        word.UpdateTime = DateTime.Now;
        return await _sensitiveRepo.UpdateAsync(word);
    }
}