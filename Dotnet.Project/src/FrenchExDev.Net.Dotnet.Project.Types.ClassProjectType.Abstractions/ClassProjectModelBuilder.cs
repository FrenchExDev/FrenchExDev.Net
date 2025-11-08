using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.Dotnet.Project.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.ClassProjectType.Abstractions;

/// <summary>
/// Provides functionality to construct and configure instances of the <see cref="ClassProjectModel"/> class.
/// </summary>
/// <remarks>This class extends <see cref="AbstractProjectModelBuilder{TModel, TBuilder}"/> to provide a fluent
/// API  for building <see cref="ClassProjectModel"/> objects. Use this builder to configure and validate  the model
/// before creating an instance.</remarks>
public class ClassProjectModelBuilder : AbstractProjectModelBuilder<ClassProjectModel, ClassProjectModelBuilder>, IBuilder<ClassProjectModel>
{
    /// <summary>
    /// Builds internal collections of project references, package references, analyzers, and declaration models for the
    /// current object.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during the build process. This helps prevent
    /// redundant processing and circular references.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        BuildList(_projectReferences, visitedCollector);
        BuildList(_packageReferences, visitedCollector);
        BuildList(_analyzers, visitedCollector);
        BuildList(_declarationModels, visitedCollector);
    }

    /// <summary>
    /// Performs validation of the object's required properties and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each entry represents a property that failed validation
    /// and the associated error information.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(Name), failures, (s) => new InvalidDataException(s));
        AssertNotNullOrEmptyOrWhitespace(_directory, nameof(Directory), failures, (s) => new InvalidDataException(s));
        AssertNotNullOrEmptyOrWhitespace(_sdk, nameof(Sdk), failures, (s) => new InvalidDataException(s));
        AssertNotNullOrEmptyOrWhitespace(_targetFramework, nameof(TargetFramework), failures, (s) => new InvalidDataException(s));
        AssertNotNullOrEmptyOrWhitespace(_outputType, nameof(OutputType), failures, (s) => new InvalidDataException(s));
        AssertNotNullOrEmptyOrWhitespace(_langVersion, nameof(LangVersion), failures, (s) => new InvalidDataException(s));
        AssertNotNullOrEmptyOrWhitespace(_version, nameof(Version), failures, (s) => new InvalidDataException(s));
        AssertNotNullOrEmptyOrWhitespace(_authors, nameof(Authors), failures, (s) => new InvalidDataException(s));
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ClassProjectModel"/> class using the current configuration settings.
    /// </summary>
    /// <returns>A <see cref="ClassProjectModel"/> object initialized with the configured project properties.</returns>
    protected override ClassProjectModel Instantiate()
    {
        return new ClassProjectModel(
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
}
