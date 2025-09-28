
namespace FrenchExDev.Net.CSharp.Object.Result;

/// <summary>
/// Exception class for all Result exceptions.
/// </summary>
public class ResultException : Exception { }

/// <summary>
/// Invalid result access exception class.
/// </summary>
public class InvalidResultAccessException : ResultException { }

/// <summary>
/// Represents the result of an operation that may succeed or fail, encapsulating an optional object of type T.
/// </summary>
/// <remarks>Use the Success and Failure static methods to create instances representing successful or failed
/// operations. The IsSuccess property indicates whether the result contains a valid object. Access the contained object
/// using Object, ObjectOrNull(), or ObjectOrThrow().</remarks>
/// <typeparam name="T">The type of the object contained in the result.</typeparam>
public readonly struct Result<T>
{
    /// <summary>
    /// Stores the result object on success.
    /// </summary>
    private readonly T? _object;

    /// <summary>
    /// Gets a value indicating whether the result contains a valid object.
    /// </summary>
    public bool IsSuccess => Object is not null;

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
    /// Returns the contained object if available; otherwise, returns null.
    /// </summary>
    /// <returns>The object of type T if present; otherwise, null.</returns>
    public T? ObjectOrNull() => Object;

    /// <summary>
    /// Returns the contained object if it is available; otherwise, throws an exception.
    /// </summary>
    /// <returns>The object of type <typeparamref name="T"/> if it is present.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the object is not available.</exception>
    public T ObjectOrThrow() => Object is not null ? Object : throw new InvalidOperationException("result is empty");

    /// <summary>
    /// Attempts to retrieve the successful result value if the operation completed successfully.
    /// </summary>
    /// <param name="result">When this method returns, contains the result value if the operation was successful; otherwise, the default
    /// value for type <typeparamref name="T"/>.</param>
    /// <returns>true if the operation was successful and the result was retrieved; otherwise, false.</returns>
    public bool TryGetSuccess(out T? result)
    {
        if (IsSuccess)
        {
            result = Object;
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