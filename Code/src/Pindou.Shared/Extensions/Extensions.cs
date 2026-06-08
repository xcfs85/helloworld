using System.Text.RegularExpressions;

namespace Pindou.Shared.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);
    public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);
    public static string Or(this string? str, string defaultValue) => string.IsNullOrEmpty(str) ? defaultValue : str;

    /// <summary>截断字符串</summary>
    public static string Truncate(this string str, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(str) || str.Length <= maxLength) return str;
        return str[..maxLength] + suffix;
    }

    /// <summary>脱敏手机号</summary>
    public static string MaskPhone(this string? phone) => Utilities.ValidationHelper.MaskPhone(phone ?? string.Empty);

    /// <summary>脱敏邮箱</summary>
    public static string MaskEmail(this string? email) => Utilities.ValidationHelper.MaskEmail(email ?? string.Empty);
}

public static class DateTimeExtensions
{
    public static long ToUnixTime(this DateTime dt) => new DateTimeOffset(dt).ToUnixTimeSeconds();
    public static long ToUnixTimeMs(this DateTime dt) => new DateTimeOffset(dt).ToUnixTimeMilliseconds();
    public static DateTime FromUnixTime(this long unix) => DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime;
    public static DateTime FromUnixTimeMs(this long unixMs) => DateTimeOffset.FromUnixTimeMilliseconds(unixMs).LocalDateTime;
    public static DateTime StartOfDay(this DateTime dt) => dt.Date;
    public static DateTime EndOfDay(this DateTime dt) => dt.Date.AddDays(1).AddTicks(-1);
    public static DateTime StartOfWeek(this DateTime dt) => dt.AddDays(-(int)dt.DayOfWeek).Date;
    public static DateTime StartOfMonth(this DateTime dt) => new(dt.Year, dt.Month, 1);
}

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();
        var name = value.ToString();
        var field = type.GetField(name);
        if (field == null) return name;
        var attr = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
        return attr.Length > 0 ? ((System.ComponentModel.DescriptionAttribute)attr[0]).Description : name;
    }
}
