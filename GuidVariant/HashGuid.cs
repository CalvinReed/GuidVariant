using GuidVariant.HashGuidInternal;

namespace GuidVariant;

/// <summary>
/// Methods for generating namespace-based Guids.
/// </summary>
public static class HashGuid
{
    /// <summary>
    /// Generates namespace-based Guids based on MD5.
    /// </summary>
    public static IHashGuid V3 { get; } = new HashGuidV3();

    /// <summary>
    /// Generates namespace-based Guids based on SHA-1.
    /// </summary>
    public static IHashGuid V5 { get; } = new HashGuidV5();

    /// <summary>
    /// Generates namespace-based Guids based on SHA-256.
    /// </summary>
    public static IHashGuid Sha256 { get; } = new HashGuidSha256();
}
