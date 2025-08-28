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
    public abstract Task<IBuildResult<TClass>> BuildAsync(CancellationToken cancellationToken = default);
}
