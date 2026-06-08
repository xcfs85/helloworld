using Microsoft.Extensions.Logging;
using Pindou.Infrastructure.Options;

namespace Pindou.Infrastructure.ExternalServices.Storage;

public class LocalStorageService : IStorageService
{
    private readonly StorageOptions _options;
    private readonly ILogger<LocalStorageService> _logger;
    private readonly string _rootPath;

    public LocalStorageService(StorageOptions options, ILogger<LocalStorageService> logger)
    {
        _options = options;
        _logger = logger;
        _rootPath = Path.GetFullPath(options.LocalPath);
        if (!Directory.Exists(_rootPath))
        {
            Directory.CreateDirectory(_rootPath);
        }
    }

    public async Task<UploadResult> UploadAsync(Stream stream, string fileName, string contentType, string? folder = null)
    {
        try
        {
            var ext = Path.GetExtension(fileName);
            var key = $"{folder ?? "default"}/{DateTime.Now:yyyy/MM/dd}/{Guid.NewGuid():N}{ext}";
            var path = Path.Combine(_rootPath, key);
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            await using (var fs = File.Create(path))
            {
                await stream.CopyToAsync(fs);
            }
            return new UploadResult
            {
                Success = true,
                Key = key,
                Url = GetPublicUrl(key),
                Size = stream.Length
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload file failed: {FileName}", fileName);
            return new UploadResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<UploadResult> UploadBytesAsync(byte[] data, string fileName, string contentType, string? folder = null)
    {
        using var ms = new MemoryStream(data);
        return await UploadAsync(ms, fileName, contentType, folder);
    }

    public Task<bool> DeleteAsync(string key)
    {
        try
        {
            var path = Path.Combine(_rootPath, key);
            if (File.Exists(path))
            {
                File.Delete(path);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete file failed: {Key}", key);
            return Task.FromResult(false);
        }
    }

    public async Task<Stream?> DownloadAsync(string key)
    {
        var path = Path.Combine(_rootPath, key);
        if (!File.Exists(path)) return null;
        var bytes = await File.ReadAllBytesAsync(path);
        return new MemoryStream(bytes);
    }

    public string GetPublicUrl(string key)
    {
        return $"{_options.PublicBaseUrl.TrimEnd('/')}/uploads/{key}";
    }
}
