using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Provides a builder for creating instances of <see cref="MajorMinorPatchModuleVersion"/>  by specifying major, minor,
/// and patch version components.
/// </summary>
/// <remarks>This class follows the builder pattern, allowing the caller to configure version components  (major,
/// minor, and patch) incrementally using method chaining. Once all desired components  are set, the <see cref="Build"/>
/// method can be called to produce a new  <see cref="MajorMinorPatchModuleVersion"/> instance.</remarks>
public class MajorMinorPatchModuleVersionBuilder : AbstractBuilder<MajorMinorPatchModuleVersion>
{
    /// <summary>
    /// Holds the major, minor, and patch version components.
    /// </summary>
    private int? _major;
    private int? _minor;
    private int? _patch;

    /// <summary>
    /// Sets the major version number.
    /// </summary>
    /// <param name="major">The major version number. Typically incremented for breaking changes.</param>
    /// <returns>The current instance of <see cref="MajorMinorPatchModuleVersionBuilder"/> for method chaining.</returns>
    public MajorMinorPatchModuleVersionBuilder Major(int major)
    {
        _major = major;
        return this;
    }
    /// <summary>
    /// Sets the minor version number.
    /// </summary>
    /// <param name="minor">The minor version number. Typically incremented for backward-compatible feature additions.</param>
    /// <returns>The current instance of <see cref="MajorMinorPatchModuleVersionBuilder"/> for method chaining.</returns>
    public MajorMinorPatchModuleVersionBuilder Minor(int minor)
    {
        _minor = minor;
        return this;
    }
    /// <summary>
    /// Sets the patch version number.
    /// </summary>
    /// <param name="patch">The patch version number. Typically incremented for backward-compatible bug fixes.</param>
    /// <returns>The current instance of <see cref="MajorMinorPatchModuleVersionBuilder"/> for method chaining.</returns>
    public MajorMinorPatchModuleVersionBuilder Patch(int patch)
    {
        _patch = patch;
        return this;
    }


    /// <summary>
    /// Creates a new instance of the module version using the specified major, minor, and patch values.
    /// </summary>
    /// <remarks>Throws an <see cref="ArgumentNullException"/> if any of the major, minor, or patch values are
    /// not set. This method is typically called by derived classes to construct a version object based on validated
    /// input.</remarks>
    /// <returns>A <see cref="MajorMinorPatchModuleVersion"/> object initialized with the current major, minor, and patch values.</returns>
    protected override MajorMinorPatchModuleVersion Instantiate()
    {
        ArgumentNullException.ThrowIfNull(_major);
        ArgumentNullException.ThrowIfNull(_minor);
        ArgumentNullException.ThrowIfNull(_patch);

        return new(_major.Value, _minor.Value, _patch.Value);
    }

    /// <summary>
    /// Performs validation of the version components and records any validation failures encountered.
    /// </summary>
    /// <remarks>This method checks that the major, minor, and patch version fields are present and valid. If
    /// any required component is missing, an appropriate failure is recorded in the provided failures
    /// dictionary.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any missing or invalid version components are reported here.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (_major == null || !_major.HasValue)
        {
            failures.Failure(nameof(_major), new ArgumentNullException(nameof(_major), "Major version is required."));
        }

        if (_minor == null || !_minor.HasValue)
        {
            failures.Failure(nameof(_minor), new ArgumentNullException(nameof(_minor), "Minor version is required."));
        }

        if (_patch == null || !_patch.HasValue)
        {
            failures.Failure(nameof(_patch), new ArgumentNullException(nameof(_patch), "Patch version is required."));
        }
    }
}
