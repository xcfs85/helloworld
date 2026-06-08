using Pindou.Shared.Utilities;

namespace Pindou.Tests.Utilities;

public class OrderNoHelperTests
{
    [Fact]
    public void Generate_DefaultPrefix_IsPD()
    {
        var orderNo = OrderNoHelper.Generate();
        Assert.StartsWith("PD" + DateTime.Now.ToString("yyyyMMddHHmmss"), orderNo);
    }

    [Fact]
    public void Generate_CustomPrefix_Respected()
    {
        var orderNo = OrderNoHelper.Generate("PD");
        Assert.StartsWith("PD", orderNo);
    }

    [Fact]
    public void Generate_UniqueValues_OverManyCalls()
    {
        var set = new HashSet<string>();
        for (var i = 0; i < 100; i++) set.Add(OrderNoHelper.Generate());
        Assert.Equal(100, set.Count);
    }

    [Fact]
    public void Generate_LengthIsReasonable()
    {
        // 2 (prefix) + 14 (yyyyMMddHHmmss) + 6 (random) = 22
        var orderNo = OrderNoHelper.Generate("PD");
        Assert.Equal(22, orderNo.Length);
    }

    [Fact]
    public void Generate_TrailingSixDigits_AllNumeric()
    {
        var orderNo = OrderNoHelper.Generate("PD");
        var tail = orderNo.Substring(orderNo.Length - 6);
        Assert.Matches("^[0-9]{6}$", tail);
    }
}
