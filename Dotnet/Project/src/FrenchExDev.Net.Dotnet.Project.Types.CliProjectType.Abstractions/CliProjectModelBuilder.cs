using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
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
public class CliProjectModelBuilder : AbstractProjectModelBuilder<CliProjectModel, CliProjectModelBuilder>
{
    /// <summary>
    /// Builds the <see cref="CliProjectModel"/> instance, validating required properties and collecting exceptions.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    /// <remarks>
    /// This method ensures that all required project metadata is set. If any required property is missing, a failure result is returned.
    /// Example:
    /// <code>
    /// var result = builder.Build();
    /// if (result.IsSuccess) { /* use result.Success<CliProjectModel>() */ }
    /// </code>
    /// </remarks>
    protected override IObjectBuildResult<CliProjectModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        VisiteObjectAndCollectExceptions(visited, exceptions);

        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        return Success(new CliProjectModel(
            _name,
            _directory,
            _sdk,
            _targetFramework,
            _outputType,
            _langVersion,
            _nullable,
            _implicitUsings,
            _projectReferences,
            _packageReferences,
            _analyzers,
            _additionalProperties,
            _declarationModels,
            _version,
            _generatePackageOnBuild,
            _packageTags,
            _authors
        ));
    }
}