using System.Security.Cryptography;

namespace CReed.HashGuidInternal;

internal sealed class GuidV256 : HashGuid
{
    private protected override int Version => 0x80;

    private protected override void HashData(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        SHA256.HashData(source, destination);
    }

    private protected override void HashData(Stream source, Span<byte> destination)
    {
        SHA256.HashData(source, destination);
    }

    private protected override ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token)
    {
        return SHA256.HashDataAsync(source, token);
    }
}
