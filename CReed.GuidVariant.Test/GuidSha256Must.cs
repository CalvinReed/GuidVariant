using System.Text;

namespace CReed.Test;

public class GuidSha256Must
{
    [Theory, MemberData(nameof(TestVectors))]
    public void BeCorrect(Guid prefix, byte[] data, Guid expected)
    {
        var actual = GuidSha256.NewGuid(prefix, data);
        Assert.Equal(expected, actual);
    }

    [Theory, MemberData(nameof(GenerateInputs))]
    public void BeDeterministic(Guid prefix, byte[] data)
    {
        var first = GuidSha256.NewGuid(prefix, data);
        var second = GuidSha256.NewGuid(prefix, data);
        Assert.Equal(first, second);
    }

    [Theory, MemberData(nameof(GenerateInputs))]
    public void BeConsistent(Guid prefix, byte[] data)
    {
        var guid1 = GuidSha256.NewGuid(prefix, data);

        using var dataStream = new MemoryStream(data);
        var guid2 = GuidSha256.NewGuid(prefix, dataStream);

        var dataFull = new byte[16 + data.Length];
        prefix.TryWriteBytes(dataFull, true, out _);
        data.CopyTo(dataFull.AsSpan(16));
        var guid3 = GuidSha256.NewGuid(dataFull);

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
                Guid.Parse("5c146b14-3c52-8afd-938a-375d0df1fbf6")
            },
            // Source: https://www.di-mgt.com.au/sha_testvectors.html ("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq")
            {
                new Guid("abcdbcdecdefdefg"u8, true),
                "efghfghighijhijkijkljklmklmnlmnomnopnopq"u8.ToArray(),
                Guid.Parse("248d6a61-d206-88b8-a5c0-26930c3e6039")
            },
            // Source: https://www.di-mgt.com.au/sha_testvectors.html ("abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmnhijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu")
            {
                new Guid("abcdefghbcdefghi"u8, true),
                "cdefghijdefghijkefghijklfghijklmghijklmnhijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu"u8.ToArray(),
                Guid.Parse("cf5b16a7-78af-8380-836c-e59e7b049237")
            },
            // Source: https://www.di-mgt.com.au/sha_testvectors.html (1,000,000 repetitions of "a")
            {
                new Guid("aaaaaaaaaaaaaaaa"u8, true),
                Encoding.UTF8.GetBytes(new string('a', 1_000_000 - 16)),
                Guid.Parse("cdc76e5c-9914-8b92-81a1-c7e284d73e67")
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
