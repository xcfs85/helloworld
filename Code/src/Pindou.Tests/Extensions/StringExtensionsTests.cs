using Pindou.Shared.Extensions;

namespace Pindou.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_TrueForNullAndEmpty()
    {
        string? s = null;
        Assert.True(s.IsNullOrEmpty());
        Assert.True("".IsNullOrEmpty());
        Assert.False("a".IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrWhiteSpace_TrueForWhitespace()
    {
        string? s = null;
        Assert.True(s.IsNullOrWhiteSpace());
        Assert.True("".IsNullOrWhiteSpace());
        Assert.True("   ".IsNullOrWhiteSpace());
        Assert.False("a".IsNullOrWhiteSpace());
    }

    [Fact]
    public void Or_ReturnsDefault_WhenEmpty()
    {
        string? s = null;
        Assert.Equal("def", s.Or("def"));
        Assert.Equal("def", "".Or("def"));
        Assert.Equal("x", "x".Or("def"));
    }

    [Fact]
    public void Truncate_ShortString_Unchanged()
    {
        Assert.Equal("abc", "abc".Truncate(10));
    }

    [Fact]
    public void Truncate_LongString_Truncated()
    {
        var s = "abcdefghij".Truncate(5);
        Assert.Equal("abcde...", s);
    }

    [Fact]
    public void Truncate_ExactLength_NoSuffix()
    {
        Assert.Equal("abcde", "abcde".Truncate(5));
    }

    [Fact]
    public void Truncate_EmptyString_Empty()
    {
        Assert.Equal("", "".Truncate(5));
    }

    [Fact]
    public void MaskPhone_HidesMiddle4()
    {
        Assert.Equal("138****8000", "13800138000".MaskPhone());
    }

    [Fact]
    public void MaskPhone_Null_ReturnsEmpty()
    {
        string? s = null;
        Assert.Equal("", s.MaskPhone());
    }

    [Fact]
    public void MaskEmail_HidesLocalPart()
    {
        Assert.Equal("pi****@example.com", "pindou@example.com".MaskEmail());
    }
}

public class DateTimeExtensionsTests
{
    [Fact]
    public void ToUnixTime_RoundTrip()
    {
        var now = DateTime.Now;
        var unix = now.ToUnixTime();
        var back = unix.FromUnixTime();
        // 注意精度: UnixTime 是秒,DateTime 在转换时丢失毫秒
        Assert.InRange((back - now).TotalSeconds, -1, 1);
    }

    [Fact]
    public void ToUnixTimeMs_RoundTrip()
    {
        var now = DateTime.UtcNow;
        var unixMs = now.ToUnixTimeMs();
        var back = unixMs.FromUnixTimeMs();
        // FromUnixTimeMs 转 LocalDateTime, 时区可能不同; 只比较 ticks 的差
        var diffTicks = Math.Abs((back.ToUniversalTime() - now).Ticks);
        Assert.True(diffTicks < TimeSpan.TicksPerSecond, $"diff was {TimeSpan.FromTicks(diffTicks)}");
    }

    [Fact]
    public void StartOfDay_ReturnsMidnight()
    {
        var dt = new DateTime(2026, 6, 7, 15, 30, 45);
        Assert.Equal(new DateTime(2026, 6, 7), dt.StartOfDay());
    }

    [Fact]
    public void EndOfDay_Returns235959()
    {
        var dt = new DateTime(2026, 6, 7, 0, 0, 0);
        Assert.Equal(new DateTime(2026, 6, 7, 23, 59, 59).AddTicks(9999999), dt.EndOfDay());
    }

    [Fact]
    public void StartOfMonth_ReturnsFirstDay()
    {
        var dt = new DateTime(2026, 6, 15);
        Assert.Equal(new DateTime(2026, 6, 1), dt.StartOfMonth());
    }

    [Fact]
    public void StartOfWeek_ResetsToSunday()
    {
        // DayOfWeek.Sunday = 0
        var dt = new DateTime(2026, 6, 7); // 2026-06-07 是 Sunday
        Assert.Equal(new DateTime(2026, 6, 7), dt.StartOfWeek());
    }
}

public class EnumExtensionsTests
{
    private enum Sample { None, Active, Inactive }

    [Fact]
    public void GetDescription_NoAttribute_ReturnsName()
    {
        Assert.Equal("Active", Sample.Active.GetDescription());
    }

    [Fact]
    public void GetDescription_WithAttribute_ReturnsDescription()
    {
        Assert.Equal("有效", SampleWithAttr.Active.GetDescription());
    }

    private enum SampleWithAttr
    {
        None,
        [System.ComponentModel.Description("有效")]
        Active
    }
}
