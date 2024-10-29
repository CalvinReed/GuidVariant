using System.Buffers;
using System.Text.Unicode;

namespace CReed.CustomStream;

internal sealed class Utf8Stream(ReadOnlyMemory<char> chars) : ReadOnlyStream
{
    private int charsReadTotal;

    public override int Read(Span<byte> buffer)
    {
        var status = Utf8.FromUtf16(
            chars.Span[charsReadTotal..],
            buffer,
            out var charsRead,
            out var bytesWritten,
            false);
        if (status == OperationStatus.InvalidData)
        {
            throw new ArgumentException("Input has an invalid byte sequence.");
        }

        charsReadTotal += charsRead;
        return bytesWritten;
    }
}
