namespace CReed.Test;

public class GuidV7Must
{
    private const int Trials = 1_000_000;

    [Fact]
    public void HaveCorrectMarkers()
    {
        Span<byte> span = stackalloc byte[16];
        for (var i = 0; i < Trials; i++)
        {
            var guid = GuidV7.NextGuid();
            Assert.True(guid.TryWriteBytes(span, true, out _));
            Assert.Equal(0x70, span[6] & 0xF0); // version
            Assert.Equal(0b1000_0000, span[8] & 0b1100_0000); // variant
        }
    }

    [Fact]
    public void BeMonotonic()
    {
        var previous = GuidV7.NextGuid();
        for (var i = 0; i < Trials; i++)
        {
            var current = GuidV7.NextGuid();
            Assert.True(previous < current);
            previous = current;
        }
    }
}