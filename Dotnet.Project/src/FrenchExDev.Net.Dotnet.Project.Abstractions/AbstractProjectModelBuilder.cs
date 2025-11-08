using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Model.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Abstract builder base class for constructing project models implementing <see cref="IProjectModel"/>.
/// </summary>
/// <typeparam name="TProjectModel">The concrete project model type produced by the builder.</typeparam>
/// <typeparam name="TBuilder">The concrete builder type (fluent API return type).</typeparam>
/// <remarks>
/// This base provides a fluent API for configuring common MSBuild / NuGet metadata used to produce
/// an <see cref="IProjectModel"/>. It centralizes validation and common helper methods used by
/// concrete project-type builders (for example class, library, webapi project builders).
///
/// Design notes:
/// - Builders derived from this class should implement the actual model creation inside their
///   <c>BuildInternal</c> implementation (see <see cref="AbstractObjectBuilder{TClass,TBuilder}"/>).
/// - Validation is performed by calling <see cref="VisiteObjectAndCollectExceptions"/> from the
///   concrete builder's build pipeline; that method populates an <see cref="ExceptionBuildDictionary"/>
///   with missing or invalid required properties.
/// - Instances of this builder are not thread-safe.
///
/// Example (fluent usage):
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
///     .Description("A sample project");
/// var result = builder.Build();
/// </code>
/// </remarks>
public abstract class AbstractProjectModelBuilder<TProjectModel, TBuilder> : AbstractBuilder<TProjectModel>
    where TProjectModel : class, IProjectModel
    where TBuilder : class, IBuilder<TProjectModel>, new()
{
    /// <summary>
    /// Error message used when the project name is missing or empty.
    /// </summary>
    public const string ErrorProjectNameCannotBeNullOrEmpty = "Project name cannot be null or empty.";

    /// <summary>
    /// Error message used when the project directory is missing or empty.
    /// </summary>
    public const string ErrorProjectDirectoryCannotBeNullOrEmpty = "Project directory cannot be null or empty.";

    /// <summary>
    /// Error message used when the project SDK identifier is missing or empty.
    /// </summary>
    public const string ErrorProjectSdkCannotBeNullOrEmpty = "Project SDK cannot be null or empty.";

    /// <summary>
    /// Error message used when the target framework is missing or empty.
    /// </summary>
    public const string ErrorProjectTargetFrameworkCannotBeNullOrEmpty = "Project target framework cannot be null or empty.";

    /// <summary>
    /// Error message used when the project output type is missing or empty.
    /// </summary>
    public const string ErrorProjectOutputTypeCannotBeNullOrEmpty = "Project output type cannot be null or empty.";

    /// <summary>
    /// Error message used when the C# language version is missing or empty.
    /// </summary>
    public const string ErrorProjectLanguageVersionCannotBeNullOrEmpty = "Project language version cannot be null or empty.";

    /// <summary>
    /// Error message used when the nullable setting has not been supplied.
    /// </summary>
    public const string ErrorProjectNullableCannotBeNullOrEmpty = "Project nullable setting cannot be null.";

    /// <summary>
    /// Error message used when the implicit usings setting has not been supplied.
    /// </summary>
    public const string ErrorPojectImplicitUsingsCannotBeNullOrEmpty = "Project implicit usings setting cannot be null.";

    /// <summary>
    /// Backing field for the project <see cref="Name"/> value.
    /// </summary>
    protected string? _name;

    /// <summary>
    /// Backing field for the project <see cref="Directory(string)"/> value.
    /// </summary>
    protected string? _directory;

    /// <summary>
    /// Backing field for the MSBuild SDK (for example "Microsoft.NET.Sdk").
    /// </summary>
    protected string? _sdk;

    /// <summary>
    /// Backing field for the project's target framework (for example "net9.0").
    /// </summary>
    protected string? _targetFramework;

    /// <summary>
    /// Backing field for the project's output type (for example "Exe" or "Library").
    /// </summary>
    protected string? _outputType;

    /// <summary>
    /// Backing field for the C# language version (for example "13.0").
    /// </summary>
    protected string? _langVersion;

    /// <summary>
    /// Backing field indicating whether nullable reference types are enabled. Null means not set.
    /// </summary>
    protected bool? _nullable;

    /// <summary>
    /// Backing field indicating whether implicit using directives are enabled. Null means not set.
    /// </summary>
    protected bool? _implicitUsings;

    /// <summary>
    /// Represents the package version metadata for the project. May be <c>null</c> if not configured.
    /// </summary>
    /// <remarks>
    /// Derived builders may inspect <c>_packageVersion</c> when composing the final project model or
    /// when deciding whether to generate a package on build.
    /// </remarks>
    protected Reference<IPackageVersion>? _packageVersion;

    /// <summary>
    /// Backing field indicating whether a NuGet package should be generated during build.
    /// </summary>
    protected bool? _generatePackageOnBuild;

    /// <summary>
    /// Short description for the project / package.
    /// </summary>
    protected string? _description;

    /// <summary>
    /// Copyright text to include in the project/package metadata.
    /// </summary>
    protected string? _copyright;

    /// <summary>
    /// The project URL used for the NuGet package metadata.
    /// </summary>
    protected string? _packageProjectUrl;

    /// <summary>
    /// Repository URL for the project (for example the Git repository URL).
    /// </summary>
    protected string? _repositoryUrl;

    /// <summary>
    /// Repository type (common value: "git").
    /// </summary>
    protected string? _repositoryType;

    /// <summary>
    /// Tags to associate with the NuGet package.
    /// </summary>
    protected string? _packageTags;

    /// <summary>
    /// Whether to include symbols (.snupkg) when packaging.
    /// </summary>
    protected bool? _includeSymbols;

    /// <summary>
    /// Whether to enforce code style as part of the build.
    /// </summary>
    protected bool? _enforceCodeStyleInBuild;

    /// <summary>
    /// Stores the version string associated with the containing type.
    /// </summary>
    protected string? _version;

    /// <summary>
    /// Stores the author information associated with the containing type.
    /// </summary>
    protected string? _authors;

    /// <summary>
    /// Sets the version string for the builder and returns the builder instance for method chaining.
    /// </summary>
    /// <param name="version">The version identifier to assign. This value can be any string representing the desired version.</param>
    /// <returns>The builder instance with the updated version, enabling fluent configuration.</returns>
    /// <exception cref="InvalidCastException">Thrown if the current instance cannot be cast to the type specified by TBuilder.</exception>
    public TBuilder Version(string version)
    {
        _version = version;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the author information for the builder and returns the current builder instance.
    /// </summary>
    /// <param name="authors">The author name or names to associate with the builder. This value can be a single name or a delimited list of
    /// names, depending on usage.</param>
    /// <returns>The current builder instance with the specified author information applied.</returns>
    /// <exception cref="InvalidCastException">Thrown if the current instance cannot be cast to the type specified by <typeparamref name="TBuilder"/>.</exception>
    public TBuilder Authors(string authors)
    {
        _authors = authors;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Configure package version details using a <see cref="PackageVersionBuilder"/> action.
    /// </summary>
    /// <param name="body">An action that configures the package version builder.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    /// <remarks>
    /// The provided <paramref name="body"/> configures an internal <see cref="PackageVersionBuilder"/>
    /// which is then materialized and stored in <see cref="_packageVersion"/>. This method throws an
    /// <see cref="InvalidCastException"/> if the generic <typeparamref name="TBuilder"/> cannot be
    /// returned from this instance; that would indicate a misuse of the generic constraints.
    ///
    /// Example:
    /// <code>
    /// builder.PackageVersion(b => b.Version("1.2.3").Suffix("-preview"));
    /// </code>
    /// </remarks>
    public TBuilder PackageVersion(Action<PackageVersionBuilder> body)
    {
        var builder = new PackageVersionBuilder();
        body(builder);
        _packageVersion = builder.Reference();
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Set whether to generate a NuGet package during the build process.
    /// </summary>
    /// <param name="generatePackageOnBuild">True to generate a package; false to skip; null to leave unset.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    /// <remarks>
    /// Many consumers use this to control CI/CD packaging behavior. If left unset, downstream code may decide
    /// a default behavior.
    /// </remarks>
    public TBuilder GeneratePackageOnBuild(bool? generatePackageOnBuild = true)
    {
        _generatePackageOnBuild = generatePackageOnBuild;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Set the project/package description.
    /// </summary>
    /// <param name="description">Human-readable description for the project/package.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder Description(string description)
    {
        _description = description;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Set the copyright metadata for the project/package.
    /// </summary>
    /// <param name="copyright">Copyright statement.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder Copyright(string copyright)
    {
        _copyright = copyright;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Set the project URL to include in NuGet metadata.
    /// </summary>
    /// <param name="packageProjectUrl">URL pointing to the project home or documentation.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder PackageProjectUrl(string packageProjectUrl)
    {
        _packageProjectUrl = packageProjectUrl;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Set the repository URL for the project/package metadata.
    /// </summary>
    /// <param name="repositoryUrl">Repository URL (for example git remote HTTPS URL).</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder RepositoryUrl(string repositoryUrl)
    {
        _repositoryUrl = repositoryUrl;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Set the repository type (most commonly "git").
    /// </summary>
    /// <param name="repositoryType">Repository type identifier.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder RepositoryType(string repositoryType)
    {
        _repositoryType = repositoryType;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Set tags to be associated with the NuGet package.
    /// </summary>
    /// <param name="packageTags">A set of tag strings (duplicates are allowed).</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    /// <remarks>
    /// Tags are often used by package discovery tools and should be lowercase and concise.
    /// Example: <c>builder.PackageTags("library", "dotnet", "csharp")</c>.
    /// </remarks>
    public TBuilder PackageTags(params string[] packageTags)
    {
        _packageTags = string.Join(System.Environment.NewLine, packageTags);
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Controls whether symbols (.snupkg) should be produced during packaging.
    /// </summary>
    /// <param name="includeSymbols">True to include symbols, false to exclude.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder IncludeSymbols(bool includeSymbols)
    {
        _includeSymbols = includeSymbols;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Controls whether code style enforcement should run as part of the build.
    /// </summary>
    /// <param name="enforceCodeStyleInBuild">True to enable enforcement; false to disable; null to leave unset.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    /// <remarks>
    /// Enabling this setting may make builds fail if code style analyzers detect rule violations.
    /// </remarks>
    public TBuilder EnforceCodeStyleInBuild(bool? enforceCodeStyleInBuild = true)
    {
        _enforceCodeStyleInBuild = enforceCodeStyleInBuild;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Backing storage for declared project references.
    /// </summary>
    protected BuilderList<ProjectReference, ProjectReferenceBuilder> _projectReferences = new();

    /// <summary>
    /// Backing storage for NuGet package references.
    /// </summary>
    protected BuilderList<IPackageReference, PackageReferenceBuilder> _packageReferences = new();

    /// <summary>
    /// Backing storage for analyzer package references (Roslyn analyzers, etc.).
    /// </summary>
    protected BuilderList<IPackageReference, PackageReferenceBuilder> _analyzers = new();

    /// <summary>
    /// Additional arbitrary MSBuild properties to include in the project model.
    /// </summary>
    protected Reference<Dictionary<string, object>> _additionalProperties = new();

    /// <summary>
    /// Code declaration models (classes, interfaces, enums) that belong to the project.
    /// </summary>
    protected BuilderList<IDeclarationModel, DeclarationModelBuilder> _declarationModels = new();

    /// <summary>
    /// Set the logical project name.
    /// </summary>
    /// <param name="name">Project name (for example the assembly name / package id).</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    /// <remarks>
    /// The project name is required; if not provided, <see cref="VisiteObjectAndCollectExceptions"/> will
    /// add an exception indicating the missing value.
    /// </remarks>
    public TBuilder Name(string? name)
    {
        _name = name;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public string? GivenName() => _name;

    /// <summary>
    /// Set the file-system directory where the project resides.
    /// </summary>
    /// <param name="directory">Relative or absolute path to the project directory.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder Directory(string? directory)
    {
        _directory = directory;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public string? GivenDirectory() => _directory;

    /// <summary>
    /// Set the MSBuild SDK identifier to use in the project file.
    /// </summary>
    /// <param name="sdk">SDK identifier, e.g. "Microsoft.NET.Sdk".</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder Sdk(string? sdk)
    {
        _sdk = sdk;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Gets the identifier of the SDK associated with the current context.
    /// </summary>
    /// <returns>A string containing the SDK identifier if available; otherwise, null.</returns>
    public string? GivenSdk() => _sdk;

    /// <summary>
    /// Set the target framework used by the project.
    /// </summary>
    /// <param name="targetFramework">Target framework moniker, e.g. "net9.0".</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder TargetFramework(string? targetFramework)
    {
        _targetFramework = targetFramework;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Gets the target framework identifier for the current context, if available.
    /// </summary>
    /// <returns>A string containing the target framework identifier, or null if no target framework is set.</returns>
    public string? GivenTargetFramework() => _targetFramework;

    /// <summary>
    /// Set the project output type.
    /// </summary>
    /// <param name="outputType">Output type such as "Exe" or "Library".</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder OutputType(string? outputType)
    {
        _outputType = outputType;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Gets the output type associated with the current instance.
    /// </summary>
    /// <returns>A string representing the output type, or null if no output type has been set.</returns>
    public string? GivenOutputType() => _outputType;

    /// <summary>
    /// Set the C# language version used to compile the project.
    /// </summary>
    /// <param name="langVersion">Language version string, e.g. "13.0".</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder LangVersion(string? langVersion)
    {
        _langVersion = langVersion;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Gets the C# language version specified for the current context.
    /// </summary>
    /// <returns></returns>
    public string? GivenLangVersion() => _langVersion;

    /// <summary>
    /// Configure nullable reference types behavior for the project.
    /// </summary>
    /// <param name="nullable">True to enable nullable reference types; false to disable.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder Nullable(bool? nullable = true)
    {
        _nullable = nullable;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Configure implicit using directives behavior for the project.
    /// </summary>
    /// <param name="implicitUsings">True to enable implicit usings; false to disable.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder ImplicitUsings(bool? implicitUsings = true)
    {
        _implicitUsings = implicitUsings;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Adds a project reference to the builder and returns the current builder instance for method chaining.
    /// </summary>
    /// <remarks>This method enables fluent configuration by returning the builder instance. The added project
    /// reference will be included in the final build configuration.</remarks>
    /// <param name="projectReferenceBuilder">The project reference to add to the builder. Cannot be null.</param>
    /// <returns>The current builder instance, cast to the type parameter <typeparamref name="TBuilder"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown if the current instance cannot be cast to <typeparamref name="TBuilder"/>.</exception>
    public TBuilder ProjectReference(ProjectReferenceBuilder projectReferenceBuilder)
    {
        _projectReferences.Add(projectReferenceBuilder);
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Adds a package reference to the builder and returns the current builder instance for method chaining.
    /// </summary>
    /// <param name="packageReference">The package reference to add. Cannot be null.</param>
    /// <returns>The current builder instance, allowing for fluent configuration.</returns>
    /// <exception cref="InvalidCastException">Thrown if the current instance cannot be cast to the type specified by <typeparamref name="TBuilder"/>.</exception>
    public TBuilder PackageReference(PackageReferenceBuilder packageReference)
    {
        _packageReferences.Add(packageReference);
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Provide analyzer package references (typically Roslyn analyzers) to include with the project.
    /// </summary>
    /// <param name="analyzer">An analyzer <see cref="IPackageReference"/> instances.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder Analyzer(PackageReferenceBuilder analyzer)
    {
        _analyzers.Add(analyzer);
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Set additional arbitrary MSBuild properties to include in the project model.
    /// </summary>
    /// <param name="additionalProperties">Dictionary of property name to value.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    /// <remarks>
    /// Values are serialized by the project model consumer; prefer primitive values and strings.
    /// </remarks>
    public TBuilder AdditionalProperties(Reference<Dictionary<string, object>> additionalProperties)
    {
        _additionalProperties = additionalProperties;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Sets the collection of additional properties to be associated with the builder instance.
    /// </summary>
    /// <remarks>Use this method to attach custom metadata or configuration options that are not covered by
    /// standard builder properties. The provided dictionary is stored and may be used during subsequent build
    /// operations.</remarks>
    /// <param name="additionalProperties">A dictionary containing key-value pairs representing additional properties to attach. Keys must be non-null and
    /// unique within the dictionary.</param>
    /// <returns>The current builder instance with the specified additional properties applied.</returns>
    /// <exception cref="InvalidCastException">Thrown if the current instance cannot be cast to the generic builder type <typeparamref name="TBuilder"/>.</exception>
    public TBuilder AdditionalProperties(Dictionary<string, object>? additionalProperties)
    {
        _additionalProperties = new Reference<Dictionary<string, object>>(additionalProperties);
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Provide code declaration models (classes, interfaces, enums) that belong to this project.
    /// </summary>
    /// <param name="declarationModel">List of <see cref="IDeclarationModel"/> instances.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public TBuilder DeclarationModel(DeclarationModelBuilder declarationModel)
    {
        _declarationModels.Add(declarationModel);
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Adds the specified package references to the builder.
    /// </summary>
    /// <param name="packageReferences">An enumerable collection of package references to add. Each reference represents a NuGet package dependency to
    /// include in the build configuration.</param>
    /// <returns>The builder instance with the added package references.</returns>
    /// <exception cref="InvalidCastException">Thrown if the current instance cannot be cast to the type specified by <typeparamref name="TBuilder"/>.</exception>
    public TBuilder PackageReferences(IEnumerable<IPackageReference> packageReferences)
    {
        var builder = new BuilderList<IPackageReference, PackageReferenceBuilder>();
        foreach (var packageReference in packageReferences)
        {
            var itemBuilder = new PackageReferenceBuilder();
            itemBuilder.Existing(packageReference);
            builder.Add(itemBuilder);
        }
        _packageReferences.AddRange(builder);
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Adds a package reference to the builder using the specified configuration action.
    /// </summary>
    /// <remarks>Use this method to fluently add and configure package references when building a project or
    /// component. This method supports method chaining.</remarks>
    /// <param name="body">An action that configures the <see cref="PackageReferenceBuilder"/> for the new package reference. Cannot be
    /// null.</param>
    /// <returns>The current builder instance with the added package reference.</returns>
    /// <exception cref="InvalidCastException">Thrown if the current instance cannot be cast to <typeparamref name="TBuilder"/>.</exception>
    public TBuilder PackageReferences(Action<BuilderList<IPackageReference, PackageReferenceBuilder>> body)
    {
        var builder = new BuilderList<IPackageReference, PackageReferenceBuilder>();
        body(builder);
        _packageReferences.AddRange(builder);
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Adds the specified project references to the builder.
    /// </summary>
    /// <param name="projectReferences">An enumerable collection of <see cref="ProjectReference"/> instances to be added as references. Each item
    /// represents a project to reference in the build configuration.</param>
    /// <returns>The current builder instance with the specified project references added.</returns>
    /// <exception cref="InvalidCastException">Thrown if the builder instance cannot be cast to <typeparamref name="TBuilder"/>.</exception>
    public TBuilder ProjectReferences(IEnumerable<ProjectReference> projectReferences)
    {
        var builder = new BuilderList<ProjectReference, ProjectReferenceBuilder>();
        foreach (var projectReference in projectReferences)
        {
            var itemBuilder = new ProjectReferenceBuilder();
            itemBuilder.Existing(projectReference);
            builder.Add(itemBuilder);
        }
        _projectReferences.AddRange(builder);
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    /// <summary>
    /// Validate required project properties and collect exceptions for missing or invalid values.
    /// </summary>
    /// <param name="visited">Visited objects list used for cycle detection when building graphs.</param>
    /// <param name="exceptions">Dictionary to collect exceptions keyed by the invalid property name.</param>
    /// <remarks>
    /// Concrete builders should call this early in their build pipeline to ensure required metadata is
    /// validated consistently. This method does not throw; it records exceptions into the provided
    /// <paramref name="exceptions"/> collection so the build pipeline can return a failure result.
    ///
    /// Example usage from a concrete <c>BuildInternal</c> implementation:
    /// <code>
    /// var exceptions = new ExceptionBuildDictionary();
    /// VisiteObjectAndCollectExceptions(visited, exceptions);
    /// if (exceptions.Count > 0) return Failure(exceptions, visited);
    /// </code>
    /// </remarks>
    protected void VisiteObjectAndCollectExceptions(VisitedObjectDictionary visited, FailuresDictionary exceptions)
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            exceptions.Failure(nameof(_name), new ArgumentNullException(nameof(_name), ErrorProjectNameCannotBeNullOrEmpty));
        }

        if (string.IsNullOrWhiteSpace(_directory))
        {
            exceptions.Failure(nameof(_directory), new ArgumentNullException(nameof(_directory), ErrorProjectDirectoryCannotBeNullOrEmpty));
        }

        if (string.IsNullOrWhiteSpace(_sdk))
        {
            exceptions.Failure(nameof(_sdk), new ArgumentNullException(nameof(_sdk), ErrorProjectSdkCannotBeNullOrEmpty));
        }

        if (string.IsNullOrWhiteSpace(_targetFramework))
        {
            exceptions.Failure(nameof(_targetFramework), new ArgumentNullException(nameof(_targetFramework), ErrorProjectTargetFrameworkCannotBeNullOrEmpty));
        }

        if (string.IsNullOrWhiteSpace(_outputType))
        {
            exceptions.Failure(nameof(_outputType), new ArgumentNullException(nameof(_outputType), ErrorProjectOutputTypeCannotBeNullOrEmpty));
        }

        if (string.IsNullOrWhiteSpace(_langVersion))
        {
            exceptions.Failure(nameof(_langVersion), new ArgumentNullException(nameof(_langVersion), ErrorProjectLanguageVersionCannotBeNullOrEmpty));
        }

        if (_nullable == null)
        {
            exceptions.Failure(nameof(_nullable), new ArgumentNullException(nameof(_nullable), ErrorProjectNullableCannotBeNullOrEmpty));
        }

        if (_implicitUsings == null)
        {
            exceptions.Failure(nameof(_implicitUsings), new ArgumentNullException(nameof(_implicitUsings), ErrorPojectImplicitUsingsCannotBeNullOrEmpty));
        }
    }
}
