namespace CReed;

internal sealed class MemoryShim(Guid prefix, ReadOnlyMemory<byte> data) : Shim(prefix)
{
    private int bytesRead;

    public override int Read(Span<byte> buffer)
    {
        var prefixBytesRead = ReadPrefix(buffer);
        var destination = buffer[prefixBytesRead..];
        var end = Math.Min(data.Length, bytesRead + destination.Length);
        var slice = data.Span[bytesRead..end];
        slice.CopyTo(destination);
        bytesRead = end;
        return prefixBytesRead + slice.Length;
    }
}
