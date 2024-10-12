namespace CReed;

internal sealed class MemoryShim(Guid prefix, ReadOnlyMemory<byte> data) : Shim(prefix)
{
    private int bytesRead;

    public override int Read(Span<byte> buffer)
    {
        var prefixBytesRead = ReadPrefix(buffer);
        if (prefixBytesRead != 0)
        {
            return prefixBytesRead;
        }

        var end = Math.Min(data.Length, bytesRead + buffer.Length);
        var slice = data.Span[bytesRead..end];
        slice.CopyTo(buffer);
        bytesRead = end;
        return slice.Length;
    }
}
