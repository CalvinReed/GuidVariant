using System.Security.Cryptography;

namespace CReed;

internal sealed class GuidV3 : HashGuid
{
    private protected override int Version => 0x30;

    private protected override void HashData(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        MD5.HashData(source, destination);
    }

    private protected override void HashData(Stream source, Span<byte> destination)
    {
        MD5.HashData(source, destination);
    }

    private protected override ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token)
    {
        return MD5.HashDataAsync(source, token);
    }
}
