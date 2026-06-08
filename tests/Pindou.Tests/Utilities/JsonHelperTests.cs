using Pindou.Shared.Utilities;

namespace Pindou.Tests.Utilities;

public class JsonHelperTests
{
    private record Sample(int Id, string Name, DateTime CreatedAt);

    [Fact]
    public void Serialize_CamelCase_ByDefault()
    {
        var json = JsonHelper.Serialize(new { FirstName = "pindou" });
        Assert.Contains("firstName", json);
    }

    [Fact]
    public void Serialize_NullField_IsSkipped()
    {
        var json = JsonHelper.Serialize(new { A = "x", B = (string?)null });
        Assert.DoesNotContain("\"b\"", json);
    }

    [Fact]
    public void RoundTrip_Primitive()
    {
        var json = JsonHelper.Serialize(42);
        Assert.Equal(42, JsonHelper.Deserialize<int>(json));
    }

    [Fact]
    public void RoundTrip_Object()
    {
        var sample = new Sample(1, "pindou", DateTime.UnixEpoch);
        var json = JsonHelper.Serialize(sample);
        var back = JsonHelper.Deserialize<Sample>(json);
        Assert.NotNull(back);
        Assert.Equal(sample.Id, back!.Id);
        Assert.Equal(sample.Name, back.Name);
    }

    [Fact]
    public void Deserialize_Empty_ReturnsDefault()
    {
        Assert.Null(JsonHelper.Deserialize<Sample>(""));
        Assert.Null(JsonHelper.Deserialize<Sample>(null));
    }

    [Fact]
    public void ToJson_IsAliasForSerialize()
    {
        var sample = new Sample(7, "alias", DateTime.UnixEpoch);
        Assert.Equal(JsonHelper.Serialize(sample), sample.ToJson());
    }

    [Fact]
    public void FromJson_IsAliasForDeserialize()
    {
        var sample = new Sample(7, "alias", DateTime.UnixEpoch);
        var json = sample.ToJson();
        Assert.Equal(sample, json.FromJson<Sample>());
    }
}
