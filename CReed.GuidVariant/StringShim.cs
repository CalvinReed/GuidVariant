using System.Text.Unicode;

namespace CReed;

internal sealed class StringShim(Guid prefix, ReadOnlyMemory<char> data) : PrefixShim(prefix)
{
    private int charsReadTotal;

    public override int Read(Span<byte> buffer)
    {
        var prefixBytesRead = base.Read(buffer);
        Utf8.FromUtf16(data.Span[charsReadTotal..], buffer[prefixBytesRead..], out var charsRead, out var bytesWritten);
        charsReadTotal += charsRead;
        return prefixBytesRead + bytesWritten;
    }
}
