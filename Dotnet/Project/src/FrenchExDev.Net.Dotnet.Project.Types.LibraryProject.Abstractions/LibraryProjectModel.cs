using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using FrenchExDev.Net.Dotnet.Project.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.LibraryProject.Abstractions;

/// <summary>
/// Represents a library project model, providing metadata and configuration details for a library project within a
/// solution.
/// </summary>
/// <remarks>This class encapsulates information about a library project, including its name, directory, SDK,
/// target framework, output type, language version, and various project-specific settings such as nullability and
/// implicit usings. It also includes references to other projects, package dependencies, analyzers, and additional
/// properties or declarations associated with the project.</remarks>
public class LibraryProjectModel : AbstractProjectModel<LibraryProjectModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryProjectModel"/> class with the specified project configuration details.
    /// </summary>
    /// <param name="name">The name of the library project.</param>
    /// <param name="directory">The directory path where the project is located.</param>
    /// <param name="sdk">The SDK used by the project (e.g., "Microsoft.NET.Sdk").</param>
    /// <param name="targetFramework">The target framework for the project (e.g., "net9.0").</param>
    /// <param name="outputType">The output type of the project (e.g., "Library").</param>
    /// <param name="langVersion">The C# language version used by the project (e.g., "13.0").</param>
    /// <param name="nullable">Indicates whether nullable reference types are enabled.</param>
    /// <param name="implicitUsings">Indicates whether implicit global usings are enabled.</param>
    /// <param name="projectReferences">A list of project references included in the project.</param>
    /// <param name="packageReferences">A list of NuGet package references included in the project.</param>
    /// <param name="analyzers">A list of analyzers applied to the project.</param>
    /// <param name="additionalProperties">A dictionary of additional properties for the project.</param>
    /// <param name="declarationModels">A list of declaration models representing the code structure of the project.</param>
    /// <remarks>
    /// Example usage:
    /// <code>
    /// var model = new LibraryProjectModel(
    ///     "MyLibrary",
    ///     "src/MyLibrary",
    ///     "Microsoft.NET.Sdk",
    ///     "net9.0",
    ///     "Library",
    ///     "13.0",
    ///     true,
    ///     true,
    ///     new List<ProjectReference>(),
    ///     new List<IPackageReference>(),
    ///     new List<IPackageReference>(),
    ///     new Dictionary<string, object>(),
    ///     new List<IDeclarationModel>()
    /// );
    /// </code>
    /// </remarks>
    public LibraryProjectModel(
        string name,
        string directory,
        string sdk,
        string targetFramework,
        string outputType,
        string langVersion,
        bool nullable,
        bool implicitUsings,
        ReferenceList<ProjectReference> projectReferences,
        ReferenceList<IPackageReference> packageReferences,
        ReferenceList<IPackageReference> analyzers,
        Reference<Dictionary<string, object>> additionalProperties,
        ReferenceList<IDeclarationModel> declarationModels,
        string version,
        bool generatePackageOnBuild,
        string packageTags,
        string authors
    ) : base(name, directory, sdk, targetFramework, outputType, langVersion, nullable, implicitUsings, projectReferences, packageReferences, analyzers, additionalProperties, declarationModels, version, generatePackageOnBuild, packageTags, authors)
    {
    }
}