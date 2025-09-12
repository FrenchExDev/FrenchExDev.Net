using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Provides an abstract base class for building asynchronous objects of type <typeparamref name="TClass"/>.
/// </summary>
/// <remarks>This class defines the contract for implementing asynchronous builders, requiring derived classes to
/// provide an implementation of the <see cref="BuildAsync(CancellationToken)"/> method.</remarks>
/// <typeparam name="TClass">The type of object to be built asynchronously.</typeparam>
public abstract class AbstractAsyncObjectBuilder<TClass, TBuilder> : IAsyncObjectBuilder<TClass> where TBuilder : IAsyncObjectBuilder<TClass>
{
    /// <summary>
    /// Asynchronously builds an object of type <typeparamref name="TClass"/>.
    /// </summary>
    /// <remarks>The method performs the build operation asynchronously and returns an <see
    /// cref="IObjectBuildResult{TClass}"/> that encapsulates the result of the operation, including any errors or success
    /// state. Callers can use the <paramref name="cancellationToken"/> to cancel the operation if needed.</remarks>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="IObjectBuildResult{TClass}"/> representing the outcome of the build operation.</returns>
    public async Task<IObjectBuildResult<TClass>> BuildAsync(VisitedObjectsList? visited = null, CancellationToken cancellationToken = default)
    {
        var exceptions = new ExceptionBuildDictionary();
        visited ??= new VisitedObjectsList();

        if (visited.TryGetValue(this, out var existing))
        {
            return new SuccessObjectBuildResult<TClass>((TClass)existing);
        }

        visited[this] = new Dictionary<object, object>();

        return await BuildInternalAsync(exceptions, visited, cancellationToken);
    }

    /// <summary>
    /// Creates an asynchronous failure result with the specified exceptions, and visited objects.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder implementing <see cref="IAsyncObjectBuilder{TClass}"/>.</typeparam>
    /// <param name="message">The error message describing the failure.</param>
    /// <param name="exceptions">A collection of exceptions associated with the failure.</param>
    /// <param name="visited">A dictionary of objects that have already been processed, used to prevent cyclic references.</param>
    /// <returns>An <see cref="FailureAsyncObjectBuildResult{TClass, TBuilder}"/> representing the failure state.</returns>
    protected FailureAsyncObjectBuildResult<TClass, TBuilder> AsyncFailureResult(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        return new FailureAsyncObjectBuildResult<TClass, TBuilder>((TBuilder)(this as IAsyncObjectBuilder<TClass>), exceptions, visited);
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
    /// cref="IObjectBuildResult{TClass}"/> representing the outcome of the build process.</returns>
    protected abstract Task<IObjectBuildResult<TClass>> BuildInternalAsync(ExceptionBuildDictionary exceptions, VisitedObjectsList visited, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously builds a collection of objects from the specified list of builders, recording any build
    /// failures in the provided exception dictionary.
    /// </summary>
    /// <remarks>If a builder fails to construct an object, the failure is recorded in the <paramref
    /// name="exceptions"/> dictionary under the specified <paramref name="memberName"/>. Builders that return
    /// references may defer addition of their results until resolved. The method does not throw on individual build
    /// failures; all exceptions are aggregated in the provided dictionary.</remarks>
    /// <typeparam name="TOtherClass">The type of object to be built by each builder in the list.</typeparam>
    /// <typeparam name="TOtherBuilder">The type of builder used to construct objects of type <typeparamref name="TOtherClass"/>. Must implement
    /// <see cref="IAsyncObjectBuilder{TOtherClass}"/>.</typeparam>
    /// <param name="memberName">The name of the member associated with the builders. Used for exception tracking and reporting.</param>
    /// <param name="list">The list of builders that will be used to asynchronously construct objects of type <typeparamref
    /// name="TOtherClass"/>.</param>
    /// <param name="exceptions">A dictionary for recording exceptions that occur during the build process for each member.</param>
    /// <param name="visited">A list of objects that have already been visited during the build process to prevent cycles or redundant
    /// operations.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous build operation.</param>
    /// <returns>A collection of successfully built objects of type <typeparamref name="TOtherClass"/>. The collection may be
    /// empty if no objects are built successfully.</returns>
    protected async Task<IEnumerable<TOtherClass>> BuildListAsync<TOtherClass, TOtherBuilder>(string memberName, List<TOtherBuilder> list, ExceptionBuildDictionary exceptions, VisitedObjectsList visited, CancellationToken cancellationToken = default) where TOtherBuilder : class, IAsyncObjectBuilder<TOtherClass>
    {
        var built = new List<TOtherClass>();
        foreach (var builder in list)
        {
            var buildResult = await builder.BuildAsync(visited);
            switch (buildResult)
            {
                case SuccessObjectBuildResult<TOtherClass> successResult:
                    built.Add(successResult.Result);
                    break;
                case FailureAsyncObjectBuildResult<TOtherClass, TOtherBuilder> failureResult:
                    exceptions.Add(memberName, failureResult.Exceptions);
                    break;
                case AsyncBuildReference<TOtherClass, TOtherBuilder> buildRefResult:
                    buildRefResult.AddAction(built.Add);
                    break;
            }
        }

        return built;
    }
}
