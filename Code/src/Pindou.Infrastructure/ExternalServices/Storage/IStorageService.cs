namespace Pindou.Infrastructure.ExternalServices.Storage;

public class UploadResult
{
    public bool Success { get; set; }
    public string? Url { get; set; }
    public string? Key { get; set; }
    public long Size { get; set; }
    public string? ErrorMessage { get; set; }
}

public class StorageFile
{
    public string Key { get; set; } = string.Empty;
    public Stream Content { get; set; } = Stream.Null;
    public string ContentType { get; set; } = "application/octet-stream";
    public long Size { get; set; }
}

public interface IStorageService
{
    Task<UploadResult> UploadAsync(Stream stream, string fileName, string contentType, string? folder = null);
    Task<UploadResult> UploadBytesAsync(byte[] data, string fileName, string contentType, string? folder = null);
    Task<bool> DeleteAsync(string key);
    Task<Stream?> DownloadAsync(string key);
    string GetPublicUrl(string key);
}
