using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Provides a builder for constructing instances of <see cref="SemanticModuleVersion"/>.
/// </summary>
/// <remarks>This class follows the builder pattern, allowing incremental configuration of the  <see
/// cref="SemanticModuleVersion"/> components such as major, minor, patch, pre-release,  and build metadata. Once all
/// desired components are set, the <see cref="Build"/> method  can be called to produce a fully constructed <see
/// cref="SemanticModuleVersion"/> instance.</remarks>
public class SemanticModuleVersionBuilder : AbstractBuilder<SemanticModuleVersion>
{
    /// <summary>
    /// Holds the components of the semantic version being built.
    /// </summary>
    private int _major;

    /// <summary>
    /// Holds the components of the semantic version being built.
    /// </summary>
    private int _minor;

    /// <summary>
    /// Holds the components of the semantic version being built.
    /// </summary>
    private int _patch;

    /// <summary>
    /// Holds the components of the semantic version being built.
    /// </summary>
    private string _preRelease = string.Empty;

    /// <summary>
    /// Holds the components of the semantic version being built.
    /// </summary>
    private string _buildMetadata = string.Empty;

    /// <summary>
    /// Sets the major version number.
    /// </summary>
    /// <param name="major"></param>
    /// <returns></returns>
    public SemanticModuleVersionBuilder Major(int major)
    {
        _major = major;
        return this;
    }

    /// <summary>
    /// Sets the minor version number.
    /// </summary>
    /// <param name="minor"></param>
    /// <returns></returns>
    public SemanticModuleVersionBuilder Minor(int minor)
    {
        _minor = minor;
        return this;
    }

    /// <summary>
    /// Sets the patch version number.
    /// </summary>
    /// <param name="patch"></param>
    /// <returns></returns>
    public SemanticModuleVersionBuilder Patch(int patch)
    {
        _patch = patch;
        return this;
    }

    /// <summary>
    /// Sets the pre-release version identifier.
    /// </summary>
    /// <param name="preRelease"></param>
    /// <returns></returns>
    public SemanticModuleVersionBuilder PreRelease(string preRelease)
    {
        _preRelease = preRelease;
        return this;
    }

    /// <summary>
    /// Sets the build metadata identifier.
    /// </summary>
    /// <param name="buildMetadata"></param>
    /// <returns></returns>
    public SemanticModuleVersionBuilder BuildMetadata(string buildMetadata)
    {
        _buildMetadata = buildMetadata;
        return this;
    }

    /// <summary>
    /// Validates the version components and records any failures for negative values.
    /// </summary>
    /// <remarks>This method checks that the major, minor, and patch version components are non-negative. Any
    /// violations are added to the failures dictionary for further processing.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where any detected errors are recorded.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (_major < 0)
        {
            failures.Failure(nameof(_major), new InvalidDataException($"The {nameof(Major)} version component cannot be negative."));
        }

        if (_minor < 0)
        {
            failures.Failure(nameof(_minor), new InvalidDataException($"The {nameof(Minor)} version component cannot be negative."));
        }

        if (_patch < 0)
        {
            failures.Failure(nameof(_patch), new InvalidDataException($"The {nameof(Patch)} version component cannot be negative."));
        }
    }

    /// <summary>
    /// Creates a new instance of the semantic module version using the current major, minor, patch, pre-release, and
    /// build metadata values.
    /// </summary>
    /// <returns>A <see cref="SemanticModuleVersion"/> object representing the semantic version constructed from the current
    /// state.</returns>
    protected override SemanticModuleVersion Instantiate()
    {
        return new SemanticModuleVersion(_major, _minor, _patch, _preRelease, _buildMetadata);
    }
}
