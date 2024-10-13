namespace CReed;

internal sealed class MemoryShim(Guid prefix, ReadOnlyMemory<byte> data) : PrefixShim(prefix)
{
    private int bytesRead;

    public override int Read(Span<byte> buffer)
    {
        var prefixBytesRead = base.Read(buffer);
        var destination = buffer[prefixBytesRead..];
        var end = Math.Min(data.Length, bytesRead + destination.Length);
        var slice = data.Span[bytesRead..end];
        slice.CopyTo(destination);
        bytesRead = end;
        return prefixBytesRead + slice.Length;
    }
}
