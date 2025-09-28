using FrenchExDev.Net.CSharp.Object.Builder2;
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
public class LibraryProjectModelBuilder : AbstractProjectModelBuilder<LibraryProjectModel, LibraryProjectModelBuilder>, IBuilder<LibraryProjectModel>
{
    /// <summary>
    /// Creates and returns a new instance of the library project model using the configured properties.
    /// </summary>
    /// <remarks>This method should be called after all necessary properties have been configured. All
    /// required fields must be set before calling this method; otherwise, an exception will be thrown.</remarks>
    /// <returns>A <see cref="LibraryProjectModel"/> initialized with the current configuration values.</returns>
    /// <exception cref="InvalidDataException">Thrown if any required configuration property is not set or contains invalid data.</exception>
    protected override LibraryProjectModel Instantiate()
    {
        return new LibraryProjectModel(
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
            _additionalProperties,
            _declarationModels.AsReferenceList() ?? throw new InvalidDataException(nameof(_declarationModels)),
            _version ?? throw new InvalidDataException(nameof(_version)),
            _generatePackageOnBuild ?? throw new InvalidDataException(nameof(_generatePackageOnBuild)),
            _packageTags ?? throw new InvalidDataException(nameof(_packageTags)),
            _authors ?? throw new InvalidDataException(nameof(_authors))
            );
    }

    /// <summary>
    /// Performs validation on the current object and collects any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and handle circular references.</param>
    /// <param name="failures">A dictionary that accumulates validation failures found during the validation process. Each entry represents a
    /// specific failure associated with an object.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        VisiteObjectAndCollectExceptions(visitedCollector, failures);
    }
}
