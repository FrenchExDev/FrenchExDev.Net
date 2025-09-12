using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.Dotnet.Project.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.ClassProjectType.Abstractions;

/// <summary>
/// Provides functionality to construct and configure instances of the <see cref="ClassProjectModel"/> class.
/// </summary>
/// <remarks>This class extends <see cref="AbstractProjectModelBuilder{TModel, TBuilder}"/> to provide a fluent
/// API  for building <see cref="ClassProjectModel"/> objects. Use this builder to configure and validate  the model
/// before creating an instance.</remarks>
public class ClassProjectModelBuilder : AbstractProjectModelBuilder<ClassProjectModel, ClassProjectModelBuilder>
{
    /// <summary>
    /// Constructs an object of type <see cref="ClassProjectModel"/> based on the provided context.
    /// </summary>
    /// <param name="exceptions">A collection to which any exceptions encountered during the build process are added. Cannot be null.</param>
    /// <param name="visited">A list of objects that have already been visited during the build process to prevent circular references. Cannot
    /// be null.</param>
    /// <returns>An <see cref="IObjectBuildResult{T}"/> containing the result of the build operation for <see
    /// cref="ClassProjectModel"/>.</returns>
    protected override IObjectBuildResult<ClassProjectModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        VisiteObjectAndCollectExceptions(visited, exceptions);

        if (exceptions.Count > 0)
        {
            return Failure(exceptions, visited);
        }

        return Success(new ClassProjectModel(
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
