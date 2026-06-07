using System.Text;
using System.Text.Json;

namespace Pindou.Shared.Utilities;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, DefaultOptions);
    }

    public static T? Deserialize<T>(string json)
    {
        if (string.IsNullOrEmpty(json)) return default;
        return JsonSerializer.Deserialize<T>(json, DefaultOptions);
    }

    public static string ToJson<T>(this T value) => Serialize(value);

    public static T? FromJson<T>(this string json) => Deserialize<T>(json);
}

public static class SnowflakeId
{
    private static readonly object _lock = new();
    private static long _lastTimestamp = -1L;
    private static long _sequence = 0L;
    private const long Twepoch = 1700000000000L; // 2023-11-14
    private const int WorkerIdBits = 5;
    private const int DatacenterIdBits = 5;
    private const int SequenceBits = 12;
    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
    private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);
    private const int WorkerIdShift = SequenceBits;
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
    private static readonly long WorkerId = 1L;
    private static readonly long DatacenterId = 1L;

    public static long NextId()
    {
        lock (_lock)
        {
            var timestamp = GetCurrentTimestamp();
            if (timestamp < _lastTimestamp) throw new Exception("时间回拨");
            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0) timestamp = WaitNextMillis(_lastTimestamp);
            }
            else
            {
                _sequence = 0L;
            }
            _lastTimestamp = timestamp;
            return ((timestamp - Twepoch) << TimestampLeftShift)
                | (DatacenterId << DatacenterIdShift)
                | (WorkerId << WorkerIdShift)
                | _sequence;
        }
    }

    private static long GetCurrentTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    private static long WaitNextMillis(long lastTimestamp)
    {
        var timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp) timestamp = GetCurrentTimestamp();
        return timestamp;
    }
}

public static class OrderNoHelper
{
    /// <summary>生成订单号: 时间戳+随机6位</summary>
    public static string Generate(string prefix = "PD")
    {
        return $"{prefix}{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(100000, 999999)}";
    }
}
