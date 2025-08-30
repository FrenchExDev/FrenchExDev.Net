namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

public class FailureObjectBuildResult<TClass, TBuilder> : IObjectBuildResult<TClass> where TBuilder : IObjectBuilder<TClass>
{
    /// <summary>
    /// Gets the builder instance used to configure and construct the object.
    /// </summary>
    public TBuilder Builder { get; init; }

    /// <summary>
    /// Exceptions that occurred during the build operation, or <see langword="null"/> if no exceptions
    /// </summary>
    public IEnumerable<Exception> Exceptions { get; init; }

    public Dictionary<object, object> Visited { get; init; }

    /// <summary>
    /// Constructor for creating a new instance of <see cref="FailureObjectBuildResult{TClass, TBuilder}"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="exceptions"></param>
    /// <param name="visited"></param>
    public FailureObjectBuildResult(TBuilder builder, IEnumerable<Exception> exceptions, VisitedObjectsList visited)
    {
        Builder = builder;
        Exceptions = exceptions;
        Visited = visited;
    }
}
