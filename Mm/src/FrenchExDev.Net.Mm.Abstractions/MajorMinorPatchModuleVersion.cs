namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Represents a version number consisting of major, minor, and patch components.
/// </summary>
/// <remarks>This class provides functionality to manage and manipulate version numbers following the semantic
/// versioning format (major.minor.patch). It supports incrementing and decrementing individual version
/// components.</remarks>
public class MajorMinorPatchModuleVersion : IModuleVersion
{
    /// <summary>
    /// Represents the components of a version number.
    /// </summary>
    /// <remarks>This enumeration is typically used to identify and work with specific parts of a version
    /// number, such as major, minor, or patch versions.</remarks>
    public enum Part
    {
        Major,
        Minor,
        Patch
    }

    /// <summary>
    /// Get the major version number.
    /// </summary>
    public int Major { get; protected set; }

    /// <summary>
    /// Gets the minor version number of the object.
    /// </summary>
    public int Minor { get; protected set; }

    /// <summary>
    /// Gets the patch version number of the object.
    /// </summary>
    public int Patch { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MajorMinorPatchModuleVersion"/> class with the specified version
    /// components.
    /// </summary>
    /// <param name="major">The major version number. Typically incremented for breaking changes.</param>
    /// <param name="minor">The minor version number. Typically incremented for backward-compatible feature additions.</param>
    /// <param name="patch">The patch version number. Typically incremented for backward-compatible bug fixes.</param>
    public MajorMinorPatchModuleVersion(int major, int minor, int patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    /// <summary>
    /// Returns a string representation of the version in the format "Major.Minor.Patch".
    /// </summary>
    /// <returns>A string representing the version, formatted as "Major.Minor.Patch".</returns>
    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }

    /// <summary>
    /// Increments the specified part of the version (Major, Minor, or Patch) by one.
    /// </summary>
    /// <param name="part"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Increment(Part part)
    {
        switch (part)
        {
            case Part.Major:
                Major++;
                break;
            case Part.Minor:
                Minor++;
                break;
            case Part.Patch:
                Patch++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(part), part, null);
        }
    }

    /// <summary>
    /// Decrements the specified part of the version number, if its value is greater than zero.
    /// </summary>
    /// <param name="part">The part of the version to decrement. Must be one of <see cref="Part.Major"/>, <see cref="Part.Minor"/>, or <see
    /// cref="Part.Patch"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="part"/> is not a valid <see cref="Part"/> value.</exception>
    public void Decrement(Part part)
    {
        switch (part)
        {
            case Part.Major:
                if (Major > 0) Major--;
                break;
            case Part.Minor:
                if (Minor > 0) Minor--;
                break;
            case Part.Patch:
                if (Patch > 0) Patch--;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(part), part, null);
        }
    }

}
