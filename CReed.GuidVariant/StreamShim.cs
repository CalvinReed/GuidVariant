namespace CReed;

internal sealed class StreamShim(Guid prefix, Stream data) : Shim(prefix)
{
    public override int Read(Span<byte> buffer)
    {
        var prefixBytesRead = ReadPrefix(buffer);
        return prefixBytesRead != 0
            ? prefixBytesRead
            : data.Read(buffer);
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var prefixBytesRead = ReadPrefix(buffer.Span);
        if (prefixBytesRead != 0)
        {
            return prefixBytesRead;
        }

        return await data.ReadAsync(buffer, cancellationToken);
    }
}
