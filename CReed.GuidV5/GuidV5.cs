using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace CReed;

public static class GuidV5
{
    public static Guid NewGuid(Guid prefix, Stream data)
    {
        Span<byte> hash = stackalloc byte[20];
        using var shim = new StreamShim(prefix, data);
        SHA1.HashData(shim, hash);
        WriteMarkers(hash);
        return new Guid(hash[..16], true);
    }

    public static async ValueTask<Guid> NewGuidAsync(Guid prefix, Stream data)
    {
        await using var shim = new StreamShim(prefix, data);
        var hash = await SHA1.HashDataAsync(shim);
        WriteMarkers(hash);
        return new Guid(hash.AsSpan(..16), true);
    }

    [Pure]
    public static Guid NewGuid(Guid prefix, ReadOnlyMemory<byte> data)
    {
        Span<byte> hash = stackalloc byte[20];
        using var shim = new MemoryShim(prefix, data);
        SHA1.HashData(shim, hash);
        WriteMarkers(hash);
        return new Guid(hash[..16], true);
    }

    [Pure]
    public static Guid NewGuid(Guid prefix, ReadOnlySpan<byte> data)
    {
        const int stackAllocMax = 0x400;
        var totalLength = data.Length + 16;
        using var owner = totalLength > stackAllocMax
            ? MemoryPool<byte>.Shared.Rent(totalLength)
            : null;
        var buffer = owner is not null
            ? owner.Memory.Span
            : stackalloc byte[stackAllocMax];
        prefix.TryWriteBytes(buffer, true, out _);
        data.CopyTo(buffer[16..]);
        return NewGuid(buffer[..totalLength]);
    }

    [Pure]
    public static Guid NewGuid(ReadOnlySpan<byte> data)
    {
        Span<byte> hash = stackalloc byte[20];
        SHA1.HashData(data, hash);
        WriteMarkers(hash);
        return new Guid(hash[..16], true);
    }

    private static void WriteMarkers(Span<byte> hash)
    {
        hash[6] = (byte)(hash[6] & 0x0F | 0x50);
        hash[8] = (byte)(hash[8] & 0x3F | 0x80);
    }

    private sealed class StreamShim(Guid prefix, Stream data) : Shim(prefix)
    {
        public override int Read(Span<byte> buffer)
        {
            var prefixBytesRead = base.Read(buffer);
            if (prefixBytesRead != 0)
            {
                return prefixBytesRead;
            }

            return data.Read(buffer);
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var prefixBytesRead = base.Read(buffer.Span);
            if (prefixBytesRead != 0)
            {
                return prefixBytesRead;
            }

            return await data.ReadAsync(buffer, cancellationToken);
        }
    }

    private sealed class MemoryShim(Guid prefix, ReadOnlyMemory<byte> data) : Shim(prefix)
    {
        private int bytesRead;

        public override int Read(Span<byte> buffer)
        {
            var prefixBytesRead = base.Read(buffer);
            if (prefixBytesRead != 0)
            {
                return prefixBytesRead;
            }

            var end = Math.Min(data.Length, bytesRead + buffer.Length);
            var slice = data.Span[bytesRead..end];
            slice.CopyTo(buffer);
            bytesRead = end;
            return slice.Length;
        }
    }

    private abstract class Shim(Guid prefix) : Stream
    {
        private bool prefixRead;

        public override int Read(Span<byte> buffer)
        {
            if (prefixRead)
            {
                return 0;
            }

            if (prefix.TryWriteBytes(buffer, true, out var bytesWritten))
            {
                prefixRead = true;
                return bytesWritten;
            }

            throw new UnreachableException();
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
