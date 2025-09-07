using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using FrenchExDev.Net.Dotnet.Project.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.CliProjectType.Abstractions;

/// <summary>
/// Represents a CLI (Command-Line Interface) project model, providing metadata and configuration details for a CLI application within a solution.
/// </summary>
/// <remarks>
/// This class encapsulates information about a CLI project, including its name, directory, SDK, target framework, output type, language version, nullability, implicit usings, references, packages, analyzers, additional properties, and code declarations.
/// Example usage:
/// <code>
/// var cliProject = new CliProjectModel(
///     "MyCliApp",
///     "src/MyCliApp",
///     "Microsoft.NET.Sdk",
///     "net9.0",
///     "Exe",
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
public class CliProjectModel : AbstractProjectModel<CliProjectModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CliProjectModel"/> class with the specified project configuration details.
    /// </summary>
    /// <param name="name">The name of the CLI project.</param>
    /// <param name="directory">The directory path where the project is located.</param>
    /// <param name="sdk">The SDK used by the project (e.g., "Microsoft.NET.Sdk").</param>
    /// <param name="targetFramework">The target framework for the project (e.g., "net9.0").</param>
    /// <param name="outputType">The output type of the project (e.g., "Exe").</param>
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
    /// var cliProject = new CliProjectModel(
    ///     "MyCliApp",
    ///     "src/MyCliApp",
    ///     "Microsoft.NET.Sdk",
    ///     "net9.0",
    ///     "Exe",
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
    public CliProjectModel(
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
    ) : base(name, directory, sdk, targetFramework, outputType, langVersion, nullable, implicitUsings, projectReferences, packageReferences, analyzers, additionalProperties, declarationModels)
    {
    }
}
