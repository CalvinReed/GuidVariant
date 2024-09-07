using System.Buffers;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace CReed;

public static class GuidV5
{
    public static Guid DnsNamespace { get; } = Guid.ParseExact("6ba7b810-9dad-11d1-80b4-00c04fd430c8", "D");

    public static Guid UrlNamespace { get; } = Guid.ParseExact("6ba7b811-9dad-11d1-80b4-00c04fd430c8", "D");

    public static Guid OidNamespace { get; } = Guid.ParseExact("6ba7b812-9dad-11d1-80b4-00c04fd430c8", "D");

    public static Guid X500Namespace { get; } = Guid.ParseExact("6ba7b814-9dad-11d1-80b4-00c04fd430c8", "D");

    [Pure]
    public static Guid NewGuid(Guid @namespace, ReadOnlySpan<byte> data)
    {
        var totalLength = 16 + data.Length;
        using var owner = MemoryPool<byte>.Shared.Rent(totalLength);
        @namespace.TryWriteBytes(owner.Memory.Span, true, out _);
        data.CopyTo(owner.Memory.Span[16..]);
        return NewGuid(owner.Memory.Span[..totalLength]);
    }

    [Pure]
    public static Guid NewGuid(ReadOnlySpan<byte> data)
    {
        Span<byte> hash = stackalloc byte[20];
        SHA1.HashData(data, hash);
        hash[6] = (byte)(hash[6] & 0x0F | 0x50);
        hash[8] = (byte)(hash[8] & 0x3F | 0x80);
        return new Guid(hash[..16], true);
    }
}
