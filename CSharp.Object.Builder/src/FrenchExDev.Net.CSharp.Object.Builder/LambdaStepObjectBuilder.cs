using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Provides a builder for constructing objects of type <typeparamref name="TClass"/> using a lambda-based build action.
/// </summary>
/// <remarks>This builder allows for stepwise construction of an object, with the ability to execute a custom
/// build action and track intermediate states, exceptions, and visited objects during the build process.</remarks>
/// <typeparam name="TClass">The type of object being built.</typeparam>
public class LambdaStepObjectBuilder<TClass> : IStepObjectBuilder<TClass>
{
    /// <summary>
    /// Holds the action to be executed during the build process.
    /// </summary>
    private readonly Action<LambdaStepObjectBuilder<TClass>, ExceptionBuildDictionary, IntermediateObjectDictionary, VisitedObjectsList> _buildAction;

    /// <summary>
    /// Holds the result of the building process for this step.
    /// </summary>
    private TClass? _result;

    /// <summary>
    /// Returns the result of the building process for this step.
    /// </summary>
    public virtual TClass Result() => _result is not null ? _result : throw new InvalidOperationException("Result not available");


    /// <summary>
    /// Constructor for creating a new instance of <see cref="LambdaStepObjectBuilder{TClass}"/> with the specified
    /// </summary>
    /// <param name="isFinal"></param>
    /// <param name="buildAction"></param>
    public LambdaStepObjectBuilder(Action<LambdaStepObjectBuilder<TClass>, ExceptionBuildDictionary, IntermediateObjectDictionary, VisitedObjectsList> buildAction)
    {
        _buildAction = buildAction;
    }

    /// <summary>
    /// Determines whether the current instance contains a valid result of the expected type.
    /// </summary>
    /// <returns><see langword="true"/> if the result is not <see langword="null"/> and is of the expected type; otherwise, <see
    /// langword="false"/>.</returns>
    public virtual bool HasResult()
    {
        return _result is not null && _result is TClass;
    }

    /// <summary>
    /// Sets the result of the step to the provided instance of <typeparamref name="TClass"/>.
    /// </summary>
    /// <param name="result"></param>
    public void Set(TClass result)
    {
        _result = result;
    }

    /// <summary>
    /// Executes the build process using the specified lists of exceptions, intermediate objects, and visited objects.
    /// </summary>
    /// <param name="exceptions">A list to collect exceptions encountered during the build process.</param>
    /// <param name="intermediates">A list to store intermediate objects generated during the build process.</param>
    /// <param name="visited">A list to track objects that have already been processed to prevent duplication.</param>
    /// <inheritdoc/>
    public void Build(ExceptionBuildDictionary exceptions, IntermediateObjectDictionary intermediates, VisitedObjectsList visited)
    {
        _buildAction(this, exceptions, intermediates, visited);
    }
}
