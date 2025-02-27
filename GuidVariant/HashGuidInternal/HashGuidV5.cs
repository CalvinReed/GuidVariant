using System.Security.Cryptography;

namespace GuidVariant.HashGuidInternal;

internal sealed class HashGuidV5 : HashGuidBase
{
    protected override int Version => 0x50;

    protected override void HashData(Stream source, Span<byte> destination)
    {
        SHA1.HashData(source, destination);
    }

    protected override ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token)
    {
        return SHA1.HashDataAsync(source, token);
    }
}
