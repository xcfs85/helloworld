using Microsoft.AspNetCore.Mvc;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Auth;
using Pindou.Application.DTOs.User;
using Pindou.Application.Interfaces.User;
using Pindou.Infrastructure.ExternalServices.Storage;
using Pindou.Shared.Attributes;

namespace Pindou.Api.Controllers;

[ApiController]
[Route("api/v1/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IStorageService _storage;
    public UserController(IUserService userService, IStorageService storage)
    {
        _userService = userService;
        _storage = storage;
    }

    [HttpGet("info")]
    public async Task<ApiResponse<UserInfo>> Info()
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _userService.GetUserInfoAsync(userId);
        return ApiResponse<UserInfo>.Ok(data);
    }

    [HttpPut("info")]
    public async Task<ApiResponse<UserInfo>> UpdateInfo([FromBody] UpdateUserRequest request)
    {
        var userId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var data = await _userService.UpdateUserInfoAsync(userId, request);
        return ApiResponse<UserInfo>.Ok(data);
    }
}

[ApiController]
[Route("api/v1/upload")]
public class UploadController : ControllerBase
{
    private readonly IStorageService _storage;
    public UploadController(IStorageService storage) { _storage = storage; }

    /// <summary>上传图片</summary>
    [HttpPost("image")]
    public async Task<ApiResponse<UploadFileResponse>> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return ApiResponse<UploadFileResponse>.Fail("请选择文件", 1001);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (Array.IndexOf(allowed, ext) < 0)
            return ApiResponse<UploadFileResponse>.Fail("不支持的文件格式", 1001);

        if (file.Length > 10 * 1024 * 1024)
            return ApiResponse<UploadFileResponse>.Fail("文件大小超过10MB", 1001);

        await using var stream = file.OpenReadStream();
        var result = await _storage.UploadAsync(stream, file.FileName, file.ContentType, "images");
        if (!result.Success)
            return ApiResponse<UploadFileResponse>.Fail(result.ErrorMessage ?? "上传失败", 5001);

        return ApiResponse<UploadFileResponse>.Ok(new UploadFileResponse
        {
            Url = result.Url,
            Key = result.Key,
            Size = result.Size
        });
    }

    /// <summary>通用文件上传</summary>
    [HttpPost("file")]
    public async Task<ApiResponse<UploadFileResponse>> UploadFile(IFormFile file, [FromQuery] string folder = "files")
    {
        if (file == null || file.Length == 0)
            return ApiResponse<UploadFileResponse>.Fail("请选择文件", 1001);

        await using var stream = file.OpenReadStream();
        var result = await _storage.UploadAsync(stream, file.FileName, file.ContentType, folder);
        if (!result.Success)
            return ApiResponse<UploadFileResponse>.Fail(result.ErrorMessage ?? "上传失败", 5001);

        return ApiResponse<UploadFileResponse>.Ok(new UploadFileResponse
        {
            Url = result.Url,
            Key = result.Key,
            Size = result.Size
        });
    }
}

public class UploadFileResponse
{
    public string Url { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public long Size { get; set; }
}
