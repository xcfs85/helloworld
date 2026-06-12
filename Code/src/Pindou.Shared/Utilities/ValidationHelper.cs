using System.Text.RegularExpressions;

namespace Pindou.Shared.Utilities;

/// <summary>
/// 验证工具
/// </summary>
public static class ValidationHelper
{
    /// <summary>手机号正则</summary>
    private static readonly Regex PhoneRegex = new(@"^1[3-9]\d{9}$", RegexOptions.Compiled);

    /// <summary>邮箱正则</summary>
    private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

    public static bool IsPhone(string phone) => !string.IsNullOrEmpty(phone) && PhoneRegex.IsMatch(phone);
    public static bool IsEmail(string email) => !string.IsNullOrEmpty(email) && EmailRegex.IsMatch(email);
    public static bool IsIdCard(string idCard) => !string.IsNullOrEmpty(idCard) && (idCard.Length == 15 || idCard.Length == 18);

    /// <summary>手机号脱敏 138****8000</summary>
    public static string MaskPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 7) return phone;
        return $"{phone[..3]}****{phone[^4..]}";
    }

    /// <summary>身份证脱敏</summary>
    public static string MaskIdCard(string idCard)
    {
        if (string.IsNullOrEmpty(idCard) || idCard.Length < 8) return idCard;
        return $"{idCard[..4]}**********{idCard[^4..]}";
    }

    /// <summary>邮箱脱敏</summary>
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains('@')) return email;
        var parts = email.Split('@');
        var name = parts[0];
        if (string.IsNullOrEmpty(name)) return email;
        var masked = name.Length == 1 ? name + "*" : $"{name[..2]}****";
        return $"{masked}@{parts[1]}";
    }
}
