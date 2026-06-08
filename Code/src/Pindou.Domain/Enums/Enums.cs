namespace Pindou.Domain.Enums;

/// <summary>
/// 用户状态
/// </summary>
public enum UserStatus
{
    /// <summary>激活</summary>
    Active = 1,
    /// <summary>禁用</summary>
    Disabled = 0
}

/// <summary>
/// 性别
/// </summary>
public enum Gender
{
    /// <summary>未知</summary>
    Unknown = 0,
    /// <summary>男</summary>
    Male = 1,
    /// <summary>女</summary>
    Female = 2
}

/// <summary>
/// 登录方式
/// </summary>
public enum LoginType
{
    /// <summary>手机号</summary>
    Phone = 1,
    /// <summary>微信</summary>
    Wechat = 2,
    /// <summary>Apple</summary>
    Apple = 3,
    /// <summary>游客</summary>
    Guest = 4
}

/// <summary>
/// 图纸状态
/// </summary>
public enum DiagramStatus
{
    /// <summary>草稿</summary>
    Draft = 0,
    /// <summary>已完成</summary>
    Completed = 1,
    /// <summary>已拼豆</summary>
    Beaded = 2
}

/// <summary>
/// 难度等级
/// </summary>
public enum Difficulty
{
    /// <summary>简单</summary>
    Easy = 1,
    /// <summary>中等</summary>
    Medium = 2,
    /// <summary>困难</summary>
    Hard = 3,
    /// <summary>专家</summary>
    Expert = 4
}

/// <summary>
/// 风格
/// </summary>
public enum StyleType
{
    /// <summary>像素</summary>
    Pixel = 1,
    /// <summary>卡通</summary>
    Cartoon = 2,
    /// <summary>写实</summary>
    Realistic = 3,
    /// <summary>Q版</summary>
    Chibi = 4
}

/// <summary>
/// 任务状态
/// </summary>
public enum TaskStatus
{
    /// <summary>待处理</summary>
    Pending = 0,
    /// <summary>处理中</summary>
    Processing = 1,
    /// <summary>已完成</summary>
    Completed = 2,
    /// <summary>失败</summary>
    Failed = 3
}

/// <summary>
/// 帖子类型
/// </summary>
public enum PostType
{
    /// <summary>作品</summary>
    Work = 1,
    /// <summary>求拼豆</summary>
    Request = 2,
    /// <summary>教程</summary>
    Tutorial = 3,
    /// <summary>讨论</summary>
    Discussion = 4
}

/// <summary>
/// 审核状态
/// </summary>
public enum ReviewStatus
{
    /// <summary>待审核</summary>
    Pending = 0,
    /// <summary>已通过</summary>
    Approved = 1,
    /// <summary>已拒绝</summary>
    Rejected = 2
}

/// <summary>
/// 通用状态
/// </summary>
public enum CommonStatus
{
    /// <summary>禁用</summary>
    Disabled = 0,
    /// <summary>启用</summary>
    Enabled = 1
}

/// <summary>
/// 消息类型
/// </summary>
public enum MessageType
{
    /// <summary>评论</summary>
    Comment = 1,
    /// <summary>点赞</summary>
    Like = 2,
    /// <summary>关注</summary>
    Follow = 3,
    /// <summary>@</summary>
    At = 4,
    /// <summary>系统</summary>
    System = 5,
    /// <summary>营销</summary>
    Marketing = 6
}

/// <summary>
/// 会员类型
/// </summary>
public enum MemberType
{
    /// <summary>月度</summary>
    Month = 1,
    /// <summary>季度</summary>
    Quarter = 2,
    /// <summary>年度</summary>
    Year = 3,
    /// <summary>终身</summary>
    Lifetime = 4
}

/// <summary>
/// 订单状态
/// </summary>
public enum OrderStatus
{
    /// <summary>待支付</summary>
    Pending = 0,
    /// <summary>已支付</summary>
    Paid = 1,
    /// <summary>已取消</summary>
    Canceled = 2,
    /// <summary>已退款</summary>
    Refunded = 3
}

/// <summary>
/// Banner跳转类型
/// </summary>
public enum BannerLinkType
{
    /// <summary>URL</summary>
    Url = 1,
    /// <summary>帖子</summary>
    Post = 2,
    /// <summary>模板</summary>
    Template = 3
}

/// <summary>
/// 推送类型
/// </summary>
public enum PushType
{
    /// <summary>系统</summary>
    System = 1,
    /// <summary>活动</summary>
    Activity = 2
}

/// <summary>
/// 推送目标类型
/// </summary>
public enum PushTargetType
{
    /// <summary>全部</summary>
    All = 1,
    /// <summary>标签</summary>
    Tag = 2,
    /// <summary>用户</summary>
    User = 3
}

/// <summary>
/// 敏感词类型
/// </summary>
public enum SensitiveType
{
    /// <summary>政治</summary>
    Politics = 1,
    /// <summary>色情</summary>
    Porn = 2,
    /// <summary>暴力</summary>
    Violence = 3,
    /// <summary>广告</summary>
    Ad = 4,
    /// <summary>其他</summary>
    Other = 5
}

/// <summary>
/// 敏感词级别
/// </summary>
public enum SensitiveLevel
{
    /// <summary>警告</summary>
    Warning = 1,
    /// <summary>替换</summary>
    Replace = 2,
    /// <summary>拦截</summary>
    Block = 3
}

/// <summary>
/// 举报状态
/// </summary>
public enum ReportStatus
{
    /// <summary>待处理</summary>
    Pending = 0,
    /// <summary>忽略</summary>
    Ignored = 1,
    /// <summary>警告</summary>
    Warned = 2,
    /// <summary>封禁内容</summary>
    BanContent = 3,
    /// <summary>封禁用户</summary>
    BanUser = 4
}
