using Pindou.Domain.Entities.Admin;
using Pindou.Domain.Entities.Member;
using Pindou.Domain.Entities.System;
using Pindou.Domain.Entities.Template;
using Pindou.Infrastructure.Repositories;
using System.Text.Json;

namespace Pindou.Infrastructure.Data.SeedData;

/// <summary>
/// 初始数据种子
/// </summary>
public class DataSeeder
{
    private readonly IRepository<Role> _roleRepo;
    private readonly IRepository<AdminUser> _adminRepo;
    private readonly IRepository<TemplateCategory> _categoryRepo;
    private readonly IRepository<MardColor> _mardRepo;
    private readonly IRepository<SystemConfig> _configRepo;
    private readonly IRepository<MemberProduct> _productRepo;

    public DataSeeder(
        IRepository<Role> roleRepo,
        IRepository<AdminUser> adminRepo,
        IRepository<TemplateCategory> categoryRepo,
        IRepository<MardColor> mardRepo,
        IRepository<SystemConfig> configRepo,
        IRepository<MemberProduct> productRepo)
    {
        _roleRepo = roleRepo;
        _adminRepo = adminRepo;
        _categoryRepo = categoryRepo;
        _mardRepo = mardRepo;
        _configRepo = configRepo;
        _productRepo = productRepo;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
        await SeedTemplateCategoriesAsync();
        await SeedMardColorsAsync();
        await SeedSystemConfigsAsync();
        await SeedMemberProductsAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (await _roleRepo.AnyAsync(r => r.Id == 1)) return;
        var roles = new List<Role>
        {
            new() { Id = 1, Name = "超级管理员", Code = "super_admin", Description = "拥有全部权限", Permissions = "[\"*\"]" },
            new() { Id = 2, Name = "运营", Code = "operator", Description = "用户、模板、运营、统计", Permissions = "[\"user:view\",\"template:*\",\"operation:*\",\"stats:view\"]" },
            new() { Id = 3, Name = "审核", Code = "reviewer", Description = "内容审核、模板审核", Permissions = "[\"post:review\",\"comment:review\",\"template:review\",\"report:handle\"]" },
            new() { Id = 4, Name = "客服", Code = "customer_service", Description = "内容查看、举报处理", Permissions = "[\"post:view\",\"comment:view\",\"report:handle\"]" }
        };
        await _roleRepo.InsertRangeAsync(roles);
    }

    private async Task SeedAdminUserAsync()
    {
        if (await _adminRepo.AnyAsync(a => a.Username == "admin")) return;
        var admin = new AdminUser
        {
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Nickname = "系统管理员",
            RoleId = 1,
            Status = 1
        };
        await _adminRepo.InsertAsync(admin);
    }

    private async Task SeedTemplateCategoriesAsync()
    {
        if (await _categoryRepo.AnyAsync(c => true)) return;
        var cats = new List<TemplateCategory>
        {
            new() { Id = Guid.NewGuid().ToString(), Name = "节日", Sort = 1 },
            new() { Id = Guid.NewGuid().ToString(), Name = "卡通", Sort = 2 },
            new() { Id = Guid.NewGuid().ToString(), Name = "二次元", Sort = 3 },
            new() { Id = Guid.NewGuid().ToString(), Name = "宠物", Sort = 4 },
            new() { Id = Guid.NewGuid().ToString(), Name = "风景", Sort = 5 },
            new() { Id = Guid.NewGuid().ToString(), Name = "像素游戏", Sort = 6 },
            new() { Id = Guid.NewGuid().ToString(), Name = "国风", Sort = 7 },
            new() { Id = Guid.NewGuid().ToString(), Name = "表情包", Sort = 8 },
            new() { Id = Guid.NewGuid().ToString(), Name = "文字", Sort = 9 },
            new() { Id = Guid.NewGuid().ToString(), Name = "其他", Sort = 10 }
        };
        await _categoryRepo.InsertRangeAsync(cats);
    }

    private async Task SeedMardColorsAsync()
    {
        if (await _mardRepo.AnyAsync(c => true)) return;
        var colors = new List<MardColor>
        {
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M01", ColorName = "纯白", Rgb = "255,255,255", Category = "white", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M02", ColorName = "黑色", Rgb = "0,0,0", Category = "black", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M03", ColorName = "正红", Rgb = "237,28,36", Category = "red", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M04", ColorName = "橙色", Rgb = "242,101,34", Category = "orange", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M05", ColorName = "黄色", Rgb = "255,222,23", Category = "yellow", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M06", ColorName = "草绿", Rgb = "34,177,76", Category = "green", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M07", ColorName = "天蓝", Rgb = "0,162,232", Category = "blue", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M08", ColorName = "深蓝", Rgb = "63,72,204", Category = "blue", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M09", ColorName = "紫色", Rgb = "163,73,164", Category = "purple", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M10", ColorName = "粉色", Rgb = "255,174,201", Category = "special", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M11", ColorName = "灰色", Rgb = "128,128,128", Category = "gray", IsCommon = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M12", ColorName = "浅灰", Rgb = "200,200,200", Category = "gray", IsCommon = 1 }
        };
        await _mardRepo.InsertRangeAsync(colors);
    }

    private async Task SeedSystemConfigsAsync()
    {
        if (await _configRepo.AnyAsync(c => true)) return;
        var configs = new List<SystemConfig>
        {
            new() { Id = Guid.NewGuid().ToString(), ConfigKey = "app_name", ConfigValue = "拼豆", ConfigType = "string", Description = "应用名称" },
            new() { ConfigKey = "user_agreement", ConfigValue = "https://pindou.com/agreement", ConfigType = "string" },
            new() { ConfigKey = "privacy_policy", ConfigValue = "https://pindou.com/privacy", ConfigType = "string" },
            new() { ConfigKey = "default_bead_count", ConfigValue = "841", ConfigType = "number", Description = "默认颗粒数(29x29)" },
            new() { ConfigKey = "default_difficulty", ConfigValue = "easy", ConfigType = "string" },
            new() { ConfigKey = "default_style", ConfigValue = "pixel", ConfigType = "string" },
            new() { ConfigKey = "free_daily_generations", ConfigValue = "3", ConfigType = "number", Description = "免费用户每日生成次数" },
            new() { ConfigKey = "generation_timeout", ConfigValue = "60", ConfigType = "number", Description = "生成超时(秒)" },
            new() { ConfigKey = "max_image_size", ConfigValue = "10485760", ConfigType = "number", Description = "最大图片字节数(10MB)" },
            new() { ConfigKey = "comment_enabled", ConfigValue = "true", ConfigType = "boolean" },
            new() { ConfigKey = "sensitive_filter", ConfigValue = "true", ConfigType = "boolean" }
        };
        await _configRepo.InsertRangeAsync(configs);
    }

    private async Task SeedMemberProductsAsync()
    {
        if (await _productRepo.AnyAsync(p => true)) return;
        var products = new List<MemberProduct>
        {
            new() { Id = Guid.NewGuid().ToString(), ProductId = "monthly_vip", ProductName = "月度会员", Grade = "month", DurationDays = 30, Price = 19.90m, OriginalPrice = 29.90m, DailyGenerations = 10, Features = "[\"每日10次生成\",\"去广告\",\"专属色号\"]" },
            new() { ProductId = "quarterly_vip", ProductName = "季度会员", Grade = "quarter", DurationDays = 90, Price = 49.90m, OriginalPrice = 89.70m, DailyGenerations = 20, Features = "[\"每日20次生成\",\"去广告\",\"专属色号\",\"优先处理\"]" },
            new() { ProductId = "yearly_vip", ProductName = "年度会员", Grade = "year", DurationDays = 365, Price = 199.00m, OriginalPrice = 358.80m, DailyGenerations = 50, Features = "[\"每日50次生成\",\"去广告\",\"专属色号\",\"优先处理\",\"无限收藏\"]" },
            new() { ProductId = "lifetime_vip", ProductName = "终身会员", Grade = "lifetime", DurationDays = 36500, Price = 499.00m, OriginalPrice = 999.00m, DailyGenerations = -1, Features = "[\"无限生成\",\"全部权益\",\"终身有效\"]" }
        };
        await _productRepo.InsertRangeAsync(products);
    }
}
