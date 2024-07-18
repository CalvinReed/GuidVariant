namespace CReed.UUIDv7.Test;

public class TimestampGuidMust
{
    private const int Trials = 1_000_000;

    [Fact]
    public void HaveCorrectVersion()
    {
        Span<byte> span = stackalloc byte[16];
        for (var i = 0; i < Trials; i++)
        {
            Assert.True(TimestampGuid.NextGuid().TryWriteBytes(span, true, out _));
            Assert.Equal(0x70, span[6] & 0xF0);
        }
    }

    [Fact]
    public void HaveCorrectVariant()
    {
        Span<byte> span = stackalloc byte[16];
        for (var i = 0; i < Trials; i++)
        {
            Assert.True(TimestampGuid.NextGuid().TryWriteBytes(span, true, out _));
            Assert.Equal(0b1000_0000, span[8] & 0b1100_0000);
        }
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
        var previous = TimestampGuid.NextGuid();
        for (var i = 0; i < Trials; i++)
        {
            var current = TimestampGuid.NextGuid();
            Assert.True(previous < current);
            previous = current;
        }
    }
}
