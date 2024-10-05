namespace CReed.Test;

public class GuidV5Must
{
    [Theory, MemberData(nameof(GenerateInputs))]
    public void BeDeterministic(Guid prefix, byte[] data)
    {
        var first = GuidV5.NewGuid(prefix, data);
        var second = GuidV5.NewGuid(prefix, data);
        Assert.Equal(first, second);
    }

    [Theory, MemberData(nameof(GenerateInputs))]
    public void BeConsistent(Guid prefix, byte[] data)
    {
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

    public static IEnumerable<object[]> GenerateInputs()
    {
        yield return [Guid.Empty, Array.Empty<byte>()];

        var data = new byte[0x1_0000];
        Random.Shared.NextBytes(data);
        yield return [Guid.NewGuid(), data];
    }
}
