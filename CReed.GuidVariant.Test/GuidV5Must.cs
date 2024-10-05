namespace CReed.Test;

public class GuidV5Must
{
    [Theory, MemberData(nameof(TestVectors))]
    public void BeCorrect(Guid prefix, byte[] data, Guid expected)
    {
        var actual = GuidV5.NewGuid(prefix, data);
        Assert.Equal(expected, actual);
    }

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

    public static TheoryData<Guid, byte[], Guid> TestVectors()
    {
        return new TheoryData<Guid, byte[], Guid>
        {
            // Source: https://datatracker.ietf.org/doc/html/rfc9562#name-example-of-a-uuidv5-value
            {
                Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                "www.example.com"u8.ToArray(),
                Guid.Parse("2ed6657d-e927-568b-95e1-2665a8aea6a2")
            }
        };
    }

    public static TheoryData<Guid, byte[]> GenerateInputs()
    {
        var prefix = Guid.NewGuid();
        var data = new byte[0x1003 - 16];
        Random.Shared.NextBytes(data);
        return new TheoryData<Guid, byte[]>
        {
            { Guid.Empty, [] },
            { prefix, data }
        };
    }
}
