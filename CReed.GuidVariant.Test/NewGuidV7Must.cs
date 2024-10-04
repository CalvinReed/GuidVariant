namespace CReed.Test;

public class NewGuidV7Must
{
    [Fact]
    public void HaveCorrectMarkers()
    {
        Span<byte> span = stackalloc byte[16];
        for (var i = 0; i < 1_000_000; i++)
        {
            Assert.True(GuidV7.NewGuid().TryWriteBytes(span, true, out _));
            Assert.Equal(0x70, span[6] & 0xF0); // version
            Assert.Equal(0b1000_0000, span[8] & 0b1100_0000); // variant
        }
    }

    [Fact]
    public async Task BeMonotonic()
    {
        var previous = GuidV7.NewGuid();
        for (var i = 0; i < 100; i++)
        {
            await Task.Delay(10);
            var current = GuidV7.NewGuid();
            Assert.True(previous < current);
            previous = current;
        }
    }
}
