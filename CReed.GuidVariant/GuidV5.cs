using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace CReed;

public static class GuidV5
{
    public static Guid NewGuid(Guid prefix, Stream data)
    {
        using var shim = new StreamShim(prefix, data);
        return YieldGuid(shim);
    }

    public static async ValueTask<Guid> NewGuidAsync(Guid prefix, Stream data, CancellationToken token = default)
    {
        await using var shim = new StreamShim(prefix, data);
        var hash = await SHA1.HashDataAsync(shim, token);
        return YieldGuid(hash);
    }

    [Pure]
    public static Guid NewGuid(Guid prefix, ReadOnlyMemory<byte> data)
    {
        using var shim = new MemoryShim(prefix, data);
        return YieldGuid(shim);
    }

    [Pure]
    public static Guid NewGuid(Guid prefix, ReadOnlyMemory<char> data)
    {
        using var shim = new StringShim(prefix, data);
        return YieldGuid(shim);
    }

    [Pure]
    public static Guid NewGuid(Guid prefix, string? data)
    {
        return NewGuid(prefix, data.AsMemory());
    }

    [Pure]
    public static Guid NewGuid(ReadOnlySpan<byte> data)
    {
        Span<byte> hash = stackalloc byte[SHA1.HashSizeInBytes];
        SHA1.HashData(data, hash);
        return YieldGuid(hash);
    }

    private static Guid YieldGuid(Stream stream)
    {
        Span<byte> hash = stackalloc byte[SHA1.HashSizeInBytes];
        SHA1.HashData(stream, hash);
        return YieldGuid(hash);
    }

    private static Guid YieldGuid(Span<byte> hash)
    {
        hash[6] = (byte)(hash[6] & 0x0F | 0x50);
        hash[8] = (byte)(hash[8] & 0x3F | 0x80);
        return new Guid(hash[..16], true);
    }
}
