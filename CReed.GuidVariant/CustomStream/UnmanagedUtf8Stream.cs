namespace CReed.CustomStream;

internal unsafe class UnmanagedUtf8Stream(char* ptr, int length) : ReadOnlyStream
{
    private int charsReadTotal;

    public override int Read(Span<byte> buffer)
    {
        var chars = new ReadOnlySpan<char>(ptr, length);
        var (charsRead, bytesWritten) = Utf8Util.ConvertChars(chars[charsReadTotal..], buffer);
        charsReadTotal += charsRead;
        return bytesWritten;
    }
}
