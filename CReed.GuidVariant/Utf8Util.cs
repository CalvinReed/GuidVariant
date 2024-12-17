using System.Buffers;
using System.Text.Unicode;

namespace CReed;

internal static class Utf8Util
{
    public static (int charsRead, int bytesWritten) ConvertChars(ReadOnlySpan<char> chars, Span<byte> buffer)
    {
        var status = Utf8.FromUtf16(chars, buffer, out var charsRead, out var bytesWritten, false);
        if (status == OperationStatus.InvalidData)
        {
            throw new ArgumentException("Input has an invalid byte sequence.");
        }

        return (charsRead, bytesWritten);
    }
}
