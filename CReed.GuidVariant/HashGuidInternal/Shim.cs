namespace CReed.HashGuidInternal;

internal abstract class Shim(Guid prefix) : Stream
{
    private bool prefixRead;

    public sealed override int Read(Span<byte> buffer)
    {
        var prefixBytesRead = ReadPrefix(buffer);
        var dataBytesRead = ReadData(buffer[prefixBytesRead..]);
        return prefixBytesRead + dataBytesRead;
    }

    public sealed override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var prefixBytesRead = ReadPrefix(buffer.Span);
        var dataBytesRead = await ReadDataAsync(buffer[prefixBytesRead..], cancellationToken);
        return prefixBytesRead + dataBytesRead;
    }

    protected abstract int ReadData(Span<byte> buffer);

    protected virtual ValueTask<int> ReadDataAsync(Memory<byte> buffer, CancellationToken token)
    {
        return ValueTask.FromResult(ReadData(buffer.Span));
    }

    private int ReadPrefix(Span<byte> buffer)
    {
        if (prefixRead) return 0;
        prefixRead = prefix.TryWriteBytes(buffer, true, out var bytesWritten);
        if (prefixRead) return bytesWritten;
        throw new ArgumentException("Buffer is too short for prefix GUID", nameof(buffer));
    }

    public override void Flush() { }
    public sealed override int Read(byte[] buffer, int offset, int count) => Read(buffer.AsSpan(offset, count));
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public sealed override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
}
