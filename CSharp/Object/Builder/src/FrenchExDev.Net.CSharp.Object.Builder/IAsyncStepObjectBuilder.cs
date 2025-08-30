using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

public interface IAsyncStepObjectBuilder<TClass> : IAbstractStep<TClass>
{
    Task BuildAsync(IntermediateObjectsList intermediate, VisitedObjectsList visited, CancellationToken cancellationToken = default);
}
