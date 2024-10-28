namespace CReed;

public interface IHashGuid
{
    Guid NewGuid(ReadOnlySpan<byte> data);
    Guid NewGuid(Guid prefix, ReadOnlyMemory<byte> data);
    Guid NewGuid(Guid prefix, ReadOnlyMemory<char> data);
    Guid NewGuid(Guid prefix, Stream data);
    ValueTask<Guid> NewGuidAsync(Guid prefix, Stream data, CancellationToken token = default);
}
