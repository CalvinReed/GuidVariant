namespace CReed.CustomStream;

internal abstract class ReadOnlyStream : Stream
{
    public abstract override int Read(Span<byte> buffer);

    public sealed override int Read(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        return Read(buffer.AsSpan(offset, count));
    }

    public sealed override void Flush() { }
    public sealed override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public sealed override void SetLength(long value) => throw new NotSupportedException();
    public sealed override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public sealed override bool CanRead => true;
    public sealed override bool CanSeek => false;
    public sealed override bool CanWrite => false;
    public sealed override long Length => throw new NotSupportedException();
    public sealed override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
}
