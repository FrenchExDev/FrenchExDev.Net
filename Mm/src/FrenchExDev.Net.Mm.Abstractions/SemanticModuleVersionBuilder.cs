using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Provides a builder for constructing instances of <see cref="SemanticModuleVersion"/>.
/// </summary>
/// <remarks>This class follows the builder pattern, allowing incremental configuration of the  <see
/// cref="SemanticModuleVersion"/> components such as major, minor, patch, pre-release,  and build metadata. Once all
/// desired components are set, the <see cref="Build"/> method  can be called to produce a fully constructed <see
/// cref="SemanticModuleVersion"/> instance.</remarks>
public class SemanticModuleVersionBuilder : IBuilder<SemanticModuleVersion>
{
    /// <summary>
    /// Holds the components of the semantic version being built.
    /// </summary>
    private int _major;
    private int _minor;
    private int _patch;
    private string _preRelease = string.Empty;
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
    /// Builds and returns a result containing the semantic module version.
    /// </summary>
    /// <returns>An <see cref="IBuildResult{T}"/> containing the constructed <see cref="SemanticModuleVersion"/>. The result
    /// indicates a successful build operation.</returns>
    public IBuildResult<SemanticModuleVersion> Build()
    {
        return new BasicBuildResult<SemanticModuleVersion>(true,
            new SemanticModuleVersion(_major, _minor, _patch, _preRelease, _buildMetadata));
    }
}
