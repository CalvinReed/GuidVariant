using Xunit.Abstractions;

namespace CReed.Test;

public class GuidV7Must(ITestOutputHelper output)
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
    public async Task BeMonotonic()
    {
        var previous = GuidV7.NextGuid();
        for (var i = 0; i < Trials; i++)
        {
            var current = GuidV7.NextGuid();
            Assert.True(previous < current);
            previous = current;
        }

        for (var i = 0; i < 10; i++)
        {
            await Task.Delay(100);
            var current = GuidV7.NextGuid();
            Assert.True(previous < current);
            previous = current;
        }
    }

    [Fact]
    public async Task LookNice()
    {
        output.WriteLine("[Sequential]");
        for (var i = 0; i < 10; i++)
        {
            var guid = GuidV7.NextGuid();
            output.WriteLine(guid.ToString());
        }

        output.WriteLine(string.Empty);
        output.WriteLine("[Spaced]");
        for (var i = 0; i < 10; i++)
        {
            await Task.Delay(100);
            var guid = GuidV7.NextGuid();
            output.WriteLine(guid.ToString());
        }
    }
}
