namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Logic to compare two MajorMinorPatchModuleVersion instances for equality.
/// </summary>
public class MajorMinorPatchModuleVersionComparer : IEqualityComparer<MajorMinorPatchModuleVersion>
{
    /// <summary>
    /// Compares two MajorMinorPatchModuleVersion instances for equality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(MajorMinorPatchModuleVersion? x, MajorMinorPatchModuleVersion? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);

        if (x is null && y is null) return true;
        if (x is null || y is null) return false;

        return x.Major == y.Major && x.Minor == y.Minor && x.Patch == y.Patch;
    }

    /// <summary>
    /// Computes a hash code for the specified <see cref="MajorMinorPatchModuleVersion"/> instance.
    /// </summary>
    /// <param name="obj">The <see cref="MajorMinorPatchModuleVersion"/> instance for which to compute the hash code.</param>
    /// <returns>A 32-bit signed integer hash code that represents the specified object.</returns>
    public int GetHashCode(MajorMinorPatchModuleVersion obj)
    {
        return HashCode.Combine(obj.Major, obj.Minor, obj.Patch);
    }
}
