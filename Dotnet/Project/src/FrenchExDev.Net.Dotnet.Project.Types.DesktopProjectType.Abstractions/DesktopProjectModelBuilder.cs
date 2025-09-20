using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.Dotnet.Project.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.DesktopProjectType.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="DesktopProjectModel"/> instances.
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
public class DesktopProjectModelBuilder : AbstractProjectModelBuilder<DesktopProjectModel, DesktopProjectModelBuilder>, IBuilder<DesktopProjectModel>
{
    /// <summary>
    /// Creates and returns a new instance of the desktop project model using the current configuration values.
    /// </summary>
    /// <remarks>All configuration fields must be set before calling this method. If any field is null or
    /// invalid, an <see cref="InvalidDataException"/> is thrown to indicate incomplete or incorrect project
    /// setup.</remarks>
    /// <returns>A <see cref="DesktopProjectModel"/> initialized with the configured project settings.</returns>
    /// <exception cref="InvalidDataException">Thrown if any required configuration value is missing or invalid.</exception>
    protected override DesktopProjectModel Instantiate()
    {
        return new DesktopProjectModel(
             _name ?? throw new InvalidDataException(nameof(_name)),
             _directory ?? throw new InvalidDataException(nameof(_directory)),
             _sdk ?? throw new InvalidDataException(nameof(_sdk)),
             _targetFramework ?? throw new InvalidDataException(nameof(_targetFramework)),
             _outputType ?? throw new InvalidDataException(nameof(_outputType)),
             _langVersion ?? throw new InvalidDataException(nameof(_langVersion)),
             _nullable ?? throw new InvalidDataException(nameof(_nullable)),
             _implicitUsings ?? throw new InvalidDataException(nameof(_implicitUsings)),
             _projectReferences.AsReferenceList() ?? throw new InvalidDataException(nameof(_projectReferences)),
             _packageReferences.AsReferenceList() ?? throw new InvalidDataException(nameof(_packageReferences)),
             _analyzers.AsReferenceList() ?? throw new InvalidDataException(nameof(_analyzers)),
             _additionalProperties ?? throw new InvalidDataException(nameof(_additionalProperties)),
             _declarationModels.AsReferenceList() ?? throw new InvalidDataException(nameof(_declarationModels)),
             _version ?? throw new InvalidDataException(nameof(_version)),
             _generatePackageOnBuild ?? throw new InvalidDataException(nameof(_generatePackageOnBuild)),
             _packageTags ?? throw new InvalidDataException(nameof(_packageTags)),
             _authors ?? throw new InvalidDataException(nameof(_authors))
         );
    }

    /// <summary>
    /// Validates the current object and collects any validation failures.
    /// </summary>
    /// <param name="visitedCollector"></param>
    /// <param name="failures"></param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        VisiteObjectAndCollectExceptions(visitedCollector, failures);
    }
}