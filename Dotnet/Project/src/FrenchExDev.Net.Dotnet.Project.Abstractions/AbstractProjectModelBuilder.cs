using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Abstract builder base class for constructing project models implementing <see cref="IProjectModel"/>.
/// Provides a fluent API for configuring project metadata, references, package information, and code declarations.
/// Validates required properties and supports extensibility for custom project types.
/// </summary>
/// <remarks>
/// This builder is intended for use in code generation, project scaffolding, and automated tooling scenarios.
/// Example usage:
/// <code>
/// var builder = new MyProjectModelBuilder()
///     .Name("MyProject")
///     .Directory("src/MyProject")
///     .Sdk("Microsoft.NET.Sdk")
///     .TargetFramework("net9.0")
///     .OutputType("Exe")
///     .LangVersion("13.0")
///     .Nullable(true)
///     .ImplicitUsings(true)
///     .PackageVersion(b => b.Version("1.0.0"))
///     .Description("A sample project")
///     .Copyright("Copyright 2024")
///     .PackageProjectUrl("https://github.com/example")
///     .RepositoryUrl("https://github.com/example/repo")
///     .RepositoryType("git")
///     .PackageTags("sample", "dotnet")
///     .IncludeSymbols(true)
///     .EnforceCodeStyleInBuild(true)
///     .ProjectReferences(new List<ProjectReference> { ... })
///     .PackageReferences(new List<IPackageReference> { ... })
///     .DeclarationModels(new List<IDeclarationModel> { ... });
/// var result = builder.Build();
/// </code>
/// </remarks>
public abstract class AbstractProjectModelBuilder<TProjectModel, TBuilder> : AbstractObjectBuilder<TProjectModel, TBuilder>
    where TProjectModel : IProjectModel
    where TBuilder : class, IObjectBuilder<TProjectModel>
{
    /// <summary>
    /// Error message for missing project name.
    /// </summary>
    public const string ErrorProjectNameCannotBeNullOrEmpty = "Project name cannot be null or empty.";

    /// <summary>
    /// Error message for missing project directory.
    /// </summary>
    public const string ErrorProjectDirectoryCannotBeNullOrEmpty = "Project directory cannot be null or empty.";

    /// <summary>
    /// Error message for missing project SDK.
    /// </summary>
    public const string ErrorProjectSdkCannotBeNullOrEmpty = "Project SDK cannot be null or empty.";

    /// <summary>
    /// Error message for missing target framework.
    /// </summary>
    public const string ErrorProjectTargetFrameworkCannotBeNullOrEmpty = "Project target framework cannot be null or empty.";

    /// <summary>
    /// Error message for missing output type.
    /// </summary>
    public const string ErrorProjectOutputTypeCannotBeNullOrEmpty = "Project output type cannot be null or empty.";

    /// <summary>
    /// Error message for missing language version.
    /// </summary>
    public const string ErrorProjectLanguageVersionCannotBeNullOrEmpty = "Project language version cannot be null or empty.";

    /// <summary>
    /// Error message for missing nullable setting.
    /// </summary>
    public const string ErrorProjectNullableCannotBeNullOrEmpty = "Project nullable setting cannot be null.";

    /// <summary>
    /// Error message for missing implicit usings setting.
    /// </summary>
    public const string ErrorPojectImplicitUsingsCannotBeNullOrEmpty = "Project implicit usings setting cannot be null.";

    /// <summary>
    /// Stores the project name.
    /// </summary>
    protected string? _name;

    /// <summary>
    /// Stores the project directory path.
    /// </summary>
    protected string? _directory;

    /// <summary>
    /// Stores the project SDK identifier.
    /// </summary>
    protected string? _sdk;

    /// <summary>
    /// Stores the target framework (e.g., "net9.0").
    /// </summary>
    protected string? _targetFramework;

    /// <summary>
    /// Stores the output type (e.g., "Exe", "Library").
    /// </summary>
    protected string? _outputType;

    /// <summary>
    /// Stores the C# language version (e.g., "13.0").
    /// </summary>
    protected string? _langVersion;

    /// <summary>
    /// Indicates whether nullable reference types are enabled.
    /// </summary>
    protected bool? _nullable;

    /// <summary>
    /// Indicates whether implicit usings are enabled.
    /// </summary>
    protected bool? _implicitUsings;

    /// <summary>
    /// Represents the version information of the package associated with this instance.
    /// </summary>
    /// <remarks>This field is intended for use within derived classes to access or modify the package
    /// version.</remarks>
    protected IPackageVersion? _packageVersion;

    protected bool? _generatePackageOnBuild;

    protected string? _description;

    protected string? _copyright;

    protected string? _packageProjectUrl;

    protected string? _repositoryUrl;
    protected string? _repositoryType;
    protected string[]? _packageTags;
    protected bool? _includeSymbols;
    protected bool? _enforceCodeStyleInBuild;

    /// <summary>
    /// Sets the package version using a builder action.
    /// </summary>
    /// <param name="body">An action to configure the package version builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Example:
    /// <code>
    /// builder.PackageVersion(b => b.Version("1.2.3"));
    /// </code>
    /// </remarks>
    public TBuilder PackageVersion(Action<PackageVersionBuilder> body)
    {
        var builder = new PackageVersionBuilder();
        body(builder);
        _packageVersion = builder.Build().Success();
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets whether to generate a NuGet package on build.
    /// </summary>
    /// <param name="generatePackageOnBuild">True to generate package on build; otherwise, false.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder GeneratePackageOnBuild(bool? generatePackageOnBuild = true)
    {
        _generatePackageOnBuild = generatePackageOnBuild;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the project description.
    /// </summary>
    /// <param name="description">The project description.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder Description(string description)
    {
        _description = description;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the copyright information for the project.
    /// </summary>
    /// <param name="copyright">The copyright string.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder Copyright(string copyright)
    {
        _copyright = copyright;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the project URL for the NuGet package.
    /// </summary>
    /// <param name="packageProjectUrl">The project URL.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder PackageProjectUrl(string packageProjectUrl)
    {
        _packageProjectUrl = packageProjectUrl;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the repository URL for the project.
    /// </summary>
    /// <param name="repositoryUrl">The repository URL.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder RepositoryUrl(string repositoryUrl)
    {
        _repositoryUrl = repositoryUrl;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the repository type (e.g., "git").
    /// </summary>
    /// <param name="repositoryType">The repository type.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder RepositoryType(string repositoryType)
    {
        _repositoryType = repositoryType;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the package tags for the NuGet package.
    /// </summary>
    /// <param name="packageTags">An array of tags.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Example:
    /// <code>
    /// builder.PackageTags("library", "dotnet", "csharp");
    /// </code>
    /// </remarks>
    public TBuilder PackageTags(params string[] packageTags)
    {
        _packageTags = packageTags;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets whether to include symbols in the NuGet package.
    /// </summary>
    /// <param name="includeSymbols">True to include symbols; otherwise, false.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder IncludeSymbols(bool includeSymbols)
    {
        _includeSymbols = includeSymbols;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets whether to enforce code style rules during build.
    /// </summary>
    /// <param name="enforceCodeStyleInBuild">True to enforce code style; otherwise, false.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder EnforceCodeStyleInBuild(bool? enforceCodeStyleInBuild = true)
    {
        _enforceCodeStyleInBuild = enforceCodeStyleInBuild;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Stores the list of project references.
    /// </summary>
    protected List<ProjectReference> _projectReferences = new();

    /// <summary>
    /// Stores the list of NuGet package references.
    /// </summary>
    protected List<IPackageReference> _packageReferences = new();

    /// <summary>
    /// Stores the list of analyzer package references.
    /// </summary>
    protected List<IPackageReference> _analyzers = new();

    /// <summary>
    /// Stores additional custom MSBuild properties.
    /// </summary>
    protected Dictionary<string, object> _additionalProperties = new();

    /// <summary>
    /// Stores code declaration models (classes, interfaces, etc.) for the project.
    /// </summary>
    protected List<IDeclarationModel> _declarationModels = new();

    /// <summary>
    /// Sets the project name.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder Name(string name)
    {
        _name = name;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the project directory.
    /// </summary>
    /// <param name="directory">The directory path.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder Directory(string directory)
    {
        _directory = directory;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the project SDK.
    /// </summary>
    /// <param name="sdk">The SDK identifier (e.g., "Microsoft.NET.Sdk").</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder Sdk(string sdk)
    {
        _sdk = sdk;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the target framework.
    /// </summary>
    /// <param name="targetFramework">The target framework (e.g., "net9.0").</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder TargetFramework(string targetFramework)
    {
        _targetFramework = targetFramework;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the output type.
    /// </summary>
    /// <param name="outputType">The output type (e.g., "Exe", "Library").</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder OutputType(string outputType)
    {
        _outputType = outputType;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the C# language version.
    /// </summary>
    /// <param name="langVersion">The language version (e.g., "13.0").</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder LangVersion(string langVersion)
    {
        _langVersion = langVersion;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the nullable reference types setting.
    /// </summary>
    /// <param name="nullable">True to enable nullable reference types; otherwise, false.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder Nullable(bool nullable)
    {
        _nullable = nullable;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the implicit usings setting.
    /// </summary>
    /// <param name="implicitUsings">True to enable implicit usings; otherwise, false.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder ImplicitUsings(bool implicitUsings)
    {
        _implicitUsings = implicitUsings;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the list of project references.
    /// </summary>
    /// <param name="projectReferences">The list of project references.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder ProjectReferences(List<ProjectReference> projectReferences)
    {
        _projectReferences = projectReferences;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the list of NuGet package references.
    /// </summary>
    /// <param name="packageReferences">The list of package references.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder PackageReferences(List<IPackageReference> packageReferences)
    {
        _packageReferences = packageReferences;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the list of NuGet package references using a builder action.
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public TBuilder PackageReferences(Action<ListBuilder<IPackageReference, PackageReferenceBuilder>> body)
    {
        var listBuilder = new ListBuilder<IPackageReference, PackageReferenceBuilder>();
        body(listBuilder);
        _packageReferences = listBuilder.Build();
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }


    /// <summary>
    /// Sets the list of analyzer package references.
    /// </summary>
    /// <param name="analyzers">The list of analyzer references.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder Analyzers(List<IPackageReference> analyzers)
    {
        _analyzers = analyzers;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets additional custom MSBuild properties for the project.
    /// </summary>
    /// <param name="additionalProperties">A dictionary of additional properties.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder AdditionalProperties(Dictionary<string, object> additionalProperties)
    {
        _additionalProperties = additionalProperties;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the code declaration models (classes, interfaces, etc.) for the project.
    /// </summary>
    /// <param name="declarationModels">A list of code declaration models.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder DeclarationModels(List<IDeclarationModel> declarationModels)
    {
        _declarationModels = declarationModels;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Validates required project properties and adds exceptions for missing or invalid values.
    /// </summary>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <remarks>
    /// This method checks that all required project metadata is set before building the project model.
    /// </remarks>
    protected void VisiteObjectAndCollectExceptions(VisitedObjectsList visited, ExceptionBuildList exceptions)
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            exceptions.Add(new ArgumentNullException(nameof(_name), ErrorProjectNameCannotBeNullOrEmpty));
        }

        if (string.IsNullOrWhiteSpace(_directory))
        {
            exceptions.Add(new ArgumentNullException(nameof(_directory), ErrorProjectDirectoryCannotBeNullOrEmpty));
        }

        if (string.IsNullOrWhiteSpace(_sdk))
        {
            exceptions.Add(new ArgumentNullException(nameof(_sdk), ErrorProjectSdkCannotBeNullOrEmpty));
        }

        if (string.IsNullOrWhiteSpace(_targetFramework))
        {
            exceptions.Add(new ArgumentNullException(nameof(_targetFramework), ErrorProjectTargetFrameworkCannotBeNullOrEmpty));
        }

        if (string.IsNullOrWhiteSpace(_outputType))
        {
            exceptions.Add(new ArgumentNullException(nameof(_outputType), ErrorProjectOutputTypeCannotBeNullOrEmpty));
        }

        if (string.IsNullOrWhiteSpace(_langVersion))
        {
            exceptions.Add(new ArgumentNullException(nameof(_langVersion), ErrorProjectLanguageVersionCannotBeNullOrEmpty));
        }

        if (_nullable == null)
        {
            exceptions.Add(new ArgumentNullException(nameof(_nullable), ErrorProjectNullableCannotBeNullOrEmpty));
        }

        if (_implicitUsings == null)
        {
            exceptions.Add(new ArgumentNullException(nameof(_implicitUsings), ErrorPojectImplicitUsingsCannotBeNullOrEmpty));
        }
    }
}
