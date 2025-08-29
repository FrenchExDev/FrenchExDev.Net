namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Provides a mechanism to compare two <see cref="SemanticModuleVersion"/> objects for equality  based on their
/// semantic version components.
/// </summary>
/// <remarks>This comparer considers two <see cref="SemanticModuleVersion"/> objects equal if their  <c>Major</c>,
/// <c>Minor</c>, <c>Patch</c>, <c>PreRelease</c>, and <c>BuildMetadata</c>  components are all equal. It is designed to
/// be used in scenarios where semantic versioning  equality is required, such as in collections or
/// dictionaries.</remarks>
public class SemanticModuleVersionComparer : IEqualityComparer<SemanticModuleVersion>
{
    /// <summary>
    /// Compares two <see cref="SemanticModuleVersion"/> instances for equality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(SemanticModuleVersion? x, SemanticModuleVersion? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);

        if (x is null && y is null) return true;
        if (x is null || y is null) return false;

        return x.Major == y.Major && x.Minor == y.Minor && x.Patch == y.Patch && x.PreRelease == y.PreRelease && x.BuildMetadata == y.BuildMetadata;
    }

    /// <summary>
    /// Computes a hash code for the specified <see cref="SemanticModuleVersion"/> instance.
    /// </summary>
    /// <remarks>The hash code is computed based on the values of the <see cref="SemanticModuleVersion"/>
    /// properties: <c>Major</c>, <c>Minor</c>, <c>Patch</c>, <c>PreRelease</c>, and <c>BuildMetadata</c>.</remarks>
    /// <param name="obj">The <see cref="SemanticModuleVersion"/> instance for which to compute the hash code.</param>
    /// <returns>A 32-bit signed integer hash code that represents the specified <see cref="SemanticModuleVersion"/>.</returns>
    public int GetHashCode(SemanticModuleVersion obj)
    {
        return HashCode.Combine(obj.Major, obj.Minor, obj.Patch, obj.PreRelease, obj.BuildMetadata);
    }
}