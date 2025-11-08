namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Defines a contract for accessing metadata and configuration properties of a .NET project model, such as target
/// framework, version, authors, and repository information.
/// </summary>
/// <remarks>Implementations of this interface provide read-only access to common project attributes used for
/// build, packaging, and publishing scenarios. Property values may be null if the corresponding metadata is not
/// specified in the project. This interface is typically used to inspect project settings in tooling or automation
/// workflows.</remarks>
public interface IProjectModel
{
    /// <summary>
    /// Gets the target .NET framework identifier for the current project or assembly.
    /// </summary>
    string? TargetFramework { get; }

    /// <summary>
    /// Gets a value indicating whether implicit global using directives are enabled for the project.
    /// </summary>
    /// <remarks>When enabled, commonly used namespaces are automatically imported, reducing the need to
    /// specify them explicitly in each source file. This property is typically used in project configuration scenarios
    /// to control code generation and compilation behavior.</remarks>
    bool? ImplicitUsings { get; }

    /// <summary>
    /// Gets a value indicating whether the associated state is enabled, disabled, or unspecified.
    /// </summary>
    /// <remarks>The property returns <see langword="true"/> if the state is enabled, <see langword="false"/>
    /// if disabled, or <see langword="null"/> if the state is not set. This is useful for scenarios where a tri-state
    /// value is required, such as optional configuration or indeterminate status.</remarks>
    bool? Nullable { get; }

    /// <summary>
    /// Gets the version identifier associated with the current instance.
    /// </summary>
    string? Version { get; }

    /// <summary>
    /// Gets the optional textual description associated with the object.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Gets the copyright information associated with the object.
    /// </summary>
    string? Copyright { get; }

    /// <summary>
    /// Gets the URL of the project site associated with the package.
    /// </summary>
    string? PackageProjectUrl { get; }

    /// <summary>
    /// Gets the URL of the source code repository associated with the current instance.
    /// </summary>
    string? RepositoryUrl { get; }

    /// <summary>
    /// Gets the type of repository used for data storage or retrieval.
    /// </summary>
    string? RepositoryType { get; }

    /// <summary>
    /// Gets the list of authors associated with the item, as a single string.
    /// </summary>
    string? Authors { get; }

    /// <summary>
    /// Gets the list of tags associated with the package.
    /// </summary>
    /// <remarks>The returned list may be null if no tags are defined. Tags are typically used to categorize
    /// or describe the package for search and filtering purposes.</remarks>
    List<string>? PackageTags { get; }

    /// <summary>
    /// Gets a value indicating whether symbol files should be included in the operation.
    /// </summary>
    /// <remarks>If the value is <see langword="true"/>, symbol files will be included; if <see
    /// langword="false"/>, they will be excluded. If <c>null</c>, the default behavior is applied, which may vary
    /// depending on the implementation.</remarks>
    bool? IncludeSymbols { get; }
}
