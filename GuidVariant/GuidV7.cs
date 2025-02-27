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
        Span<byte> span = stackalloc byte[16];
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        BinaryPrimitives.WriteInt64BigEndian(span, timestamp << 16);
        RandomNumberGenerator.Fill(span[6..]);
        span[6] = (byte)(span[6] & 0x0F | 0x70);
        span[8] = (byte)(span[8] & 0x3F | 0x80);
        return new Guid(span, true);
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
        const int counterMax = 0x1000;
        Span<byte> buffer = stackalloc byte[16];
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long counter = RandomNumberGenerator.GetInt32(counterMax);
        foreach (ref var guid in span)
        {
            BinaryPrimitives.WriteInt64BigEndian(buffer, timestamp << 16 | 0x7000 | counter);
            RandomNumberGenerator.Fill(buffer[8..]);
            buffer[8] = (byte)(buffer[8] & 0x3F | 0x80);
            guid = new Guid(buffer, true);
            if (++counter < counterMax) continue;
            timestamp++;
            counter = RandomNumberGenerator.GetInt32(counterMax);
        }
    }
}
