namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

public class SuccessResult<TClass> : IBuildResult<TClass>
{
    /// <summary>
    /// Get the resulting object from the build operation, or <see langword="null"/> if the build failed.
    /// </summary>
    public TClass Result { get; init; }

    /// </summary>
    public IEnumerable<Exception>? Exceptions { get; init; }

    public SuccessResult(TClass result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a successful build result with the specified result value.
    /// </summary>
    /// <param name="result">The result value to associate with the successful build.</param>
    /// <returns>A <see cref="SuccessResult{TClass}"/> instance representing a successful build, containing the specified
    /// result value.</returns>
    public static SuccessResult<TClass> Success(TClass result)
    {
        return new SuccessResult<TClass>(result);
    }
}

public class FailureResult<TClass, TBuilder> : IBuildResult<TClass> where TBuilder : IBuilder<TClass>
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
    /// Constructor for creating a new instance of <see cref="FailureResult{TClass, TBuilder}"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="exceptions"></param>
    /// <param name="visited"></param>
    public FailureResult(TBuilder builder, IEnumerable<Exception> exceptions, Dictionary<object, object> visited)
    {
        Builder = builder;
        Exceptions = exceptions;
        Visited = visited;
    }
}

/// <summary>
/// Asynchronous failure result for a build operation, encapsulating the builder instance and any exceptions that
/// </summary>
/// <typeparam name="TClass"></typeparam>
/// <typeparam name="TBuilder"></typeparam>
public class AsyncFailureResult<TClass, TBuilder> : IBuildResult<TClass> where TBuilder : IAsyncBuilder<TClass>
{
    /// <summary>
    /// Gets the builder instance used to configure and construct the object.
    /// </summary>
    public TBuilder Builder { get; init; }

    /// <summary>
    /// Exceptions that occurred during the build operation, or <see langword="null"/> if no exceptions
    /// </summary>
    public IEnumerable<Exception> Exceptions { get; init; }

    /// <summary>
    /// Gets the dictionary of visited objects during the build process.
    /// </summary>
    public Dictionary<object, object> Visited { get; init; }

    /// <summary>
    /// Constructor for creating a new instance of <see cref="AsyncFailureResult{TClass, TBuilder}"/>.
    /// </summary>t
    /// <param name="builder"></param>
    /// <param name="exceptions"></param>
    public AsyncFailureResult(TBuilder builder, IEnumerable<Exception> exceptions, Dictionary<object, object> visited)
    {
        Builder = builder;
        Exceptions = exceptions;
        Visited = visited;
    }
}

/// <summary>
/// Represents an exception that occurs during a build operation, providing context about the builder and visited
/// objects.
/// </summary>
/// <remarks>This exception is typically used to capture and propagate errors that occur during a build process.
/// It provides access to the builder instance and an optional dictionary of visited objects for additional
/// context.</remarks>
/// <typeparam name="TBuilder">The type of the builder associated with the exception.</typeparam>
public class BasicBuildException<TClass, TBuilder> : Exception where TBuilder : IBuilder<TClass>
{
    private Dictionary<object, object>? _visited;

    private TBuilder? _builder;

    public Dictionary<object, object> Visited => _visited ?? throw new InvalidOperationException("Visited is not set.");

    public TBuilder Builder => _builder ?? throw new InvalidOperationException("Builder is not set.");

    public BasicBuildException(string message, TBuilder builder, Dictionary<object, object>? visited = null) : base(message)
    {
        _builder = builder;
        _visited = visited;
    }
}

public class BasicAsyncBuildException<TClass, TBuilder> : Exception where TBuilder : IAsyncBuilder<TClass>
{
    private Dictionary<object, object>? _visited;

    private TBuilder? _builder;

    public Dictionary<object, object> Visited => _visited ?? throw new InvalidOperationException("Visited is not set.");

    public TBuilder Builder => _builder ?? throw new InvalidOperationException("Builder is not set.");

    public BasicAsyncBuildException(string message, TBuilder builder, Dictionary<object, object>? visited = null) : base(message)
    {
        _builder = builder;
        _visited = visited;
    }
}