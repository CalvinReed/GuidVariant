namespace CReed;

internal sealed class MemoryShim(Guid prefix, ReadOnlyMemory<byte> data) : Shim(prefix)
{
    private int start;

    protected override int ReadData(Span<byte> buffer)
    {
        var end = Math.Min(data.Length, start + buffer.Length);
        var slice = data.Span[start..end];
        slice.CopyTo(buffer);
        start = end;
        return slice.Length;
    }
}
