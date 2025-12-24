namespace FrenchExDev.Net.CSharp.Object.Result2;

/// <summary>
/// Represents the result of an operation, indicating whether it was successful.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    bool IsSuccess { get; }
}

/// <summary>
/// Represents errors that occur when an operation returns a result indicating failure.
/// </summary>
/// <remarks>Use this exception to signal that an operation completed but produced a result that represents an
/// error or failure condition, rather than throwing a different exception type. This can be useful in scenarios where
/// operations return result objects that encapsulate both success and failure states.</remarks>
public class ResultException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ResultException class.
    /// </summary>
    public ResultException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ResultException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error. This value can be null.</param>
    public ResultException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ResultException class with a specified error message and a reference to the
    /// inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error, or null to use the default message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
    public ResultException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// The exception that is thrown when an invalid operation is performed on a result object, such as accessing a value
/// that is not present.
/// </summary>
/// <remarks>This exception typically indicates a misuse of a result type, such as attempting to retrieve a value
/// from a failed result or an error from a successful result. Catch this exception to handle cases where result access
/// patterns are violated.</remarks>
public class InvalidResultAccessOperationException : ResultException
{
    /// <summary>
    /// Initializes a new instance of the InvalidResultAccessOperationException class.
    /// </summary>
    public InvalidResultAccessOperationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the InvalidResultAccessOperationException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidResultAccessOperationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the InvalidResultAccessOperationException class with a specified error message and
    /// a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error. This value can be null.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
    public InvalidResultAccessOperationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Represents the outcome of an operation that can succeed or fail, encapsulating either a result value or an
/// exception.
/// </summary>
/// <remarks>Use this struct to model operations that may fail, providing a unified way to handle both successful
/// results and errors without relying on exceptions for control flow. The result indicates success or failure via the
/// IsSuccess property, and provides access to the value or exception as appropriate.</remarks>
/// <typeparam name="TResult">The type of the value returned if the operation is successful.</typeparam>
public readonly struct Result<TResult> : IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public readonly bool IsSuccess { get; }

    /// <summary>
    /// Stores the result value if the operation was successful; otherwise, null.
    /// </summary>
    private readonly TResult? _value;

    /// <summary>
    /// Stores the exception if the operation failed; otherwise, null.  
    /// </summary>
    private readonly Exception? _exception;

    /// <summary>
    /// Initializes a new instance of the Result class with the specified success state, value, and exception.
    /// </summary>
    /// <param name="isSuccess">A value indicating whether the result represents a successful operation.</param>
    /// <param name="value">The value associated with a successful result, or null if the operation failed.</param>
    /// <param name="exception">The exception associated with a failed result, or null if the operation was successful.</param>
    private Result(bool isSuccess, TResult? value, Exception? exception)
    {
        IsSuccess = isSuccess;
        _value = value;
        _exception = exception;
    }

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <param name="value">The value to associate with the successful result.</param>
    /// <returns>A <see cref="Result{TResult}"/> representing a successful operation with the specified value.</returns>
    public static Result<TResult> Success(TResult value)
    {
        return new Result<TResult>(true, value, null);
    }

    /// <summary>
    /// Creates a failed result containing the specified exception.
    /// </summary>
    /// <typeparam name="TException">The type of exception to associate with the failed result. Must derive from Exception.</typeparam>
    /// <param name="exception">The exception that describes the reason for the failure. Cannot be null.</param>
    /// <returns>A Result<TResult> instance representing a failure, containing the specified exception.</returns>
    public static Result<TResult> Failure<TException>(TException exception) where TException : Exception
    {
        return new Result<TResult>(false, default, exception);
    }

    /// <summary>
    /// Gets the value contained in the result if the operation was successful.
    /// </summary>
    /// <remarks>Accessing this property when the result represents a failure will throw an exception. To
    /// avoid exceptions, check the IsSuccess property before accessing Value.</remarks>
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
    /// Retrieves the exception associated with a failed result, cast to the specified exception type.
    /// </summary>
    /// <remarks>Use this method to access the exception when the result indicates failure. If the result is
    /// successful, or if the exception is not of the specified type, an exception is thrown.</remarks>
    /// <typeparam name="TException">The type of exception to return. Must derive from Exception.</typeparam>
    /// <returns>The exception instance of type TException associated with the failed result.</returns>
    /// <exception cref="InvalidResultAccessOperationException">Thrown if the result represents a success, or if the exception cannot be cast to the specified type.</exception>
    public TException Exception<TException>() where TException : Exception
    {
        if (IsSuccess)
        {
            throw new InvalidResultAccessOperationException("Cannot access Exception when the result is a success.");
        }
        return _exception as TException ?? throw new InvalidResultAccessOperationException("Exception is null after cast to TException");
    }

    /// <summary>
    /// Attempts to retrieve the stored exception as the specified exception type.
    /// </summary>
    /// <remarks>Use this method to safely attempt to access the stored exception as a specific type without
    /// throwing an exception. If the operation was successful, or if the stored exception is not of the requested type,
    /// the method returns false and sets exception to null.</remarks>
    /// <typeparam name="TException">The type of exception to retrieve. Must derive from Exception.</typeparam>
    /// <param name="exception">When this method returns, contains the exception of type TException if one is available; otherwise, null. This
    /// parameter is passed uninitialized.</param>
    /// <returns>true if the stored exception is of type TException; otherwise, false.</returns>
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
    /// Attempts to retrieve the result value if the operation was successful.
    /// </summary>
    /// <param name="value">When this method returns, contains the result value if the operation was successful; otherwise, the default
    /// value for the type.</param>
    /// <returns>true if the operation was successful and the result value was retrieved; otherwise, false.</returns>
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
    /// Asynchronously invokes the specified delegate based on the result state, calling the success handler if the
    /// operation succeeded or the failure handler if it failed.
    /// </summary>
    /// <remarks>Both delegates are awaited. If the result is successful, only the success handler is called;
    /// otherwise, only the failure handler is called. Neither delegate may be null.</remarks>
    /// <param name="onSuccess">A delegate to invoke if the result represents a successful operation. The delegate receives the result value as
    /// its parameter.</param>
    /// <param name="onFailure">A delegate to invoke if the result represents a failed operation. The delegate receives the exception associated
    /// with the failure as its parameter.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is this result instance.</returns>
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
    /// Asynchronously invokes the specified delegate based on the result state. Executes the success delegate if the
    /// result is successful; otherwise, executes the failure delegate for the specified exception type.
    /// </summary>
    /// <remarks>If the result is not successful and the exception is not of type TException, the onFailure
    /// delegate will receive a default instance of TException. This method allows for custom asynchronous handling of
    /// both success and failure cases.</remarks>
    /// <typeparam name="TException">The type of exception to handle when the result is not successful. Must derive from Exception.</typeparam>
    /// <param name="onSuccess">A delegate to invoke asynchronously if the result is successful. Receives the result value as its parameter.</param>
    /// <param name="onFailure">A delegate to invoke asynchronously if the result is not successful and the exception is of type TException.
    /// Receives the exception as its parameter.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the original result instance.</returns>
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
    /// Invokes the specified action depending on whether the result represents a success or a failure.
    /// </summary>
    /// <remarks>Use this method to handle both success and failure cases in a single call. Only one of the
    /// provided actions will be invoked, depending on the state of the result.</remarks>
    /// <param name="onSuccess">The action to execute if the result is successful. The result value is passed as a parameter to this action.
    /// Cannot be null.</param>
    /// <param name="onFailure">The action to execute if the result represents a failure. The exception is passed as a parameter to this action.
    /// Cannot be null.</param>
    /// <returns>The current result instance, allowing for method chaining.</returns>
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
    /// Invokes the specified action for a successful result or an error action for a failure, enabling pattern matching
    /// on the result state.
    /// </summary>
    /// <remarks>If the result is successful, only the onSuccess action is invoked. If the result is a
    /// failure, only the onFailure action is invoked with the exception cast to TException. This method enables
    /// functional-style handling of result states.</remarks>
    /// <typeparam name="TException">The type of exception to handle when the result represents a failure. Must derive from Exception.</typeparam>
    /// <param name="onSuccess">The action to execute if the result is successful. The result value is passed as a parameter.</param>
    /// <param name="onFailure">The action to execute if the result represents a failure. The exception of type TException is passed as a
    /// parameter.</param>
    /// <returns>The current Result<TResult> instance, allowing for method chaining.</returns>
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
    /// Invokes the specified action if the result represents a successful outcome.
    /// </summary>
    /// <remarks>This method enables fluent handling of successful results by executing the provided action
    /// only when the result is successful. If the result is not successful, the action is not invoked.</remarks>
    /// <param name="onSuccess">The action to execute with the result value if the operation was successful. Cannot be null.</param>
    /// <returns>The current result instance. If the result is successful, the action is invoked before returning.</returns>
    public Result<TResult> IfSuccess(Action<TResult> onSuccess)
    {
        if (IsSuccess)
        {
            onSuccess(Value);
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified asynchronous action if the result represents a successful outcome.
    /// </summary>
    /// <remarks>If the result is not successful, the specified action is not invoked. This method enables
    /// fluent chaining of actions to be performed only on successful results.</remarks>
    /// <param name="onSuccess">A function to execute asynchronously if the result is successful. The function receives the result value as its
    /// parameter.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains this result instance.</returns>
    public async Task<Result<TResult>> IfSuccessAsync(Func<TResult, Task> onSuccess)
    {
        if (IsSuccess)
        {
            await onSuccess(Value);
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified action if the result represents a failure due to an exception.
    /// </summary>
    /// <remarks>Use this method to perform custom error handling or logging when the result contains an
    /// exception. The action is not called if the result is successful.</remarks>
    /// <param name="onFailure">The action to execute with the exception if the result is not successful. Cannot be null.</param>
    /// <returns>The current result instance, enabling method chaining.</returns>
    public Result<TResult> IfException(Action<Exception> onFailure)
    {
        if (!IsSuccess)
        {
            onFailure(Exception<Exception>());
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified asynchronous action if the result represents a failure due to an exception.
    /// </summary>
    /// <remarks>Use this method to perform additional actions, such as logging or cleanup, when an exception
    /// has occurred. The action is not invoked if the result is successful.</remarks>
    /// <param name="onFailure">A function to execute when the result contains an exception. The function receives the exception as a parameter
    /// and returns a task that represents the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the original result instance.</returns>
    public async Task<Result<TResult>> IfExceptionAsync(Func<Exception, Task> onFailure)
    {
        if (!IsSuccess)
        {
            await onFailure(Exception<Exception>());
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified asynchronous delegate if the result represents a failure caused by the specified exception
    /// type.
    /// </summary>
    /// <remarks>Use this method to perform additional asynchronous actions, such as logging or cleanup, when
    /// a specific exception type is present in the result. The delegate is not invoked if the result is successful or
    /// if the exception is not of type TException.</remarks>
    /// <typeparam name="TException">The type of exception to match. Must derive from Exception.</typeparam>
    /// <param name="onFailure">An asynchronous delegate to execute if the result contains an exception of type TException. The delegate
    /// receives the exception instance as its parameter.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the original result object.</returns>
    public async Task<Result<TResult>> IfExceptionAsync<TException>(Func<TException, Task> onFailure) where TException : Exception
    {
        if (!IsSuccess)
        {
            await onFailure(Exception<TException>());
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified action if the result represents a failure caused by an exception of the specified type.
    /// </summary>
    /// <remarks>Use this method to perform custom handling or logging when a specific exception type is
    /// present in the result. If the result is successful or the exception is not of type TException, the action is not
    /// invoked.</remarks>
    /// <typeparam name="TException">The type of exception to match. Must derive from Exception.</typeparam>
    /// <param name="onFailure">The action to execute if the result contains an exception of type TException. The exception instance is passed
    /// as a parameter to the action.</param>
    /// <returns>The current result instance, allowing for method chaining.</returns>
    public Result<TResult> IfException<TException>(Action<TException> onFailure) where TException : Exception
    {
        if (!IsSuccess)
        {
            onFailure(Exception<TException>());
        }
        return this;
    }

    /// <summary>
    /// Transforms the value of a successful result using the specified mapping function.
    /// </summary>
    /// <remarks>If the result is a failure, the failure is propagated without invoking the mapping function.
    /// This method is useful for transforming the contained value while preserving the result structure.</remarks>
    /// <typeparam name="TNew">The type of the transformed value.</typeparam>
    /// <param name="map">A function that transforms the current value to a new value.</param>
    /// <returns>A new result containing the transformed value if successful; otherwise, a failure result with the original exception.</returns>
    public Result<TNew> Map<TNew>(Func<TResult, TNew> map)
    {
        if (IsSuccess)
        {
            return Result<TNew>.Success(map(Value));
        }
        return Result<TNew>.Failure(Exception<Exception>());
    }

    /// <summary>
    /// Asynchronously transforms the value of a successful result using the specified mapping function.
    /// </summary>
    /// <remarks>If the result is a failure, the failure is propagated without invoking the mapping function.
    /// This method is useful for transforming the contained value asynchronously while preserving the result structure.</remarks>
    /// <typeparam name="TNew">The type of the transformed value.</typeparam>
    /// <param name="map">An asynchronous function that transforms the current value to a new value.</param>
    /// <returns>A task containing a new result with the transformed value if successful; otherwise, a failure result with the original exception.</returns>
    public async Task<Result<TNew>> MapAsync<TNew>(Func<TResult, Task<TNew>> map)
    {
        if (IsSuccess)
        {
            return Result<TNew>.Success(await map(Value));
        }
        return Result<TNew>.Failure(Exception<Exception>());
    }

    /// <summary>
    /// Chains a result-producing operation to this result, enabling flat composition without nesting.
    /// </summary>
    /// <remarks>If this result is successful, the binder function is invoked with the value and its result is returned.
    /// If this result is a failure, the failure is propagated without invoking the binder function.
    /// This method is also known as FlatMap or SelectMany in other functional libraries.</remarks>
    /// <typeparam name="TNew">The type of the value in the result returned by the binder function.</typeparam>
    /// <param name="bind">A function that takes the current value and returns a new result.</param>
    /// <returns>The result of the binder function if this result is successful; otherwise, a failure result with the original exception.</returns>
    public Result<TNew> Bind<TNew>(Func<TResult, Result<TNew>> bind)
    {
        if (IsSuccess)
        {
            return bind(Value);
        }
        return Result<TNew>.Failure(Exception<Exception>());
    }

    /// <summary>
    /// Asynchronously chains a result-producing operation to this result, enabling flat composition without nesting.
    /// </summary>
    /// <remarks>If this result is successful, the binder function is invoked with the value and its result is returned.
    /// If this result is a failure, the failure is propagated without invoking the binder function.
    /// This method is also known as FlatMap or SelectMany in other functional libraries.</remarks>
    /// <typeparam name="TNew">The type of the value in the result returned by the binder function.</typeparam>
    /// <param name="bind">An asynchronous function that takes the current value and returns a new result.</param>
    /// <returns>A task containing the result of the binder function if this result is successful; otherwise, a failure result with the original exception.</returns>
    public async Task<Result<TNew>> BindAsync<TNew>(Func<TResult, Task<Result<TNew>>> bind)
    {
        if (IsSuccess)
        {
            return await bind(Value);
        }
        return Result<TNew>.Failure(Exception<Exception>());
    }
}
