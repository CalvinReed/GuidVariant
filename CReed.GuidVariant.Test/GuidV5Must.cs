namespace CReed.Test;

public class GuidV5Must
{
    [Theory, MemberData(nameof(TestVectors))]
    public void BeCorrect(Guid prefix, string data, Guid expected)
    {
        var guidV5 = new GuidV5(prefix);
        var actual = guidV5.NewGuid(data);
        Assert.Equal(expected, actual);
    }

    [Theory, MemberData(nameof(GenerateInputs))]
    public void BeDeterministic(Guid prefix, byte[] data)
    {
        var guidV5 = new GuidV5(prefix);
        var first = guidV5.NewGuid(data);
        var second = guidV5.NewGuid(data);
        Assert.Equal(first, second);
    }

    [Theory, MemberData(nameof(GenerateInputs))]
    public void BeConsistent(Guid prefix, byte[] data)
    {
        var guidV5 = new GuidV5(prefix);
        var guid1 = guidV5.NewGuid(data);

        using var dataStream = new MemoryStream(data);
        var guid2 = guidV5.NewGuid(dataStream);

        Assert.Equal(guid1, guid2);
    }

    public static TheoryData<Guid, string, Guid> TestVectors()
    {
        return new TheoryData<Guid, string, Guid>
        {
            // Source: https://datatracker.ietf.org/doc/html/rfc9562#name-example-of-a-uuidv5-value
            {
                Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                "www.example.com",
                Guid.Parse("2ed6657d-e927-568b-95e1-2665a8aea6a2")
            },
            // Source: https://www.di-mgt.com.au/sha_testvectors.html ("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq")
            {
                new Guid("abcdbcdecdefdefg"u8, true),
                "efghfghighijhijkijkljklmklmnlmnomnopnopq",
                Guid.Parse("84983e44-1c3b-526e-baae-4aa1f95129e5")
            },
            // Source: https://www.di-mgt.com.au/sha_testvectors.html ("abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmnhijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu")
            {
                new Guid("abcdefghbcdefghi"u8, true),
                "cdefghijdefghijkefghijklfghijklmghijklmnhijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu",
                Guid.Parse("a49b2446-a02c-545b-b419-f995b6709125")
            },
            // Source: https://www.di-mgt.com.au/sha_testvectors.html (1,000,000 repetitions of "a")
            {
                new Guid("aaaaaaaaaaaaaaaa"u8, true),
                new string('a', 1_000_000 - 16),
                Guid.Parse("34aa973c-d4c4-5aa4-b61e-eb2bdbad2731")
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
