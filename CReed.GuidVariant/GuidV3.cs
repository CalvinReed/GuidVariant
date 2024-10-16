using System.Security.Cryptography;

namespace CReed;

internal sealed class GuidV3 : HashGuid
{
    protected override int Version => 0x30;

    protected override void HashData(Stream source, Span<byte> destination)
    {
        MD5.HashData(source, destination);
    }

    protected override ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token)
    {
        return MD5.HashDataAsync(source, token);
    }
}
