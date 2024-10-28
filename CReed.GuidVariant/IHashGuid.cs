using System.Diagnostics.Contracts;

namespace CReed;

public interface IHashGuid
{
    [Pure]
    Guid NewGuid(ReadOnlySpan<byte> data);

    [Pure]
    Guid NewGuid(Guid prefix, ReadOnlyMemory<byte> data);

    [Pure]
    Guid NewGuid(Guid prefix, ReadOnlyMemory<char> data);

    Guid NewGuid(Guid prefix, Stream data);

    ValueTask<Guid> NewGuidAsync(Guid prefix, Stream data, CancellationToken token = default);
}
