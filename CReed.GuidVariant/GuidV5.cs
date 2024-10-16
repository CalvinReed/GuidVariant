using System.Security.Cryptography;

namespace CReed;

public sealed class GuidV5(Guid prefix) : HashGuid(prefix)
{
    protected override int Version => 0x50;

    protected override void HashData(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        SHA1.HashData(source, destination);
    }

    protected override void HashData(Stream source, Span<byte> destination)
    {
        SHA1.HashData(source, destination);
    }

    protected override ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken token)
    {
        return SHA1.HashDataAsync(source, token);
    }
}
