using Pindou.Shared.Utilities;

namespace Pindou.Tests.Utilities;

public class CryptoHelperTests
{
    #region Md5

    [Fact]
    public void Md5_KnownInput_ReturnsExpectedHash()
    {
        Assert.Equal("5d41402abc4b2a76b9719d911017c592", CryptoHelper.Md5("hello"));
        Assert.Equal("d41d8cd98f00b204e9800998ecf8427e", CryptoHelper.Md5(""));
        Assert.Equal("e10adc3949ba59abbe56e057f20f883e", CryptoHelper.Md5("123456"));
    }

    [Fact]
    public void Md5_SameInput_ReturnsSameHash()
    {
        Assert.Equal(CryptoHelper.Md5("pindou"), CryptoHelper.Md5("pindou"));
    }

    [Fact]
    public void Md5_DifferentCase_DifferentHash()
    {
        Assert.NotEqual(CryptoHelper.Md5("PINDOU"), CryptoHelper.Md5("pindou"));
    }

    [Fact]
    public void Md5_NullOrEmpty_Handled()
    {
        Assert.Equal("d41d8cd98f00b204e9800998ecf8427e", CryptoHelper.Md5(""));
    }

    [Fact]
    public void Md5_ChineseInput_Returns32Hex()
    {
        var hash = CryptoHelper.Md5("拼豆");
        Assert.Equal(32, hash.Length);
        Assert.Matches("^[a-f0-9]{32}$", hash);
    }

    [Fact]
    public void Md5WithSalt_DiffersFromPlainMd5()
    {
        var plain = CryptoHelper.Md5("password");
        var salted = CryptoHelper.Md5WithSalt("password", "abc");
        Assert.NotEqual(plain, salted);
    }

    [Fact]
    public void Md5WithSalt_SameInputSalt_Stable()
    {
        Assert.Equal(CryptoHelper.Md5WithSalt("p", "s"), CryptoHelper.Md5WithSalt("p", "s"));
    }

    [Fact]
    public void Md5WithSalt_DifferentSalt_DifferentHash()
    {
        Assert.NotEqual(CryptoHelper.Md5WithSalt("p", "s1"), CryptoHelper.Md5WithSalt("p", "s2"));
    }

    #endregion

    #region Sha256

    [Fact]
    public void Sha256_KnownInput_ReturnsExpectedHash()
    {
        Assert.Equal("2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824",
            CryptoHelper.Sha256("hello"));
    }

    [Fact]
    public void Sha256_Returns64HexChars()
    {
        Assert.Equal(64, CryptoHelper.Sha256("any").Length);
    }

    #endregion

    #region AesEncrypt / AesDecrypt

    [Fact]
    public void Aes_RoundTrip_PreservesPlaintext()
    {
        const string key = "01234567890123456789012345678901"; // 32 chars
        const string iv = "0123456789012345"; // 16 chars
        const string plain = "PindouProjectSecretMessage";

        var cipher = CryptoHelper.AesEncrypt(plain, key, iv);
        var decrypted = CryptoHelper.AesDecrypt(cipher, key, iv);

        Assert.Equal(plain, decrypted);
    }

    [Fact]
    public void Aes_DecryptWithoutEncrypt_Throws()
    {
        Assert.Throws<System.Security.Cryptography.CryptographicException>(() =>
            CryptoHelper.AesDecrypt("not-a-valid-base64", "01234567890123456789012345678901", "0123456789012345"));
    }

    [Fact]
    public void Aes_OutputIsBase64()
    {
        var cipher = CryptoHelper.AesEncrypt("data", "k".PadRight(32, 'k'), "i".PadRight(16, 'i'));
        Assert.Matches("^[A-Za-z0-9+/=]+$", cipher);
    }

    [Fact]
    public void Aes_ShortKey_AutoPadded()
    {
        const string key = "abc";
        const string iv = "iv";
        const string plain = "test";

        var cipher = CryptoHelper.AesEncrypt(plain, key, iv);
        var decrypted = CryptoHelper.AesDecrypt(cipher, key, iv);

        Assert.Equal(plain, decrypted);
    }

    [Fact]
    public void Aes_LongKey_Truncated()
    {
        const string key = "this-is-a-very-long-key-over-thirtytwo-characters";
        const string iv = "long-iv-over-16";
        const string plain = "data";

        var cipher = CryptoHelper.AesEncrypt(plain, key, iv);
        var decrypted = CryptoHelper.AesDecrypt(cipher, key, iv);

        Assert.Equal(plain, decrypted);
    }

    [Fact]
    public void Aes_DifferentIv_DifferentCipher()
    {
        const string key = "01234567890123456789012345678901";
        const string plain = "Pindou";
        var c1 = CryptoHelper.AesEncrypt(plain, key, "1111111111111111");
        var c2 = CryptoHelper.AesEncrypt(plain, key, "2222222222222222");
        Assert.NotEqual(c1, c2);
    }

    [Fact]
    public void Aes_EmptyPlaintext_RoundTrip()
    {
        const string key = "01234567890123456789012345678901";
        const string iv = "0123456789012345";
        var cipher = CryptoHelper.AesEncrypt("", key, iv);
        var decrypted = CryptoHelper.AesDecrypt(cipher, key, iv);
        Assert.Equal("", decrypted);
    }

    #endregion

    #region GenerateSalt

    [Fact]
    public void GenerateSalt_Default_LengthIs6()
    {
        Assert.Equal(6, CryptoHelper.GenerateSalt().Length);
    }

    [Fact]
    public void GenerateSalt_CustomLength_Respected()
    {
        Assert.Equal(10, CryptoHelper.GenerateSalt(10).Length);
    }

    [Fact]
    public void GenerateSalt_OnlyLowercaseOrDigits()
    {
        var salt = CryptoHelper.GenerateSalt(20);
        Assert.Matches("^[a-z0-9]+$", salt);
    }

    [Fact]
    public void GenerateSalt_HighUniqueness()
    {
        var set = new HashSet<string>();
        for (var i = 0; i < 200; i++) set.Add(CryptoHelper.GenerateSalt(8));
        Assert.True(set.Count > 190, $"Got {set.Count} unique salts out of 200");
    }

    #endregion
}
