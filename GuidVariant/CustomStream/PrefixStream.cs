using System.Diagnostics;

namespace GuidVariant.CustomStream;

internal sealed class PrefixStream(Guid prefix, Stream stream) : ReadOnlyStream
{
    private bool prefixRead;

    public override int Read(Span<byte> buffer)
    {
        var prefixBytes = ReadPrefix(buffer);
        var streamBytes = stream.Read(buffer[prefixBytes..]);
        return prefixBytes + streamBytes;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var prefixBytes = ReadPrefix(buffer.Span);
        var streamBytes = await stream.ReadAsync(buffer[prefixBytes..], cancellationToken);
        return prefixBytes + streamBytes;
    }

    private int ReadPrefix(Span<byte> buffer)
    {
        if (prefixRead) return 0;
        prefixRead = prefix.TryWriteBytes(buffer, true, out var bytesWritten);
        if (prefixRead) return bytesWritten;
        throw new UnreachableException();
    }
}
