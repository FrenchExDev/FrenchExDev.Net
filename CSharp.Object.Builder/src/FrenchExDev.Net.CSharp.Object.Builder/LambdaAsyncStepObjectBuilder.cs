using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Provides a builder for asynchronously constructing an object of type <typeparamref name="TClass"/>  using a
/// user-defined asynchronous build action.
/// </summary>
/// <remarks>This builder allows for the execution of a custom asynchronous build action and provides mechanisms 
/// to set the result of the build process. It is designed to be used in workflows where intermediate  state and
/// cancellation support are required.</remarks>
/// <typeparam name="TClass">The type of the object being built.</typeparam>
public class LambdaAsyncStepObjectBuilder<TClass> : IAsyncStepObjectBuilder<TClass>
{
    /// <summary>
    /// Holds the user-defined asynchronous build action to be executed during the build process.
    /// </summary>
    private readonly Func<ExceptionBuildDictionary, IntermediateObjectDictionary, VisitedObjectsList, CancellationToken, Task> _buildAction;

    /// <summary>
    /// Holds the result of the build process, which can be set using the <see cref="Set"/> method.
    /// </summary>
    private TClass? _result;

    /// <summary>
    /// Gets the result of the operation as a task.
    /// </summary>
    public TClass Result() => _result is not null ? _result : throw new InvalidOperationException();

    /// <summary>
    /// Constructor for creating a new instance of <see cref="LambdaAsyncStepObjectBuilder{TClass}"/> with the specified
    /// </summary>
    /// <param name="buildAction"></param>
    public LambdaAsyncStepObjectBuilder(Func<ExceptionBuildDictionary, IntermediateObjectDictionary, VisitedObjectsList, CancellationToken, Task> buildAction)
    {
        _buildAction = buildAction;
    }

    /// <summary>
    /// Sets the result of the step to the provided instance of <typeparamref name="TClass"/>.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public LambdaAsyncStepObjectBuilder<TClass> Set(TClass result)
    {
        _result = result;
        return this;
    }

    /// <summary>
    /// Calls the provided build action to perform the step asynchronously.
    /// </summary>
    /// <param name="intermediates"></param>
    /// <param name="visited"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task BuildAsync(ExceptionBuildDictionary exceptions, IntermediateObjectDictionary intermediates, VisitedObjectsList visited, CancellationToken cancellationToken = default)
    {
        return _buildAction(exceptions, intermediates, visited, cancellationToken);
    }

    /// <summary>
    /// Returns whether the operation has a result.
    /// </summary>
    /// <returns></returns>
    public bool HasResult()
    {
        return _result is not null && _result is TClass;
    }
}
