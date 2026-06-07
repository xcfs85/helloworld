using SqlSugar;
using Pindou.Domain.Common;

namespace Pindou.Domain.Entities.Admin;

/// <summary>
/// 管理员表
/// </summary>
[SugarTable("admin_users")]
public class AdminUser : BaseEntity
{
    /// <summary>用户名</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Username { get; set; } = string.Empty;

    /// <summary>密码(BCrypt)</summary>
    [SugarColumn(Length = 255, IsNullable = false)]
    public string Password { get; set; } = string.Empty;

    /// <summary>昵称</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Nickname { get; set; }

    /// <summary>角色ID</summary>
    [SugarColumn(IsNullable = false)]
    public long RoleId { get; set; }

    /// <summary>状态</summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public int Status { get; set; } = 1;

    /// <summary>最后登录时间</summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>最后登录IP</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LastLoginIp { get; set; }
}

/// <summary>
/// 角色表
/// </summary>
[SugarTable("roles")]
public class Role : BaseEntity
{
    /// <summary>角色名称</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>角色编码</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Code { get; set; } = string.Empty;

    /// <summary>描述</summary>
    [SugarColumn(Length = 255, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>权限列表(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Permissions { get; set; } = "[]";
}

/// <summary>
/// 操作日志表
/// </summary>
[SugarTable("operation_logs")]
public class OperationLog : BaseEntity
{
    /// <summary>操作人ID</summary>
    [SugarColumn(IsNullable = false)]
    public long UserId { get; set; }

    /// <summary>用户名</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Username { get; set; } = string.Empty;

    /// <summary>昵称</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Nickname { get; set; }

    /// <summary>操作</summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Operation { get; set; } = string.Empty;

    /// <summary>内容</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Content { get; set; }

    /// <summary>方法</summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Method { get; set; }

    /// <summary>参数(JSON)</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Params { get; set; }

    /// <summary>IP地址</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Ip { get; set; }

    /// <summary>UA</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? UserAgent { get; set; }
}
