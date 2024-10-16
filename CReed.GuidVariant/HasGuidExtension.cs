using System.Diagnostics.Contracts;

namespace CReed;

public static class HasGuidExtension
{
    [Pure]
    public static Guid NewGuid(this HashGuid hashGuid, Guid prefix, string? data)
    {
        return hashGuid.NewGuid(prefix, data.AsMemory());
    }
}
