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
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        BuildList(_projectReferences, visitedCollector);
        BuildList(_packageReferences, visitedCollector);
        BuildList(_analyzers, visitedCollector);
        BuildList(_declarationModels, visitedCollector);
    }

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

    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        VisiteObjectAndCollectExceptions(visitedCollector, failures);
    }
}
