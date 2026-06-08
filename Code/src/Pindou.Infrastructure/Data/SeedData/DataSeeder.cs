using Pindou.Domain.Entities.Admin;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.Creation;
using Pindou.Domain.Entities.Member;
using Pindou.Domain.Entities.Messaging;
using Pindou.Domain.Entities.Operation;
using Pindou.Domain.Entities.System;
using Pindou.Domain.Entities.Template;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;

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
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Device> _deviceRepo;
    private readonly IRepository<MessageSetting> _messageSettingRepo;
    private readonly IRepository<Member> _memberRepo;
    private readonly IRepository<Token> _tokenRepo;
    private readonly IRepository<Template> _templateRepo;
    private readonly IRepository<TemplateTag> _templateTagRepo;
    private readonly IRepository<Topic> _topicRepo;
    private readonly IRepository<Post> _postRepo;
    private readonly IRepository<Comment> _commentRepo;
    private readonly IRepository<Follow> _followRepo;
    private readonly IRepository<Diagram> _diagramRepo;
    private readonly IRepository<DiagramTask> _diagramTaskRepo;
    private readonly IRepository<Banner> _bannerRepo;
    private readonly IRepository<OperationTopic> _operationTopicRepo;
    private readonly IRepository<BeadKit> _beadKitRepo;
    private readonly IRepository<SensitiveWord> _sensitiveWordRepo;
    private readonly IRepository<OperationLog> _operationLogRepo;

    public DataSeeder(
        IRepository<Role> roleRepo,
        IRepository<AdminUser> adminRepo,
        IRepository<TemplateCategory> categoryRepo,
        IRepository<MardColor> mardRepo,
        IRepository<SystemConfig> configRepo,
        IRepository<MemberProduct> productRepo,
        IRepository<User> userRepo,
        IRepository<Device> deviceRepo,
        IRepository<MessageSetting> messageSettingRepo,
        IRepository<Member> memberRepo,
        IRepository<Token> tokenRepo,
        IRepository<Template> templateRepo,
        IRepository<TemplateTag> templateTagRepo,
        IRepository<Topic> topicRepo,
        IRepository<Post> postRepo,
        IRepository<Comment> commentRepo,
        IRepository<Follow> followRepo,
        IRepository<Diagram> diagramRepo,
        IRepository<DiagramTask> diagramTaskRepo,
        IRepository<Banner> bannerRepo,
        IRepository<OperationTopic> operationTopicRepo,
        IRepository<BeadKit> beadKitRepo,
        IRepository<SensitiveWord> sensitiveWordRepo,
        IRepository<OperationLog> operationLogRepo)
    {
        _roleRepo = roleRepo;
        _adminRepo = adminRepo;
        _categoryRepo = categoryRepo;
        _mardRepo = mardRepo;
        _configRepo = configRepo;
        _productRepo = productRepo;
        _userRepo = userRepo;
        _deviceRepo = deviceRepo;
        _messageSettingRepo = messageSettingRepo;
        _memberRepo = memberRepo;
        _tokenRepo = tokenRepo;
        _templateRepo = templateRepo;
        _templateTagRepo = templateTagRepo;
        _topicRepo = topicRepo;
        _postRepo = postRepo;
        _commentRepo = commentRepo;
        _followRepo = followRepo;
        _diagramRepo = diagramRepo;
        _diagramTaskRepo = diagramTaskRepo;
        _bannerRepo = bannerRepo;
        _operationTopicRepo = operationTopicRepo;
        _beadKitRepo = beadKitRepo;
        _sensitiveWordRepo = sensitiveWordRepo;
        _operationLogRepo = operationLogRepo;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
        await SeedTemplateCategoriesAsync();
        await SeedMardColorsAsync();
        await SeedSystemConfigsAsync();
        await SeedMemberProductsAsync();
        await SeedBeadKitsAsync();
        await SeedSensitiveWordsAsync();
        await SeedTemplateTagsAsync();
        await SeedUsersAsync();
        await SeedMessageSettingsAsync();
        await SeedMembersAsync();
        await SeedTokensAsync();
        await SeedDevicesAsync();
        await SeedTopicsAsync();
        await SeedOperationTopicsAsync();
        await SeedFollowsAsync();
        await SeedTemplatesAsync();
        await SeedDiagramsAsync();
        await SeedDiagramTasksAsync();
        await SeedPostsAsync();
        await SeedCommentsAsync();
        await SeedBannersAsync();
        await SeedOperationLogsAsync();
    }

    #region 基础数据

    private async Task SeedRolesAsync()
    {
        if (await _roleRepo.AnyAsync(r => r.Id == 1)) return;
        var roles = new List<Role>
        {
            new() { Id = 1, Name = "超级管理员", Code = "super_admin", Description = "拥有系统全部权限", Permissions = "[\"*\"]" },
            new() { Id = 2, Name = "运营", Code = "operator", Description = "运营管理 / 数据统计 / 模板", Permissions = "[\"template\",\"operation\",\"stats\"]" },
            new() { Id = 3, Name = "审核", Code = "reviewer", Description = "内容审核", Permissions = "[\"post.approve\",\"comment.approve\",\"sensitive\",\"report\"]" },
            new() { Id = 4, Name = "客服", Code = "customer_service", Description = "用户管理查看", Permissions = "[\"user.approve\"]" }
        };
        await _roleRepo.InsertRangeAsync(roles);
    }

    private async Task SeedAdminUserAsync()
    {
        if (await _adminRepo.AnyAsync(a => a.Username == "admin")) return;

        var admins = new List<AdminUser>
        {
            new() { Username = "admin", Password = BCrypt.Net.BCrypt.HashPassword("admin123"), Nickname = "系统管理员", RoleId = 1, Status = 1 },
            // 来自前端 mock 数据的管理员
            new() { Username = "lin_ops", Password = BCrypt.Net.BCrypt.HashPassword("lin123"), Nickname = "林运营", RoleId = 2, Status = 1 },
            new() { Username = "li_audit", Password = BCrypt.Net.BCrypt.HashPassword("li123"), Nickname = "李审核", RoleId = 3, Status = 1 },
            new() { Username = "chen_cs", Password = BCrypt.Net.BCrypt.HashPassword("chen123"), Nickname = "陈客服", RoleId = 4, Status = 1 },
            new() { Username = "zhang_admin", Password = BCrypt.Net.BCrypt.HashPassword("zhang123"), Nickname = "张管理员", RoleId = 1, Status = 1 }
        };
        await _adminRepo.InsertRangeAsync(admins);
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
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M01", ColorName = "纯白", Rgb = "255,255,255", Category = "white", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M02", ColorName = "黑色", Rgb = "0,0,0", Category = "black", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M03", ColorName = "正红", Rgb = "237,28,36", Category = "red", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M04", ColorName = "橙色", Rgb = "242,101,34", Category = "orange", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M05", ColorName = "黄色", Rgb = "255,222,23", Category = "yellow", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M06", ColorName = "草绿", Rgb = "34,177,76", Category = "green", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M07", ColorName = "天蓝", Rgb = "0,162,232", Category = "blue", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M08", ColorName = "深蓝", Rgb = "63,72,204", Category = "blue", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M09", ColorName = "紫色", Rgb = "163,73,164", Category = "purple", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M10", ColorName = "粉色", Rgb = "255,174,201", Category = "special", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M11", ColorName = "灰色", Rgb = "128,128,128", Category = "gray", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "M12", ColorName = "浅灰", Rgb = "200,200,200", Category = "gray", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "H01", ColorName = "肤色", Rgb = "255,220,178", Category = "special", IsCommon = 1, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "H02", ColorName = "深咖", Rgb = "101,67,33", Category = "special", IsCommon = 0, Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), ColorNo = "H03", ColorName = "亮金", Rgb = "255,201,14", Category = "special", IsCommon = 0, Status = 1 }
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
            new() { Id = Guid.NewGuid().ToString(), ProductId = "monthly_vip", ProductName = "月度会员", Grade = "month", DurationDays = 30, Price = 19.90m, OriginalPrice = 29.90m, DailyGenerations = 10, Features = "[\"每日10次生成\",\"去广告\",\"专属色号\"]", Status = 1 },
            new() { ProductId = "quarterly_vip", ProductName = "季度会员", Grade = "quarter", DurationDays = 90, Price = 49.90m, OriginalPrice = 89.70m, DailyGenerations = 20, Features = "[\"每日20次生成\",\"去广告\",\"专属色号\",\"优先处理\"]", Status = 1 },
            new() { ProductId = "yearly_vip", ProductName = "年度会员", Grade = "year", DurationDays = 365, Price = 199.00m, OriginalPrice = 358.80m, DailyGenerations = 50, Features = "[\"每日50次生成\",\"去广告\",\"专属色号\",\"优先处理\",\"无限收藏\"]", Status = 1 },
            new() { ProductId = "lifetime_vip", ProductName = "终身会员", Grade = "lifetime", DurationDays = 36500, Price = 499.00m, OriginalPrice = 999.00m, DailyGenerations = -1, Features = "[\"无限生成\",\"全部权益\",\"终身有效\"]", Status = 1 }
        };
        await _productRepo.InsertRangeAsync(products);
    }

    private async Task SeedBeadKitsAsync()
    {
        if (await _beadKitRepo.AnyAsync(c => true)) return;
        var kits = new List<BeadKit>
        {
            new() { Id = Guid.NewGuid().ToString(), KitId = "KIT-48", KitName = "MARD 48色基础套装", Brand = "MARD", ColorCount = 48, BeadCount = 6000, Price = 39.90m, PurchaseUrl = "https://item.jd.com/10001.html", Status = 1 },
            new() { KitId = "KIT-72", KitName = "MARD 72色进阶套装", Brand = "MARD", ColorCount = 72, BeadCount = 9000, Price = 69.90m, PurchaseUrl = "https://item.jd.com/10002.html", Status = 1 },
            new() { KitId = "KIT-128", KitName = "MARD 128色全色套装", Brand = "MARD", ColorCount = 128, BeadCount = 16000, Price = 159.00m, PurchaseUrl = "https://item.jd.com/10003.html", Status = 1 }
        };
        await _beadKitRepo.InsertRangeAsync(kits);
    }

    private async Task SeedSensitiveWordsAsync()
    {
        if (await _sensitiveWordRepo.AnyAsync(c => true)) return;

        // 敏感词级别映射: 1-警告 2-替换 3-拦截
        var levelMap = new Dictionary<string, int>
        {
            { "severe", 3 },
            { "medium", 2 },
            { "minor", 1 }
        };
        var typeMap = new Dictionary<string, string>
        {
            { "political", "politics" },
            { "porn", "porn" },
            { "violence", "violence" },
            { "ads", "ad" },
            { "copyright", "other" },
            { "other", "other" }
        };

        var words = new List<SensitiveWord>
        {
            // 来自前端的敏感词数据
            new() { Id = Guid.NewGuid().ToString(), Word = "习近平", Level = 3, Type = "politics", ReplaceWord = "***", Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), Word = "性感荷官", Level = 3, Type = "porn", ReplaceWord = "***", Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), Word = "微信二维码", Level = 2, Type = "ad", ReplaceWord = "[联系方式]", Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), Word = "盗版", Level = 2, Type = "other", ReplaceWord = "未经授权", Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), Word = "咒骂词", Level = 1, Type = "other", ReplaceWord = "**", Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), Word = "赌博", Level = 3, Type = "other", ReplaceWord = "***", Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), Word = "暴力血腥", Level = 3, Type = "violence", ReplaceWord = "***", Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), Word = "拼多多砍一刀", Level = 2, Type = "ad", ReplaceWord = "[其他平台]", Status = 1 },
            // 原有基础敏感词
            new() { Id = Guid.NewGuid().ToString(), Word = "广告", Level = 1, Type = "ad", ReplaceWord = "[推广]", Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), Word = "代刷", Level = 3, Type = "ad", Status = 1 },
            new() { Id = Guid.NewGuid().ToString(), Word = "兼职", Level = 2, Type = "ad", ReplaceWord = "[工作]", Status = 1 }
        };
        await _sensitiveWordRepo.InsertRangeAsync(words);
    }

    private async Task SeedTemplateTagsAsync()
    {
        if (await _templateTagRepo.AnyAsync(c => true)) return;
        var tags = new List<TemplateTag>
        {
            new() { Id = Guid.NewGuid().ToString(), Name = "可爱", Category = "style", Type = "style", UseCount = 0, Status = 1 },
            new() { Name = "简约", Category = "style", Type = "style", UseCount = 0, Status = 1 },
            new() { Name = "复古", Category = "style", Type = "style", UseCount = 0, Status = 1 },
            new() { Name = "清新", Category = "style", Type = "style", UseCount = 0, Status = 1 },
            new() { Name = "圣诞", Category = "theme", Type = "theme", UseCount = 0, Status = 1 },
            new() { Name = "春节", Category = "theme", Type = "theme", UseCount = 0, Status = 1 },
            new() { Name = "情人节", Category = "theme", Type = "theme", UseCount = 0, Status = 1 },
            new() { Name = "生日", Category = "theme", Type = "theme", UseCount = 0, Status = 1 },
            new() { Name = "简单", Category = "difficulty", Type = "difficulty", UseCount = 0, Status = 1 },
            new() { Name = "中等", Category = "difficulty", Type = "difficulty", UseCount = 0, Status = 1 },
            new() { Name = "高难度", Category = "difficulty", Type = "difficulty", UseCount = 0, Status = 1 }
        };
        await _templateTagRepo.InsertRangeAsync(tags);
    }

    #endregion

    #region 用户与会员

    private async Task SeedUsersAsync()
    {
        if (await _userRepo.AnyAsync(u => true)) return;
        var users = new List<User>
        {
            new() { Nickname = "小明", Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=xiaoming", Gender = "male", City = "北京", Bio = "拼豆爱好者", Status = "active" },
            new() { Nickname = "小红", Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=xiaohong", Gender = "female", City = "上海", Bio = "一起来拼豆呀~", Status = "active" },
            new() { Nickname = "豆豆妈", Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=doudouma", Gender = "female", City = "广州", Bio = "亲子拼豆分享", Status = "active" },
            new() { Nickname = "PixelArt", Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=pixelart", Gender = "unknown", City = "深圳", Bio = "像素风创作", Status = "active" },
            new() { Nickname = "拼豆新手", Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=newbie", Gender = "male", City = "杭州", Bio = "刚入门,请多指教", Status = "active" }
        };
        await _userRepo.InsertRangeAsync(users);
    }

    private async Task SeedMessageSettingsAsync()
    {
        if (await _messageSettingRepo.AnyAsync(c => true)) return;
        var users = await _userRepo.GetListAsync();
        var settings = users.Select(u => new MessageSetting
        {
            UserId = u.Id
        }).ToList();
        if (settings.Count > 0) await _messageSettingRepo.InsertRangeAsync(settings);
    }

    private async Task SeedMembersAsync()
    {
        if (await _memberRepo.AnyAsync(m => true)) return;
        var users = await _userRepo.GetListAsync();
        if (users.Count < 2) return;
        var now = DateTime.Now;
        var members = new List<Member>
        {
            new() { UserId = users[0].Id, MemberType = "year", StartTime = now.AddDays(-30), ExpireTime = now.AddDays(335) },
            new() { UserId = users[1].Id, MemberType = "month", StartTime = now.AddDays(-5), ExpireTime = now.AddDays(25) }
        };
        await _memberRepo.InsertRangeAsync(members);

        // 同步更新用户表的是否会员标识
        foreach (var m in members)
        {
            var u = users.FirstOrDefault(x => x.Id == m.UserId);
            if (u != null)
            {
                u.IsMember = true;
                u.MemberExpireTime = m.ExpireTime;
                await _userRepo.UpdateAsync(u);
            }
        }
    }

    private async Task SeedTokensAsync()
    {
        if (await _tokenRepo.AnyAsync(t => true)) return;
        var users = await _userRepo.GetListAsync();
        if (users.Count == 0) return;
        var tokens = new List<Token>();
        foreach (var u in users)
        {
            tokens.Add(new Token
            {
                UserId = u.Id,
                AccessToken = $"seed-access-{u.Id}",
                RefreshToken = $"seed-refresh-{u.Id}",
                DeviceId = $"seed-device-{u.Id}",
                ExpiresAt = DateTime.Now.AddDays(30)
            });
        }
        await _tokenRepo.InsertRangeAsync(tokens);
    }

    private async Task SeedDevicesAsync()
    {
        if (await _deviceRepo.AnyAsync(d => true)) return;
        var users = await _userRepo.GetListAsync();
        if (users.Count == 0) return;
        var platforms = new[] { "ios", "android" };
        var devices = new List<Device>();
        var idx = 0;
        foreach (var u in users)
        {
            devices.Add(new Device
            {
                UserId = u.Id,
                DeviceId = $"seed-device-{u.Id}",
                Platform = platforms[idx % platforms.Length],
                PushToken = $"seed-push-token-{u.Id}",
                AppVersion = "1.0.0",
                LastActiveTime = DateTime.Now.AddMinutes(-idx * 10)
            });
            idx++;
        }
        await _deviceRepo.InsertRangeAsync(devices);
    }

    #endregion

    #region 社区

    private async Task SeedTopicsAsync()
    {
        if (await _topicRepo.AnyAsync(t => true)) return;
        var topics = new List<Topic>
        {
            new() { Id = Guid.NewGuid().ToString(), Name = "圣诞主题", Description = "圣诞相关拼豆作品", CoverUrl = "https://picsum.photos/seed/topic-christmas/400/400", PostCount = 0, IsHot = true },
            new() { Name = "动漫角色", Description = "动漫角色拼豆", CoverUrl = "https://picsum.photos/seed/topic-anime/400/400", PostCount = 0, IsHot = true },
            new() { Name = "可爱动物", Description = "可爱的小动物", CoverUrl = "https://picsum.photos/seed/topic-animal/400/400", PostCount = 0, IsHot = false },
            new() { Name = "像素游戏", Description = "复古像素风", CoverUrl = "https://picsum.photos/seed/topic-game/400/400", PostCount = 0, IsHot = true },
            new() { Name = "美食咖啡", Description = "咖啡饮品主题", CoverUrl = "https://picsum.photos/seed/topic-coffee/400/400", PostCount = 0, IsHot = false }
        };
        await _topicRepo.InsertRangeAsync(topics);
    }

    private async Task SeedOperationTopicsAsync()
    {
        if (await _operationTopicRepo.AnyAsync(t => true)) return;
        var now = DateTime.Now;
        var topics = new List<OperationTopic>
        {
            new() { Id = Guid.NewGuid().ToString(), TopicId = "christmas2024", Name = "🎄 圣诞创作大赛", Description = "上传你的圣诞主题拼豆赢取会员", CoverUrl = "https://picsum.photos/seed/op-christmas/800/400", IsOfficial = 1, Status = "active", PostCount = 0, ParticipantCount = 0 },
            new() { TopicId = "newyear2025", Name = "🎉 新年祝福拼豆", Description = "用拼豆送上你的新年祝福", CoverUrl = "https://picsum.photos/seed/op-newyear/800/400", IsOfficial = 1, Status = "active", PostCount = 0, ParticipantCount = 0 }
        };
        await _operationTopicRepo.InsertRangeAsync(topics);
    }

    private async Task SeedFollowsAsync()
    {
        if (await _followRepo.AnyAsync(f => true)) return;
        var users = await _userRepo.GetListAsync();
        if (users.Count < 3) return;
        var follows = new List<Follow>
        {
            new() { Id = Guid.NewGuid().ToString(), UserId = users[0].Id, FollowUserId = users[1].Id },
            new() { UserId = users[0].Id, FollowUserId = users[2].Id },
            new() { UserId = users[1].Id, FollowUserId = users[0].Id },
            new() { UserId = users[2].Id, FollowUserId = users[0].Id },
            new() { UserId = users[3].Id, FollowUserId = users[0].Id }
        };
        await _followRepo.InsertRangeAsync(follows);
    }

    private async Task SeedPostsAsync()
    {
        if (await _postRepo.AnyAsync(p => true)) return;
        var users = await _userRepo.GetListAsync();
        var topics = await _topicRepo.GetListAsync();
        if (users.Count == 0 || topics.Count == 0) return;

        var posts = new List<Post>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[0].Id,
                Type = "work",
                Title = "我的第一个圣诞树拼豆",
                Content = "第一次尝试29x29的圣诞树,配色参考了网上的教程,完成后超有成就感!",
                Media = "[\"https://picsum.photos/seed/post1/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[0].Id }),
                LikeCount = 12, CommentCount = 0, FavoriteCount = 5, ViewCount = 88,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[1].Id,
                Type = "work",
                Title = "柴犬拼豆分享",
                Content = "我家狗子同款,毛色用了6个色号,舌头用粉色点缀,可可爱爱~",
                Media = "[\"https://picsum.photos/seed/post2/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[2].Id }),
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":580,\"totalColors\":8}",
                LikeCount = 36, CommentCount = 0, FavoriteCount = 18, ViewCount = 220,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-3)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[2].Id,
                Type = "tutorial",
                Title = "【教程】如何选色让拼豆更协调",
                Content = "分享我选色的三个小技巧:1.主色不超过3个 2.邻近色搭配 3.点缀色提亮...",
                Media = "[\"https://picsum.photos/seed/post3/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[0].Id, topics[1].Id }),
                LikeCount = 128, CommentCount = 0, FavoriteCount = 80, ViewCount = 1500,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-7)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[3].Id,
                Type = "work",
                Title = "像素风小怪兽",
                Content = "今天完成的小怪兽,白色底板+高饱和度配色,放在桌面上超治愈!",
                Media = "[\"https://picsum.photos/seed/post4/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[3].Id }),
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":820,\"totalColors\":12}",
                LikeCount = 65, CommentCount = 0, FavoriteCount = 22, ViewCount = 410,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[4].Id,
                Type = "request",
                Title = "求一个皮卡丘的图纸~",
                Content = "想要29x29大小的皮卡丘,有成品图的姐妹求分享,感谢!",
                Media = "[]",
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddHours(-6)
            }
        };
        await _postRepo.InsertRangeAsync(posts);
    }

    private async Task SeedCommentsAsync()
    {
        if (await _commentRepo.AnyAsync(c => true)) return;
        var users = await _userRepo.GetListAsync();
        var posts = await _postRepo.GetListAsync();
        if (users.Count == 0 || posts.Count == 0) return;
        var comments = new List<Comment>
        {
            new() { Id = Guid.NewGuid().ToString(), PostId = posts[0].Id, UserId = users[1].Id, Content = "好可爱!请问用的是什么色号?", LikeCount = 3, Status = "active" },
            new() { PostId = posts[0].Id, UserId = users[2].Id, Content = "配色很好看,期待更多作品~", LikeCount = 5, Status = "active" },
            new() { PostId = posts[1].Id, UserId = users[0].Id, Content = "柴犬同款,哈哈好可爱", LikeCount = 2, Status = "active" },
            new() { PostId = posts[2].Id, UserId = users[0].Id, Content = "教程很实用,收藏了!", LikeCount = 8, Status = "active" },
            new() { PostId = posts[3].Id, UserId = users[1].Id, Content = "配色绝了,请问配色比例怎么选?", LikeCount = 4, Status = "active" }
        };
        await _commentRepo.InsertRangeAsync(comments);
    }

    #endregion

    #region 创作

    private async Task SeedDiagramsAsync()
    {
        if (await _diagramRepo.AnyAsync(d => true)) return;
        var users = await _userRepo.GetListAsync();
        if (users.Count == 0) return;
        var diagrams = new List<Diagram>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[0].Id,
                Name = "圣诞树图纸",
                Status = "completed",
                SourceImageUrl = "https://picsum.photos/seed/diagram1/600/600",
                PreviewUrl = "https://picsum.photos/seed/preview1/600/600",
                PreviewNoGridUrl = "https://picsum.photos/seed/preview1-nogrid/600/600",
                BoardSize = "29x29",
                BeadCount = 841,
                Difficulty = "easy",
                Style = "pixel",
                TotalColors = 6,
                TotalBeads = 600,
                Tags = "[\"圣诞\",\"节日\"]",
                Version = 1,
                SourceType = "create"
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[1].Id,
                Name = "柴犬图纸",
                Status = "completed",
                SourceImageUrl = "https://picsum.photos/seed/diagram2/600/600",
                PreviewUrl = "https://picsum.photos/seed/preview2/600/600",
                BoardSize = "29x29",
                BeadCount = 841,
                Difficulty = "medium",
                Style = "cartoon",
                TotalColors = 9,
                TotalBeads = 720,
                Tags = "[\"宠物\",\"柴犬\"]",
                Version = 1,
                SourceType = "create"
            }
        };
        await _diagramRepo.InsertRangeAsync(diagrams);
    }

    private async Task SeedDiagramTasksAsync()
    {
        if (await _diagramTaskRepo.AnyAsync(t => true)) return;
        var users = await _userRepo.GetListAsync();
        if (users.Count == 0) return;
        var tasks = new List<DiagramTask>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[2].Id,
                Status = "completed",
                Progress = 100,
                CurrentStage = "完成",
                SourceImageUrl = "https://picsum.photos/seed/task1/600/600",
                Params = "{\"boardSize\":\"29x29\",\"difficulty\":\"easy\",\"style\":\"pixel\"}",
                IsSync = true,
                CompleteTime = DateTime.Now.AddHours(-2)
            }
        };
        await _diagramTaskRepo.InsertRangeAsync(tasks);
    }

    #endregion

    #region 模板

    private async Task SeedTemplatesAsync()
    {
        if (await _templateRepo.AnyAsync(t => true)) return;
        var categories = await _categoryRepo.GetListAsync();
        if (categories.Count == 0) return;
        var templates = new List<Template>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "圣诞树-经典款",
                CategoryId = categories[0].Id,
                Tags = "[\"圣诞\",\"节日\",\"简单\"]",
                CoverUrl = "https://picsum.photos/seed/tpl1/600/600",
                PreviewUrls = "[\"https://picsum.photos/seed/tpl1-1/600/600\",\"https://picsum.photos/seed/tpl1-2/600/600\"]",
                BoardSize = "29x29",
                BeadCount = 841,
                Difficulty = "easy",
                TotalColors = 6,
                SourceType = "official",
                ViewCount = 320, LikeCount = 88, UseCount = 25,
                Status = "active", ReviewStatus = "approved", IsFeatured = true
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "HelloKitty",
                CategoryId = categories[1].Id,
                Tags = "[\"卡通\",\"可爱\"]",
                CoverUrl = "https://picsum.photos/seed/tpl2/600/600",
                PreviewUrls = "[\"https://picsum.photos/seed/tpl2-1/600/600\"]",
                BoardSize = "29x29",
                BeadCount = 841,
                Difficulty = "medium",
                TotalColors = 8,
                SourceType = "official",
                ViewCount = 1024, LikeCount = 256, UseCount = 80,
                Status = "active", ReviewStatus = "approved", IsFeatured = true
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "像素蘑菇",
                CategoryId = categories[5].Id,
                Tags = "[\"像素游戏\",\"复古\"]",
                CoverUrl = "https://picsum.photos/seed/tpl3/600/600",
                PreviewUrls = "[\"https://picsum.photos/seed/tpl3-1/600/600\"]",
                BoardSize = "29x29",
                BeadCount = 841,
                Difficulty = "easy",
                TotalColors = 5,
                SourceType = "official",
                ViewCount = 480, LikeCount = 130, UseCount = 35,
                Status = "active", ReviewStatus = "approved", IsFeatured = false
            }
        };
        await _templateRepo.InsertRangeAsync(templates);
    }

    #endregion

    #region 运营

    private async Task SeedBannersAsync()
    {
        if (await _bannerRepo.AnyAsync(b => true)) return;
        var now = DateTime.Now;
        var banners = new List<Banner>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Title = "圣诞活动开启",
                ImageUrl = "https://picsum.photos/seed/banner1/1200/400",
                LinkType = "url",
                LinkValue = "https://pindou.com/christmas",
                Position = "home_top",
                Sort = 1,
                Status = "active",
                StartTime = now.AddDays(-5),
                EndTime = now.AddDays(25)
            },
            new()
            {
                Title = "新用户专享-7天会员",
                ImageUrl = "https://picsum.photos/seed/banner2/1200/400",
                LinkType = "template",
                LinkValue = "welcome_template",
                Position = "home_top",
                Sort = 2,
                Status = "active",
                StartTime = now.AddDays(-10),
                EndTime = now.AddDays(60)
            }
        };
        await _bannerRepo.InsertRangeAsync(banners);
    }

    private async Task SeedOperationLogsAsync()
    {
        if (await _operationLogRepo.AnyAsync(o => true)) return;
        var logs = new List<OperationLog>
        {
            new()
            {
                Id = 1, // 由基类自增赋值即可,这里占位
                UserId = 1,
                Username = "admin",
                Nickname = "系统管理员",
                Operation = "初始化数据库",
                Content = "系统首次启动初始化",
                Method = "DataSeeder.SeedAsync",
                Ip = "127.0.0.1"
            }
        };
        // 由于 OperationLog 的 Id 是 BaseEntity (long),让数据库自增
        try
        {
            await _operationLogRepo.InsertRangeAsync(logs);
        }
        catch
        {
            // 忽略:种子日志非关键数据
        }
    }

    #endregion
}
