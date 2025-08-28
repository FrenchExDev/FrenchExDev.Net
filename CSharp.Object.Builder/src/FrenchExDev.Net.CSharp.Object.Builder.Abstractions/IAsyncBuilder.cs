namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Defines a contract for building an instance of <typeparamref name="TClass"/> asynchronously.
/// </summary>
/// <typeparam name="TClass">The type of the object to be built.</typeparam>
public interface IAsyncBuilder<TClass>
{
    /// <summary>
    /// Asynchronously builds an object of type <typeparamref name="TClass"/> and returns the result.
    /// </summary>
    /// <remarks>The method supports cancellation via the <paramref name="cancellationToken"/> parameter. If
    /// the operation is canceled, the returned task will be in the <see cref="TaskStatus.Canceled"/> state.</remarks>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="IBuildResult{TClass}"/> representing the outcome of the build process.</returns>
    Task<IBuildResult<TClass>> BuildAsync(CancellationToken cancellationToken = default);
}
