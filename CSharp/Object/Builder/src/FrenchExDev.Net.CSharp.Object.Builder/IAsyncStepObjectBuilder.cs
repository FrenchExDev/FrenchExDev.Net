using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Defines a contract for building asynchronous step objects of type <typeparamref name="TClass"/>.
/// </summary>
/// <remarks>This interface is intended to be implemented by classes that perform asynchronous operations to
/// construct or configure objects of type <typeparamref name="TClass"/>. It extends the  <see
/// cref="IAbstractStep{TClass}"/> interface, providing additional functionality for asynchronous workflows.</remarks>
/// <typeparam name="TClass">The type of the object being built by the implementation.</typeparam>
public interface IAsyncStepObjectBuilder<TClass> : IAbstractStep<TClass>
{
    /// <summary>
    /// Asynchronously builds the final output by processing the provided lists of exceptions, intermediate objects, and
    /// visited objects.
    /// </summary>
    /// <remarks>This method processes the provided lists to generate the final output. Ensure that the lists
    /// are properly populated before calling this method. The operation can be canceled by passing a cancellation
    /// token.</remarks>
    /// <param name="exceptions">A list of exceptions to be processed during the build operation.</param>
    /// <param name="intermediate">A list of intermediate objects that contribute to the final output.</param>
    /// <param name="visited">A list of objects that have already been visited to avoid redundant processing.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous build operation.</returns>
    Task BuildAsync(
        ExceptionBuildList exceptions,
        IntermediateObjectDictionary intermediate,
        VisitedObjectsList visited,
        CancellationToken cancellationToken = default);
}
