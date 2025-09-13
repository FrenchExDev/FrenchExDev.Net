namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents an exception that is thrown when an object build result is invalid or cannot be processed as expected.
/// </summary>
/// <remarks>This exception is typically used to indicate that an object construction or build operation has
/// produced a result that does not meet required validity criteria. The associated <see
/// cref="IObjectBuildResult{object}"/> instance can be accessed via the <see cref="Result"/> property for further
/// inspection. This exception may be thrown by object factories, builders, or dependency injection frameworks when a
/// build result is malformed or incomplete.</remarks>
public class InvalidObjectBuildResultException : Exception
{
    /// <summary>
    /// Gets the result of the object build operation, including status and any output or errors encountered.
    /// </summary>
    /// <remarks>Use this property to access details about the outcome of the build process. The result may
    /// contain information about success, failure, and any associated data or exceptions. This property is
    /// read-only.</remarks>
    public IObjectBuildResult<object> Result { get; }

    /// <summary>
    /// Initializes a new instance of the InvalidObjectBuildResultException class with the specified object build
    /// result.
    /// </summary>
    /// <remarks>Use this constructor to provide context about the failed object build operation when throwing
    /// the exception. The supplied result is accessible via the Result property for further inspection.</remarks>
    /// <param name="result">The object build result that caused the exception. Cannot be null.</param>
    public InvalidObjectBuildResultException(IObjectBuildResult<object> result)
    {
        Result = result;
    }
}

/// <summary>
/// Extension methods for <see cref="IObjectBuildResult{TClass}"/> to simplify success and failure result handling.
/// </summary>
public static class IObjectBuildResultExtensions
{
    /// <summary>
    /// Retrieves the successful result from an <see cref="IObjectBuildResult{TClass}"/>.
    /// Throws an exception if the result is not a success.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="result">The build result to extract from.</param>
    /// <returns>The built object if the result is successful.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is not a success.</exception>
    public static TClass Success<TClass>(this IObjectBuildResult<TClass> result) where TClass : class
    {
        return result switch
        {
            SuccessObjectBuildResult<TClass> success => success.Result,
            _ => throw new InvalidObjectBuildResultException((IObjectBuildResult<object>)result)
        };
    }

    /// <summary>
    /// Retrieves the failure result from an <see cref="IObjectBuildResult{TClass}"/>.
    /// Throws an exception if the result is not a failure.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <typeparam name="TBuilder">The type of the builder used.</typeparam>
    /// <param name="result">The build result to extract from.</param>
    /// <returns>The failure result if the build failed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is not a failure.</exception>
    public static FailureObjectBuildResult<TClass, TBuilder> Failure<TClass, TBuilder>(this IObjectBuildResult<TClass> result)
        where TClass : class
        where TBuilder : IObjectBuilder<TClass>
    {
        return result switch
        {
            FailureObjectBuildResult<TClass, TBuilder> failure => failure,
            _ => throw new InvalidOperationException("Not a failure object")
        };
    }
}
