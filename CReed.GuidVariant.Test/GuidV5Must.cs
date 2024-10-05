namespace CReed.Test;

public class GuidV5Must
{
    [Fact]
    public void BeDeterministic()
    {
        var prefix = Guid.NewGuid();
        var data = new byte[0x10_0000];
        Random.Shared.NextBytes(data);
        var guid1 = GuidV5.NewGuid(prefix, data);

        using var dataStream = new MemoryStream(data);
        var guid2 = GuidV5.NewGuid(prefix, dataStream);

        var dataFull = new byte[16 + data.Length];
        prefix.TryWriteBytes(dataFull, true, out _);
        data.CopyTo(dataFull.AsSpan(16));
        var guid3 = GuidV5.NewGuid(dataFull);

        Assert.Equal(guid1, guid2);
        Assert.Equal(guid1, guid3);
    }
}
