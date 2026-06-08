using Pindou.Application.Common;

namespace Pindou.Tests.Common;

public class ApiResponseTests
{
    [Fact]
    public void Ok_WithData_ReturnsCodeZero()
    {
        var resp = ApiResponse<object>.Ok(new { x = 1 });
        Assert.Equal(0, resp.Code);
        Assert.NotNull(resp.Data);
    }

    [Fact]
    public void Ok_WithMessage_IncludesMessage()
    {
        var resp = ApiResponse<string>.Ok("ok", "操作成功");
        Assert.Equal("操作成功", resp.Message);
        Assert.Equal("ok", resp.Data);
    }

    [Fact]
    public void Ok_DefaultMessage_IsSuccess()
    {
        var resp = ApiResponse<int>.Ok(42);
        Assert.Equal("success", resp.Message);
    }

    [Fact]
    public void Fail_ReturnsError()
    {
        var resp = ApiResponse<string>.Fail("服务器错误", 500);
        Assert.Equal(500, resp.Code);
        Assert.Equal("服务器错误", resp.Message);
        Assert.Null(resp.Data);
    }

    [Fact]
    public void Fail_DefaultCode_IsMinusOne()
    {
        var resp = ApiResponse<int>.Fail("失败");
        Assert.Equal(-1, resp.Code);
    }

    [Fact]
    public void Timestamp_IsRecent()
    {
        var before = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var resp = ApiResponse<int>.Ok(1);
        var after = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Assert.InRange(resp.Timestamp, before, after);
    }

    [Fact]
    public void NonGeneric_Ok()
    {
        var resp = ApiResponse.Ok("ok");
        Assert.Equal(0, resp.Code);
    }

    [Fact]
    public void NonGeneric_Fail()
    {
        var resp = ApiResponse.Fail("x", 100);
        Assert.Equal(100, resp.Code);
        Assert.Equal("x", resp.Message);
    }
}
