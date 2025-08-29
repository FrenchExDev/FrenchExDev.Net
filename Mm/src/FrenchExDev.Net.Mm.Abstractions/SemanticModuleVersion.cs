namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Represents a semantic version of a module, adhering to the Semantic Versioning (SemVer) specification.
/// </summary>
/// <remarks>A semantic version consists of three required numeric components (major, minor, and patch) and two
/// optional string components  (pre-release and build metadata). This class provides methods to increment or decrement
/// specific version parts and to  generate a string representation of the version in SemVer format.</remarks>
public class SemanticModuleVersion : IModuleVersion
{
    /// <summary>
    /// Parts of a semantic version.
    /// </summary>
    public enum Part
    {
        Major,
        Minor,
        Patch,
        PreRelease,
        BuildMetadata
    }

    /// <summary>
    /// Gets the major version number.
    /// </summary>
    public int Major { get; protected set; }

    /// <summary>
    /// Get the minor version number.
    /// </summary>
    public int Minor { get; protected set; }

    /// <summary>
    /// Get the patch version number.
    /// </summary>
    public int Patch { get; protected set; }

    /// <summary>
    /// Get the pre-release version identifier.
    /// </summary>
    public string PreRelease { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the version is a pre-release version.
    /// </summary>
    public bool IsPreRelease => !string.IsNullOrEmpty(PreRelease);

    /// <summary>
    /// Get the build metadata identifier.
    /// </summary>
    public string BuildMetadata { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the current instance has associated build metadata.
    /// </summary>
    public bool HasBuildMetadata => !string.IsNullOrEmpty(BuildMetadata);

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticModuleVersion"/> class with the specified version
    /// components.
    /// </summary>
    /// <remarks>This constructor allows you to create a semantic version representation by specifying the
    /// major, minor, and patch numbers, along with optional pre-release and build metadata components. Semantic
    /// versioning is commonly used to indicate compatibility and changes in software versions.</remarks>
    /// <param name="major">The major version number. Must be a non-negative integer.</param>
    /// <param name="minor">The minor version number. Must be a non-negative integer.</param>
    /// <param name="patch">The patch version number. Must be a non-negative integer.</param>
    /// <param name="preRelease">An optional pre-release label, such as "alpha" or "beta". Defaults to an empty string if not specified.</param>
    /// <param name="buildMetadata">An optional build metadata string, such as a build number or commit hash. Defaults to an empty string if not
    /// specified.</param>
    public SemanticModuleVersion(int major, int minor, int patch, string preRelease = "", string buildMetadata = "")
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        PreRelease = preRelease;
        BuildMetadata = buildMetadata;
    }

    /// <summary>
    /// Returns a string representation of the version, including optional pre-release and build metadata.
    /// </summary>
    /// <remarks>The returned string follows the Semantic Versioning (SemVer) format: 
    /// "Major.Minor.Patch[-PreRelease][+BuildMetadata]".</remarks>
    /// <returns>A string representing the version. The format includes the major, minor, and patch versions,  and optionally the
    /// pre-release and build metadata if they are not null or empty.</returns>
    public override string ToString()
    {
        var version = $"{Major}.{Minor}.{Patch}";
        if (!string.IsNullOrEmpty(PreRelease))
        {
            version += $"-{PreRelease}";
        }
        if (!string.IsNullOrEmpty(BuildMetadata))
        {
            version += $"+{BuildMetadata}";
        }
        return version;
    }

    /// <summary>
    /// Increments the specified part of the version.
    /// </summary>
    /// <param name="part">The part to increment</param>
    /// <remarks>It does not handle PreRelease and BuildMetadata</remarks>
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
            case Part.PreRelease:
            case Part.BuildMetadata:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(part), part, null);
        }
    }

    /// <summary>
    /// Decrements the specified part of the version.
    /// </summary>
    /// <param name="part">The part to decrement</param>
    /// <remarks>This method decreases the specified part of the version (Major, Minor, or Patch) by one, ensuring that
    /// it does not go below zero. It does not handle PreRelease and BuildMetadata</remarks>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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
            case Part.PreRelease:
            case Part.BuildMetadata:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(part), part, null);
        }
    }
}
