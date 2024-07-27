using System.Buffers.Binary;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace CReed;

public static class GuidV7
{
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

    [Pure]
    public static Guid[] NewGuidBatch(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0) return [];
        var batch = new Guid[count];
        NewGuidBatch(batch);
        return batch;
    }

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
