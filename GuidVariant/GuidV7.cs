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
        return NewGuid(DateTimeOffset.UtcNow);
    }

    /// <inheritdoc cref="NewGuid()"/>
    [Pure]
    public static Guid NewGuid(DateTimeOffset timestamp)
    {
        return Guid.CreateVersion7(timestamp);
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

    public static DateTimeOffset GetTimestamp(this Guid guid)
    {
        if (guid.TryGetTimestamp(out var timestamp))
        {
            return timestamp;
        }

        throw new ArgumentException("Timestamps are only present in version 7 GUIDs.", nameof(guid));
    }

    public static bool TryGetTimestamp(this Guid guid, out DateTimeOffset timestamp)
    {
        if (!guid.IsVersion7())
        {
            timestamp = default;
            return false;
        }

        Span<byte> span = stackalloc byte[16];
        guid.TryWriteBytes(span, true, out _);
        var head = BinaryPrimitives.ReadInt64BigEndian(span);
        try
        {
            timestamp = DateTimeOffset.FromUnixTimeMilliseconds(head >>> 16);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentOutOfRangeException(nameof(guid), guid, "GUID contains timestamp too large for DateTimeOffset.");
        }

        return true;
    }

    public static Guid ToBlank(this Guid guid)
    {
        if (guid.TryToBlank(out var blank))
        {
            return blank;
        }

        throw new ArgumentException("Only version 7 GUIDs can be blanked out", nameof(guid));
    }

    public static bool TryToBlank(this Guid guid, out Guid blank)
    {
        if (!guid.IsVersion7())
        {
            blank = Guid.Empty;
            return false;
        }

        Span<byte> span = stackalloc byte[16];
        guid.TryWriteBytes(span);
        span[6..].Clear();
        span[6] = 0x70;
        span[8] = 0x80;
        blank = new Guid(span);
        return true;
    }

    public static Guid CreateBlank(DateTimeOffset timestamp)
    {
        var ms = timestamp.ToUnixTimeMilliseconds();
        ArgumentOutOfRangeException.ThrowIfNegative(ms, nameof(timestamp));
        Span<byte> span = stackalloc byte[16];
        span.Clear();
        BinaryPrimitives.WriteInt64BigEndian(span, ms << 16);
        span[6] = 0x70;
        span[8] = 0x80;
        return new Guid(span, true);
    }

    private static bool IsVersion7(this Guid guid)
    {
        return guid.Version == 7;
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
