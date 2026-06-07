using Pindou.Application.Common;

namespace Pindou.Tests.Common;

public class PagedResultTests
{
    [Fact]
    public void Default_EmptyList()
    {
        var result = new PagedResult<string>();
        Assert.NotNull(result.List);
        Assert.Empty(result.List);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public void PageRequest_DefaultValues()
    {
        var req = new PageRequest();
        Assert.Equal(1, req.Page);
        Assert.Equal(20, req.Size);
        Assert.Equal("desc", req.OrderDir);
    }

    [Fact]
    public void PageRequest_CustomValues()
    {
        var req = new PageRequest { Page = 3, Size = 50, OrderBy = "CreateTime", OrderDir = "asc" };
        Assert.Equal(3, req.Page);
        Assert.Equal(50, req.Size);
        Assert.Equal("CreateTime", req.OrderBy);
        Assert.Equal("asc", req.OrderDir);
    }

    [Fact]
    public void QueryRequest_InheritsPageRequest()
    {
        var q = new TestQuery { Page = 5, Keyword = "x" };
        Assert.Equal(5, q.Page);
        Assert.Equal(20, q.Size); // inherited
        Assert.Equal("x", q.Keyword);
    }

    [Fact]
    public void PagedResult_AssignValues()
    {
        var r = new PagedResult<int> { Page = 1, Size = 10, Total = 100 };
        r.List.AddRange(new[] { 1, 2, 3 });
        Assert.Equal(100, r.Total);
        Assert.Equal(3, r.List.Count);
    }

    private class TestQuery : QueryRequest
    {
    }
}
