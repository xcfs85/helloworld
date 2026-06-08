using Pindou.Application.Common;

namespace Pindou.Tests.Common;

public class BizExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_StoresMessage()
    {
        var ex = new BizException("错误信息");
        Assert.Equal("错误信息", ex.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndCode_StoresBoth()
    {
        var ex = new BizException("用户不存在", 404);
        Assert.Equal("用户不存在", ex.Message);
        Assert.Equal(404, ex.Code);
    }

    [Fact]
    public void IsException_Subsclass()
    {
        var ex = new BizException("x");
        Assert.IsAssignableFrom<Exception>(ex);
    }
}
