using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using CReed.CustomStream;

namespace CReed;

public static class HashGuidExtension
{
    /// <inheritdoc cref="NewGuid(CReed.IHashGuid,System.Guid,ReadOnlyMemory{char})"/>
    [Pure]
    public static Guid NewGuid(this IHashGuid hashGuid, Guid prefix, string data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return hashGuid.NewGuid(prefix, data.AsMemory());
    }

    /// <inheritdoc cref="IHashGuid.NewGuid"/>
    /// <remarks>
    /// Characters are treated as UTF-8 bytes
    /// </remarks>
    [Pure]
    public static Guid NewGuid(this IHashGuid hashGuid, Guid prefix, ReadOnlyMemory<char> data)
    {
        using var stream = new Utf8Stream(data);
        return hashGuid.NewGuid(prefix, stream);
    }

    /// <inheritdoc cref="IHashGuid.NewGuid"/>
    [Pure]
    public static unsafe Guid NewGuid(this IHashGuid hashGuid, Guid prefix, ReadOnlySpan<char> data)
    {
        if (data.IsEmpty)
        {
            return hashGuid.NewGuid(prefix, Stream.Null);
        }

        fixed (char* ptr = data)
        {
            using var stream = new UnmanagedUtf8Stream(ptr, data.Length);
            return hashGuid.NewGuid(prefix, stream);
        }
    }

    /// <inheritdoc cref="IHashGuid.NewGuid"/>
    [Pure]
    public static Guid NewGuid(this IHashGuid hashGuid, Guid prefix, byte[] data)
    {
        using var stream = new MemoryStream(data, false);
        return hashGuid.NewGuid(prefix, stream);
    }

    /// <inheritdoc cref="IHashGuid.NewGuid"/>
    [Pure]
    public static Guid NewGuid(this IHashGuid hashGuid, Guid prefix, ReadOnlyMemory<byte> data)
    {
        if (!MemoryMarshal.TryGetArray(data, out var segment))
        {
            return hashGuid.NewGuid(prefix, data.Span);
        }

        using var stream = new MemoryStream(segment.Array!, segment.Offset, segment.Count, false);
        return hashGuid.NewGuid(prefix, stream);
    }

    /// <inheritdoc cref="IHashGuid.NewGuid"/>
    [Pure]
    public static unsafe Guid NewGuid(this IHashGuid hashGuid, Guid prefix, ReadOnlySpan<byte> data)
    {
        // Handle empty spans to avoid null pointer issues when creating UnmanagedMemoryStream
        if (data.IsEmpty)
        {
            return hashGuid.NewGuid(prefix, Stream.Null);
        }

        fixed (byte* ptr = data)
        {
            using var stream = new UnmanagedMemoryStream(ptr, data.Length);
            return hashGuid.NewGuid(prefix, stream);
        }
    }
}
