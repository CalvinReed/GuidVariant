namespace CReed.HashGuidInternal;

internal sealed class StreamShim(Guid prefix, Stream data) : Shim(prefix)
{
    protected override int ReadData(Span<byte> buffer)
    {
        return data.Read(buffer);
    }

    protected override ValueTask<int> ReadDataAsync(Memory<byte> buffer, CancellationToken token)
    {
        return data.ReadAsync(buffer, token);
    }
}
