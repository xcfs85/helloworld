using System.Text.Json;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Template;
using Pindou.Application.Interfaces.Template;
using Pindou.Domain.Entities.Template;
using Pindou.Domain.Entities.Creation;
using Pindou.Infrastructure.Repositories;
using SqlSugar;

namespace Pindou.Infrastructure.Services.Template;

public class TemplateService : ITemplateService
{
    private readonly IRepository<Template> _templateRepo;
    private readonly IRepository<TemplateCategory> _categoryRepo;
    private readonly IRepository<TemplateTag> _tagRepo;
    private readonly IRepository<TemplateFavorite> _favoriteRepo;
    private readonly IRepository<Diagram> _diagramRepo;

    public TemplateService(
        IRepository<Template> templateRepo,
        IRepository<TemplateCategory> categoryRepo,
        IRepository<TemplateTag> tagRepo,
        IRepository<TemplateFavorite> favoriteRepo,
        IRepository<Diagram> diagramRepo)
    {
        _templateRepo = templateRepo;
        _categoryRepo = categoryRepo;
        _tagRepo = tagRepo;
        _favoriteRepo = favoriteRepo;
        _diagramRepo = diagramRepo;
    }

    public async Task<PagedResult<TemplateDto>> GetTemplatesAsync(TemplateQuery query)
    {
        var exp = Expressionable.Create<Template>().And(t => t.Status == "active" && t.ReviewStatus == "approved");

        if (!string.IsNullOrWhiteSpace(query.CategoryId))
            exp.And(t => t.CategoryId == query.CategoryId);
        if (!string.IsNullOrWhiteSpace(query.Difficulty))
            exp.And(t => t.Difficulty == query.Difficulty);
        if (!string.IsNullOrWhiteSpace(query.BoardSize))
            exp.And(t => t.BoardSize == query.BoardSize);
        if (query.IsFeatured.HasValue)
            exp.And(t => t.IsFeatured == query.IsFeatured.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
            exp.And(t => t.Name.Contains(query.Keyword));

        var (list, total) = await _templateRepo.GetPagedAsync(
            exp.ToExpression(),
            query.Page,
            query.Size,
            t => t.ViewCount,
            true);

        var result = new PagedResult<TemplateDto>
        {
            Page = query.Page,
            Size = query.Size,
            Total = total,
            List = new List<TemplateDto>()
        };

        foreach (var template in list)
        {
            var tags = new List<string>();
            if (!string.IsNullOrWhiteSpace(template.Tags))
            {
                try { tags = JsonSerializer.Deserialize<List<string>>(template.Tags) ?? new(); }
                catch { }
            }

            var category = await _categoryRepo.GetByIdAsync(template.CategoryId);

            result.List.Add(new TemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                CategoryId = template.CategoryId,
                CategoryName = category?.Name,
                Tags = tags,
                CoverUrl = template.CoverUrl,
                BoardSize = template.BoardSize,
                BeadCount = template.BeadCount,
                Difficulty = template.Difficulty,
                TotalColors = template.TotalColors,
                SourceType = template.SourceType,
                CreatorName = template.CreatorName,
                ViewCount = template.ViewCount,
                LikeCount = template.LikeCount,
                UseCount = template.UseCount,
                IsFeatured = template.IsFeatured,
                IsFavorited = false
            });
        }

        return result;
    }

    public async Task<TemplateDetailDto> GetTemplateDetailAsync(string templateId)
    {
        var template = await _templateRepo.GetByIdAsync(templateId);
        if (template == null || template.Status == "deleted") throw new BizException("模板不存在", ErrorCodes.NotFound);

        // 增加浏览次数
        template.ViewCount++;
        await _templateRepo.UpdateAsync(template);

        var tags = new List<string>();
        if (!string.IsNullOrWhiteSpace(template.Tags))
        {
            try { tags = JsonSerializer.Deserialize<List<string>>(template.Tags) ?? new(); }
            catch { }
        }

        var previewUrls = new List<string>();
        if (!string.IsNullOrWhiteSpace(template.PreviewUrls))
        {
            try { previewUrls = JsonSerializer.Deserialize<List<string>>(template.PreviewUrls) ?? new(); }
            catch { }
        }

        var category = await _categoryRepo.GetByIdAsync(template.CategoryId);

        return new TemplateDetailDto
        {
            Id = template.Id,
            Name = template.Name,
            CategoryId = template.CategoryId,
            CategoryName = category?.Name,
            Tags = tags,
            CoverUrl = template.CoverUrl,
            BoardSize = template.BoardSize,
            BeadCount = template.BeadCount,
            Difficulty = template.Difficulty,
            TotalColors = template.TotalColors,
            SourceType = template.SourceType,
            CreatorName = template.CreatorName,
            CreatorId = template.CreatorId,
            ViewCount = template.ViewCount,
            LikeCount = template.LikeCount,
            UseCount = template.UseCount,
            IsFeatured = template.IsFeatured,
            IsFavorited = false,
            PreviewUrls = previewUrls,
            CreateTime = template.CreateTime
        };
    }

    public async Task<PagedResult<TemplateCategoryDto>> GetCategoriesAsync()
    {
        var categories = await _categoryRepo.GetListAsync(c => true);

        var result = new PagedResult<TemplateCategoryDto>
        {
            Page = 1,
            Size = categories.Count,
            Total = categories.Count,
            List = new List<TemplateCategoryDto>()
        };

        foreach (var cat in categories.OrderBy(c => c.Sort))
        {
            var templateCount = await _templateRepo.CountAsync(t => t.CategoryId == cat.Id && t.Status == "active");
            result.List.Add(new TemplateCategoryDto
            {
                Id = cat.Id,
                Name = cat.Name,
                Icon = cat.Icon,
                Sort = cat.Sort,
                TemplateCount = templateCount
            });
        }

        return result;
    }

    public async Task<PagedResult<TemplateTagDto>> GetTagsAsync(string? type, PageRequest request)
    {
        var exp = Expressionable.Create<TemplateTag>().And(t => t.Status == 1);
        if (!string.IsNullOrWhiteSpace(type))
            exp.And(t => t.Type == type);

        var (list, total) = await _tagRepo.GetPagedAsync(
            exp.ToExpression(),
            request.Page,
            request.Size,
            t => t.UseCount,
            true);

        var result = new PagedResult<TemplateTagDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<TemplateTagDto>()
        };

        foreach (var tag in list)
        {
            result.List.Add(new TemplateTagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Category = tag.Category,
                Type = tag.Type,
                UseCount = tag.UseCount
            });
        }

        return result;
    }

    public async Task<bool> FavoriteAsync(string userId, string templateId)
    {
        var exists = await _favoriteRepo.AnyAsync(f => f.UserId == userId && f.TemplateId == templateId);
        if (exists) return true;

        await _favoriteRepo.InsertAsync(new TemplateFavorite
        {
            UserId = userId,
            TemplateId = templateId
        });

        // 更新模板点赞数
        var template = await _templateRepo.GetByIdAsync(templateId);
        if (template != null)
        {
            template.LikeCount++;
            await _templateRepo.UpdateAsync(template);
        }

        return true;
    }

    public async Task<bool> UnfavoriteAsync(string userId, string templateId)
    {
        var fav = await _favoriteRepo.FirstOrDefaultAsync(f => f.UserId == userId && f.TemplateId == templateId);
        if (fav == null) return true;

        await _favoriteRepo.DeleteAsync(fav.Id);

        var template = await _templateRepo.GetByIdAsync(templateId);
        if (template != null && template.LikeCount > 0)
        {
            template.LikeCount--;
            await _templateRepo.UpdateAsync(template);
        }

        return true;
    }

    public async Task<PagedResult<TemplateDto>> GetFavoritesAsync(string userId, PageRequest request)
    {
        var (favs, total) = await _favoriteRepo.GetPagedAsync(
            f => f.UserId == userId,
            request.Page,
            request.Size,
            f => f.CreateTime,
            true);

        var result = new PagedResult<TemplateDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<TemplateDto>()
        };

        foreach (var fav in favs)
        {
            var template = await _templateRepo.GetByIdAsync(fav.TemplateId);
            if (template == null || template.Status == "deleted") continue;

            var tags = new List<string>();
            if (!string.IsNullOrWhiteSpace(template.Tags))
            {
                try { tags = JsonSerializer.Deserialize<List<string>>(template.Tags) ?? new(); }
                catch { }
            }

            var category = await _categoryRepo.GetByIdAsync(template.CategoryId);

            result.List.Add(new TemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                CategoryId = template.CategoryId,
                CategoryName = category?.Name,
                Tags = tags,
                CoverUrl = template.CoverUrl,
                BoardSize = template.BoardSize,
                BeadCount = template.BeadCount,
                Difficulty = template.Difficulty,
                TotalColors = template.TotalColors,
                SourceType = template.SourceType,
                CreatorName = template.CreatorName,
                ViewCount = template.ViewCount,
                LikeCount = template.LikeCount,
                UseCount = template.UseCount,
                IsFeatured = template.IsFeatured,
                IsFavorited = true
            });
        }

        return result;
    }

    public async Task<string> UseTemplateAsync(string userId, string templateId)
    {
        var template = await _templateRepo.GetByIdAsync(templateId);
        if (template == null || template.Status == "deleted") throw new BizException("模板不存在", ErrorCodes.NotFound);

        // 增加使用次数
        template.UseCount++;
        await _templateRepo.UpdateAsync(template);

        // 创建基于模板的图纸
        var diagram = new Diagram
        {
            UserId = userId,
            Name = template.Name,
            Status = "draft",
            SourceImageUrl = template.CoverUrl,
            PreviewUrl = template.CoverUrl,
            BoardSize = template.BoardSize,
            BeadCount = template.BeadCount,
            Difficulty = template.Difficulty,
            Style = "pixel",
            TotalColors = template.TotalColors,
            TotalBeads = template.BeadCount,
            SourceType = "template",
            TemplateId = templateId
        };
        await _diagramRepo.InsertAsync(diagram);

        return diagram.Id;
    }
}