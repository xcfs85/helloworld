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
    private readonly IRepository<Report> _reportRepo;

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
        IRepository<OperationLog> operationLogRepo,
        IRepository<Report> reportRepo)
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
        _reportRepo = reportRepo;
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
        await SeedReportsAsync();
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
            new() { ConfigKey = "sensitive_filter", ConfigValue = "true", ConfigType = "boolean" },

            // 推送配置 - JPush
            new() { ConfigKey = "push_jpush_appkey", ConfigValue = "", ConfigType = "string", Description = "极光推送AppKey" },
            new() { ConfigKey = "push_jpush_master_secret", ConfigValue = "", ConfigType = "string", Description = "极光推送MasterSecret" },

            // 推送配置 - 短信
            new() { ConfigKey = "push_sms_provider", ConfigValue = "aliyun", ConfigType = "string", Description = "短信服务商:aliyun/tencent" },
            new() { ConfigKey = "push_sms_access_key", ConfigValue = "", ConfigType = "string", Description = "短信AccessKey" },
            new() { ConfigKey = "push_sms_access_secret", ConfigValue = "", ConfigType = "string", Description = "短信AccessSecret" },
            new() { ConfigKey = "push_sms_sign_name", ConfigValue = "", ConfigType = "string", Description = "短信签名" },
            new() { ConfigKey = "push_sms_template_code", ConfigValue = "", ConfigType = "string", Description = "短信模板编号" },

            // 推送配置 - 邮件
            new() { ConfigKey = "push_email_smtp_host", ConfigValue = "", ConfigType = "string", Description = "SMTP服务器" },
            new() { ConfigKey = "push_email_smtp_port", ConfigValue = "465", ConfigType = "number", Description = "SMTP端口" },
            new() { ConfigKey = "push_email_username", ConfigValue = "", ConfigType = "string", Description = "SMTP用户名" },
            new() { ConfigKey = "push_email_password", ConfigValue = "", ConfigType = "string", Description = "SMTP密码" },
            new() { ConfigKey = "push_email_from", ConfigValue = "", ConfigType = "string", Description = "发件人地址" },
            new() { ConfigKey = "push_email_ssl", ConfigValue = "true", ConfigType = "boolean", Description = "启用SSL" }
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
            new() { Id = Guid.NewGuid().ToString(), TopicId = "christmas2024", Name = "🎄 圣诞创作大赛", Description = "上传你的圣诞主题拼豆赢取会员", CoverUrl = "https://picsum.photos/seed/op-christmas/800/400", IsOfficial = 1, Status = "active", PostCount = 156, ParticipantCount = 89 },
            new() { Id = Guid.NewGuid().ToString(), TopicId = "newyear2025", Name = "🎉 新年祝福拼豆", Description = "用拼豆送上你的新年祝福", CoverUrl = "https://picsum.photos/seed/op-newyear/800/400", IsOfficial = 1, Status = "active", PostCount = 234, ParticipantCount = 167 },
            new() { Id = Guid.NewGuid().ToString(), TopicId = "anime2025", Name = "🎭 动漫角色大赏", Description = "你最爱的动漫角色拼豆作品", CoverUrl = "https://picsum.photos/seed/op-anime/800/400", IsOfficial = 1, Status = "active", PostCount = 312, ParticipantCount = 205 },
            new() { Id = Guid.NewGuid().ToString(), TopicId = "pet2025", Name = "🐾 萌宠拼豆秀", Description = "用拼豆记录你家毛孩子的可爱瞬间", CoverUrl = "https://picsum.photos/seed/op-pet/800/400", IsOfficial = 1, Status = "active", PostCount = 89, ParticipantCount = 56 },
            new() { Id = Guid.NewGuid().ToString(), TopicId = "pixel2024", Name = "👾 像素游戏回忆", Description = "复古像素风拼豆创作", CoverUrl = "https://picsum.photos/seed/op-pixel/800/400", IsOfficial = 1, Status = "closed", PostCount = 456, ParticipantCount = 278 },
            new() { Id = Guid.NewGuid().ToString(), TopicId = "food2024", Name = "🍰 美食咖啡时光", Description = "咖啡甜点拼豆创作", CoverUrl = "https://picsum.photos/seed/op-food/800/400", IsOfficial = 0, Status = "active", PostCount = 67, ParticipantCount = 43 },
            new() { Id = Guid.NewGuid().ToString(), TopicId = "diy2024", Name = "🎨 手工DIY分享", Description = "拼豆手工制作过程与心得", CoverUrl = "https://picsum.photos/seed/op-diy/800/400", IsOfficial = 0, Status = "active", PostCount = 128, ParticipantCount = 95 },
            new() { Id = Guid.NewGuid().ToString(), TopicId = "spring2024", Name = "🌸 春日花语", Description = "春天的花卉拼豆创作", CoverUrl = "https://picsum.photos/seed/op-spring/800/400", IsOfficial = 0, Status = "closed", PostCount = 201, ParticipantCount = 134 }
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
        var users = await _userRepo.GetListAsync();
        var topics = await _topicRepo.GetListAsync();
        if (users.Count == 0 || topics.Count == 0) return;

        var existingTitles = (await _postRepo.GetListAsync()).Select(p => p.Title).ToHashSet();

        var posts = new List<Post>
        {
            // ========== work (作品) ==========
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[0].Id,
                Type = "work",
                Title = "我的第一个圣诞树拼豆",
                Content = "第一次尝试29x29的圣诞树,配色参考了网上的教程,完成后超有成就感!用了红绿白三种主色,树顶的星星用金色点缀,放在书桌上很有节日氛围。",
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
                Content = "我家狗子同款,毛色用了6个色号,舌头用粉色点缀,可可爱爱~底板选的29x29白色,整体效果很温馨。",
                Media = "[\"https://picsum.photos/seed/post2/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[2].Id }),
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":580,\"totalColors\":8}",
                LikeCount = 36, CommentCount = 0, FavoriteCount = 18, ViewCount = 220,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-3)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[3].Id,
                Type = "work",
                Title = "像素风小怪兽",
                Content = "今天完成的小怪兽,白色底板+高饱和度配色,放在桌面上超治愈!眼睛部分用了黑色和白色对比,很有神。",
                Media = "[\"https://picsum.photos/seed/post4/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[3].Id }),
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":820,\"totalColors\":12}",
                LikeCount = 65, CommentCount = 0, FavoriteCount = 22, ViewCount = 410,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[2].Id,
                Type = "work",
                Title = "二次元老婆 - 雏田",
                Content = "用拼豆还原了我最喜欢的火影忍者角色雏田,29x29的底板,皮肤用了肤色系,眼睛用蓝色,头发用淡紫色,完成度很高!",
                Media = "[\"https://picsum.photos/seed/post-work-1/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[1].Id }),
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":700,\"totalColors\":15}",
                LikeCount = 188, CommentCount = 0, FavoriteCount = 92, ViewCount = 2560,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-2)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[4].Id,
                Type = "work",
                Title = "星巴克咖啡拉花图案",
                Content = "跟风做了一个星巴克的Logo拼豆,用了咖啡色的色号,配上白色底板,很有质感。适合放在办公桌上。",
                Media = "[\"https://picsum.photos/seed/post-work-2/600/600\"]",
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":450,\"totalColors\":5}",
                LikeCount = 45, CommentCount = 0, FavoriteCount = 15, ViewCount = 320,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-4)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[0].Id,
                Type = "work",
                Title = "宠物柯基小短腿",
                Content = "柯基的屁屁真的太可爱了!用拼豆做了一个侧躺的柯基,黄色的毛色用了3个色号渐变,尾巴短短的超级萌。",
                Media = "[\"https://picsum.photos/seed/post-work-3/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[2].Id }),
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":620,\"totalColors\":7}",
                LikeCount = 98, CommentCount = 0, FavoriteCount = 45, ViewCount = 890,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-6)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[1].Id,
                Type = "work",
                Title = "像素版超级马里奥",
                Content = "经典的红帽子水管工来啦!全像素风格还原,用了红蓝黄黑四种经典色,唤起童年回忆~",
                Media = "[\"https://picsum.photos/seed/post-work-4/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[3].Id }),
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":580,\"totalColors\":4}",
                LikeCount = 156, CommentCount = 0, FavoriteCount = 78, ViewCount = 1800,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddHours(-12)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[3].Id,
                Type = "work",
                Title = "国风仙鹤图案",
                Content = "尝试了国风主题,仙鹤用了白色和红色渐变,背景用了淡蓝色,整体很有传统美感。",
                Media = "[\"https://picsum.photos/seed/post-work-5/600/600\"]",
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":750,\"totalColors\":8}",
                LikeCount = 72, CommentCount = 0, FavoriteCount = 28, ViewCount = 560,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddHours(-3)
            },

            // ========== request (求图) ==========
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[4].Id,
                Type = "request",
                Title = "求一个皮卡丘的图纸~",
                Content = "想要29x29大小的皮卡丘,有成品图的姐妹求分享,感谢!最好是颜色鲜艳一点的",
                Media = "[]",
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddHours(-6)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[2].Id,
                Type = "request",
                Title = "求动漫头像拼豆图纸",
                Content = "想要一个动漫风格的女生头像,29x29或37x37都可以,最好有详细的色号标注,谢谢各位大神!",
                Media = "[]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[1].Id }),
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[0].Id,
                Type = "request",
                Title = "求简单的新年祝福图案",
                Content = "过年想做一些拼豆送人,想要简单易上手的,29x29以内,有财神、福字之类的最好啦",
                Media = "[]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[0].Id }),
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-2)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[4].Id,
                Type = "request",
                Title = "求猫咪图案图纸",
                Content = "想给女儿做一个猫咪拼豆,求简单可爱的猫咪图案,颜色不要太多,10色以内为宜",
                Media = "[]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[2].Id }),
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddHours(-18)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[1].Id,
                Type = "request",
                Title = "求情侣头像拼豆图纸",
                Content = "和男朋友想做一个情侣款的拼豆,有没有简单的情侣头像推荐呀,不要太大,方便放在桌上",
                Media = "[]",
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddHours(-8)
            },

            // ========== tutorial (教程) ==========
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[2].Id,
                Type = "tutorial",
                Title = "【教程】如何选色让拼豆更协调",
                Content = "分享我选色的三个小技巧:\n\n1.主色不超过3个\n2.邻近色搭配\n3.点缀色提亮\n\n还有更多配色心得欢迎评论区交流~",
                Media = "[\"https://picsum.photos/seed/post3/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[0].Id, topics[1].Id }),
                LikeCount = 128, CommentCount = 0, FavoriteCount = 80, ViewCount = 15000,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-7)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[0].Id,
                Type = "tutorial",
                Title = "【教程】新手入门指南 - 底板选择",
                Content = "很多新手问我底板怎么选,今天详细介绍一下:\n\n1. 29x29 适合简单图案,新手推荐\n2. 37x37 中等难度,细节更多\n3. 50x50 适合复杂图案\n\n新手建议从29开始,循序渐进!",
                Media = "[\"https://picsum.photos/seed/post-tutorial-1/600/600\"]",
                LikeCount = 256, CommentCount = 0, FavoriteCount = 168, ViewCount = 28000,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-10)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[3].Id,
                Type = "tutorial",
                Title = "【教程】如何快速对位拼豆图纸",
                Content = "分享一个提高效率的小技巧:使用透明定位板!\n\n1. 先把图纸铺平\n2. 透明板放在图纸上\n3. 按颜色分区对位\n\n这样可以省去很多对齐的时间,亲测效率提升50%!",
                Media = "[\"https://picsum.photos/seed/post-tutorial-2/600/600\"]",
                LikeCount = 89, CommentCount = 0, FavoriteCount = 45, ViewCount = 9200,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[1].Id,
                Type = "tutorial",
                Title = "【教程】MARD色号入门指南",
                Content = "很多小伙伴不知道色号怎么选,整理了一份常用色号推荐:\n\n基础色: M01白 M02黑 M03红 M05黄 M06绿\n进阶色: M04橙 M07天蓝 M08深蓝 M09紫 M10粉\n肤色推荐: H01肤色\n\n建议新手先买基础48色套装~",
                Media = "[\"https://picsum.photos/seed/post-tutorial-3/600/600\"]",
                LikeCount = 198, CommentCount = 0, FavoriteCount = 112, ViewCount = 15600,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-3)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[4].Id,
                Type = "tutorial",
                Title = "【教程】如何保存完成的拼豆作品",
                Content = "拼豆完成后怎么保存?分享我的方法:\n\n1. 用塑封机简单塑封\n2. 放到相框里做装饰\n3. 用热熔胶固定(要小心烫伤)\n\n推荐第二种,既美观又能长期保存!",
                Media = "[\"https://picsum.photos/seed/post-tutorial-4/600/600\"]",
                LikeCount = 76, CommentCount = 0, FavoriteCount = 38, ViewCount = 6800,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-1)
            },

            // ========== discussion (讨论) ==========
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[0].Id,
                Type = "discussion",
                Title = "大家一般在哪买拼豆材料?",
                Content = "入坑半年了,试过好几个渠道:\n- 淘宝: 品类全但质量参差不齐\n- 京东: 物流快但价格偏贵\n- 拼多多: 便宜但色号经常不全\n\n大家有什么推荐吗?或者有什么避坑指南?",
                Media = "[]",
                LikeCount = 45, CommentCount = 0, FavoriteCount = 8, ViewCount = 1200,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-2)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[1].Id,
                Type = "discussion",
                Title = "拼豆到底是拼豆还是拼拼豆?",
                Content = "今天和朋友争论起来了,我说是拼豆(chuan dou),朋友说是拼拼豆(pin pin dou),快来评评理!",
                Media = "[]",
                LikeCount = 89, CommentCount = 0, FavoriteCount = 12, ViewCount = 2800,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-4)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[2].Id,
                Type = "discussion",
                Title = "你们觉得29x29够用吗?",
                Content = "我入坑的时候只买了29x29的底板,现在想做更复杂的图案发现不够用了。你们都用什么尺寸的?建议新手直接买多大?",
                Media = "[]",
                LikeCount = 56, CommentCount = 0, FavoriteCount = 10, ViewCount = 1560,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[3].Id,
                Type = "discussion",
                Title = "拼豆算不算一种艺术创作?",
                Content = "最近在思考这个问题。拼豆需要设计图纸、选色搭配、细节处理,感觉和画画、十字绣很像。你们觉得拼豆算是艺术吗?",
                Media = "[]",
                LikeCount = 124, CommentCount = 0, FavoriteCount = 25, ViewCount = 3200,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddHours(-20)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[4].Id,
                Type = "discussion",
                Title = "送给朋友什么拼豆礼物比较好?",
                Content = "朋友生日快到了,想亲手做一个拼豆送给她。她喜欢可达鸭和草莓,有没有什么好的创意推荐?预算100以内~",
                Media = "[]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[1].Id, topics[2].Id }),
                LikeCount = 67, CommentCount = 0, FavoriteCount = 15, ViewCount = 1890,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddHours(-10)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[0].Id,
                Type = "discussion",
                Title = "拼豆会褪色吗?怎么保养?",
                Content = "做了几个拼豆作品放在窗边,最近发现颜色好像变淡了...想问下大家是怎么保养的?要避免阳光直射吗?",
                Media = "[]",
                LikeCount = 38, CommentCount = 0, FavoriteCount = 6, ViewCount = 980,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddDays(-3)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[1].Id,
                Type = "discussion",
                Title = "你们都用哪个AI生成拼豆图纸?",
                Content = "试了几个AI工具,感觉效果参差不齐。有的是颜色太多,有的是细节丢失严重。大家都在用什么工具?求推荐!",
                Media = "[]",
                LikeCount = 92, CommentCount = 0, FavoriteCount = 18, ViewCount = 2400,
                Status = "active", ReviewStatus = "approved", PublishTime = DateTime.Now.AddHours(-5)
            },

            // ========== 审核状态 + AI风险等级测试 ==========

            // pending + none (正常帖子待审核)
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[2].Id,
                Type = "work",
                Title = "测试待审核帖子-无风险",
                Content = "这是一条待审核的正常帖子内容,用于测试审核流程。刚提交的作品,等待管理员审核。",
                Media = "[\"https://picsum.photos/seed/post-pending-none/600/600\"]",
                LikeCount = 0, CommentCount = 0, FavoriteCount = 0, ViewCount = 10,
                Status = "active", ReviewStatus = "pending", RiskLevel = "none", PublishTime = DateTime.Now.AddHours(-1)
            },

            // pending + low (低风险-含广告词)
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[0].Id,
                Type = "discussion",
                Title = "测试低风险帖子-疑似广告",
                Content = "拼豆兼职赚钱，日入500，有兴趣的可以了解一下~只是分享经验啦",
                Media = "[]",
                LikeCount = 0, CommentCount = 0, FavoriteCount = 0, ViewCount = 5,
                Status = "active", ReviewStatus = "pending", RiskLevel = "low",
                RiskTags = "[\"广告\",\"兼职\"]", PublishTime = DateTime.Now.AddHours(-2)
            },

            // pending + mid (中风险-疑似引流)
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[3].Id,
                Type = "work",
                Title = "测试中风险帖子-疑似引流",
                Content = "加我微信二维码，免费送拼豆材料包！拼豆微信群招新中",
                Media = "[\"https://picsum.photos/seed/post-pending-mid/600/600\"]",
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":300,\"totalColors\":4}",
                LikeCount = 0, CommentCount = 0, FavoriteCount = 0, ViewCount = 8,
                Status = "active", ReviewStatus = "pending", RiskLevel = "mid",
                RiskTags = "[\"引流\",\"联系方式\"]", PublishTime = DateTime.Now.AddHours(-3)
            },

            // pending + high (高风险-违规内容)
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[1].Id,
                Type = "work",
                Title = "测试高风险帖子-违规内容",
                Content = "赌博代刷暴力血腥内容测试帖子,包含多个拦截级敏感词",
                Media = "[]",
                LikeCount = 0, CommentCount = 0, FavoriteCount = 0, ViewCount = 3,
                Status = "active", ReviewStatus = "pending", RiskLevel = "high",
                RiskTags = "[\"赌博\",\"暴力\",\"代刷\"]", PublishTime = DateTime.Now.AddHours(-4)
            },

            // pending + low (求图帖子含轻微广告)
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[4].Id,
                Type = "request",
                Title = "测试低风险求图帖子",
                Content = "求一个可爱猫咪的图纸,拼多多砍一刀帮我凑个套装呗~",
                Media = "[]",
                LikeCount = 0, CommentCount = 0, FavoriteCount = 0, ViewCount = 2,
                Status = "active", ReviewStatus = "pending", RiskLevel = "low",
                RiskTags = "[\"其他平台\"]", PublishTime = DateTime.Now.AddMinutes(-30)
            },

            // pending + none (教程帖子待审核)
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[2].Id,
                Type = "tutorial",
                Title = "测试无风险教程帖子",
                Content = "【教程】拼豆熨烫技巧分享:温度控制在中等,熨烫时间10-15秒,注意不要移动底板~",
                Media = "[\"https://picsum.photos/seed/post-pending-tutorial/600/600\"]",
                LikeCount = 0, CommentCount = 0, FavoriteCount = 0, ViewCount = 6,
                Status = "active", ReviewStatus = "pending", RiskLevel = "none", PublishTime = DateTime.Now.AddMinutes(-45)
            },

            // rejected + mid (中风险被拒绝)
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[4].Id,
                Type = "work",
                Title = "测试被拒绝的帖子-中风险",
                Content = "这是一条被拒绝的帖子,内容包含疑似引流信息。",
                Media = "[]",
                LikeCount = 0, CommentCount = 0, FavoriteCount = 0, ViewCount = 5,
                Status = "active", ReviewStatus = "rejected", ReviewReason = "内容包含敏感信息", RiskLevel = "mid",
                RiskTags = "[\"引流\"]", PublishTime = DateTime.Now.AddDays(-2)
            },

            // approved + low (已通过但低风险-被替换过敏感词)
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[0].Id,
                Type = "work",
                Title = "圣诞拼豆分享-含替换词",
                Content = "今年的圣诞拼豆完成啦!红色和绿色的搭配很有[推广]感,推荐大家试试",
                Media = "[\"https://picsum.photos/seed/post-approved-low/600/600\"]",
                TopicIds = System.Text.Json.JsonSerializer.Serialize(new[] { topics[0].Id }),
                BeadParams = "{\"boardSize\":\"29x29\",\"totalBeads\":560,\"totalColors\":6}",
                LikeCount = 22, CommentCount = 0, FavoriteCount = 8, ViewCount = 150,
                Status = "active", ReviewStatus = "approved", RiskLevel = "low",
                RiskTags = "[\"广告(已替换)\"]", PublishTime = DateTime.Now.AddDays(-8)
            }
        };

        // 按标题去重，只插入不存在的帖子
        var newPosts = posts.Where(p => !existingTitles.Contains(p.Title)).ToList();
        if (newPosts.Count > 0)
            await _postRepo.InsertRangeAsync(newPosts);
    }

    private async Task SeedCommentsAsync()
    {
        if (await _commentRepo.AnyAsync(c => true)) return;
        var users = await _userRepo.GetListAsync();
        var posts = await _postRepo.GetListAsync();
        if (users.Count == 0 || posts.Count == 0) return;
        var comments = new List<Comment>
        {
            // 作品帖评论
            new() { Id = Guid.NewGuid().ToString(), PostId = posts[0].Id, UserId = users[1].Id, Content = "好可爱!请问用的是什么色号?", LikeCount = 3, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[0].Id, UserId = users[2].Id, Content = "配色很好看,期待更多作品~", LikeCount = 5, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[1].Id, UserId = users[0].Id, Content = "柴犬同款,哈哈好可爱", LikeCount = 2, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[3].Id, UserId = users[1].Id, Content = "配色绝了,请问配色比例怎么选?", LikeCount = 4, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[4].Id, UserId = users[0].Id, Content = "雏田太美了!色号能分享一下吗?", LikeCount = 12, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[4].Id, UserId = users[3].Id, Content = "二次元拼豆永远的神!", LikeCount = 8, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[7].Id, UserId = users[2].Id, Content = "马里奥经典!童年回忆杀", LikeCount = 6, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[8].Id, UserId = users[0].Id, Content = "国风太美了,仙鹤的渐变做得真好", LikeCount = 3, ReviewStatus = "approved", Status = "active" },

            // 教程帖评论
            new() { PostId = posts[9].Id, UserId = users[0].Id, Content = "教程很实用,收藏了!", LikeCount = 8, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[10].Id, UserId = users[4].Id, Content = "新手福音!终于知道怎么选底板了", LikeCount = 15, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[10].Id, UserId = users[2].Id, Content = "建议补充一下底板材质的区别", LikeCount = 4, ReviewStatus = "pending", Status = "active" },
            new() { PostId = posts[12].Id, UserId = users[3].Id, Content = "色号指南太及时了,刚入坑正需要!", LikeCount = 9, ReviewStatus = "approved", Status = "active" },

            // 求图帖评论
            new() { PostId = posts[14].Id, UserId = users[3].Id, Content = "我也想要!同求!", LikeCount = 2, ReviewStatus = "pending", Status = "active" },
            new() { PostId = posts[15].Id, UserId = users[0].Id, Content = "推荐用AI生成一个,效果不错", LikeCount = 5, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[17].Id, UserId = users[2].Id, Content = "情侣款好主意!做好了记得分享", LikeCount = 3, ReviewStatus = "pending", Status = "active" },

            // 讨论帖评论
            new() { PostId = posts[18].Id, UserId = users[3].Id, Content = "我一直在淘宝买,选对店铺质量还是不错的", LikeCount = 7, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[18].Id, UserId = users[4].Id, Content = "推荐MARD官方店,色号最全", LikeCount = 10, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[19].Id, UserId = users[0].Id, Content = "哈哈当然是拼豆!拼拼豆太绕口了", LikeCount = 15, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[19].Id, UserId = users[2].Id, Content = "我们这边叫拼拼豆诶...", LikeCount = 8, ReviewStatus = "pending", Status = "active" },
            new() { PostId = posts[20].Id, UserId = users[1].Id, Content = "29x29确实不够,建议直接上37x37", LikeCount = 6, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[21].Id, UserId = users[0].Id, Content = "当然是艺术!每一颗豆都是创作者的心血", LikeCount = 18, ReviewStatus = "approved", Status = "active" },
            new() { PostId = posts[23].Id, UserId = users[3].Id, Content = "会褪色的!一定要避免阳光直射,我之前晒褪色了哭死", LikeCount = 5, ReviewStatus = "pending", Status = "active" },
            new() { PostId = posts[24].Id, UserId = users[0].Id, Content = "拼豆自带的AI生成功能就很好用啊", LikeCount = 7, ReviewStatus = "approved", Status = "active" },

            // 待审核评论（模拟用户刚发布的评论）
            new() { PostId = posts[0].Id, UserId = users[4].Id, Content = "请问这个难度大吗?新手能做吗", LikeCount = 0, ReviewStatus = "pending", Status = "active" },
            new() { PostId = posts[5].Id, UserId = users[1].Id, Content = "哇!超美的星空拼豆!", LikeCount = 0, ReviewStatus = "pending", Status = "active" },
            new() { PostId = posts[11].Id, UserId = users[4].Id, Content = "学废了,感谢分享!", LikeCount = 0, ReviewStatus = "pending", Status = "active" },
            new() { PostId = posts[16].Id, UserId = users[2].Id, Content = "这个图案好复杂,有简化版吗?", LikeCount = 0, ReviewStatus = "pending", Status = "active" },
            new() { PostId = posts[22].Id, UserId = users[0].Id, Content = "参加!期待大家的作品", LikeCount = 0, ReviewStatus = "pending", Status = "active" },

            // 被拒绝的评论（含违规内容）
            new() { PostId = posts[0].Id, UserId = users[4].Id, Content = "加我微信xxx免费领拼豆", LikeCount = 0, ReviewStatus = "rejected", Status = "active" },
            new() { PostId = posts[3].Id, UserId = users[2].Id, Content = "广告位招租,联系QQ12345", LikeCount = 0, ReviewStatus = "rejected", Status = "active" },

            // 已隐藏的评论
            new() { PostId = posts[6].Id, UserId = users[4].Id, Content = "这个不好看,浪费材料", LikeCount = 0, ReviewStatus = "approved", Status = "hidden" },
            new() { PostId = posts[9].Id, UserId = users[1].Id, Content = "没什么用,别看了", LikeCount = 0, ReviewStatus = "approved", Status = "hidden" },
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

    #region 举报

    private async Task SeedReportsAsync()
    {
        if (await _reportRepo.AnyAsync(r => true)) return;
        var users = await _userRepo.GetListAsync();
        var posts = await _postRepo.GetListAsync();
        var admins = await _adminRepo.GetListAsync();
        if (users.Count < 3 || posts.Count < 5) return;

        var now = DateTime.Now;
        var reports = new List<Report>
        {
            // ===== 待处理 =====
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260619-001",
                ReporterId = users[1].Id,
                TargetType = "post",
                TargetId = posts[3].Id,
                TargetUserId = users[3].Id,
                Reason = "spam",
                Content = "帖子内容疑似广告引流，包含外部联系方式",
                Status = "pending"
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260619-002",
                ReporterId = users[2].Id,
                TargetType = "post",
                TargetId = posts[5].Id,
                TargetUserId = users[0].Id,
                Reason = "violation",
                Content = "帖子包含违规内容，涉及敏感话题",
                Status = "pending"
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260619-003",
                ReporterId = users[0].Id,
                TargetType = "comment",
                TargetId = posts[0].Id,
                TargetUserId = users[4].Id,
                Reason = "attack",
                Content = "评论中存在人身攻击和辱骂行为",
                Status = "pending"
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260619-004",
                ReporterId = users[3].Id,
                TargetType = "post",
                TargetId = posts[7].Id,
                TargetUserId = users[1].Id,
                Reason = "fake",
                Content = "帖子内容为虚假信息，误导其他用户",
                Status = "pending"
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260619-005",
                ReporterId = users[4].Id,
                TargetType = "user",
                TargetId = users[2].Id,
                TargetUserId = users[2].Id,
                Reason = "infringement",
                Content = "该用户盗用他人作品，侵犯知识产权",
                Status = "pending"
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260619-006",
                ReporterId = users[1].Id,
                TargetType = "post",
                TargetId = posts[10].Id,
                TargetUserId = users[0].Id,
                Reason = "other",
                Content = "帖子内容与拼豆无关，属于无关内容",
                Status = "pending"
            },

            // ===== 已处理 - 警告 =====
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260618-001",
                ReporterId = users[0].Id,
                TargetType = "post",
                TargetId = posts[1].Id,
                TargetUserId = users[1].Id,
                Reason = "spam",
                Content = "帖子中包含推广链接",
                Status = "warned",
                HandleResult = "已对发布者发送警告通知，要求删除推广内容",
                HandlerId = admins.Count > 0 ? admins[0].Id.ToString() : null,
                HandleTime = now.AddDays(-1).AddHours(3)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260618-002",
                ReporterId = users[2].Id,
                TargetType = "comment",
                TargetId = posts[4].Id,
                TargetUserId = users[3].Id,
                Reason = "attack",
                Content = "评论中使用了不文明用语",
                Status = "warned",
                HandleResult = "已警告该用户，评论已标记",
                HandlerId = admins.Count > 0 ? admins[0].Id.ToString() : null,
                HandleTime = now.AddDays(-1).AddHours(5)
            },

            // ===== 已处理 - 封禁内容 =====
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260617-001",
                ReporterId = users[3].Id,
                TargetType = "post",
                TargetId = posts[6].Id,
                TargetUserId = users[4].Id,
                Reason = "violation",
                Content = "帖子包含严重违规内容，涉及赌博信息",
                Status = "ban_content",
                HandleResult = "内容已下架，对发布者进行警告处理",
                HandlerId = admins.Count > 1 ? admins[1].Id.ToString() : null,
                HandleTime = now.AddDays(-2).AddHours(2)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260617-002",
                ReporterId = users[0].Id,
                TargetType = "post",
                TargetId = posts[8].Id,
                TargetUserId = users[2].Id,
                Reason = "spam",
                Content = "帖子为纯广告内容，无拼豆相关内容",
                Status = "ban_content",
                HandleResult = "广告内容已删除",
                HandlerId = admins.Count > 0 ? admins[0].Id.ToString() : null,
                HandleTime = now.AddDays(-2).AddHours(8)
            },

            // ===== 已处理 - 封禁用户 =====
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260616-001",
                ReporterId = users[1].Id,
                TargetType = "user",
                TargetId = users[4].Id,
                TargetUserId = users[4].Id,
                Reason = "spam",
                Content = "该用户多次发布垃圾广告，屡教不改",
                Status = "ban_user",
                HandleResult = "用户已被封禁，所有违规内容已清理",
                HandlerId = admins.Count > 0 ? admins[0].Id.ToString() : null,
                HandleTime = now.AddDays(-3).AddHours(1)
            },

            // ===== 已忽略 =====
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260615-001",
                ReporterId = users[4].Id,
                TargetType = "post",
                TargetId = posts[0].Id,
                TargetUserId = users[0].Id,
                Reason = "other",
                Content = "我觉得这个帖子不好看",
                Status = "ignored",
                HandleResult = "举报理由不充分，帖子内容未违反社区规范",
                HandlerId = admins.Count > 1 ? admins[1].Id.ToString() : null,
                HandleTime = now.AddDays(-4).AddHours(6)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260615-002",
                ReporterId = users[2].Id,
                TargetType = "comment",
                TargetId = posts[2].Id,
                TargetUserId = users[1].Id,
                Reason = "attack",
                Content = "评论语气不太好",
                Status = "ignored",
                HandleResult = "评论内容属于正常讨论范畴，未构成人身攻击",
                HandlerId = admins.Count > 0 ? admins[0].Id.ToString() : null,
                HandleTime = now.AddDays(-4).AddHours(10)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportId = "RPT-20260615-003",
                ReporterId = users[0].Id,
                TargetType = "post",
                TargetId = posts[9].Id,
                TargetUserId = users[2].Id,
                Reason = "infringement",
                Content = "疑似盗图，但无法确认",
                Status = "ignored",
                HandleResult = "举报人未能提供有效证据，暂不处理",
                HandlerId = admins.Count > 1 ? admins[1].Id.ToString() : null,
                HandleTime = now.AddDays(-5)
            }
        };

        await _reportRepo.InsertRangeAsync(reports);
    }

    #endregion
}
