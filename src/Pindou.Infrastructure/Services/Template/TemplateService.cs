using Pindou.Application.Common;
using Pindou.Application.DTOs.Template;
using Pindou.Application.Interfaces.Template;
using Pindou.Domain.Entities.Template;
using Pindou.Infrastructure.Repositories;

namespace Pindou.Infrastructure.Services.Template;

public class TemplateService : ITemplateService
{
    private readonly IRepository<Template> _templateRepo;
    private readonly IRepository<TemplateCategory> _categoryRepo;
    private readonly IRepository<TemplateTag> _tagRepo;
    private readonly IRepository<TemplateFavorite> _favoriteRepo;
    public TemplateService(
        IRepository<Template> templateRepo,
        IRepository<TemplateCategory> categoryRepo,
        IRepository<TemplateTag> tagRepo,
        IRepository<TemplateFavorite> favoriteRepo)
    {
        _templateRepo = templateRepo;
        _categoryRepo = categoryRepo;
        _tagRepo = tagRepo;
        _favoriteRepo = favoriteRepo;
    }

    public Task<PagedResult<TemplateDto>> GetTemplatesAsync(TemplateQuery query) { throw new NotImplementedException(); }
    public Task<TemplateDetailDto> GetTemplateDetailAsync(string templateId) { throw new NotImplementedException(); }
    public Task<PagedResult<TemplateCategoryDto>> GetCategoriesAsync() { throw new NotImplementedException(); }
    public Task<PagedResult<TemplateTagDto>> GetTagsAsync(string? type, PageRequest request) { throw new NotImplementedException(); }
    public Task<bool> FavoriteAsync(string userId, string templateId) { throw new NotImplementedException(); }
    public Task<bool> UnfavoriteAsync(string userId, string templateId) { throw new NotImplementedException(); }
    public Task<PagedResult<TemplateDto>> GetFavoritesAsync(string userId, PageRequest request) { throw new NotImplementedException(); }
    public Task<string> UseTemplateAsync(string userId, string templateId) { throw new NotImplementedException(); }
}
