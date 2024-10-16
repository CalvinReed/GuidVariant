using System.Security.Cryptography;

namespace CReed;

internal sealed class GuidV256 : HashGuid
{
    protected override int Version => 0x80;

    protected override void HashData(Stream source, Span<byte> destination)
    {
        SHA256.HashData(source, destination);
    }

    protected override ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token)
    {
        return SHA256.HashDataAsync(source, token);
    }
}
