using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Model.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Represents an abstract base for project models, providing common properties and behaviors for .NET project metadata.
/// </summary>
/// <typeparam name="T">A type implementing <see cref="IProjectModel"/>. Used for type-safe inheritance and model operations.</typeparam>
/// <remarks>
/// This class is intended to be inherited by concrete project model classes representing specific .NET project types.
/// Example usage:
/// <code>
/// public class MyProjectModel : AbstractProjectModel<MyProjectModel> { ... }
/// </code>
/// </remarks>
public abstract class AbstractProjectModel<T> : IProjectModel where T : IProjectModel
{
    /// <summary>
    /// Gets the project name.
    /// </summary>
    /// <example>"MyLibrary"</example>
    public string? Name { get; protected set; }

    /// <summary>
    /// Gets the directory path where the project is located.
    /// </summary>
    /// <example>"C:/Projects/MyLibrary"</example>
    public string? Directory { get; protected set; }

    /// <summary>
    /// Gets the SDK used by the project (e.g., "Microsoft.NET.Sdk").
    /// </summary>
    /// <example>"Microsoft.NET.Sdk"</example>
    public string? Sdk { get; protected set; }

    /// <summary>
    /// Gets the target framework for the project (e.g., ".NET 9").
    /// </summary>
    /// <example>"net9.0"</example>
    public string? TargetFramework { get; protected set; }

    /// <summary>
    /// Gets the output type of the project (e.g., "Library", "Exe").
    /// </summary>
    /// <example>"Library"</example>
    public string? OutputType { get; protected set; }

    /// <summary>
    /// Gets the C# language version used in the project.
    /// </summary>
    /// <example>"13.0"</example>
    public string? LangVersion { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether nullable reference types are enabled.
    /// </summary>
    /// <remarks>Defaults to <c>true</c>.</remarks>
    public bool? Nullable { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether implicit global usings are enabled.
    /// </summary>
    /// <remarks>Defaults to <c>true</c>.</remarks>
    public bool? ImplicitUsings { get; protected set; }

    /// <summary>
    /// Holds a reference to the package version information.
    /// </summary>
    private IReference<IPackageVersion>? _packageVersion;

    /// <summary>
    /// Gets the resolved version information for the associated package.
    /// </summary>
    /// <remarks>Accessing this property requires that the package version has been resolved. If the
    /// underlying package version is not resolved, a <see cref="NotResolvedException"/> is thrown.</remarks>
    public IPackageVersion PackageVersion => _packageVersion?.Resolved() ?? throw new NotResolvedException(nameof(PackageVersion));

    /// <summary>
    /// Gets or sets the descriptive text associated with the object.
    /// </summary>
    public string? Description { get; protected set; }

    /// <summary>
    /// Gets or sets the copyright information associated with the object.
    /// </summary>
    public string? Copyright { get; protected set; }

    /// <summary>
    /// Gets or sets the URL of the project homepage associated with the package.
    /// </summary>
    public string? PackageProjectUrl { get; protected set; }

    /// <summary>
    /// Gets or sets the URL of the source code repository associated with this instance.
    /// </summary>
    public string? RepositoryUrl { get; protected set; }

    /// <summary>
    /// Gets or sets the type of repository used for data storage or retrieval.
    /// </summary>
    public string? RepositoryType { get; protected set; }

    /// <summary>
    /// Gets or sets the list of tags associated with the package.
    /// </summary>
    /// <remarks>Tags can be used to categorize or describe the package for search and filtering purposes. The
    /// list may be null if no tags are specified.</remarks>
    public List<string>? PackageTags { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether to include debug symbols in the build output.
    /// </summary>
    public bool? IncludeSymbols { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether coding style rules are enforced during the build process.
    /// </summary>
    /// <remarks>When set to <see langword="true"/>, coding style violations will cause the build to fail.
    /// When set to <see langword="false"/>, coding style violations are reported but do not affect the build outcome.
    /// If <c>null</c>, the default enforcement behavior is used as determined by the build system.</remarks>
    public bool? EnforceCodingStyleInBuild { get; protected set; }

    /// <summary>
    /// Holds the list of project references.
    /// </summary>
    private readonly ReferenceList<ProjectReference> _projectReferences = new();

    /// <summary>
    /// Gets an enumerable collection of project references that have been resolved.
    /// </summary>
    /// <remarks>The returned collection contains only references that are currently resolved. If there are no
    /// resolved project references, the collection will be empty.</remarks>
    public IEnumerable<ProjectReference> ProjectReferences => _projectReferences.AsEnumerable();

    /// <summary>
    /// Holds the list of NuGet package references.
    /// </summary>
    private readonly ReferenceList<IPackageReference> _packageReferences = new();

    /// <summary>
    /// Gets the list of NuGet package references for this project.
    /// </summary>
    /// <remarks>
    /// Example: referencing Newtonsoft.Json package.
    /// <code>
    /// PackageReferences.Add(new PackageReference("Newtonsoft.Json", "13.0.1"));
    /// </code>
    /// </remarks>
    public IEnumerable<IPackageReference> PackageReferences => _packageReferences.AsEnumerable();

    /// <summary>
    /// Holds the list of analyzer package references.
    /// </summary>
    private readonly ReferenceList<IPackageReference> _analyzers = new();

    /// <summary>
    /// Gets the list of analyzer package references for this project.
    /// </summary>
    /// <remarks>
    /// Example: referencing Roslyn analyzers.
    /// <code>
    /// Analyzers.Add(new PackageReference("Microsoft.CodeAnalysis.FxCopAnalyzers", "3.3.2"));
    /// </code>
    /// </remarks>
    public IEnumerable<IPackageReference>? Analyzers => _analyzers.AsEnumerable();

    /// <summary>
    /// Holds additional custom properties for the project.
    /// </summary>
    private Reference<Dictionary<string, object>>? _additionalProperties = new();

    /// <summary>
    /// Gets additional custom properties for the project.
    /// </summary>
    /// <remarks>
    /// Useful for storing extra metadata not covered by standard fields.
    /// Example:
    /// <code>
    /// AdditionalProperties["CustomProperty"] = "Value";
    /// </code>
    /// </remarks>
    public Dictionary<string, object> AdditionalProperties => _additionalProperties?.Resolved() ?? throw new NotResolvedException(nameof(AdditionalProperties));

    /// <summary>
    /// Contains references to declaration models associated with this instance.
    /// </summary>
    /// <remarks>The list may be null or empty if no declaration models are present. Modifying the collection
    /// does not affect the underlying models themselves.</remarks>
    private readonly ReferenceList<IDeclarationModel> _declarationModels = [];

    /// <summary>
    /// Gets the list of declaration models (e.g., classes, interfaces) defined in the project.
    /// </summary>
    /// <remarks>
    /// Example:
    /// <code>
    /// DeclarationModels.Add(new ClassDeclarationModel("MyClass"));
    /// </code>
    /// </remarks>
    public IEnumerable<IDeclarationModel>? DeclarationModels => _declarationModels.AsEnumerable();

    /// <summary>
    /// Gets the version identifier for the current instance.
    /// </summary>
    public string? Version { get; protected set; }

    /// <summary>
    /// Gets the value indicating whether to generate a NuGet package on build.
    /// </summary>
    public bool? GeneratePackageOnBuild { get; }

    /// <summary>
    /// Gets or sets the list of authors associated with the item.
    /// </summary>
    public string? Authors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractProjectModel"/> class with the specified project
    /// configuration details.
    /// </summary>
    /// <param name="name">The name of the project. This value cannot be <see langword="null"/> or empty.</param>
    /// <param name="directory">The directory path where the project is located. This value cannot be <see langword="null"/> or empty.</param>
    /// <param name="sdk">The SDK used by the project (e.g., "Microsoft.NET.Sdk"). This value cannot be <see langword="null"/> or empty.</param>
    /// <param name="targetFramework">The target framework for the project (e.g., "net6.0"). This value cannot be <see langword="null"/> or empty.</param>
    /// <param name="outputType">The output type of the project (e.g., "Library", "Exe"). This value cannot be <see langword="null"/> or empty.</param>
    /// <param name="langVersion">The C# language version used by the project (e.g., "10.0"). This value cannot be <see langword="null"/> or
    /// empty.</param>
    /// <param name="nullable">A value indicating whether nullable reference types are enabled for the project. Set to <see langword="true"/>
    /// to enable nullable reference types; otherwise, <see langword="false"/>.</param>
    /// <param name="implicitUsings">A value indicating whether implicit global usings are enabled for the project. Set to <see langword="true"/> to
    /// enable implicit usings; otherwise, <see langword="false"/>.</param>
    /// <param name="projectReferences">A list of project references included in the project. This value cannot be <see langword="null"/> but may be
    /// empty.</param>
    /// <param name="packageReferences">A list of NuGet package references included in the project. This value cannot be <see langword="null"/> but may
    /// be empty.</param>
    /// <param name="analyzers">A list of analyzers applied to the project. This value cannot be <see langword="null"/> but may be empty.</param>
    /// <param name="additionalProperties">A dictionary of additional properties for the project. Keys represent property names, and values represent their
    /// corresponding values. This value cannot be <see langword="null"/> but may be empty.</param>
    /// <param name="declarationModels">A list of declaration models representing the code structure of the project. This value cannot be <see
    /// langword="null"/> but may be empty.</param>
    protected AbstractProjectModel(
        string? name,
        string? directory,
        string? sdk,
        string? targetFramework,
        string? outputType,
        string? langVersion,
        bool? nullable,
        bool? implicitUsings,
        ReferenceList<ProjectReference>? projectReferences,
        ReferenceList<IPackageReference>? packageReferences,
        ReferenceList<IPackageReference>? analyzers,
        Reference<Dictionary<string, object>>? additionalProperties,
        ReferenceList<IDeclarationModel>? declarationModels,
        string? version,
        bool? generatePackageOnBuild,
        string? packageTags,
        string? authors
    )
    {
        Name = name;
        Directory = directory;
        Sdk = sdk;
        TargetFramework = targetFramework;
        OutputType = outputType;
        LangVersion = langVersion;
        Nullable = nullable;
        ImplicitUsings = implicitUsings;
        _projectReferences = projectReferences ?? new();
        _packageReferences = packageReferences ?? new();
        _analyzers = analyzers ?? new();
        _additionalProperties = additionalProperties;
        _declarationModels = declarationModels ?? new();
        Version = version;
        GeneratePackageOnBuild = generatePackageOnBuild;
        PackageTags = packageTags?.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();
        Authors = authors;
    }

    /// <summary>
    /// Sets the package version to be used for subsequent operations.
    /// </summary>
    /// <param name="packageVersion">The package version to associate with the current context. Cannot be null.</param>
    public void WithPackageVersion(IPackageVersion packageVersion)
    {
        _packageVersion = new Reference<IPackageVersion>().Resolve(packageVersion);
    }

}
