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
    /// Initializes a new instance of the <see cref="AbstractProjectModel{T}"/> class with the specified project metadata.
    /// </summary>
    /// <param name="name">Project name.</param>
    /// <param name="directory">Project directory path.</param>
    /// <param name="sdk">Project SDK.</param>
    /// <param name="targetFramework">Target framework.</param>
    /// <param name="outputType">Output type.</param>
    /// <param name="langVersion">C# language version.</param>
    /// <param name="nullable">Enable nullable reference types (default: true).</param>
    /// <param name="implicitUsings">Enable implicit global usings (default: true).</param>
    /// <remarks>
    /// This constructor sets up the basic project metadata. Derived classes may extend or customize initialization.
    /// </remarks>
    public AbstractProjectModel(string name, string directory, string sdk, string targetFramework, string outputType, string langVersion, bool nullable = true, bool implicitUsings = true)
    {
        Name = name;
        Directory = directory;
        Sdk = sdk;
        TargetFramework = targetFramework;
        OutputType = outputType;
        LangVersion = langVersion;
        Nullable = nullable;
        ImplicitUsings = implicitUsings;
    }
}
