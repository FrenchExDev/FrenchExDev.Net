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
    public string Name { get; protected set; }

    /// <summary>
    /// Gets the directory path where the project is located.
    /// </summary>
    /// <example>"C:/Projects/MyLibrary"</example>
    public string Directory { get; protected set; }

    /// <summary>
    /// Gets the SDK used by the project (e.g., "Microsoft.NET.Sdk").
    /// </summary>
    /// <example>"Microsoft.NET.Sdk"</example>
    public string Sdk { get; protected set; }

    /// <summary>
    /// Gets the target framework for the project (e.g., ".NET 9").
    /// </summary>
    /// <example>"net9.0"</example>
    public string TargetFramework { get; protected set; }

    /// <summary>
    /// Gets the output type of the project (e.g., "Library", "Exe").
    /// </summary>
    /// <example>"Library"</example>
    public string OutputType { get; protected set; }

    /// <summary>
    /// Gets the C# language version used in the project.
    /// </summary>
    /// <example>"13.0"</example>
    public string LangVersion { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether nullable reference types are enabled.
    /// </summary>
    /// <remarks>Defaults to <c>true</c>.</remarks>
    public bool Nullable { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether implicit global usings are enabled.
    /// </summary>
    /// <remarks>Defaults to <c>true</c>.</remarks>
    public bool ImplicitUsings { get; protected set; }

    /// <summary>
    /// Gets the list of project references included in this project.
    /// </summary>
    /// <remarks>
    /// Example: referencing another project in the solution.
    /// <code>
    /// ProjectReferences.Add(new ProjectReference("..\\OtherProject\\OtherProject.csproj"));
    /// </code>
    /// </remarks>
    public List<ProjectReference> ProjectReferences { get; protected set; } = new();

    /// <summary>
    /// Gets the list of NuGet package references for this project.
    /// </summary>
    /// <remarks>
    /// Example: referencing Newtonsoft.Json package.
    /// <code>
    /// PackageReferences.Add(new PackageReference("Newtonsoft.Json", "13.0.1"));
    /// </code>
    /// </remarks>
    public List<IPackageReference> PackageReferences { get; protected set; } = new();

    /// <summary>
    /// Gets the list of analyzer package references for this project.
    /// </summary>
    /// <remarks>
    /// Example: referencing Roslyn analyzers.
    /// <code>
    /// Analyzers.Add(new PackageReference("Microsoft.CodeAnalysis.FxCopAnalyzers", "3.3.2"));
    /// </code>
    /// </remarks>
    public List<IPackageReference> Analyzers { get; protected set; } = new();

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
    public Dictionary<string, object> AdditionalProperties { get; protected set; } = new();

    /// <summary>
    /// Gets the list of declaration models (e.g., classes, interfaces) defined in the project.
    /// </summary>
    /// <remarks>
    /// Example:
    /// <code>
    /// DeclarationModels.Add(new ClassDeclarationModel("MyClass"));
    /// </code>
    /// </remarks>
    public List<IDeclarationModel> DeclarationModels { get; protected set; } = new List<IDeclarationModel>();

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
        string name,
        string directory,
        string sdk,
        string targetFramework,
        string outputType,
        string langVersion,
        bool nullable,
        bool implicitUsings,
        List<ProjectReference> projectReferences,
        List<IPackageReference> packageReferences,
        List<IPackageReference> analyzers,
        Dictionary<string, object> additionalProperties,
        List<IDeclarationModel> declarationModels
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
        ProjectReferences = projectReferences;
        PackageReferences = packageReferences;
        Analyzers = analyzers;
        AdditionalProperties = additionalProperties;
        DeclarationModels = declarationModels;
    }
}
