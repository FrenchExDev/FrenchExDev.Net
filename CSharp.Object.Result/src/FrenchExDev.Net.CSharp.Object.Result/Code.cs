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
    /// Initializes a new instance of the Result class with the specified success state.
    /// </summary>
    /// <param name="isSuccess">A value indicating whether the result represents a successful outcome. Specify <see langword="true"/> for
    /// success; otherwise, <see langword="false"/>.</param>
    private Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents a failure state.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Creates a new <see cref="Result"/> instance representing a successful operation.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating success.</returns>
    public static Result Success() => new(true);

    /// <summary>
    /// Creates a new <see cref="Result"/> instance representing a failed operation.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating failure.</returns>
    public static Result Failure() => new(false);
}

/// <summary>
/// Represents the result of an operation that may succeed or fail, encapsulating an optional object of type T.
/// </summary>
/// <remarks>Use the Success and Failure static methods to create instances representing successful or failed
/// operations. The IsSuccess property indicates whether the result contains a valid object. Access the contained object
/// using Object, ObjectOrNull(), or ObjectOrThrow().</remarks>
/// <typeparam name="T">The type of the object contained in the result.</typeparam>
public readonly struct Result<T> : IResult
{
    /// <summary>
    /// Stores the result object on success.
    /// </summary>
    private readonly T? _object;

    /// <summary>
    /// Gets a value indicating whether the result contains a valid object.
    /// </summary>
    public bool IsSuccess => _object is not null;

    /// <summary>
    /// Gets a value indicating whether the result represents a failure state.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the value of the object contained by this instance.
    /// </summary>
    public T Object => _object is not null ? _object : throw new InvalidResultAccessException();

    /// <summary>
    /// Initializes a new instance of the Result class with the specified result value.
    /// </summary>
    /// <param name="result">The value to be stored in the result. May be null if the type T is nullable.</param>
    private Result(T result)
    {
        _object = result;
    }

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
    /// Initializes a new instance of the Result class with the specified collection of failures.
    /// </summary>
    /// <param name="failures">A dictionary containing failure details to associate with this result. Cannot be null.</param>
    private Result(FailureDictionary failures)
    {
        Failures = failures;
    }

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
    /// Returns the contained object if available; otherwise, returns null.
    /// </summary>
    /// <returns>The object of type T if present; otherwise, null.</returns>
    public T? ObjectOrNull() => _object;

    /// <summary>
    /// Returns the contained object if it is available; otherwise, throws an exception.
    /// </summary>
    /// <returns>The object of type <typeparamref name="T"/> if it is present.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the object is not available.</exception>
    public T ObjectOrThrow() => _object is not null ? _object : throw new InvalidOperationException("result is empty");

    /// <summary>
    /// Attempts to retrieve the successful result value if the operation completed successfully.
    /// </summary>
    /// <param name="result">When this method returns, contains the result value if the operation was successful; otherwise, the default
    /// value for type <typeparamref name="T"/>.</param>
    /// <returns>true if the operation was successful and the result was retrieved; otherwise, false.</returns>
    public bool TryGetSuccess(out T? result)
    {
        if (_object is not null)
        {
            result = _object;
            return true;
        }
        result = default;
        return false;
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
            list = [];
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
    /// <summary>
    /// Stores the internal.
    /// </summary>
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
            list = [];
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
        return new FailureDictionary(_internal);
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
    public static Result<T> ToSuccess<T>(this T value)
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
    /// Creates a failed result for the specified subject, associating it with the provided failure value.
    /// </summary>
    /// <remarks>The returned result will include both the failure value and the subject in its failure
    /// details. Use this method to conveniently create a failure result when you have an object and a corresponding
    /// failure reason.</remarks>
    /// <typeparam name="T">The type of the subject for which the failure result is created.</typeparam>
    /// <param name="subject">The subject instance to associate with the failure. Cannot be null.</param>
    /// <param name="value">The value describing the failure to associate with the result.</param>
    /// <returns>A failed result containing the subject and the specified failure value.</returns>
    public static Result<T> ToFailure<T>(this T subject, object value)
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
    public static Result<T> ToFailure<T>(this Exception ex)
    {
        return Result<T>.Exception(ex);
    }
}
