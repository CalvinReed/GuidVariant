using System.Security.Cryptography;

namespace CReed.HashGuidInternal;

internal sealed class HashGuidV3 : HashGuidBase
{
    protected override int Version => 0x30;

    protected override void HashData(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        MD5.HashData(source, destination);
    }

    protected override void HashData(Stream source, Span<byte> destination)
    {
        MD5.HashData(source, destination);
    }

    protected override ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token)
    {
        return MD5.HashDataAsync(source, token);
    }
}
