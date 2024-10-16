using System.Diagnostics.Contracts;

namespace CReed;

public abstract class HashGuid
{
    private const int MaxHashSize = 64;

    public static HashGuid V3 { get; } = new GuidV3();

    public static HashGuid V5 { get; } = new GuidV5();

    public static HashGuid Sha256 { get; } = new GuidV256();

    protected abstract int Version { get; }

    [Pure]
    public Guid NewGuid(Guid prefix, string? data)
    {
        return NewGuid(prefix, data.AsMemory());
    }

    [Pure]
    public Guid NewGuid(Guid prefix, ReadOnlyMemory<char> data)
    {
        using var shim = new StringShim(prefix, data);
        return YieldGuid(shim);
    }

    [Pure]
    public Guid NewGuid(Guid prefix, ReadOnlyMemory<byte> data)
    {
        using var shim = new MemoryShim(prefix, data);
        return YieldGuid(shim);
    }

    public Guid NewGuid(Guid prefix, Stream data)
    {
        using var shim = new StreamShim(prefix, data);
        return YieldGuid(shim);
    }

    public async ValueTask<Guid> NewGuidAsync(Guid prefix, Stream data, CancellationToken token = default)
    {
        await using var shim = new StreamShim(prefix, data);
        var hash = await HashDataAsync(shim, token);
        return YieldGuid(hash);
    }

    protected abstract void HashData(Stream source, Span<byte> destination);

    protected abstract ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token);

    private Guid YieldGuid(Stream stream)
    {
        Span<byte> hash = stackalloc byte[MaxHashSize];
        HashData(stream, hash);
        return YieldGuid(hash);
    }

    private Guid YieldGuid(Span<byte> hash)
    {
        hash[6] = (byte)(hash[6] & 0x0F | Version);
        hash[8] = (byte)(hash[8] & 0x3F | 0x80);
        return new Guid(hash[..16], true);
    }
}
