using Pindou.Application.Common;
using Pindou.Application.DTOs.Template;

namespace Pindou.Application.Interfaces.Template;

public interface ITemplateService
{
    Task<PagedResult<TemplateDto>> GetTemplatesAsync(TemplateQuery query);
    Task<TemplateDetailDto> GetTemplateDetailAsync(string templateId);
    Task<PagedResult<TemplateCategoryDto>> GetCategoriesAsync();
    Task<string> CreateCategoryAsync(string name, string? icon, int sort);
    Task<bool> UpdateCategoryAsync(string id, string name, string? icon, int sort);
    Task<bool> DeleteCategoryAsync(string id);
    Task<PagedResult<TemplateTagDto>> GetTagsAsync(string? type, PageRequest request);
    Task<bool> FavoriteAsync(string userId, string templateId);
    Task<bool> UnfavoriteAsync(string userId, string templateId);
    Task<PagedResult<TemplateDto>> GetFavoritesAsync(string userId, PageRequest request);
    Task<string> UseTemplateAsync(string userId, string templateId);
}
