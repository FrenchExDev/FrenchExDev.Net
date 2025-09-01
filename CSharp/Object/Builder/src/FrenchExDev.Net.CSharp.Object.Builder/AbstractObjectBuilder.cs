using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Abstract base class for building objects of type <typeparamref name="TClass"/>.
/// </summary>
/// <typeparam name="TClass"></typeparam>
public abstract class AbstractObjectBuilder<TClass, TBuilder> : IObjectBuilder<TClass> where TBuilder : IObjectBuilder<TClass>
{
    /// <summary>
    /// Builds an instance of <typeparamref name="TClass"/>.
    /// </summary>
    /// <returns></returns>
    public IObjectBuildResult<TClass> Build(VisitedObjectsList? visited = null)
    {
        // Initialize exceptions build list to collect exceptions during the build process.
        var exceptions = new ExceptionBuildList();

        // Initialize visited objects list if not provided.
        visited ??= new VisitedObjectsList();

        // Check if the current builder instance has already been visited to prevent cyclic dependencies.
        if (visited.TryGetValue(this, out var existing))
        {
            return new SuccessObjectBuildResult<TClass>((TClass)existing);
        }

        // Mark the current builder instance as visited with a temporary default value.
        visited[this] = default!;

        // Perform the actual build operation using the internal method.
        var built = BuildInternal(exceptions, visited);

        // Assign the built result to the visited objects list.
        visited[this] = built;

        return built;
    }

    /// <summary>
    /// Constructs the object of type <typeparamref name="TClass"/> using the specified context of visited objects.
    /// </summary>
    /// <remarks>This method is intended to be implemented by derived classes to define the specific logic for
    /// constructing  an object of type <typeparamref name="TClass"/>. The <paramref name="visited"/> dictionary can be
    /// used to  prevent cyclic dependencies or redundant processing during the build process.</remarks>
    /// <param name="visited">A dictionary used to track objects that have already been processed during the build operation.  This parameter
    /// may be <see langword="null"/> if no tracking is required.</param>
    /// <returns>An instance of <see cref="IObjectBuildResult{TClass}"/> representing the result of the build operation.</returns>
    protected abstract IObjectBuildResult<TClass> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited);


    /// <summary>
    /// Creates a failure result containing the specified error message and a record of visited objects.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to customize the behavior of
    /// failure handling.</remarks>
    /// <param name="message">The error message describing the reason for the failure.</param>
    /// <param name="visited">A dictionary of objects that have been visited during the operation, used to track state and prevent cycles.</param>
    /// <returns>A <see cref="FailureObjectBuildResult{TClass, TBuilder}"/> representing the failure, including the error details and
    /// visited objects.</returns>
    protected virtual FailureObjectBuildResult<TClass, TBuilder> Failure(string message, VisitedObjectsList visited)
    {
        return new FailureObjectBuildResult<TClass, TBuilder>((TBuilder)(object)this, [
            new BasicObjectBuildException<TClass, TBuilder>(message, (TBuilder)(this as IObjectBuilder<TClass>), visited),
        ], visited);
    }

    /// <summary>
    /// Creates a successful result containing the specified instance.
    /// </summary>
    /// <param name="instance">The instance to include in the successful result. Cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="SuccessObjectBuildResult{TClass}"/> representing a successful operation with the provided instance.</returns>
    protected virtual SuccessObjectBuildResult<TClass> Success(TClass instance)
    {
        return new SuccessObjectBuildResult<TClass>(instance);
    }
}
