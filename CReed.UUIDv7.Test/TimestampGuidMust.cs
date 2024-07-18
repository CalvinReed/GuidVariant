using System.Buffers.Binary;

namespace CReed.UUIDv7.Test;

public class TimestampGuidMust
{
    [Fact]
    public void BeMonotonic()
    {
        var prev = TimestampGuid.NextGuid();
        for (var i = 0; i < 1_000_000; i++)
        {
            var current = TimestampGuid.NextGuid();
            Assert.True(prev < current);
        }
    }

    [Fact]
    public void HaveCorrectTimestamp()
    {
        var guid = TimestampGuid.NextGuid();
        var timestamp = TimestampGuid.GetTimestamp(guid);
        Assert.Equal(DateTimeOffset.UtcNow, timestamp, TimeSpan.FromMilliseconds(3));
    }

    [Fact]
    public void HaveCorrectVersion()
    {
        var guid = TimestampGuid.NextGuid();
        Span<byte> span = stackalloc byte[16];
        guid.TryWriteBytes(span, true, out _);
        var front = BinaryPrimitives.ReadInt64BigEndian(span);
        var version = front >>> 12 & 0x0F;
        Assert.Equal(7, version);
    }

    [Fact]
    public void HaveCorrectVariant()
    {
        var guid = TimestampGuid.NextGuid();
        Span<byte> span = stackalloc byte[16];
        guid.TryWriteBytes(span, true, out _);
        var back = BinaryPrimitives.ReadInt64BigEndian(span[8..]);
        var variant = back >>> 62;
        Assert.Equal(0b10, variant);
    }
}
