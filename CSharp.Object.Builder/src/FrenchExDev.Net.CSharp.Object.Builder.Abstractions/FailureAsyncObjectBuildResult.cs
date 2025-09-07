namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Asynchronous failure result for a build operation, encapsulating the builder instance and any exceptions that
/// </summary>
/// <typeparam name="TClass"></typeparam>
/// <typeparam name="TBuilder"></typeparam>
public class FailureAsyncObjectBuildResult<TClass, TBuilder> : IObjectBuildResult<TClass> where TBuilder : IAsyncObjectBuilder<TClass>
{
    /// <summary>
    /// Gets the builder instance used to configure and construct the object.
    /// </summary>
    public TBuilder Builder { get; init; }

    /// <summary>
    /// Exceptions that occurred during the build operation, or <see langword="null"/> if no exceptions
    /// </summary>
    public ExceptionBuildList Exceptions { get; init; }

    /// <summary>
    /// Gets the dictionary of visited objects during the build process.
    /// </summary>
    public VisitedObjectsList Visited { get; init; }


    /// <summary>
    /// Constructor for creating a new instance of <see cref="FailureAsyncObjectBuildResult{TClass, TBuilder}"/>.
    /// </summary>t
    /// <param name="builder"></param>
    /// <param name="exceptions"></param>
    public FailureAsyncObjectBuildResult(TBuilder builder, ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        Builder = builder;
        Exceptions = exceptions;
        Visited = visited;
    }
}
