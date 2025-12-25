namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents a validation or build failure that can contain an exception, a message, or nested failures.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Failure"/> is an abstract base record that uses the discriminated union pattern to represent
/// three types of failures:
/// </para>
/// <list type="bullet">
///   <item><description><see cref="ExceptionFailure"/> - wraps an <see cref="Exception"/></description></item>
///   <item><description><see cref="MessageFailure"/> - contains a simple error message string</description></item>
///   <item><description><see cref="NestedFailure"/> - contains child failures from nested validation</description></item>
/// </list>
/// <para>
/// Use the <see cref="Match{TResult}(Func{Exception, TResult}, Func{string, TResult}, Func{IFailureCollector, TResult})"/>
/// method for exhaustive pattern matching, or the <c>TryGet*</c> methods for conditional access.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Create different failure types
/// var exFailure = Failure.FromException(new ArgumentNullException("name"));
/// var msgFailure = Failure.FromMessage("Name is required");
/// var nestedFailure = Failure.FromNested(childFailures);
/// 
/// // Pattern matching
/// string description = failure.Match(
///     onException: ex => $"Exception: {ex.Message}",
///     onMessage: msg => $"Error: {msg}",
///     onNested: nested => $"Nested errors: {nested.FailureCount}"
/// );
/// </code>
/// </example>
/// <seealso cref="ExceptionFailure"/>
/// <seealso cref="MessageFailure"/>
/// <seealso cref="NestedFailure"/>
/// <seealso cref="FailuresDictionary"/>
public abstract record Failure
{
    /// <summary>
    /// Creates a new <see cref="ExceptionFailure"/> from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A new <see cref="ExceptionFailure"/> instance wrapping the exception.</returns>
    public static Failure FromException(Exception exception) => new ExceptionFailure(exception);

    /// <summary>
    /// Creates a new <see cref="MessageFailure"/> from the specified error message.
    /// </summary>
    /// <param name="message">The error message describing the failure.</param>
    /// <returns>A new <see cref="MessageFailure"/> instance containing the message.</returns>
    public static Failure FromMessage(string message) => new MessageFailure(message);

    /// <summary>
    /// Creates a new <see cref="NestedFailure"/> from the specified failure collector.
    /// </summary>
    /// <param name="failures">The collector containing nested failures from child validation.</param>
    /// <returns>A new <see cref="NestedFailure"/> instance wrapping the nested failures.</returns>
    public static Failure FromNested(IFailureCollector failures) => new NestedFailure(failures);

    /// <summary>
    /// Implicitly converts an <see cref="Exception"/> to an <see cref="ExceptionFailure"/>.
    /// </summary>
    /// <param name="ex">The exception to convert.</param>
    public static implicit operator Failure(Exception ex) => new ExceptionFailure(ex);

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="MessageFailure"/>.
    /// </summary>
    /// <param name="message">The error message to convert.</param>
    public static implicit operator Failure(string message) => new MessageFailure(message);

    /// <summary>
    /// Performs exhaustive pattern matching on the failure type, returning a result.
    /// </summary>
    /// <typeparam name="TResult">The type of result to return.</typeparam>
    /// <param name="onException">Function to execute if this is an <see cref="ExceptionFailure"/>.</param>
    /// <param name="onMessage">Function to execute if this is a <see cref="MessageFailure"/>.</param>
    /// <param name="onNested">Function to execute if this is a <see cref="NestedFailure"/>.</param>
    /// <returns>The result of the matched function.</returns>
    /// <example>
    /// <code>
    /// string result = failure.Match(
    ///     onException: ex => ex.Message,
    ///     onMessage: msg => msg,
    ///     onNested: nested => "Multiple errors"
    /// );
    /// </code>
    /// </example>
    public abstract TResult Match<TResult>(
        Func<Exception, TResult> onException,
        Func<string, TResult> onMessage,
        Func<IFailureCollector, TResult> onNested);

    /// <summary>
    /// Performs exhaustive pattern matching on the failure type, executing an action.
    /// </summary>
    /// <param name="onException">Action to execute if this is an <see cref="ExceptionFailure"/>.</param>
    /// <param name="onMessage">Action to execute if this is a <see cref="MessageFailure"/>.</param>
    /// <param name="onNested">Action to execute if this is a <see cref="NestedFailure"/>.</param>
    public abstract void Match(
        Action<Exception> onException,
        Action<string> onMessage,
        Action<IFailureCollector> onNested);

    /// <summary>
    /// Gets a value indicating whether this failure is an <see cref="ExceptionFailure"/>.
    /// </summary>
    public bool IsException => this is ExceptionFailure;

    /// <summary>
    /// Gets a value indicating whether this failure is a <see cref="MessageFailure"/>.
    /// </summary>
    public bool IsMessage => this is MessageFailure;

    /// <summary>
    /// Gets a value indicating whether this failure is a <see cref="NestedFailure"/>.
    /// </summary>
    public bool IsNested => this is NestedFailure;

    /// <summary>
    /// Attempts to get the exception if this is an <see cref="ExceptionFailure"/>.
    /// </summary>
    /// <param name="exception">When this method returns, contains the exception if successful; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if this is an <see cref="ExceptionFailure"/>; otherwise, <see langword="false"/>.</returns>
    public bool TryGetException(out Exception? exception)
    {
        if (this is ExceptionFailure ef) { exception = ef.Exception; return true; }
        exception = null; return false;
    }

    /// <summary>
    /// Attempts to get the message if this is a <see cref="MessageFailure"/>.
    /// </summary>
    /// <param name="message">When this method returns, contains the message if successful; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if this is a <see cref="MessageFailure"/>; otherwise, <see langword="false"/>.</returns>
    public bool TryGetMessage(out string? message)
    {
        if (this is MessageFailure mf) { message = mf.Message; return true; }
        message = null; return false;
    }

    /// <summary>
    /// Attempts to get the nested failures if this is a <see cref="NestedFailure"/>.
    /// </summary>
    /// <param name="failures">When this method returns, contains the nested failures if successful; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if this is a <see cref="NestedFailure"/>; otherwise, <see langword="false"/>.</returns>
    public bool TryGetNested(out IFailureCollector? failures)
    {
        if (this is NestedFailure nf) { failures = nf.Failures; return true; }
        failures = null; return false;
    }
}

/// <summary>
/// Represents a failure caused by an exception.
/// </summary>
/// <param name="Exception">The exception that caused the failure.</param>
/// <seealso cref="Failure"/>
public sealed record ExceptionFailure(Exception Exception) : Failure
{
    /// <inheritdoc/>
    public override TResult Match<TResult>(Func<Exception, TResult> onException, Func<string, TResult> onMessage, Func<IFailureCollector, TResult> onNested) => onException(Exception);
    
    /// <inheritdoc/>
    public override void Match(Action<Exception> onException, Action<string> onMessage, Action<IFailureCollector> onNested) => onException(Exception);
}

/// <summary>
/// Represents a failure described by a simple error message.
/// </summary>
/// <param name="Message">The error message describing the failure.</param>
/// <seealso cref="Failure"/>
public sealed record MessageFailure(string Message) : Failure
{
    /// <inheritdoc/>
    public override TResult Match<TResult>(Func<Exception, TResult> onException, Func<string, TResult> onMessage, Func<IFailureCollector, TResult> onNested) => onMessage(Message);
    
    /// <inheritdoc/>
    public override void Match(Action<Exception> onException, Action<string> onMessage, Action<IFailureCollector> onNested) => onMessage(Message);
}

/// <summary>
/// Represents a failure containing nested failures from child validation.
/// </summary>
/// <param name="Failures">The collector containing the nested failures.</param>
/// <seealso cref="Failure"/>
public sealed record NestedFailure(IFailureCollector Failures) : Failure
{
    /// <inheritdoc/>
    public override TResult Match<TResult>(Func<Exception, TResult> onException, Func<string, TResult> onMessage, Func<IFailureCollector, TResult> onNested) => onNested(Failures);
    
    /// <inheritdoc/>
    public override void Match(Action<Exception> onException, Action<string> onMessage, Action<IFailureCollector> onNested) => onNested(Failures);
}
