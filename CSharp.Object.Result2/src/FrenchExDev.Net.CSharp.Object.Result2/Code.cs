namespace FrenchExDev.Net.CSharp.Object.Result2;

/// <summary>
/// Represents the outcome of an operation that may succeed or fail.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IResult"/> is the base interface for all result types in this library.
/// It provides a consistent way to check whether an operation completed successfully
/// without needing to know the specific result type.
/// </para>
/// </remarks>
/// <seealso cref="Result{TResult}"/>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the operation succeeded; <see langword="false"/> if it failed.
    /// </value>
    bool IsSuccess { get; }
}

/// <summary>
/// The exception that is thrown when an operation completes but produces a result indicating failure.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ResultException"/> serves as the base class for all result-related exceptions.
/// Use this exception type when signaling that an operation completed but produced an error
/// or failure condition, rather than throwing a different exception type.
/// </para>
/// <para>
/// This supports a hybrid error handling approach where operations return result objects
/// that encapsulate both success and failure states, but exceptions are thrown when
/// the result is accessed incorrectly.
/// </para>
/// </remarks>
/// <seealso cref="InvalidResultAccessOperationException"/>
/// <seealso cref="Result{TResult}"/>
public class ResultException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class.
    /// </summary>
    public ResultException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ResultException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
    public ResultException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// The exception that is thrown when an invalid access operation is performed on a result object,
/// such as accessing a value from a failed result or an exception from a successful result.
/// </summary>
/// <remarks>
/// <para>
/// This exception indicates a programming error where the result's state was not checked before
/// accessing its value or exception. To avoid this exception:
/// </para>
/// <list type="bullet">
///   <item><description>Check <see cref="IResult.IsSuccess"/> before accessing <see cref="Result{TResult}.Value"/></description></item>
///   <item><description>Use <see cref="Result{TResult}.TryGetSuccessValue"/> for safe value access</description></item>
///   <item><description>Use <see cref="Result{TResult}.TryGetException{TException}"/> for safe exception access</description></item>
///   <item><description>Use pattern matching methods like <see cref="Result{TResult}.Match"/></description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// var result = Result&lt;int&gt;.Failure(new InvalidOperationException("Error"));
/// 
/// // This will throw InvalidResultAccessOperationException
/// // var value = result.Value;
/// 
/// // Safe access patterns
/// if (result.TryGetSuccessValue(out var value))
/// {
///     Console.WriteLine($"Value: {value}");
/// }
/// 
/// result.Match(
///     onSuccess: v => Console.WriteLine($"Success: {v}"),
///     onFailure: ex => Console.WriteLine($"Error: {ex.Message}")
/// );
/// </code>
/// </example>
/// <seealso cref="ResultException"/>
/// <seealso cref="Result{TResult}"/>
public class InvalidResultAccessOperationException : ResultException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidResultAccessOperationException"/> class.
    /// </summary>
    public InvalidResultAccessOperationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidResultAccessOperationException"/> class 
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the invalid access operation.</param>
    public InvalidResultAccessOperationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidResultAccessOperationException"/> class 
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
    public InvalidResultAccessOperationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Represents the outcome of an operation that can succeed with a value or fail with an exception.
/// Provides a functional approach to error handling without relying on exceptions for control flow.
/// </summary>
/// <typeparam name="TResult">The type of the value returned if the operation is successful.</typeparam>
/// <remarks>
/// <para>
/// <see cref="Result{TResult}"/> is a discriminated union type that encapsulates either a success value
/// or a failure exception. This pattern, also known as the "Railway Oriented Programming" pattern,
/// provides several benefits:
/// </para>
/// <list type="bullet">
///   <item><description>Explicit error handling - failures must be acknowledged</description></item>
///   <item><description>Composable operations - chain operations with <see cref="Map{TNew}"/> and <see cref="Bind{TNew}"/></description></item>
///   <item><description>No exception overhead for expected failures</description></item>
///   <item><description>Clear separation between expected failures and exceptional conditions</description></item>
/// </list>
/// <para>
/// This is a <see langword="struct"/> for performance reasons - it avoids heap allocation for result objects.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Creating results
/// var success = Result&lt;int&gt;.Success(42);
/// var failure = Result&lt;int&gt;.Failure(new ArgumentException("Invalid value"));
/// 
/// // Pattern matching
/// success.Match(
///     onSuccess: value => Console.WriteLine($"Got: {value}"),
///     onFailure: ex => Console.WriteLine($"Error: {ex.Message}")
/// );
/// 
/// // Chaining operations (Railway pattern)
/// var result = Result&lt;string&gt;.Success("42")
///     .Map(int.Parse)                    // Transform to int
///     .Map(x => x * 2)                   // Double it
///     .Bind(x => x > 0 
///         ? Result&lt;int&gt;.Success(x) 
///         : Result&lt;int&gt;.Failure(new Exception("Must be positive")));
/// 
/// // Safe access
/// if (result.TryGetSuccessValue(out var value))
/// {
///     Console.WriteLine($"Result: {value}");
/// }
/// </code>
/// </example>
/// <seealso cref="IResult"/>
/// <seealso cref="ResultException"/>
/// <seealso cref="InvalidResultAccessOperationException"/>
public readonly struct Result<TResult> : IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the result contains a success value;
    /// <see langword="false"/> if it contains a failure exception.
    /// </value>
    /// <remarks>
    /// Always check this property before accessing <see cref="Value"/> to avoid
    /// <see cref="InvalidResultAccessOperationException"/>.
    /// </remarks>
    public readonly bool IsSuccess { get; }

    /// <summary>
    /// Stores the result value if the operation was successful; otherwise, <see langword="default"/>.
    /// </summary>
    private readonly TResult? _value;

    /// <summary>
    /// Stores the exception if the operation failed; otherwise, <see langword="null"/>.
    /// </summary>
    private readonly Exception? _exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TResult}"/> struct.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the result represents success.</param>
    /// <param name="value">The success value, or <see langword="default"/> for failures.</param>
    /// <param name="exception">The failure exception, or <see langword="null"/> for successes.</param>
    private Result(bool isSuccess, TResult? value, Exception? exception)
    {
        IsSuccess = isSuccess;
        _value = value;
        _exception = exception;
    }

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <param name="value">The value to wrap in a success result.</param>
    /// <returns>A <see cref="Result{TResult}"/> representing a successful operation.</returns>
    /// <example>
    /// <code>
    /// var result = Result&lt;string&gt;.Success("Hello, World!");
    /// Console.WriteLine(result.IsSuccess); // true
    /// Console.WriteLine(result.Value);     // "Hello, World!"
    /// </code>
    /// </example>
    public static Result<TResult> Success(TResult value)
    {
        return new Result<TResult>(true, value, null);
    }

    /// <summary>
    /// Creates a failed result containing the specified exception.
    /// </summary>
    /// <typeparam name="TException">The type of exception. Must derive from <see cref="Exception"/>.</typeparam>
    /// <param name="exception">The exception that describes the failure reason.</param>
    /// <returns>A <see cref="Result{TResult}"/> representing a failed operation.</returns>
    /// <example>
    /// <code>
    /// var result = Result&lt;int&gt;.Failure(new ArgumentNullException("parameter"));
    /// Console.WriteLine(result.IsSuccess); // false
    /// </code>
    /// </example>
    public static Result<TResult> Failure<TException>(TException exception) where TException : Exception
    {
        return new Result<TResult>(false, default, exception);
    }

    /// <summary>
    /// Gets the success value contained in this result.
    /// </summary>
    /// <value>The value of type <typeparamref name="TResult"/> if the operation succeeded.</value>
    /// <exception cref="InvalidResultAccessOperationException">
    /// Thrown when accessing this property on a failure result.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Always check <see cref="IsSuccess"/> before accessing this property, or use
    /// <see cref="TryGetSuccessValue"/> for safe access.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = Result&lt;int&gt;.Success(42);
    /// if (result.IsSuccess)
    /// {
    ///     Console.WriteLine(result.Value); // Safe: 42
    /// }
    /// </code>
    /// </example>
    public TResult Value
    {
        get
        {
            if (!IsSuccess)
            {
                throw new InvalidResultAccessOperationException("Cannot access Value when the result is a failure.");
            }
            return _value!;
        }
    }

    /// <summary>
    /// Gets the failure exception, cast to the specified type.
    /// </summary>
    /// <typeparam name="TException">The type to cast the exception to. Must derive from <see cref="Exception"/>.</typeparam>
    /// <returns>The exception of type <typeparamref name="TException"/>.</returns>
    /// <exception cref="InvalidResultAccessOperationException">
    /// Thrown when calling this method on a success result, or when the exception cannot be cast to <typeparamref name="TException"/>.
    /// </exception>
    /// <remarks>
    /// Use <see cref="TryGetException{TException}"/> for safe access without exceptions.
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = Result&lt;int&gt;.Failure(new ArgumentException("Invalid"));
    /// if (!result.IsSuccess)
    /// {
    ///     var ex = result.Exception&lt;ArgumentException&gt;();
    ///     Console.WriteLine(ex.Message); // "Invalid"
    /// }
    /// </code>
    /// </example>
    public TException Exception<TException>() where TException : Exception
    {
        if (IsSuccess)
        {
            throw new InvalidResultAccessOperationException("Cannot access Exception when the result is a success.");
        }
        return _exception as TException ?? throw new InvalidOperationException("Exception is null after cast to TException");
    }

    /// <summary>
    /// Attempts to retrieve the failure exception as the specified type.
    /// </summary>
    /// <typeparam name="TException">The expected type of the exception.</typeparam>
    /// <param name="exception">
    /// When this method returns, contains the exception if the result is a failure and the exception
    /// is of the specified type; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the result is a failure and the exception is of type <typeparamref name="TException"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = Result&lt;int&gt;.Failure(new ArgumentNullException("param"));
    /// 
    /// if (result.TryGetException&lt;ArgumentNullException&gt;(out var ex))
    /// {
    ///     Console.WriteLine($"Parameter: {ex.ParamName}");
    /// }
    /// </code>
    /// </example>
    public bool TryGetException<TException>(out TException? exception) where TException : Exception
    {
        if (IsSuccess)
        {
            exception = null;
            return false;
        }
        exception = _exception as TException;
        return exception != null;
    }

    /// <summary>
    /// Attempts to retrieve the success value.
    /// </summary>
    /// <param name="value">
    /// When this method returns, contains the success value if available;
    /// otherwise, the default value for <typeparamref name="TResult"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the result is a success and the value was retrieved;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = GetUserById(123);
    /// 
    /// if (result.TryGetSuccessValue(out var user))
    /// {
    ///     Console.WriteLine($"Found user: {user.Name}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("User not found");
    /// }
    /// </code>
    /// </example>
    public bool TryGetSuccessValue(out TResult? value)
    {
        if (!IsSuccess)
        {
            value = default;
            return false;
        }
        value = _value;
        return true;
    }

    /// <summary>
    /// Asynchronously executes one of two handlers based on the result state.
    /// </summary>
    /// <param name="onSuccess">
    /// An async function to execute if the result is a success. Receives the value as a parameter.
    /// </param>
    /// <param name="onFailure">
    /// An async function to execute if the result is a failure. Receives the exception as a parameter.
    /// </param>
    /// <returns>
    /// A task that completes with this result instance after the appropriate handler has executed.
    /// </returns>
    /// <remarks>
    /// Only one handler is executed based on the result state. The handlers are awaited before returning.
    /// </remarks>
    /// <example>
    /// <code>
    /// await result.MatchAsync(
    ///     onSuccess: async value => await SaveAsync(value),
    ///     onFailure: async ex => await LogErrorAsync(ex)
    /// );
    /// </code>
    /// </example>
    public async Task<Result<TResult>> MatchAsync(Func<TResult, Task> onSuccess, Func<Exception, Task> onFailure)
    {
        if (IsSuccess)
        {
            await onSuccess(Value);
            return this;
        }

        await onFailure(Exception<Exception>());
        return this;
    }

    /// <summary>
    /// Asynchronously executes one of two handlers based on the result state, with typed exception handling.
    /// </summary>
    /// <typeparam name="TException">The expected type of the failure exception.</typeparam>
    /// <param name="onSuccess">
    /// An async function to execute if the result is a success. Receives the value as a parameter.
    /// </param>
    /// <param name="onFailure">
    /// An async function to execute if the result is a failure. Receives the typed exception as a parameter.
    /// </param>
    /// <returns>
    /// A task that completes with this result instance after the appropriate handler has executed.
    /// </returns>
    /// <exception cref="InvalidResultAccessOperationException">
    /// Thrown if the result is a failure but the exception cannot be cast to <typeparamref name="TException"/>.
    /// </exception>
    public async Task<Result<TResult>> MatchAsync<TException>(Func<TResult, Task> onSuccess, Func<TException, Task> onFailure) where TException : Exception
    {
        if (IsSuccess)
        {
            await onSuccess(Value);
            return this;
        }

        await onFailure(Exception<TException>());
        return this;
    }

    /// <summary>
    /// Executes one of two handlers based on the result state.
    /// </summary>
    /// <param name="onSuccess">
    /// An action to execute if the result is a success. Receives the value as a parameter.
    /// </param>
    /// <param name="onFailure">
    /// An action to execute if the result is a failure. Receives the exception as a parameter.
    /// </param>
    /// <returns>This result instance for method chaining.</returns>
    /// <remarks>
    /// This is the primary pattern matching method for handling both success and failure cases.
    /// Only one handler is executed based on the result state.
    /// </remarks>
    /// <example>
    /// <code>
    /// result.Match(
    ///     onSuccess: value => Console.WriteLine($"Success: {value}"),
    ///     onFailure: ex => Console.WriteLine($"Error: {ex.Message}")
    /// );
    /// </code>
    /// </example>
    public Result<TResult> Match(Action<TResult> onSuccess, Action<Exception> onFailure)
    {
        if (IsSuccess)
        {
            onSuccess(Value);
            return this;
        }

        onFailure(Exception<Exception>());
        return this;
    }

    /// <summary>
    /// Executes one of two handlers based on the result state, with typed exception handling.
    /// </summary>
    /// <typeparam name="TException">The expected type of the failure exception.</typeparam>
    /// <param name="onSuccess">
    /// An action to execute if the result is a success. Receives the value as a parameter.
    /// </param>
    /// <param name="onFailure">
    /// An action to execute if the result is a failure. Receives the typed exception as a parameter.
    /// </param>
    /// <returns>This result instance for method chaining.</returns>
    /// <exception cref="InvalidResultAccessOperationException">
    /// Thrown if the result is a failure but the exception cannot be cast to <typeparamref name="TException"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// result.Match&lt;ArgumentException&gt;(
    ///     onSuccess: value => Process(value),
    ///     onFailure: ex => Console.WriteLine($"Argument error: {ex.ParamName}")
    /// );
    /// </code>
    /// </example>
    public Result<TResult> Match<TException>(Action<TResult> onSuccess, Action<TException> onFailure) where TException : Exception
    {
        if (IsSuccess)
        {
            onSuccess(Value);
            return this;
        }

        onFailure(Exception<TException>());
        return this;
    }

    /// <summary>
    /// Executes an action if the result represents success.
    /// </summary>
    /// <param name="onSuccess">An action to execute with the success value. Not called if the result is a failure.</param>
    /// <returns>This result instance for method chaining.</returns>
    /// <remarks>
    /// Use this method when you only need to handle the success case and want to ignore failures.
    /// For handling both cases, use <see cref="Match"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// result
    ///     .IfSuccess(value => Console.WriteLine($"Got: {value}"))
    ///     .IfException(ex => Logger.Error(ex));
    /// </code>
    /// </example>
    public Result<TResult> IfSuccess(Action<TResult> onSuccess)
    {
        if (IsSuccess)
        {
            onSuccess(Value);
        }
        return this;
    }

    /// <summary>
    /// Asynchronously executes an action if the result represents success.
    /// </summary>
    /// <param name="onSuccess">An async function to execute with the success value. Not called if the result is a failure.</param>
    /// <returns>A task that completes with this result instance.</returns>
    public async Task<Result<TResult>> IfSuccessAsync(Func<TResult, Task> onSuccess)
    {
        if (IsSuccess)
        {
            await onSuccess(Value);
        }
        return this;
    }

    /// <summary>
    /// Executes an action if the result represents a failure.
    /// </summary>
    /// <param name="onFailure">An action to execute with the exception. Not called if the result is a success.</param>
    /// <returns>This result instance for method chaining.</returns>
    /// <remarks>
    /// Use this method for error handling, logging, or cleanup when you only need to handle the failure case.
    /// </remarks>
    /// <example>
    /// <code>
    /// result.IfException(ex => 
    /// {
    ///     Logger.Error("Operation failed", ex);
    ///     Metrics.IncrementErrorCount();
    /// });
    /// </code>
    /// </example>
    public Result<TResult> IfException(Action<Exception> onFailure)
    {
        if (!IsSuccess)
        {
            onFailure(Exception<Exception>());
        }
        return this;
    }

    /// <summary>
    /// Asynchronously executes an action if the result represents a failure.
    /// </summary>
    /// <param name="onFailure">An async function to execute with the exception. Not called if the result is a success.</param>
    /// <returns>A task that completes with this result instance.</returns>
    public async Task<Result<TResult>> IfExceptionAsync(Func<Exception, Task> onFailure)
    {
        if (!IsSuccess)
        {
            await onFailure(Exception<Exception>());
        }
        return this;
    }

    /// <summary>
    /// Asynchronously executes an action if the result is a failure with the specified exception type.
    /// </summary>
    /// <typeparam name="TException">The expected type of the failure exception.</typeparam>
    /// <param name="onFailure">An async function to execute with the typed exception.</param>
    /// <returns>A task that completes with this result instance.</returns>
    /// <exception cref="InvalidResultAccessOperationException">
    /// Thrown if the result is a failure but the exception cannot be cast to <typeparamref name="TException"/>.
    /// </exception>
    public async Task<Result<TResult>> IfExceptionAsync<TException>(Func<TException, Task> onFailure) where TException : Exception
    {
        if (!IsSuccess)
        {
            await onFailure(Exception<TException>());
        }
        return this;
    }

    /// <summary>
    /// Executes an action if the result is a failure with the specified exception type.
    /// </summary>
    /// <typeparam name="TException">The expected type of the failure exception.</typeparam>
    /// <param name="onFailure">An action to execute with the typed exception. Not called if the result is a success.</param>
    /// <returns>This result instance for method chaining.</returns>
    /// <exception cref="InvalidResultAccessOperationException">
    /// Thrown if the result is a failure but the exception cannot be cast to <typeparamref name="TException"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// result
    ///     .IfException&lt;ArgumentNullException&gt;(ex => HandleNullArg(ex.ParamName))
    ///     .IfException&lt;FormatException&gt;(ex => HandleBadFormat());
    /// </code>
    /// </example>
    public Result<TResult> IfException<TException>(Action<TException> onFailure) where TException : Exception
    {
        if (!IsSuccess)
        {
            onFailure(Exception<TException>());
        }
        return this;
    }

    /// <summary>
    /// Transforms the success value using the specified mapping function.
    /// </summary>
    /// <typeparam name="TNew">The type of the transformed value.</typeparam>
    /// <param name="map">A function that transforms the current value to a new value.</param>
    /// <returns>
    /// A new result containing the transformed value if this result is a success;
    /// otherwise, a failure result with the original exception.
    /// </returns>
    /// <remarks>
    /// <para>
    /// If this result is a failure, the failure is propagated without invoking the mapping function.
    /// This is a functor map operation that preserves the result structure.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = Result&lt;string&gt;.Success("42")
    ///     .Map(int.Parse)      // Result&lt;int&gt; with value 42
    ///     .Map(x => x * 2);    // Result&lt;int&gt; with value 84
    /// </code>
    /// </example>
    public Result<TNew> Map<TNew>(Func<TResult, TNew> map)
    {
        if (IsSuccess)
        {
            return Result<TNew>.Success(map(Value));
        }
        return Result<TNew>.Failure(Exception<Exception>());
    }

    /// <summary>
    /// Asynchronously transforms the success value using the specified mapping function.
    /// </summary>
    /// <typeparam name="TNew">The type of the transformed value.</typeparam>
    /// <param name="map">An async function that transforms the current value to a new value.</param>
    /// <returns>
    /// A task containing a new result with the transformed value if this result is a success;
    /// otherwise, a failure result with the original exception.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = await Result&lt;int&gt;.Success(userId)
    ///     .MapAsync(async id => await userRepository.GetByIdAsync(id));
    /// </code>
    /// </example>
    public async Task<Result<TNew>> MapAsync<TNew>(Func<TResult, Task<TNew>> map)
    {
        if (IsSuccess)
        {
            return Result<TNew>.Success(await map(Value));
        }
        return Result<TNew>.Failure(Exception<Exception>());
    }

    /// <summary>
    /// Chains a result-producing operation to this result, enabling flat composition.
    /// </summary>
    /// <typeparam name="TNew">The type of the value in the result returned by the binder function.</typeparam>
    /// <param name="bind">A function that takes the current value and returns a new result.</param>
    /// <returns>
    /// The result of the binder function if this result is a success;
    /// otherwise, a failure result with the original exception.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Also known as <c>FlatMap</c> or <c>SelectMany</c> in other functional libraries.
    /// This is a monadic bind operation that enables "Railway Oriented Programming" where
    /// operations that can fail are chained together, and any failure short-circuits the chain.
    /// </para>
    /// <para>
    /// Use <see cref="Bind{TNew}"/> instead of <see cref="Map{TNew}"/> when the transformation
    /// itself returns a <see cref="Result{TResult}"/>, to avoid nested results.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Chaining operations that can fail
    /// var result = ParseUserId(input)                    // Result&lt;int&gt;
    ///     .Bind(id => FindUser(id))                      // Result&lt;User&gt;
    ///     .Bind(user => ValidateUser(user))              // Result&lt;User&gt;
    ///     .Bind(user => CreateSession(user));            // Result&lt;Session&gt;
    /// 
    /// // If any step fails, subsequent steps are skipped
    /// </code>
    /// </example>
    public Result<TNew> Bind<TNew>(Func<TResult, Result<TNew>> bind)
    {
        if (IsSuccess)
        {
            return bind(Value);
        }
        return Result<TNew>.Failure(Exception<Exception>());
    }

    /// <summary>
    /// Asynchronously chains a result-producing operation to this result.
    /// </summary>
    /// <typeparam name="TNew">The type of the value in the result returned by the binder function.</typeparam>
    /// <param name="bind">An async function that takes the current value and returns a new result.</param>
    /// <returns>
    /// A task containing the result of the binder function if this result is a success;
    /// otherwise, a failure result with the original exception.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = await ParseUserId(input)
    ///     .BindAsync(async id => await FindUserAsync(id))
    ///     .BindAsync(async user => await ValidateUserAsync(user));
    /// </code>
    /// </example>
    public async Task<Result<TNew>> BindAsync<TNew>(Func<TResult, Task<Result<TNew>>> bind)
    {
        if (IsSuccess)
        {
            return await bind(Value);
        }
        return Result<TNew>.Failure(Exception<Exception>());
    }

    public bool IsFailure()
    {
        return !IsSuccess;
    }
}
