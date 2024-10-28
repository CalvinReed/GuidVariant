using System.Buffers;
using System.Text.Unicode;

namespace CReed.HashGuidInternal;

internal sealed class StringShim(Guid prefix, ReadOnlyMemory<char> data) : Shim(prefix)
{
    private int charsReadTotal;

    protected override int ReadData(Span<byte> buffer)
    {
        var operationStatus = Utf8.FromUtf16(
            data.Span[charsReadTotal..],
            buffer,
            out var charsRead,
            out var bytesWritten,
            false);
        if (operationStatus == OperationStatus.InvalidData)
        {
            throw new ArgumentException("Input has invalid byte sequences.");
        }

        charsReadTotal += charsRead;
        return bytesWritten;
    }
}
