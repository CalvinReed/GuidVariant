using System.Text.Unicode;

namespace CReed;

internal sealed class StringShim(Guid prefix, ReadOnlyMemory<char> data) : Shim(prefix)
{
    private int charsReadTotal;

    public override int Read(Span<byte> buffer)
    {
        var prefixBytesRead = ReadPrefix(buffer);
        if (prefixBytesRead != 0)
        {
            return prefixBytesRead;
        }

        Utf8.FromUtf16(data.Span[charsReadTotal..], buffer, out var charsRead, out var bytesWritten);
        charsReadTotal += charsRead;
        return bytesWritten;
    }
}
