namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents the result of an object build operation that failed, providing access to the builder, encountered
/// exceptions, and the set of visited objects.
/// </summary>
/// <remarks>This class is typically used to capture diagnostic information when an object build process does not
/// complete successfully. It includes the builder instance, a list of exceptions that occurred, and a record of all
/// objects visited during the build. The contents of the exceptions and visited objects can be used for troubleshooting
/// or logging purposes.</remarks>
/// <typeparam name="TBuilder">The type of the builder used to configure and construct the object.</typeparam>
public class AbstractFailureObjectBuildResult<TBuilder>
{
    /// <summary>
    /// Gets the builder instance used to configure and construct the object.
    /// </summary>
    public TBuilder Builder { get; init; }

    /// <summary>
    /// Exceptions that occurred during the build operation, or <see langword="null"/> if no exceptions
    /// </summary>
    public ExceptionBuildDictionary Exceptions { get; init; }

    /// <summary>
    /// Gets the dictionary of visited objects during the build process.
    /// </summary>
    public VisitedObjectsList Visited { get; init; }


    /// <summary>
    /// Constructor for creating a new instance of <see cref="AbstractFailureObjectBuildResult{TBuilder}"/>.
    /// </summary>t
    /// <param name="builder"></param>
    /// <param name="exceptions"></param>
    public AbstractFailureObjectBuildResult(TBuilder builder, ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        Builder = builder;
        Exceptions = exceptions;
        Visited = visited;
    }
}
