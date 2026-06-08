using System.Security.Cryptography;
using System.Text;

namespace Pindou.Shared.Utilities;

/// <summary>
/// 加密工具
/// </summary>
public static class CryptoHelper
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public static string Md5(string input)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    /// <summary>
    /// MD5 + 盐
    /// </summary>
    public static string Md5WithSalt(string input, string salt)
    {
        return Md5(input + salt);
    }

    /// <summary>
    /// SHA256
    /// </summary>
    public static string Sha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    /// <summary>
    /// AES加密
    /// </summary>
    public static string AesEncrypt(string plainText, string key, string iv)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        aes.IV = Encoding.UTF8.GetBytes(iv.PadRight(16).Substring(0, 16));
        var encryptor = aes.CreateEncryptor();
        var inputBytes = Encoding.UTF8.GetBytes(plainText);
        var encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// AES解密
    /// </summary>
    public static string AesDecrypt(string cipherText, string key, string iv)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        aes.IV = Encoding.UTF8.GetBytes(iv.PadRight(16).Substring(0, 16));
        var decryptor = aes.CreateDecryptor();
        var inputBytes = Convert.FromBase64String(cipherText);
        var decrypted = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
        return Encoding.UTF8.GetString(decrypted);
    }

    /// <summary>
    /// 生成随机盐
    /// </summary>
    public static string GenerateSalt(int length = 6)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var random = new char[length];
        for (var i = 0; i < length; i++)
            random[i] = chars[Random.Shared.Next(chars.Length)];
        return new string(random);
    }
}
