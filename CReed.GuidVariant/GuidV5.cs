using System.Security.Cryptography;

namespace CReed;

internal sealed class GuidV5 : HashGuid
{
    private protected override int Version => 0x50;

    private protected override void HashData(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        SHA1.HashData(source, destination);
    }

    private protected override void HashData(Stream source, Span<byte> destination)
    {
        SHA1.HashData(source, destination);
    }

    private protected override ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token)
    {
        return SHA1.HashDataAsync(source, token);
    }
}
