using CReed.CustomStream;

namespace CReed.HashGuidInternal;

internal abstract class HashGuidBase : IHashGuid
{
    private const int MaxHashSize = 32;

    protected abstract int Version { get; }

    public Guid NewGuid(Guid prefix, Stream data)
    {
        using var prefixStream = new PrefixStream(prefix, data);
        Span<byte> hash = stackalloc byte[MaxHashSize];
        HashData(prefixStream, hash);
        return YieldGuid(hash);
    }

    public async ValueTask<Guid> NewGuidAsync(Guid prefix, Stream data, CancellationToken token = default)
    {
        await using var prefixStream = new PrefixStream(prefix, data);
        var hash = await HashDataAsync(prefixStream, token);
        return YieldGuid(hash);
    }

    protected abstract void HashData(Stream source, Span<byte> destination);

    protected abstract ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token);

    private Guid YieldGuid(Span<byte> hash)
    {
        hash[6] = (byte)(hash[6] & 0x0F | Version);
        hash[8] = (byte)(hash[8] & 0x3F | 0x80);
        return new Guid(hash[..16], true);
    }
}
