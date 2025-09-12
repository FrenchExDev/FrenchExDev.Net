using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Provides a builder for creating instances of <see cref="MajorMinorPatchModuleVersion"/>  by specifying major, minor,
/// and patch version components.
/// </summary>
/// <remarks>This class follows the builder pattern, allowing the caller to configure version components  (major,
/// minor, and patch) incrementally using method chaining. Once all desired components  are set, the <see cref="Build"/>
/// method can be called to produce a new  <see cref="MajorMinorPatchModuleVersion"/> instance.</remarks>
public class MajorMinorPatchModuleVersionBuilder : AbstractObjectBuilder<MajorMinorPatchModuleVersion, MajorMinorPatchModuleVersionBuilder>
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
    /// Builds an object representing a major, minor, and patch version, validating the required components.
    /// </summary>
    /// <remarks>This method ensures that the major, minor, and patch components are provided and have valid
    /// values. If any of these components are missing, corresponding exceptions are added to the <paramref
    /// name="exceptions"/> list.</remarks>
    /// <param name="exceptions">A collection to which any validation exceptions encountered during the build process are added.</param>
    /// <param name="visited">A list of objects that have already been visited during the build process to prevent circular references.</param>
    /// <returns>An <see cref="IObjectBuildResult{MajorMinorPatchModuleVersion}"/> representing the result of the build
    /// operation. If validation fails, the result contains the collected exceptions; otherwise, it contains the
    /// successfully built version object.</returns>
    protected override IObjectBuildResult<MajorMinorPatchModuleVersion> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        if (_major == null || !_major.HasValue)
        {
            exceptions.Add(nameof(_major), new ArgumentNullException(nameof(_major), "Major version is required."));
        }

        if (_minor == null || !_minor.HasValue)
        {
            exceptions.Add(nameof(_minor), new ArgumentNullException(nameof(_minor), "Minor version is required."));
        }

        if (_patch == null || !_patch.HasValue)
        {
            exceptions.Add(nameof(_patch), new ArgumentNullException(nameof(_patch), "Patch version is required."));
        }

        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_major);
        ArgumentNullException.ThrowIfNull(_minor);
        ArgumentNullException.ThrowIfNull(_patch);

        return Success(new MajorMinorPatchModuleVersion(_major.Value, _minor.Value, _patch.Value));
    }
}
