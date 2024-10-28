using CReed.HashGuidInternal;

namespace CReed;

public static class HashGuid
{
    public static IHashGuid V3 { get; } = new HashGuidV3();
    public static IHashGuid V5 { get; } = new HashGuidV5();
    public static IHashGuid Sha256 { get; } = new HashGuidSha256();
}
