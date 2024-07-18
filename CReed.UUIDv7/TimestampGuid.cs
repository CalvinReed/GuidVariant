using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace CReed;

public static class TimestampGuid
{
    [ThreadStatic] private static Factory? factory;

    [Pure]
    public static Guid NextGuid()
    {
        factory ??= new Factory();
        return factory.Next();
    }

    [Pure]
    public static DateTimeOffset GetTimestamp(Guid guid)
    {
        if (TryGetTimestamp(guid, out var timestamp))
        {
            return timestamp;
        }

        var message = $"\"{guid}\" is not a valid timestamp Guid.";
        throw new ArgumentException(message, nameof(guid));
    }

    [Pure]
    public static DateTimeOffset? TryGetTimestamp(Guid guid)
    {
        return TryGetTimestamp(guid, out var timestamp)
            ? timestamp
            : null;
    }

    [Pure]
    public static bool TryGetTimestamp(Guid guid, out DateTimeOffset timestamp)
    {
        Span<byte> span = stackalloc byte[16];
        if (!guid.TryWriteBytes(span, true, out _)) throw new UnreachableException();
        var front = BinaryPrimitives.ReadInt64BigEndian(span);
        if ((front & 0xF000) != 0x7000)
        {
            timestamp = default;
            return false;
        }

        timestamp = DateTimeOffset.FromUnixTimeMilliseconds(front >>> 16);
        return true;
    }

    private sealed class Factory
    {
        private const int CounterMax = 0x0FFF;

        private long timestamp;
        private long counter;

        public Guid Next()
        {
            Increment();
            return GenerateGuid();
        }

        private void Increment()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (now > timestamp)
            {
                timestamp = now;
                ResetCounter();
            }
            else if (counter < CounterMax)
            {
                counter++;
            }
            else
            {
                timestamp++;
                ResetCounter();
            }
        }

        private Guid GenerateGuid()
        {
            Span<byte> span = stackalloc byte[16];
            BinaryPrimitives.WriteInt64BigEndian(span, timestamp << 16 | 0x7000 | counter);
            RandomNumberGenerator.Fill(span[8..]);
            span[8] = (byte)(span[8] & 0x3F | 0x80);
            return new Guid(span, true);
        }

        private void ResetCounter()
        {
            counter = RandomNumberGenerator.GetInt32(CounterMax + 1);
        }
    }
}
