namespace CReed;

internal sealed class StreamShim(Guid prefix, Stream data) : Shim(prefix)
{
    protected override int ReadData(Span<byte> buffer)
    {
        return data.Read(buffer);
    }

    protected override async ValueTask<int> ReadDataAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        return await data.ReadAsync(buffer, cancellationToken);
    }
}
