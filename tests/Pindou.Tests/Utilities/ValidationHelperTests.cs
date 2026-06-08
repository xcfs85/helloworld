using Pindou.Shared.Utilities;

namespace Pindou.Tests.Utilities;

public class ValidationHelperTests
{
    #region IsPhone

    [Theory]
    [InlineData("13800138000", true)]
    [InlineData("17612345678", true)]
    [InlineData("19912345678", true)]
    [InlineData("12345678901", false)]
    [InlineData("1380013800", false)]
    [InlineData("138001380000", false)]
    [InlineData("23800138000", false)]
    [InlineData("1380013800a", false)]
    public void IsPhone_ValidatesChineseMobile(string input, bool expected)
    {
        Assert.Equal(expected, ValidationHelper.IsPhone(input));
    }

    #endregion

    #region IsEmail

    [Theory]
    [InlineData("a@b.com", true)]
    [InlineData("user@example.com", true)]
    [InlineData("user.name+tag@example.co.uk", true)]
    [InlineData("a@b", false)]
    [InlineData("@b.com", false)]
    [InlineData("user@", false)]
    [InlineData("user@@example.com", false)]
    public void IsEmail_ValidatesEmail(string input, bool expected)
    {
        Assert.Equal(expected, ValidationHelper.IsEmail(input));
    }

    #endregion

    #region IsIdCard

    [Theory]
    [InlineData("11010519491231002X", true)]
    [InlineData("110105194912310026", true)]
    [InlineData("123456789012345", true)]
    [InlineData("abc", false)]
    [InlineData("12345", false)]
    [InlineData("12345678901234567", false)]
    public void IsIdCard_ValidatesLengthOnly(string input, bool expected)
    {
        // 注: 当前实现只检查长度 (15 or 18)
        Assert.Equal(expected, ValidationHelper.IsIdCard(input));
    }

    #endregion

    #region MaskPhone

    [Fact]
    public void MaskPhone_HidesMiddle4()
    {
        Assert.Equal("138****8000", ValidationHelper.MaskPhone("13800138000"));
    }

    [Fact]
    public void MaskPhone_ShortInput_ReturnedAsIs()
    {
        Assert.Equal("12345", ValidationHelper.MaskPhone("12345"));
    }

    [Fact]
    public void MaskPhone_EmptyInput_Empty()
    {
        Assert.Equal("", ValidationHelper.MaskPhone(""));
    }

    #endregion

    #region MaskEmail

    [Fact]
    public void MaskEmail_LongName_FirstTwoShown()
    {
        Assert.Equal("pi****@example.com", ValidationHelper.MaskEmail("pindou@example.com"));
    }

    [Fact]
    public void MaskEmail_ShortName_OneStar()
    {
        Assert.Equal("a*@example.com", ValidationHelper.MaskEmail("a@example.com"));
    }

    [Fact]
    public void MaskEmail_NoAt_ReturnedAsIs()
    {
        Assert.Equal("not-an-email", ValidationHelper.MaskEmail("not-an-email"));
    }

    [Fact]
    public void MaskEmail_EmptyInput_Empty()
    {
        Assert.Equal("", ValidationHelper.MaskEmail(""));
    }

    #endregion

    #region MaskIdCard

    [Fact]
    public void MaskIdCard_LongHidesMiddle()
    {
        Assert.Equal("1101**********002X", ValidationHelper.MaskIdCard("11010519491231002X"));
    }

    [Fact]
    public void MaskIdCard_ShortInput_ReturnedAsIs()
    {
        Assert.Equal("1234567", ValidationHelper.MaskIdCard("1234567"));
    }

    #endregion
}
