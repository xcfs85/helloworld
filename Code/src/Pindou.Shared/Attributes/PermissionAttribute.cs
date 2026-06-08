namespace Pindou.Shared.Attributes;

/// <summary>
/// 权限特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class PermissionAttribute : Attribute
{
    /// <summary>权限编码 如 user:add</summary>
    public string Code { get; }

    public PermissionAttribute(string code)
    {
        Code = code;
    }
}

/// <summary>
/// 操作日志特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OperationLogAttribute : Attribute
{
    /// <summary>操作名称</summary>
    public string Name { get; }
    public string? Content { get; set; }
    public bool SaveParams { get; set; } = false;

    public OperationLogAttribute(string name)
    {
        Name = name;
    }
}

/// <summary>
/// 跳过权限验证
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AllowAnonymousAttribute : Attribute
{
}
