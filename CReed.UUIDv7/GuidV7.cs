using System.Buffers.Binary;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace CReed;

public static class GuidV7
{
    [ThreadStatic] private static Factory? factory;

    [Pure]
    public static Guid NextGuid()
    {
        factory ??= new Factory();
        return factory.Next();
    }

    [Pure]
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
