using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Provides an abstract base class for building asynchronous objects of type <typeparamref name="TClass"/>.
/// </summary>
/// <remarks>This class defines the contract for implementing asynchronous builders, requiring derived classes to
/// provide an implementation of the <see cref="BuildAsync(CancellationToken)"/> method.</remarks>
/// <typeparam name="TClass">The type of object to be built asynchronously.</typeparam>
public abstract class AbstractAsyncBuilder<TClass> : IAsyncBuilder<TClass>
{
    /// <summary>
    /// Asynchronously builds an object of type <typeparamref name="TClass"/>.
    /// </summary>
    /// <remarks>The method performs the build operation asynchronously and returns an <see
    /// cref="IBuildResult{TClass}"/> that encapsulates the result of the operation, including any errors or success
    /// state. Callers can use the <paramref name="cancellationToken"/> to cancel the operation if needed.</remarks>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="IBuildResult{TClass}"/> representing the outcome of the build operation.</returns>
    public async Task<IBuildResult<TClass>> BuildAsync(Dictionary<object, object>? visited = null, CancellationToken cancellationToken = default)
    {
        visited = new Dictionary<object, object>();

        if (visited.TryGetValue(this, out var existing))
        {
            return new SuccessResult<TClass>((TClass)existing);
        }

        return await BuildInternalAsync(visited, cancellationToken);
    }

    /// <summary>
    /// Asynchronously builds an object of type <typeparamref name="TClass"/> using the specified context and
    /// cancellation token.
    /// </summary>
    /// <remarks>This method is intended to be implemented by derived classes to define the specific logic for
    /// building an object of type <typeparamref name="TClass"/>.</remarks>
    /// <param name="visited">A dictionary used to track visited objects during the build process to prevent circular references. Can be <see
    /// langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="IBuildResult{TClass}"/> representing the outcome of the build process.</returns>
    protected abstract Task<IBuildResult<TClass>> BuildInternalAsync(Dictionary<object, object> visited, CancellationToken cancellationToken);

    /// <summary>
    /// Creates an asynchronous failure result with the specified exceptions, and visited objects.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder implementing <see cref="IAsyncBuilder{TClass}"/>.</typeparam>
    /// <param name="message">The error message describing the failure.</param>
    /// <param name="exceptions">A collection of exceptions associated with the failure.</param>
    /// <param name="visited">A dictionary of objects that have already been processed, used to prevent cyclic references.</param>
    /// <returns>An <see cref="AsyncFailureResult{TClass, TBuilder}"/> representing the failure state.</returns>
    protected AsyncFailureResult<TClass, TBuilder> AsyncFailureResult<TBuilder>(IEnumerable<Exception> exceptions, Dictionary<object, object> visited) where TBuilder : IAsyncBuilder<TClass>
    {
        return new AsyncFailureResult<TClass, TBuilder>((TBuilder)(this as IAsyncBuilder<TClass>), exceptions, visited);
    }
}
