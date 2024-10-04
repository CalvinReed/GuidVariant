namespace CReed.Test;

public class NewGuidV7BatchMust
{
    [Fact]
    public void HaveCorrectMarkers()
    {
        Span<byte> bytes = stackalloc byte[16];
        Span<Guid> batch = stackalloc Guid[10];
        for (var i = 0; i < 100_000; i++)
        {
            GuidV7.NewGuidBatch(batch);
            foreach (var guid in batch)
            {
                Assert.True(guid.TryWriteBytes(bytes, true, out _));
                Assert.Equal(0x70, bytes[6] & 0xF0); // version
                Assert.Equal(0b1000_0000, bytes[8] & 0b1100_0000); // variant
            }
        }
    }

    [Fact]
    public void BeMonotonic()
    {
        var batch = new Guid[1_000_000];
        GuidV7.NewGuidBatch(batch);
        for (var i = 1; i < batch.Length; i++)
        {
            Assert.True(batch[i - 1] < batch[i]);
        }
    }
}
