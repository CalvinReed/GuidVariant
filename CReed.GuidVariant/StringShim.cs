using System.Text.Unicode;

namespace CReed;

internal sealed class StringShim(Guid prefix, ReadOnlyMemory<char> data) : Shim(prefix)
{
    private int charsReadTotal;

    protected override int ReadData(Span<byte> buffer)
    {
        Utf8.FromUtf16(data.Span[charsReadTotal..], buffer, out var charsRead, out var bytesWritten);
        charsReadTotal += charsRead;
        return bytesWritten;
    }
}
