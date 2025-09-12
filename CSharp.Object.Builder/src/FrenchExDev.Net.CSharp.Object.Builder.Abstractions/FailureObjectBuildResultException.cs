namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents an exception that is thrown when an object build operation fails and a failure result is produced by the
/// builder.
/// </summary>
/// <remarks>This exception provides access to the failure result generated during an unsuccessful object build
/// attempt. It is typically thrown by object builder implementations to signal that the build process could not
/// complete successfully.</remarks>
/// <typeparam name="TClass">The type of object that the builder is intended to construct.</typeparam>
/// <typeparam name="TBuilder">The type of builder that implements <see cref="IObjectBuilder{TClass}"/> and was used in the failed build operation.</typeparam>
public class FailureObjectBuildResultException<TClass, TBuilder> : Exception where TBuilder : IObjectBuilder<TClass>
{
    /// <summary>
    /// Initializes a new instance of the FailureObjectBuildResultException class with the specified failure result and
    /// error message.
    /// </summary>
    /// <param name="failure">The FailureObjectBuildResult instance that contains details about the failed build operation.</param>
    /// <param name="message">The error message that describes the reason for the exception.</param>
    public FailureObjectBuildResultException(FailureObjectBuildResult<TClass, TBuilder> failure, string message) : base(message)
    {
    }
}


/// <summary>
/// Represents an exception that is thrown when an asynchronous object build operation fails and returns a failure
/// result.
/// </summary>
/// <remarks>This exception is typically thrown to indicate that an asynchronous build process did not complete
/// successfully and a failure result was produced. The exception provides context about the failed build operation,
/// allowing callers to handle build failures in a structured manner.</remarks>
/// <typeparam name="TClass">The type of object being built by the asynchronous builder.</typeparam>
/// <typeparam name="TBuilder">The type of asynchronous object builder used to construct the object.</typeparam>
public class AsyncFailureObjectBuildResultException<TClass, TBuilder> : Exception where TBuilder : IAsyncObjectBuilder<TClass>
{
    /// <summary>
    /// Initializes a new instance of the FailureAsyncObjectBuildResultException class with the specified failure result
    /// and error message.
    /// </summary>
    /// <param name="failure">The FailureObjectBuildResult instance that describes the details of the failed object build operation.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public AsyncFailureObjectBuildResultException(FailureAsyncObjectBuildResult<TClass, TBuilder> failure, string message) : base(message)
    {
    }
}