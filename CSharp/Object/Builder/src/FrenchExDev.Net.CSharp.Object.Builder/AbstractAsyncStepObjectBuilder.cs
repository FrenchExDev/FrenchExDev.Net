using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Provides an abstract base class for building asynchronous step objects of type <typeparamref name="TClass"/>.
/// </summary>
/// <remarks>This class defines the structure for implementing asynchronous step-based object builders,  including
/// the ability to track whether the current step is the final step and to produce a result asynchronously. Derived
/// classes must implement the <see cref="BuildAsync"/> method to define the specific building logic.</remarks>
/// <typeparam name="TClass">The type of the object being built by the step builder.</typeparam>
public abstract class AbstractAsyncStepObjectBuilder<TClass> : IAsyncStepObjectBuilder<TClass>
{
    protected TClass? _result;

    /// <summary>
    /// Gets a task that represents the asynchronous operation and returns the result of type <typeparamref
    /// name="TClass"/>.
    /// </summary>
    /// <remarks>The returned task may throw an exception if the operation fails. Ensure proper exception
    /// handling when awaiting the task.</remarks>
    public virtual Task<TClass> Result => throw new NotImplementedException();

    /// <summary>
    /// Asynchronously builds the final output based on the provided intermediate data.
    /// </summary>
    /// <remarks>This method processes the provided <paramref name="intermediate"/> data to produce the final
    /// output.  The optional <paramref name="visited"/> dictionary can be used to avoid processing the same objects
    /// multiple times.</remarks>
    /// <param name="exceptions">A list to collect exceptions that occur during the build process.</param>
    /// <param name="intermediate">A dictionary containing the intermediate data used to construct the final output. Cannot be null.</param>
    /// <param name="visited">An optional dictionary to track visited nodes or objects during the build process. Can be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will terminate early if the token is canceled.</param>
    /// <returns>A task that represents the asynchronous build operation.</returns>
    public abstract Task BuildAsync(
        ExceptionBuildList exceptions,
        IntermediateObjectDictionary intermediate,
        VisitedObjectsList visited,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the operation has produced a result.
    /// </summary>
    /// <returns><see langword="true"/> if the operation has a result; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool HasResult()
    {
        return _result is not null && _result is TClass;
    }

    TClass IAbstractStep<TClass>.Result()
    {
        throw new NotImplementedException();
    }
}