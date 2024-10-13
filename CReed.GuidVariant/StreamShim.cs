namespace CReed;

internal sealed class StreamShim(Guid prefix, Stream data) : Shim(prefix)
{
    public override int Read(Span<byte> buffer)
    {
        var prefixBytesRead = ReadPrefix(buffer);
        var dataBytesRead = data.Read(buffer[prefixBytesRead..]);
        return prefixBytesRead + dataBytesRead;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var prefixBytesRead = ReadPrefix(buffer.Span);
        var dataBytesRead = await data.ReadAsync(buffer[prefixBytesRead..], cancellationToken);
        return prefixBytesRead + dataBytesRead;
    }
}
