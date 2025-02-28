namespace GuidVariant;

public interface IHashGuid
{
    /// <summary>
    /// Generates a namespace-based Guid, typically by concatenating the given prefix with the given data and computing
    /// a hash from the result.
    /// </summary>
    Guid NewGuid(Guid prefix, Stream data);

    /// <inheritdoc cref="NewGuid"/>
    ValueTask<Guid> NewGuidAsync(Guid prefix, Stream data, CancellationToken token = default);
}
