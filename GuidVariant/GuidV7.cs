using System.Buffers.Binary;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace GuidVariant;

/// <summary>
/// Methods for generating timestamp-based Guids.
/// </summary>
public static class GuidV7
{
    /// <summary>
    /// Generates a new, timestamp-based <see cref="Guid"/>.
    /// </summary>
    /// <remarks>
    /// Thread-safe. This method does NOT generate Guids monotonically.
    /// </remarks>
    [Pure]
    public static Guid NewGuid()
    {
#if NET9_0_OR_GREATER
        return Guid.CreateVersion7();
#else
        Span<byte> span = stackalloc byte[16];
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        BinaryPrimitives.WriteInt64BigEndian(span, timestamp << 16);
        RandomNumberGenerator.Fill(span[6..]);
        span[6] = (byte)(span[6] & 0x0F | 0x70);
        span[8] = (byte)(span[8] & 0x3F | 0x80);
        return new Guid(span, true);
#endif
    }

    /// <summary>
    /// Generates a batch of sequential, timestamp-based GUIDs into the provided span.
    /// </summary>
    /// <remarks>
    /// Thread-safe.
    /// Monotonicity is guaranteed through a randomly-seeded counter on a per-batch basis.
    /// This counter may overflow into the timestamp.
    /// </remarks>
    public static void NewGuidBatch(Span<Guid> span)
    {
        if (span.IsEmpty) return;
        span[0] = NewGuid();
        for (var i = 1; i < span.Length; i++)
        {
            span[i] = Increment(span[i - 1]);
        }
    }

    private static Guid Increment(Guid guid)
    {
        Span<byte> span = stackalloc byte[16];
        guid.TryWriteBytes(span, true, out _);
        var head = BinaryPrimitives.ReadInt64BigEndian(span);
        var inc = (head & 0x0FFF) == 0x0FFF
            ? (uint)RandomNumberGenerator.GetInt32(0xF001, 0x1_0001)
            : 1;
        BinaryPrimitives.WriteInt64BigEndian(span, head + inc);
        RandomNumberGenerator.Fill(span[8..]);
        span[8] = (byte)(span[8] & 0x3F | 0x80);
        return new Guid(span, true);
    }
}
