using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Template;
using Pindou.Domain.Entities.Template;
using Pindou.Domain.Entities.Creation;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Template;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class TemplateServiceTests
{
    private readonly Mock<IRepository<Template>> _templateRepoMock;
    private readonly Mock<IRepository<TemplateCategory>> _categoryRepoMock;
    private readonly Mock<IRepository<TemplateTag>> _tagRepoMock;
    private readonly Mock<IRepository<TemplateFavorite>> _favoriteRepoMock;
    private readonly Mock<IRepository<Diagram>> _diagramRepoMock;
    private readonly TemplateService _templateService;

    public TemplateServiceTests()
    {
        _templateRepoMock = new Mock<IRepository<Template>>();
        _categoryRepoMock = new Mock<IRepository<TemplateCategory>>();
        _tagRepoMock = new Mock<IRepository<TemplateTag>>();
        _favoriteRepoMock = new Mock<IRepository<TemplateFavorite>>();
        _diagramRepoMock = new Mock<IRepository<Diagram>>();
        _templateService = new TemplateService(
            _templateRepoMock.Object, _categoryRepoMock.Object, _tagRepoMock.Object,
            _favoriteRepoMock.Object, _diagramRepoMock.Object);
    }

    #region GetTemplatesAsync Tests

    [Fact]
    public async Task GetTemplatesAsync_ShouldReturnPagedResult()
    {
        var templates = new List<Template>
        {
            new Template
            {
                Id = "t1", Name = "模板1", CategoryId = "c1", Status = "active",
                ReviewStatus = "approved", BoardSize = "29x29", BeadCount = 100,
                Difficulty = "easy", TotalColors = 5, CoverUrl = "cover.png",
                SourceType = "official", ViewCount = 10, LikeCount = 5, UseCount = 3
            }
        };
        var category = new TemplateCategory { Id = "c1", Name = "分类1" };

        _templateRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Template, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Template, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((templates, 1));
        _categoryRepoMock.Setup(r => r.GetByIdAsync("c1")).ReturnsAsync(category);

        var query = new TemplateQuery { Page = 1, Size = 10 };
        var result = await _templateService.GetTemplatesAsync(query);

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
        Assert.Equal("分类1", result.List[0].CategoryName);
    }

    #endregion

    #region GetTemplateDetailAsync Tests

    [Fact]
    public async Task GetTemplateDetailAsync_ShouldReturnDetail()
    {
        var template = new Template
        {
            Id = "t1", Name = "模板1", CategoryId = "c1", Status = "active",
            BoardSize = "29x29", BeadCount = 100, Difficulty = "easy",
            TotalColors = 5, CoverUrl = "cover.png", SourceType = "official",
            ViewCount = 10, LikeCount = 5, UseCount = 3, PreviewUrls = "[\"p1.png\"]"
        };
        var category = new TemplateCategory { Id = "c1", Name = "分类1" };

        _templateRepoMock.Setup(r => r.GetByIdAsync("t1")).ReturnsAsync(template);
        _templateRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Template>())).ReturnsAsync(true);
        _categoryRepoMock.Setup(r => r.GetByIdAsync("c1")).ReturnsAsync(category);

        var result = await _templateService.GetTemplateDetailAsync("t1");

        Assert.NotNull(result);
        Assert.Equal("t1", result.Id);
        Assert.Single(result.PreviewUrls);
    }

    [Fact]
    public async Task GetTemplateDetailAsync_ShouldThrow_WhenNotFound()
    {
        _templateRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((Template?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _templateService.GetTemplateDetailAsync("nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region GetCategoriesAsync Tests

    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnCategories()
    {
        var categories = new List<TemplateCategory>
        {
            new TemplateCategory { Id = "c1", Name = "分类1", Sort = 1 }
        };
        _categoryRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<TemplateCategory, bool>>>())).ReturnsAsync(categories);
        _templateRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Template, bool>>>())).ReturnsAsync(5);

        var result = await _templateService.GetCategoriesAsync();

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
        Assert.Equal(5, result.List[0].TemplateCount);
    }

    #endregion

    #region GetTagsAsync Tests

    [Fact]
    public async Task GetTagsAsync_ShouldReturnTags()
    {
        var tags = new List<TemplateTag>
        {
            new TemplateTag { Id = "tag1", Name = "动物", Type = "theme", UseCount = 10 }
        };
        _tagRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<TemplateTag, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<TemplateTag, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((tags, 1));

        var result = await _templateService.GetTagsAsync("theme", new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
    }

    #endregion

    #region FavoriteAsync Tests

    [Fact]
    public async Task FavoriteAsync_ShouldFavorite()
    {
        _favoriteRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TemplateFavorite, bool>>>())).ReturnsAsync(false);
        _favoriteRepoMock.Setup(r => r.InsertAsync(It.IsAny<TemplateFavorite>())).ReturnsAsync("f1");
        _templateRepoMock.Setup(r => r.GetByIdAsync("t1"))
            .ReturnsAsync(new Template { Id = "t1", LikeCount = 0, Status = "active" });
        _templateRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Template>())).ReturnsAsync(true);

        var result = await _templateService.FavoriteAsync("u1", "t1");

        Assert.True(result);
    }

    [Fact]
    public async Task FavoriteAsync_ShouldReturnTrue_WhenAlreadyFavorited()
    {
        _favoriteRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TemplateFavorite, bool>>>())).ReturnsAsync(true);

        var result = await _templateService.FavoriteAsync("u1", "t1");

        Assert.True(result);
    }

    #endregion

    #region UnfavoriteAsync Tests

    [Fact]
    public async Task UnfavoriteAsync_ShouldUnfavorite()
    {
        _favoriteRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<TemplateFavorite, bool>>>()))
            .ReturnsAsync(new TemplateFavorite { Id = "f1", UserId = "u1", TemplateId = "t1" });
        _favoriteRepoMock.Setup(r => r.DeleteAsync(It.IsAny<object>())).ReturnsAsync(true);
        _templateRepoMock.Setup(r => r.GetByIdAsync("t1"))
            .ReturnsAsync(new Template { Id = "t1", LikeCount = 1, Status = "active" });
        _templateRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Template>())).ReturnsAsync(true);

        var result = await _templateService.UnfavoriteAsync("u1", "t1");

        Assert.True(result);
    }

    #endregion

    #region GetFavoritesAsync Tests

    [Fact]
    public async Task GetFavoritesAsync_ShouldReturnFavorites()
    {
        var favs = new List<TemplateFavorite>
        {
            new TemplateFavorite { Id = "f1", UserId = "u1", TemplateId = "t1", CreateTime = DateTime.Now }
        };
        var template = new Template
        {
            Id = "t1", Name = "模板1", CategoryId = "c1", Status = "active",
            BoardSize = "29x29", BeadCount = 100, Difficulty = "easy",
            TotalColors = 5, CoverUrl = "cover.png", SourceType = "official",
            ViewCount = 10, LikeCount = 5, UseCount = 3
        };
        var category = new TemplateCategory { Id = "c1", Name = "分类1" };

        _favoriteRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<TemplateFavorite, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<TemplateFavorite, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((favs, 1));
        _templateRepoMock.Setup(r => r.GetByIdAsync("t1")).ReturnsAsync(template);
        _categoryRepoMock.Setup(r => r.GetByIdAsync("c1")).ReturnsAsync(category);

        var result = await _templateService.GetFavoritesAsync("u1", new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
        Assert.True(result.List[0].IsFavorited);
    }

    #endregion

    #region UseTemplateAsync Tests

    [Fact]
    public async Task UseTemplateAsync_ShouldCreateDiagram()
    {
        var template = new Template
        {
            Id = "t1", Name = "模板1", Status = "active", BoardSize = "29x29",
            BeadCount = 100, Difficulty = "easy", TotalColors = 5, CoverUrl = "cover.png",
            UseCount = 3
        };
        _templateRepoMock.Setup(r => r.GetByIdAsync("t1")).ReturnsAsync(template);
        _templateRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Template>())).ReturnsAsync(true);
        _diagramRepoMock.Setup(r => r.InsertAsync(It.IsAny<Diagram>()))
            .Callback<Diagram>(d => d.Id = "d1")
            .ReturnsAsync("d1");

        var result = await _templateService.UseTemplateAsync("u1", "t1");

        Assert.NotNull(result);
        Assert.Equal("d1", result);
    }

    [Fact]
    public async Task UseTemplateAsync_ShouldThrow_WhenTemplateNotFound()
    {
        _templateRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((Template?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _templateService.UseTemplateAsync("u1", "nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion
}