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
    /// Builds and returns a new instance of <see cref="MajorMinorPatchModuleVersion"/> with the configured version components.
    /// </summary>
    /// <returns>A new instance of <see cref="MajorMinorPatchModuleVersion"/> initialized with the specified major, minor, and patch values.</returns>
    public override IBuildResult<MajorMinorPatchModuleVersion> Build()
    {
        var exceptions = new List<Exception>();

        if (_major == null)
        {
            exceptions.Add(new ArgumentNullException(nameof(_major), "Major version is required."));
        }

        if (_minor == null)
        {
            exceptions.Add(new ArgumentNullException(nameof(_minor), "Minor version is required."));
        }

        if (_patch == null)
        {
            exceptions.Add(new ArgumentNullException(nameof(_patch), "Patch version is required."));
        }

        if (exceptions.Any())
        {
                       return new BasicBuildResult<MajorMinorPatchModuleVersion>(false, null, exceptions);
        }

        return new BasicBuildResult<MajorMinorPatchModuleVersion>(true,
                    new MajorMinorPatchModuleVersion(
                        _major ?? throw new ArgumentNullException(nameof(_major)),
                        _minor ?? throw new ArgumentNullException(nameof(_minor)),
                        _patch ?? throw new ArgumentNullException(nameof(_patch))));
    }
}
