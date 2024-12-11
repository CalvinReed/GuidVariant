using System.Buffers;
using System.Text.Unicode;

namespace CReed.CustomStream;

internal unsafe class UnmanagedUtf8Stream(char* ptr, int length) : ReadOnlyStream
{
    private int charsReadTotal;

    public override int Read(Span<byte> buffer)
    {
        var chars = new ReadOnlySpan<char>(ptr, length);
        var status = Utf8.FromUtf16(
            chars[charsReadTotal..],
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
