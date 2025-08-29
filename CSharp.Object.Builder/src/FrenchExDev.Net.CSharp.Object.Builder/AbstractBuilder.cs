using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Abstract base class for building objects of type <typeparamref name="TClass"/>.
/// </summary>
/// <typeparam name="TClass"></typeparam>
public abstract class AbstractBuilder<TClass, TBuilder> : IBuilder<TClass> where TBuilder : IBuilder<TClass>
{
    /// <summary>
    /// Builds an instance of <typeparamref name="TClass"/>.
    /// </summary>
    /// <returns></returns>
    public IBuildResult<TClass> Build(Dictionary<object, object>? visited = null)
    {
        visited ??= new Dictionary<object, object>();
        return BuildInternal(visited);
    }

    /// <summary>
    /// Constructs the object of type <typeparamref name="TClass"/> using the specified context of visited objects.
    /// </summary>
    /// <remarks>This method is intended to be implemented by derived classes to define the specific logic for
    /// constructing  an object of type <typeparamref name="TClass"/>. The <paramref name="visited"/> dictionary can be
    /// used to  prevent cyclic dependencies or redundant processing during the build process.</remarks>
    /// <param name="visited">A dictionary used to track objects that have already been processed during the build operation.  This parameter
    /// may be <see langword="null"/> if no tracking is required.</param>
    /// <returns>An instance of <see cref="IBuildResult{TClass}"/> representing the result of the build operation.</returns>
    protected abstract IBuildResult<TClass> BuildInternal(Dictionary<object, object> visited);


    /// <summary>
    /// Creates a failure result containing the specified error message and a record of visited objects.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to customize the behavior of
    /// failure handling.</remarks>
    /// <param name="message">The error message describing the reason for the failure.</param>
    /// <param name="visited">A dictionary of objects that have been visited during the operation, used to track state and prevent cycles.</param>
    /// <returns>A <see cref="FailureResult{TClass, TBuilder}"/> representing the failure, including the error details and
    /// visited objects.</returns>
    protected virtual FailureResult<TClass, TBuilder> FailureResult(string message, Dictionary<object, object> visited)
    {
        return new FailureResult<TClass, TBuilder>((TBuilder)(object)this, [
            new BasicBuildException< TClass, TBuilder >(message, (TBuilder)(this as IBuilder<TClass>), visited),
        ], visited);
    }
}
