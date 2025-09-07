using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

public interface IStepObjectBuilder<TClass> : IAbstractStep<TClass>
{
    /// <summary>
    /// Constructs the final object graph by processing the provided intermediate representation.
    /// </summary>
    /// <remarks>This method processes the <paramref name="intermediate"/> dictionary to construct the final
    /// object graph.  If circular references are possible, provide a <paramref name="visited"/> dictionary to ensure
    /// proper handling.</remarks>
    /// <param name="intermediate">A dictionary representing the intermediate state of the object graph. This dictionary must not be null.</param>
    /// <param name="visited">An optional dictionary used to track already visited objects during the build process to prevent circular
    /// references. </param>
    void Build(ExceptionBuildList exceptions, IntermediateObjectDictionary intermediate, VisitedObjectsList visited);

    /// <summary>
    /// Sets the result of the build operation.
    /// </summary>
    /// <param name="result"></param>
    void Set(TClass result);
}
