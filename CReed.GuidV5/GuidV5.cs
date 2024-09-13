using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace CReed;

public static class GuidV5
{
    [Pure]
    public static Guid NewGuid(Guid @namespace, ReadOnlyMemory<byte> data)
    {
        Span<byte> hash = stackalloc byte[20];
        using var stream = new ShimStream(@namespace, data);
        SHA1.HashData(stream, hash);
        hash[6] = (byte)(hash[6] & 0x0F | 0x50);
        hash[8] = (byte)(hash[8] & 0x3F | 0x80);
        return new Guid(hash[..16], true);
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

    private sealed class ShimStream(Guid @namespace, ReadOnlyMemory<byte> data) : Stream
    {
        private int bytesRead;

        public override int Read(Span<byte> buffer)
        {
            if (bytesRead != 0)
            {
                return ReadInternal(buffer);
            }

            if (@namespace.TryWriteBytes(buffer, true, out _))
            {
                return 16 + ReadInternal(buffer[16..]);
            }

            throw new UnreachableException();
        }

        private int ReadInternal(Span<byte> buffer)
        {
            var end = Math.Min(data.Length, bytesRead + buffer.Length);
            var slice = data.Span[bytesRead..end];
            slice.CopyTo(buffer);
            bytesRead = end;
            return slice.Length;
        }

        public override void Flush() => throw new UnreachableException();
        public override int Read(byte[] buffer, int offset, int count) => throw new UnreachableException();
        public override long Seek(long offset, SeekOrigin origin) => throw new UnreachableException();
        public override void SetLength(long value) => throw new UnreachableException();
        public override void Write(byte[] buffer, int offset, int count) => throw new UnreachableException();
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new UnreachableException();
        public override long Position { get => throw new UnreachableException(); set => throw new UnreachableException(); }
    }
}
