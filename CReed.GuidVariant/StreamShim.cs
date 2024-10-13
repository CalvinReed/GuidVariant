namespace CReed;

internal sealed class StreamShim(Guid prefix, Stream data) : PrefixShim(prefix)
{
    public override int Read(Span<byte> buffer)
    {
        var prefixBytesRead = base.Read(buffer);
        var dataBytesRead = data.Read(buffer[prefixBytesRead..]);
        return prefixBytesRead + dataBytesRead;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var prefixBytesRead = base.Read(buffer.Span);
        var dataBytesRead = await data.ReadAsync(buffer[prefixBytesRead..], cancellationToken);
        return prefixBytesRead + dataBytesRead;
    }
}
