namespace CReed.UUIDv7.Test;

public class TimestampGuidMust
{
    [Fact]
    public void HaveCorrectVersion()
    {
        var guid = TimestampGuid.NextGuid();
        Span<byte> span = stackalloc byte[16];
        guid.TryWriteBytes(span, true, out _);
        Assert.Equal(0x70, span[6] & 0xF0);
    }

    [Fact]
    public void HaveCorrectVariant()
    {
        var guid = TimestampGuid.NextGuid();
        Span<byte> span = stackalloc byte[16];
        guid.TryWriteBytes(span, true, out _);
        Assert.Equal(0b1000_0000, span[8] & 0b1100_0000);
    }

    [Fact]
    public void HaveCorrectTimestamp()
    {
        var now = DateTimeOffset.UtcNow;
        var guid = TimestampGuid.NextGuid();
        var timestamp = TimestampGuid.GetTimestamp(guid);
        Assert.Equal(now, timestamp, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public void RejectInvalidGuids()
    {
        var guid = Guid.NewGuid();
        Assert.False(TimestampGuid.TryGetTimestamp(guid, out _));
        Assert.Null(TimestampGuid.TryGetTimestamp(guid));
        Assert.Throws<ArgumentException>(() => TimestampGuid.GetTimestamp(guid));
    }

    [Fact]
    public void BeMonotonic()
    {
        var prev = TimestampGuid.NextGuid();
        for (var i = 0; i < 1_000_000; i++)
        {
            var current = TimestampGuid.NextGuid();
            if (prev >= current)
            {
                Assert.Fail($"Generated Guids must be greater than those previously generated.\nFirst:  {prev}\nSecond: {current}");
            }

            prev = current;
        }
    }
}
