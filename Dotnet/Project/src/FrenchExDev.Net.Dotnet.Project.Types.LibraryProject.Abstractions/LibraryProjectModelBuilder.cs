using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.Dotnet.Project.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.LibraryProject.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="LibraryProjectModel"/> instances.
/// Provides a fluent API for configuring library project metadata, references, and code declarations.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
/// <remarks>
/// Use this builder to create and configure a library project model for code generation, scaffolding, or automated tooling scenarios.
/// Example usage:
/// <code>
/// var builder = new LibraryProjectModelBuilder()
///     .Name("MyLibrary")
///     .Directory("src/MyLibrary")
///     .Sdk("Microsoft.NET.Sdk")
///     .TargetFramework("net9.0")
///     .OutputType("Library")
///     .LangVersion("13.0")
///     .Nullable(true)
///     .ImplicitUsings(true);
/// var result = builder.Build();
/// </code>
/// </remarks>
public class LibraryProjectModelBuilder : AbstractProjectModelBuilder<LibraryProjectModel, LibraryProjectModelBuilder>
{
    /// <summary>
    /// Builds the <see cref="LibraryProjectModel"/> instance, validating required properties and collecting exceptions.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    /// <remarks>
    /// This method ensures that all required project metadata is set. If any required property is missing, a failure result is returned.
    /// Example:
    /// <code>
    /// var result = builder.Build();
    /// if (result.IsSuccess) { /* use result.Success<LibraryProjectModel>() */ }
    /// </code>
    /// </remarks>
    protected override IObjectBuildResult<LibraryProjectModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        VisiteObjectAndCollectExceptions(visited, exceptions);

        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        return Success(new LibraryProjectModel(
            _name ?? throw new InvalidDataException(nameof(_name)),
            _directory ?? throw new InvalidDataException(nameof(_directory)),
            _sdk ?? throw new InvalidDataException(nameof(_sdk)),
            _targetFramework ?? throw new InvalidDataException(nameof(_targetFramework)),
            _outputType ?? throw new InvalidDataException(nameof(_outputType)),
            _langVersion ?? throw new InvalidDataException(nameof(_langVersion)),
            _nullable ?? throw new InvalidDataException(nameof(_nullable)),
            _implicitUsings ?? throw new InvalidDataException(nameof(_implicitUsings)),
            _projectReferences ?? throw new InvalidDataException(nameof(_projectReferences)),
            _packageReferences ?? throw new InvalidDataException(nameof(_packageReferences)),
            _analyzers ?? throw new InvalidDataException(nameof(_analyzers)),
            _additionalProperties ?? throw new InvalidDataException(nameof(_additionalProperties)),
            _declarationModels ?? throw new InvalidDataException(nameof(_declarationModels))
            ));
    }
}
