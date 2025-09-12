namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Asynchronous failure result for a build operation, encapsulating the builder instance and any exceptions that
/// </summary>
/// <typeparam name="TClass"></typeparam>
/// <typeparam name="TBuilder"></typeparam>
public class FailureAsyncObjectBuildResult<TClass, TBuilder> : AbstractFailureObjectBuildResult<TBuilder>, IObjectBuildResult<TClass> where TBuilder : IAsyncObjectBuilder<TClass>
{
    /// <summary>
    /// Initializes a new instance of the FailureAsyncObjectBuildResult class with the specified builder, exception
    /// list, and visited objects.
    /// </summary>
    /// <param name="builder">The builder instance used to construct the object. Cannot be null.</param>
    /// <param name="exceptions">The collection of exceptions encountered during the build process. Cannot be null.</param>
    /// <param name="visited">The list of objects that were visited during the build operation. Cannot be null.</param>
    public FailureAsyncObjectBuildResult(TBuilder builder, ExceptionBuildDictionary exceptions, VisitedObjectsList visited) : base(builder, exceptions, visited)
    {
    }
}
