using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.Dotnet.Project.Abstractions;
using FrenchExDev.Net.Dotnet.Project.Types.ClassProjectType.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.ClassProjectType;

/// <summary>
/// Provides functionality for working with project configurations in a .NET environment.
/// </summary>
/// <remarks>This class offers a default project configuration through the <see cref="Defaults"/> property and
/// provides a method to create a new <see cref="ClassProjectModelBuilder"/> instance preconfigured with the default
/// settings. It is designed to simplify the creation and management of project models with consistent default
/// values.</remarks>
public class ClassProjectType
{
    /// <summary>
    /// Gets the default configuration for a class project model.
    /// </summary>
    public static ClassProjectModel Defaults => new ClassProjectModelBuilder()
        .Nullable(true)
        .ImplicitUsings(true)
        .LangVersion("13.0")
        .OutputType("Library")
        .Sdk("Microsoft.NET.Sdk")
        .TargetFramework("net9.0")
        .GeneratePackageOnBuild()
        .Authors("")
        .PackageTags(Array.Empty<string>())
        .Version("0.0.1")
        .Name("Dummy")
        .Directory("/tmp")
        .Authors("non empty for defaults")
        .Build()
        .Success<ClassProjectModel>();

    /// <summary>
    /// Configures the <see cref="ClassProjectModelBuilder"/> instance with default settings.
    /// </summary>
    /// <remarks>This method initializes the builder with a predefined set of default values for various
    /// project settings,  such as nullable reference types, implicit usings, language version, output type, SDK, target
    /// framework,  package references, project references, and additional properties.  It is a convenient way to start
    /// with a standard configuration before making further customizations.</remarks>
    /// <returns>A new instance of <see cref="ClassProjectModelBuilder"/> preconfigured with default settings.</returns>
    public static ClassProjectModelBuilder Default(ClassProjectModelBuilder classProjectModelBuilder) => classProjectModelBuilder
        .Nullable(Defaults.Nullable)
        .ImplicitUsings(Defaults.ImplicitUsings)
        .LangVersion(Defaults.LangVersion)
        .OutputType(Defaults.OutputType)
        .Sdk(Defaults.Sdk)
        .TargetFramework(Defaults.TargetFramework)
        .GeneratePackageOnBuild(Defaults.GeneratePackageOnBuild)
        .PackageReferences(Defaults.PackageReferences)
        .ProjectReferences(Defaults.ProjectReferences ?? [])
        .AdditionalProperties(Defaults.AdditionalProperties ?? [])
        ;
}