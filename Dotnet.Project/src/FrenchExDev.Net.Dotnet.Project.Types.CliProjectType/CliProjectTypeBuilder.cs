using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.Dotnet.Project.Types.CliProjectType.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.CliProjectType;

/// <summary>
/// Represents a configuration for creating a default CLI project model.
/// </summary>
/// <remarks>This class provides a method to generate a default <see cref="CliProjectModel"/> instance with
/// preconfigured settings, such as enabling nullable reference types, setting the language version, and specifying the
/// target framework.</remarks>
public class CliProjectTypeBuilder
{
    /// <summary>
    /// Gets the default configuration for a CLI project model.
    /// </summary>
    public static CliProjectModel Defaults => new CliProjectModelBuilder()
        .Nullable(true)
        .ImplicitUsings(true)
        .LangVersion("13.0")
        .OutputType("Exe")
        .Sdk("Microsoft.NET.Sdk")
        .TargetFramework("net9.0")
        .PackageReferences(body: (listBuild) => listBuild.New((builder) => builder.Name("FrenchExDev.Net.Dotnet.Project.Types.CliProjectType")))
        .Build()
        .Success<CliProjectModel>();

    /// <summary>
    /// Creates a new instance of <see cref="CliProjectModelBuilder"/> initialized with default settings.
    /// </summary>
    /// <remarks>The returned <see cref="CliProjectModelBuilder"/> is preconfigured with default values for 
    /// nullable context, implicit usings, language version, output type, SDK, target framework,  package references,
    /// project references, and additional properties.</remarks>
    /// <returns>A <see cref="CliProjectModelBuilder"/> instance configured with default settings.</returns>
    public CliProjectModelBuilder Default() => new CliProjectModelBuilder()
        .Nullable(Defaults.Nullable)
        .ImplicitUsings(Defaults.ImplicitUsings)
        .LangVersion(Defaults.LangVersion)
        .OutputType(Defaults.OutputType)
        .Sdk(Defaults.Sdk)
        .TargetFramework(Defaults.TargetFramework)
        .PackageReferences(Defaults.PackageReferences)
        .ProjectReferences(Defaults.ProjectReferences)
        .AdditionalProperties(Defaults.AdditionalProperties);
}
