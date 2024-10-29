namespace CReed;

public interface IHashGuid
{
    Guid NewGuid(Guid prefix, Stream data);
    ValueTask<Guid> NewGuidAsync(Guid prefix, Stream data, CancellationToken token = default);
}
