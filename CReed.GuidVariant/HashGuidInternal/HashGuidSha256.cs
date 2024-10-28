using System.Security.Cryptography;

namespace CReed.HashGuidInternal;

internal sealed class HashGuidSha256 : HashGuidBase
{
    protected override int Version => 0x80;

    protected override void HashData(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        SHA256.HashData(source, destination);
    }

    protected override void HashData(Stream source, Span<byte> destination)
    {
        SHA256.HashData(source, destination);
    }

    protected override ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token)
    {
        return SHA256.HashDataAsync(source, token);
    }
}
