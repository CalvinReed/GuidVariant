using System.Diagnostics;

namespace CReed;

internal abstract class PrefixShim(Guid prefix) : Stream
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

    public override long Position
    {
        get => throw new UnreachableException();
        set => throw new UnreachableException();
    }
}
