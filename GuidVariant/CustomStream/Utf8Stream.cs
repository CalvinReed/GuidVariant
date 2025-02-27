namespace GuidVariant.CustomStream;

internal sealed class Utf8Stream(ReadOnlyMemory<char> chars) : ReadOnlyStream
{
    private int charsReadTotal;

    public override int Read(Span<byte> buffer)
    {
        var (charsRead, bytesWritten) = Utf8Util.ConvertChars(chars.Span[charsReadTotal..], buffer);
        charsReadTotal += charsRead;
        return bytesWritten;
    }
}
