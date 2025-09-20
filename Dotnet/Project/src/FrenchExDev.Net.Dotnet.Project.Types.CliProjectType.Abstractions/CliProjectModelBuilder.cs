using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.Dotnet.Project.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.CliProjectType.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="CliProjectModel"/> instances.
/// Provides a fluent API for configuring CLI project metadata, references, and code declarations.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
/// <remarks>
/// Use this builder to create and configure a CLI project model for code generation, scaffolding, or automated tooling scenarios.
/// Example usage:
/// <code>
/// var builder = new CliProjectModelBuilder()
///     .Name("MyCliApp")
///     .Directory("src/MyCliApp")
///     .Sdk("Microsoft.NET.Sdk")
///     .TargetFramework("net9.0")
///     .OutputType("Exe")
///     .LangVersion("13.0")
///     .Nullable(true)
///     .ImplicitUsings(true);
/// var result = builder.Build();
/// </code>
/// </remarks>
public class CliProjectModelBuilder : AbstractProjectModelBuilder<CliProjectModel, CliProjectModelBuilder>, IBuilder<CliProjectModel>
{
    protected override CliProjectModel Instantiate()
    {
        return new CliProjectModel(
             _name,
             _directory,
             _sdk,
             _targetFramework,
             _outputType,
             _langVersion,
             _nullable,
             _implicitUsings,
             _projectReferences.AsReferenceList(),
             _packageReferences.AsReferenceList(),
             _analyzers.AsReferenceList(),
             _additionalProperties,
             _declarationModels.AsReferenceList(),
             _version,
             _generatePackageOnBuild,
             _packageTags,
             _authors
         );
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        VisiteObjectAndCollectExceptions(visitedCollector, failures);
    }
}