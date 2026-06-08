using Pindou.Shared.Utilities;

namespace Pindou.Tests.Utilities;

public class SnowflakeIdTests
{
    [Fact]
    public void NextId_ProducesPositiveLong()
    {
        var id = SnowflakeId.NextId();
        Assert.True(id > 0);
    }

    [Fact]
    public void NextId_1000Calls_AllUnique()
    {
        var set = new HashSet<long>();
        for (var i = 0; i < 1000; i++) set.Add(SnowflakeId.NextId());
        Assert.Equal(1000, set.Count);
    }

    [Fact]
    public void NextId_IsRoughlyMonotonicallyIncreasing()
    {
        long prev = SnowflakeId.NextId();
        for (var i = 0; i < 100; i++)
        {
            var cur = SnowflakeId.NextId();
            Assert.True(cur >= prev, $"SnowflakeId went backwards: prev={prev} cur={cur}");
            prev = cur;
        }
    }
}
