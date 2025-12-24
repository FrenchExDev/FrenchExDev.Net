namespace FrenchExDev.Net.CSharp.Object.Result;


/// <summary>
/// Represents the result of an operation.
/// </summary>
/// <remarks>Implementations of this interface typically provide information about the outcome of an operation,
/// such as success or failure, and may include additional details relevant to the operation's result.</remarks>
public interface IResult
{
    bool IsSuccess { get; }
}

/// <summary>
/// Exception class for all Result exceptions.
/// </summary>
public class ResultException : Exception { }

/// <summary>
/// Invalid result access exception class.
/// </summary>
public class InvalidResultAccessException : ResultException { }

/// <summary>
/// Represents the outcome of an operation, indicating success or failure.
/// </summary>
/// <remarks>Use the <see cref="Success"/> and <see cref="Failure"/> methods to create instances representing
/// successful or failed results. The <see cref="IsSuccess"/> property can be used to check whether the operation was
/// successful.</remarks>
public readonly struct Result : IResult
{
    /// <summary>
    /// Creates a new <see cref="Result"/> instance representing a successful operation.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating success.</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a new <see cref="Result"/> instance representing a failed operation.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating failure.</returns>
    public static Result Failure() => new(false, null);

    /// <summary>
    /// Creates a failed result that encapsulates the specified exception.
    /// </summary>
    /// <param name="ex">The exception that describes the reason for the failure. Cannot be null.</param>
    /// <returns>A <see cref="Result"/> instance representing a failure, containing the provided exception.</returns>
    public static Result Failure(Exception ex) => new(false, ex);

    /// <summary>
    /// Gets the exception that caused the current operation to fail, if any.
    /// </summary>
    /// <remarks>Use this property to retrieve detailed information about the error that occurred during the
    /// operation. If the operation completed successfully, this property returns null.</remarks>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents a failure state.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Initializes a new instance of the Result class with the specified success state.
    /// </summary>
    /// <param name="isSuccess">A value indicating whether the result represents a successful outcome. Specify <see langword="true"/> for
    /// success; otherwise, <see langword="false"/>.</param>
    private Result(bool isSuccess, Exception? exception)
    {
        IsSuccess = isSuccess;
        Exception = exception;
    }

    /// <summary>
    /// Invokes the specified action if the result represents a successful state.
    /// </summary>
    /// <remarks>This method enables conditional execution of logic based on the success state of the result.
    /// The action is only called if <see cref="IsSuccess"/> is <see langword="true"/>.</remarks>
    /// <param name="action">The action to execute, receiving a value indicating whether the result is successful. Cannot be null.</param>
    /// <returns>The current <see cref="Result"/> instance, allowing for method chaining.</returns>
    public Result IfSuccess(Action action)
    {
        if (IsSuccess)
        {
            action();
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified asynchronous action if the current result represents a successful state.
    /// </summary>
    /// <remarks>The action is only invoked if the result is successful; otherwise, it is skipped. This method
    /// enables chaining additional asynchronous operations based on the success of the result.</remarks>
    /// <param name="action">A function to execute asynchronously if the result is successful. The function receives a Boolean indicating the
    /// success state.</param>
    /// <returns>A task that represents the asynchronous operation. The result of the task is the current <see cref="Result"/>
    /// instance.</returns>
    public async Task<Result> IfSuccessAsync(Func<Task> action)
    {
        if (IsSuccess)
        {
            await action();
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified action if the result represents a failure.
    /// </summary>
    /// <remarks>Use this method to perform custom error handling or logging when a failure occurs. The
    /// provided action is only called if the result is in a failure state.</remarks>
    /// <param name="action">An action to execute, receiving the associated exception if the result is a failure; otherwise, the action is
    /// not invoked.</param>
    /// <returns>The current <see cref="Result"/> instance, allowing for method chaining.</returns>
    public Result IfFailure(Action<Exception?> action)
    {
        if (IsFailure)
        {
            action(Exception);
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified asynchronous action if the current result represents a failure.
    /// </summary>
    /// <remarks>Use this method to perform additional processing or logging when a failure occurs. The action
    /// is not invoked if the result is successful.</remarks>
    /// <param name="action">A function to execute asynchronously when the result is a failure. The function receives the associated
    /// exception, or null if no exception is available.</param>
    /// <returns>A task that represents the asynchronous operation. The result is the current <see cref="Result"/> instance.</returns>
    public async Task<Result> IfFailureAsync(Func<Exception?, Task> action)
    {
        if (IsFailure)
        {
            await action(Exception);
        }
        return this;
    }
}

/// <summary>
/// Represents the result of an operation that can succeed with a value of type TResult or fail with an exception of
/// type TException.
/// </summary>
/// <remarks>Use ResultEx<TResult, TException> to encapsulate the outcome of operations that may either produce a
/// result or fail with a specific exception. This struct provides methods and properties to inspect the result,
/// retrieve the value or exception, and perform actions based on success or failure. It is particularly useful for
/// scenarios where you want to avoid throwing exceptions for control flow and instead handle both success and failure
/// cases explicitly.</remarks>
/// <typeparam name="TResult">The type of the value returned on success. Must be a non-nullable type.</typeparam>
/// <typeparam name="TException">The type of the exception returned on failure. Must be a non-nullable type derived from Exception.</typeparam>
public readonly struct ResultEx<TResult, TException> : IResult where TResult : notnull where TException : notnull, Exception
{
    public static ResultEx<TResult, TException> Success(TResult instance) => new(instance);
    public static ResultEx<TResult, TException> Failure(TException exception) => new(exception);
    public bool IsSuccess => _hasValue;
    public bool IsFailure => !IsSuccess;
    public TResult Object => _hasValue ? _object! : throw new InvalidResultAccessException();
    public TException Exception => !_hasValue ? _exception! : throw new InvalidResultAccessException();
    private readonly TResult? _object;
    private readonly bool _hasValue;
    private readonly TException? _exception;

    /// <summary>
    /// Initializes a new instance of the Result class with the specified result value.
    /// </summary>
    /// <param name="result">The value to be stored in the result. May be null if the type T is nullable.</param>
    private ResultEx(TResult result)
    {
        _object = result;
        _hasValue = true;
        _exception = null;
    }

    /// <summary>
    /// Initializes a new instance of the Result class with the specified collection of failures.
    /// </summary>
    /// <param name="failures">A dictionary containing failure details to associate with this result. Cannot be null.</param>
    private ResultEx(TException failures)
    {
        _exception = failures;
        _object = default;
        _hasValue = false;
    }

    /// <summary>
    /// Attempts to retrieve the stored value.
    /// </summary>
    /// <param name="result">When this method returns, contains the value if one is present; otherwise, the default value for the type of the
    /// result parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if a value is present and was assigned to result; otherwise, false.</returns>
    public bool TryGetValue(out TResult result)
    {
        if (!_hasValue)
        {
            result = default(TResult);
            return false;
        }
        result = _object!;
        return true;
    }

    /// <summary>
    /// Attempts to retrieve the stored exception if one is present.
    /// </summary>
    /// <param name="exception">When this method returns, contains the exception of type TException if one is available; otherwise, the default
    /// value for TException. This parameter is passed uninitialized.</param>
    /// <returns>true if an exception is present and assigned to the exception parameter; otherwise, false.</returns>
    public bool TryGetException(out TException exception)
    {
        if (_hasValue)
        {
            exception = default(TException);
            return false;
        }
        exception = _exception!;
        return true;
    }

    /// <summary>
    /// Invokes the specified action if the result represents a successful outcome.
    /// </summary>
    /// <remarks>If the result is not successful, the action is not invoked. This method enables fluent
    /// handling of successful results.</remarks>
    /// <param name="action">The action to execute with the successful result value. Cannot be null.</param>
    /// <returns>The current result instance, allowing for method chaining.</returns>
    public ResultEx<TResult, TException> IfSuccess(Action<TResult> action)
    {
        if (IsSuccess)
        {
            action(_object!);
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified action if the result represents a failure.
    /// </summary>
    /// <remarks>If the result is not a failure, the specified action is not invoked. This method can be used
    /// to perform error handling or logging in a fluent manner.</remarks>
    /// <param name="action">The action to execute if the result is a failure. The exception associated with the failure is passed as a
    /// parameter; this value may be null depending on the context.</param>
    /// <returns>The current result instance. Enables method chaining.</returns>
    public ResultEx<TResult, TException> IfFailure(Action<TException?> action)
    {
        if (IsFailure)
        {
            action(_exception);
        }
        return this;
    }

    /// <summary>
    /// Returns the contained result object if available; otherwise, throws an exception.
    /// </summary>
    /// <returns>The result object of type <typeparamref name="TResult"/> if the operation was successful.</returns>
    /// <exception cref="InvalidResultAccessException">Thrown if the result does not contain a valid object.</exception>
    public TResult ObjectOrThrow()
    {
        return _object ?? throw new InvalidResultAccessException();
    }

    /// <summary>
    /// Returns the stored object if available; otherwise, returns null.
    /// </summary>
    /// <returns>The stored object of type TResult if present; otherwise, null.</returns>
    public TResult? ObjectOrNull()
    {
        return _object;
    }

    /// <summary>
    /// Attempts to retrieve the successful result value.
    /// </summary>
    /// <param name="value">When this method returns, contains the result value if the operation was successful; otherwise, the default
    /// value for the type of the result parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if a successful result value is available; otherwise, false.</returns>
    public bool TryGetSuccess(out TResult value)
    {
        if (_hasValue)
        {
            value = _object!;
            return true;
        }

        value = default(TResult);
        return false;
    }

    /// <summary>
    /// Attempts to retrieve the stored exception if the operation has failed.
    /// </summary>
    /// <param name="exception">When this method returns, contains the exception associated with the failure if the operation failed; otherwise,
    /// the default value for the exception type.</param>
    /// <returns>true if an exception is available and assigned to the out parameter; otherwise, false.</returns>
    public bool TryGetFailure(out TException exception)
    {
        if (!_hasValue)
        {
            exception = _exception!;
            return true;
        }

        exception = default(TException);
        return false;
    }

    /// <summary>
    /// Executes the specified asynchronous action if the result represents a successful value.
    /// </summary>
    /// <remarks>The provided function is only called if the result is successful. If the result is not
    /// successful, the function is not invoked and the original result is returned.</remarks>
    /// <param name="value">A function to invoke asynchronously with the result value if the operation was successful.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains this result instance, allowing for
    /// method chaining.</returns>
    public async Task<ResultEx<TResult, TException>> IfSuccessAsync(Func<TResult, Task> value)
    {
        if (_hasValue)
        {
            await value(_object!);
        }
        return this;
    }

    /// <summary>
    /// Executes the specified asynchronous action if the result represents a failure, passing the associated exception
    /// to the action.
    /// </summary>
    /// <remarks>Use this method to perform additional processing or logging when the result is a failure. The
    /// action is not invoked if the result is successful.</remarks>
    /// <param name="action">A function to invoke if the result is a failure. The function receives the exception associated with the
    /// failure, or null if no exception is present.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is this result instance, allowing for method
    /// chaining.</returns>
    public async Task<ResultEx<TResult, TException>> IfFailureAsync(Func<TException?, Task> action)
    {
        if (!IsSuccess)
        {
            await action(_exception);
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified action if the result represents a successful outcome.
    /// </summary>
    /// <remarks>If the result is not successful, the specified action is not invoked. This method enables
    /// fluent handling of success scenarios.</remarks>
    /// <param name="action">The action to execute if the result is successful. Cannot be null.</param>
    /// <returns>The current result instance, allowing for method chaining.</returns>
    public ResultEx<TResult, TException> IfSuccess(Action action)
    {
        if (IsSuccess)
        {
            action();
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified action if the result represents a failure.
    /// </summary>
    /// <remarks>Use this method to perform side effects or error handling logic when the result indicates
    /// failure. The action is not invoked if the result is successful.</remarks>
    /// <param name="action">The action to execute when the result is in a failure state. Cannot be null.</param>
    /// <returns>The current result instance. Enables method chaining.</returns>
    public ResultEx<TResult, TException> IfFailure(Action action)
    {
        if (IsFailure)
        {
            action();
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified delegate based on the result state and returns the current result instance.
    /// </summary>
    /// <remarks>Use this method to perform side effects or handle both success and failure cases without
    /// altering the result. The delegates are invoked for their side effects; their return values are
    /// ignored.</remarks>
    /// <param name="onSuccess">A delegate to invoke if the result represents a success. The delegate receives the successful result value as
    /// its parameter.</param>
    /// <param name="onFailure">A delegate to invoke if the result represents a failure. The delegate receives the exception value as its
    /// parameter.</param>
    /// <returns>The current result instance, unchanged.</returns>
    public ResultEx<TResult, TException> Match(Action<TResult> onSuccess, Action<TException> onFailure)
    {
        if (IsFailure)
        {
            onFailure(_exception!);
            return this;
        }

        onSuccess(_object!);
        return this;
    }

    /// <summary>
    /// Executes the specified function and returns a result object indicating success or failure based on whether an
    /// exception is thrown.
    /// </summary>
    /// <remarks>This method is typically used to simplify error handling by encapsulating the try-catch logic
    /// and returning a standardized result object. Only exceptions of type <typeparamref name="TException"/> are
    /// caught; other exceptions will propagate.</remarks>
    /// <param name="value">A function that produces the result to be executed within the try-catch block.</param>
    /// <returns>An object representing the outcome of the operation. If the function completes successfully, a success result is
    /// returned; if a <typeparamref name="TException"/> is thrown, a failure result containing the exception is
    /// returned.</returns>
    public static object TryCatch(Func<TResult> value)
    {
        try
        {
            return Success(value());
        }
        catch (TException ex)
        {
            return Failure(ex);
        }
    }
}

/// <summary>
/// Represents the result of an operation that may succeed or fail, encapsulating an optional object of type T.
/// </summary>
/// <remarks>Use the Success and Failure static methods to create instances representing successful or failed
/// operations. The IsSuccess property indicates whether the result contains a valid object. Access the contained object
/// using Object, ObjectOrNull(), or ObjectOrThrow().</remarks>
/// <typeparam name="T">The type of the object contained in the result.</typeparam>
public readonly struct Result<T> : IResult where T : notnull
{
    /// <summary>
    /// Returns a Success
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static Result<T> Success(T instance) => new(instance);

    /// <summary>
    /// Returns a failure
    /// </summary>
    /// <returns></returns>
    public static Result<T> Failure() => new();

    /// <summary>
    /// Returns a failure with given failures.
    /// </summary>
    /// <param name="failures"></param>
    /// <returns></returns>
    public static Result<T> Failure(FailureDictionary failures) => new(failures);

    /// <summary>
    /// Returns a failure with a body builder for failures Dictionary building.
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public static Result<T> Failure(Action<FailureDictionaryBuilder> body)
    {
        var builder = new FailureDictionaryBuilder();
        body(builder);
        return Failure(builder.Build());
    }

    /// <summary>
    /// Creates a failed result containing the specified exception.
    /// </summary>
    /// <param name="ex">The exception to associate with the failure. Cannot be null.</param>
    /// <returns>A <see cref="Result{T}"/> representing a failed operation with the provided exception included in its error
    /// details.</returns>
    public static Result<T> Failure(Exception ex)
    {
        return Exception(ex);
    }

    /// <summary>
    /// Creates a failed result containing the specified exception information.
    /// </summary>
    /// <remarks>Use this method to propagate exception details in a result object, enabling error handling
    /// without throwing exceptions. The exception is stored in the result's error data under the key
    /// "Exception".</remarks>
    /// <param name="ex">The exception to associate with the failure result. Cannot be null.</param>
    /// <returns>A failed <see cref="Result{T}"/> instance that includes the provided exception details.</returns>
    public static Result<T> Exception(Exception ex)
    {
        return Failure(d => d.Add("Exception", ex));
    }

    /// <summary>
    /// Gets a value indicating whether the result contains a valid object.
    /// </summary>
    public bool IsSuccess => _hasValue;

    /// <summary>
    /// Gets a value indicating whether the result represents a failure state.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the value of the object contained by this instance.
    /// </summary>
    public T Object => _hasValue ? _object! : throw new InvalidResultAccessException();

    /// <summary>
    /// Holds failures
    /// </summary>
    public FailureDictionary? Failures { get; init; }

    /// <summary>
    /// Returns the collection of failures associated with the result, or throws an exception if no failures are
    /// present.
    /// </summary>
    /// <returns>A <see cref="FailureDictionary"/> containing all failures for the result.</returns>
    /// <exception cref="InvalidResultAccessException">Thrown if the result does not contain any failures.</exception>
    public FailureDictionary FailuresOrThrow() => Failures ?? throw new InvalidResultAccessException();

    /// <summary>
    /// Stores the result object on success.
    /// </summary>
    private readonly T? _object;

    // indicates whether this instance represents a successful value
    private readonly bool _hasValue;

    /// <summary>
    /// Initializes a new instance of the Result class with the specified result value.
    /// </summary>
    /// <param name="result">The value to be stored in the result. May be null if the type T is nullable.</param>
    private Result(T result)
    {
        _object = result;
        _hasValue = true;
        Failures = null;
    }

    /// <summary>
    /// Initializes a new instance of the Result class with the specified collection of failures.
    /// </summary>
    /// <param name="failures">A dictionary containing failure details to associate with this result. Cannot be null.</param>
    private Result(FailureDictionary failures)
    {
        Failures = failures;
        _object = default;
        _hasValue = false;
    }

    /// <summary>
    /// Returns the contained object if available; otherwise, returns null.
    /// </summary>
    /// <returns>The object of type T if present; otherwise, null.</returns>
    public T? ObjectOrNull() => _hasValue ? _object : default;

    /// <summary>
    /// Returns the contained object if it is available; otherwise, throws an exception.
    /// </summary>
    /// <returns>The object of type <typeparamref name="T"/> if it is present.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the object is not available.</exception>
    public T ObjectOrThrow() => _hasValue ? _object! : throw new InvalidOperationException("result is empty");

    /// <summary>
    /// Attempts to retrieve the successful result value if the operation completed successfully.
    /// </summary>
    /// <param name="result">When this method returns, contains the result value if the operation was successful; otherwise, the default
    /// value for type <typeparamref name="T"/>.</param>
    /// <returns>true if the operation was successful and the result was retrieved; otherwise, false.</returns>
    public bool TryGetSuccess(out T result)
    {
        if (!_hasValue)
        {
            result = default(T);
            return false;
        }
        result = _object!;
        return true;
    }

    /// <summary>
    /// Executes the specified action and returns a successful result if no exception of type TException is thrown;
    /// otherwise, returns a failure result containing the exception.
    /// </summary>
    /// <remarks>Use this method to simplify error handling by converting exceptions of a specific type into
    /// failure results, enabling fluent result-based programming patterns. Exceptions not of type TException are not
    /// caught and will propagate.</remarks>
    /// <typeparam name="TResult">The type of the value returned by the action. Must be non-null.</typeparam>
    /// <typeparam name="TException">The type of exception to catch and convert to a failure result. Must be a non-null exception type.</typeparam>
    /// <param name="action">A function that produces the result to be returned. This delegate is executed within a try-catch block.</param>
    /// <returns>A Result<TResult> representing either a successful outcome with the returned value, or a failure containing the
    /// caught exception of type TException.</returns>
    public static Result<TResult> TryCatch<TResult, TException>(Func<TResult> action) where TResult : notnull where TException : notnull, Exception
    {
        try { return action().ToSuccess(); } catch (TException ex) { return ex.ToFailure<TResult>(); }
    }

    /// <summary>
    /// Executes the specified function and returns a result that indicates success or failure, capturing any exception
    /// that occurs during execution.
    /// </summary>
    /// <remarks>Use this method to simplify error handling by encapsulating exceptions as failure results,
    /// rather than allowing them to propagate. This is useful for functional programming patterns or when chaining
    /// operations that may fail.</remarks>
    /// <typeparam name="TResult">The type of the value returned by the function. This type must not be null.</typeparam>
    /// <param name="action">The function to execute. This delegate should return a value of type TResult when called.</param>
    /// <returns>A Result<TResult> that contains the value returned by the function if execution is successful; otherwise, a
    /// failure result containing the captured exception.</returns>
    public static Result<TResult> TryCatch<TResult>(Func<Result<TResult>> action) where TResult : notnull
    {
        try { return action(); } catch (Exception ex) { return ex.ToFailure<TResult>(); }
    }

    /// <summary>
    /// Invokes the specified action if the result represents a successful outcome.
    /// </summary>
    /// <remarks>Use this method to perform additional operations only when the result is successful. If the
    /// result is not successful, the action is ignored and the original result is returned unchanged.</remarks>
    /// <param name="action">The action to execute with the value of the successful result. The action is not invoked if the result is not
    /// successful.</param>
    /// <returns>The current <see cref="Result{T}"/> instance, allowing for method chaining.</returns>
    public Result<T> IfSuccess(Action<T> action)
    {
        if (IsSuccess)
        {
            action(_object!);
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified asynchronous action if the result represents a successful outcome.
    /// </summary>
    /// <remarks>If the result is not successful, the action is not invoked and the method returns
    /// immediately. The action is awaited before returning.</remarks>
    /// <param name="action">A function to execute asynchronously if the result is successful. The function receives the successful value as
    /// its argument.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the current result instance.</returns>
    public async Task<Result<T>> IfSuccessAsync(Func<T, Task> action)
    {
        if (IsSuccess)
        {
            await action(_object!);
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified action if the result represents a failure.
    /// </summary>
    /// <remarks>This method enables handling failure cases in a fluent manner. The action is not invoked if
    /// the result is successful.</remarks>
    /// <param name="action">An action to execute with the failure details if the result is a failure. The parameter provides a dictionary
    /// containing failure information, or null if no details are available.</param>
    /// <returns>The current <see cref="Result{T}"/> instance, allowing for method chaining.</returns>
    public Result<T> IfFailure(Action<FailureDictionary?> action)
    {
        if (IsFailure)
        {
            action(Failures);
        }
        return this;
    }

    /// <summary>
    /// Invokes the specified asynchronous action if the result represents a failure.
    /// </summary>
    /// <remarks>Use this method to perform additional processing or logging when a failure occurs. The action
    /// is not invoked if the result is successful.</remarks>
    /// <param name="action">A function to execute asynchronously when the result is a failure. The function receives the collection of
    /// failures, or null if no failure details are available.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the current result instance.</returns>
    public async Task<Result<T>> IfFailureAsync(Func<FailureDictionary?, Task> action)
    {
        if (IsFailure)
        {
            await action(Failures);
        }
        return this;
    }
}

/// <summary>
/// Represents a collection that maps string keys to lists of failure objects.
/// </summary>
/// <remarks>Use this class to organize and access multiple failure details grouped by string identifiers.
/// Inherits all standard dictionary functionality, allowing enumeration, addition, and removal of entries. The
/// constructor enables initialization from an existing dictionary of string keys and associated failure
/// lists.</remarks>
public class FailureDictionary : Dictionary<string, List<object>>
{
    /// <summary>
    /// Initializes a new instance of the FailureDictionary class.
    /// </summary>
    public FailureDictionary() : base() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dictionary"></param>
    public FailureDictionary(IDictionary<string, List<object>> dictionary) : base(dictionary)
    {
    }

    /// <summary>
    /// Adds a failure entry with the specified name and value to the dictionary. If the name does not exist, a new
    /// entry is created.   
    /// </summary>
    /// <remarks>If an entry for the specified name already exists, the value is appended to its list.
    /// Otherwise, a new list is created for the name. This method enables method chaining by returning the dictionary
    /// instance.</remarks>
    /// <param name="name">The key representing the failure name to add or update. Cannot be null.</param>
    /// <param name="value">The value associated with the failure name. Can be any object.</param>
    /// <returns>The current <see cref="FailureDictionary"/> instance with the added failure entry.</returns>
    public FailureDictionary Add(string name, object value)
    {
        if (!TryGetValue(name, out var list))
        {
            list = new List<object>();
            this[name] = list;
        }

        list.Add(value);
        return this;
    }
}

/// <summary>
/// Provides a builder for constructing a FailureDictionary instance with custom failure entries.
/// </summary>
public class FailureDictionaryBuilder
{
    private readonly Dictionary<string, List<object>> _internal = new();

    /// <summary>
    /// Adds a failure entry with the specified name and value to the dictionary builder.
    /// </summary>
    /// <remarks>If an entry with the specified name does not exist, a new entry is created. This method
    /// supports fluent chaining of multiple additions.</remarks>
    /// <param name="name">The key that identifies the failure entry. Cannot be null.</param>
    /// <param name="value">The value associated with the failure entry. Cannot be null.</param>
    /// <returns>The current <see cref="FailureDictionaryBuilder"/> instance, allowing for method chaining.</returns>
    public FailureDictionaryBuilder Add<T>(string name, T value) where T : notnull
    {
        if (!_internal.TryGetValue(name, out var list))
        {
            list = new List<object>();
            _internal[name] = list;
        }

        list.Add(value);
        return this;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="FailureDictionary"/> containing the current set of failure entries.
    /// </summary>
    /// <remarks>Use this method to obtain a finalized, read-only dictionary of failures for further
    /// processing or inspection. Subsequent modifications to the builder will not affect the returned
    /// dictionary.</remarks>
    /// <returns>A <see cref="FailureDictionary"/> that includes all failures added to this builder. The returned dictionary is
    /// independent and will not reflect subsequent changes to the builder.</returns>
    public FailureDictionary Build()
    {
        // create a deep copy of the internal dictionary so the returned FailureDictionary is independent
        var copy = new Dictionary<string, List<object>>();
        foreach (var kv in _internal)
        {
            copy[kv.Key] = new List<object>(kv.Value);
        }
        return new FailureDictionary(copy);
    }
}

/// <summary>
/// Provides extension methods for converting values and objects to standardized result types.
/// </summary>
/// <remarks>These extension methods simplify the creation of success and failure result objects, enabling a more
/// fluent and expressive approach to handling operation outcomes. The methods are designed to work with generic and
/// non-generic result types, and can be used to encapsulate both successful and failed states along with relevant
/// data.</remarks>
public static class Extensions
{
    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value to include in the result.</typeparam>
    /// <param name="value">The value to wrap in a successful result.</param>
    /// <returns>A Result<T> instance representing a successful operation with the specified value.</returns>
    public static Result<T> ToSuccess<T>(this T value) where T : notnull
    {
        return Result<T>.Success(value);
    }

    /// <summary>
    /// Converts a Boolean value to a corresponding Result instance representing success or failure.
    /// </summary>
    /// <param name="isSuccess">A value indicating whether the operation was successful. If <see langword="true"/>, a successful result is
    /// returned; otherwise, a failure result is returned.</param>
    /// <returns>A Result instance representing success if <paramref name="isSuccess"/> is <see langword="true"/>; otherwise, a
    /// Result representing failure.</returns>
    public static Result ToResult(this bool isSuccess)
    {
        return isSuccess ? Result.Success() : Result.Failure();
    }

    /// <summary>
    /// Creates a failed result containing the specified value.
    /// </summary>
    /// <remarks>The returned result will include both the failure value and the subject in its failure
    /// details. Use this method to conveniently create a failure result when you have an object and a corresponding
    /// failure reason.</remarks>
    /// <typeparam name="T">The type of the subject for which the failure result is created.</typeparam>
    /// <param name="subject">The subject instance to associate with the failure. Cannot be null.</param>
    /// <param name="value">The value describing the failure to associate with the result.</param>
    /// <returns>A failed result containing the subject and the specified failure value.</returns>
    public static Result<T> ToFailure<T>(this T subject, object value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(subject);

        var failures = new FailureDictionary();
        failures.Add("Failure", value);
        failures.Add("Subject", subject);
        return Result<T>.Failure(failures);
    }

    /// <summary>
    /// Creates a failed result of type <typeparamref name="T"/> that represents the specified exception.
    /// </summary>
    /// <remarks>This extension method allows exceptions to be easily converted into failed result objects,
    /// enabling consistent error handling in result-based workflows.</remarks>
    /// <typeparam name="T">The type of the value that would be held by a successful result.</typeparam>
    /// <param name="ex">The exception to associate with the failed result. Cannot be null.</param>
    /// <returns>A <see cref="Result{T}"/> representing a failure with the specified exception.</returns>
    public static Result<T> ToFailure<T>(this Exception ex) where T : notnull
    {
        return Result<T>.Exception(ex);
    }
}
