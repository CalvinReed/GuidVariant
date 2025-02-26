namespace CReed;

public interface IHashGuid
{
    /// <summary>
    /// Generates a namespace-based Guid, typically by concatenating the namespace with the given data and computing
    /// a hash from the result.
    /// </summary>
    /// <param name="prefix">The namespace</param>
    /// <param name="data">The data</param>
    /// <returns>A new GUID object</returns>
    Guid NewGuid(Guid prefix, Stream data);

    /// <inheritdoc cref="NewGuid"/>
    ValueTask<Guid> NewGuidAsync(Guid prefix, Stream data, CancellationToken token = default);
}
