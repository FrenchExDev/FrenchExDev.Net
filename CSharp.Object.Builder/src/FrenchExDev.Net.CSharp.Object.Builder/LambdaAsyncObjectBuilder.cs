using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Represents an asynchronous builder that uses a lambda function to construct an instance of <typeparamref
/// name="TClass"/>.
/// </summary>
/// <remarks>This builder allows the construction logic to be defined as a lambda function, enabling flexible and
/// reusable build processes. The lambda function is invoked with a dictionary of visited objects and a cancellation
/// token, and it is expected to return a task that produces an <see cref="IObjectBuildResult{TClass}"/>.</remarks>
/// <typeparam name="TClass">The type of the object being built.</typeparam>
public class LambdaAsyncObjectBuilder<TClass, TBuilder> : AbstractAsyncObjectBuilder<TClass, TBuilder> where TBuilder : IAsyncObjectBuilder<TClass>
{
    /// <summary>
    /// Holds the lambda function used to build the object.
    /// </summary>
    private readonly Func<TBuilder, ExceptionBuildList, VisitedObjectsList, CancellationToken, Task<IObjectBuildResult<TClass>>> _buildFunc;

    /// <summary>
    /// Constructor for creating a new instance of <see cref="LambdaAsyncObjectBuilder{TClass}"/> with the specified build
    /// </summary>
    /// <param name="buildFunc"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public LambdaAsyncObjectBuilder(Func<TBuilder, ExceptionBuildList, VisitedObjectsList, CancellationToken, Task<IObjectBuildResult<TClass>>> buildFunc)
    {
        _buildFunc = buildFunc ?? throw new ArgumentNullException(nameof(buildFunc));
    }

    /// <summary>
    /// Internal method that invokes the provided lambda function to build the object asynchronously.
    /// </summary>
    /// <param name="visited"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<IObjectBuildResult<TClass>> BuildInternalAsync(ExceptionBuildList exceptions, VisitedObjectsList visited, CancellationToken cancellationToken)
    {
        return await _buildFunc((TBuilder)(IAsyncObjectBuilder<TClass>)this, exceptions, visited, cancellationToken);
    }
}
