namespace FrenchExDev.Net.CSharp.Object.Result.Testing;

/// <summary>
/// Provides assertion methods for testing <see cref="Result"/> and <see cref="Result{T}"/> instances.
/// </summary>
/// <remarks>
/// Use these methods in unit tests to verify that result objects have the expected state, values, and failure details.
/// All assertion methods throw <see cref="ResultAssertException"/> when the assertion fails.
/// </remarks>
public static class ResultAssert
{
    /// <summary>
    /// Asserts that the specified result represents a successful operation.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ResultAssertException">Thrown when the result is not successful.</exception>
    public static void IsSuccess(Result result, string? message = null)
    {
        if (result.IsSuccess)
        {
            return;
        }

        throw new ResultAssertException(message ?? "Expected result to be successful, but it was a failure.");
    }

    /// <summary>
    /// Asserts that the specified result represents a failed operation.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ResultAssertException">Thrown when the result is not a failure.</exception>
    public static void IsFailure(Result result, string? message = null)
    {
        if (result.IsFailure)
        {
            return;
        }

        throw new ResultAssertException(message ?? "Expected result to be a failure, but it was successful.");
    }

    /// <summary>
    /// Asserts that the specified result represents a successful operation.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ResultAssertException">Thrown when the result is not successful.</exception>
    public static void IsSuccess<T>(Result<T> result, string? message = null) where T : notnull
    {
        if (result.IsSuccess)
        {
            return;
        }

        throw new ResultAssertException(message ?? $"Expected Result<{typeof(T).Name}> to be successful, but it was a failure.");
    }

    /// <summary>
    /// Asserts that the specified result represents a failed operation.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ResultAssertException">Thrown when the result is not a failure.</exception>
    public static void IsFailure<T>(Result<T> result, string? message = null) where T : notnull
    {
        if (result.IsFailure)
        {
            return;
        }

        throw new ResultAssertException(message ?? $"Expected Result<{typeof(T).Name}> to be a failure, but it was successful.");
    }

    /// <summary>
    /// Asserts that the specified result is successful and contains the expected value.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="expectedValue">The expected value contained in the result.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ResultAssertException">Thrown when the result is not successful or the value does not match.</exception>
    public static void HasValue<T>(Result<T> result, T expectedValue, string? message = null) where T : notnull
    {
        IsSuccess(result, message);

        if (Equals(result.Object, expectedValue))
        {
            return;
        }

        throw new ResultAssertException(message ?? $"Expected result value to be '{expectedValue}', but was '{result.Object}'.");
    }

    /// <summary>
    /// Asserts that the specified result is successful and the value satisfies the given predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="predicate">A function to test the result value.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ResultAssertException">Thrown when the result is not successful or the predicate returns false.</exception>
    public static void HasValueMatching<T>(Result<T> result, Func<T, bool> predicate, string? message = null) where T : notnull
    {
        IsSuccess(result, message);

        if (predicate(result.Object))
        {
            return;
        }

        throw new ResultAssertException(message ?? $"Result value '{result.Object}' did not match the expected predicate.");
    }

    /// <summary>
    /// Asserts that the specified result has an associated exception.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ResultAssertException">Thrown when the result does not have an exception.</exception>
    public static void HasException(Result result, string? message = null)
    {
        if (result.Exception is not null)
        {
            return;
        }

        throw new ResultAssertException(message ?? "Expected result to have an exception, but it was null.");
    }

    /// <summary>
    /// Asserts that the specified result has an associated exception of the specified type.
    /// </summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The exception cast to the specified type.</returns>
    /// <exception cref="ResultAssertException">Thrown when the result does not have an exception of the specified type.</exception>
    public static TException HasException<TException>(Result result, string? message = null) where TException : Exception
    {
        HasException(result, message);

        if (result.Exception is TException typedException)
        {
            return typedException;
        }

        throw new ResultAssertException(message ?? $"Expected exception of type '{typeof(TException).Name}', but was '{result.Exception!.GetType().Name}'.");
    }

    /// <summary>
    /// Asserts that the specified result contains a failure with the given key.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="failureKey">The key to look for in the failure dictionary.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ResultAssertException">Thrown when the result does not contain the specified failure key.</exception>
    public static void HasFailureKey<T>(Result<T> result, string failureKey, string? message = null) where T : notnull
    {
        IsFailure(result, message);

        if (result.Failures is not null && result.Failures.ContainsKey(failureKey))
        {
            return;
        }

        throw new ResultAssertException(message ?? $"Expected result to contain failure key '{failureKey}', but it was not found.");
    }

    /// <summary>
    /// Asserts that the specified result contains a failure with the given key and value.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="failureKey">The key to look for in the failure dictionary.</param>
    /// <param name="expectedValue">The expected value associated with the failure key.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ResultAssertException">Thrown when the result does not contain the specified failure key/value.</exception>
    public static void HasFailure<T>(Result<T> result, string failureKey, object expectedValue, string? message = null) where T : notnull
    {
        HasFailureKey(result, failureKey, message);

        var values = result.Failures![failureKey];

        if (values.Any(v => Equals(v, expectedValue)))
        {
            return;
        }
        
        throw new ResultAssertException(message ?? $"Expected failure key '{failureKey}' to contain value '{expectedValue}', but it was not found.");
    }

    /// <summary>
    /// Asserts that the specified result contains an exception in its failure dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The exception from the failure dictionary.</returns>
    /// <exception cref="ResultAssertException">Thrown when the result does not contain an exception in failures.</exception>
    public static Exception HasFailureException<T>(Result<T> result, string? message = null) where T : notnull
    {
        HasFailureKey(result, "Exception", message);

        var exception = result.Failures!["Exception"].OfType<Exception>().FirstOrDefault();
        
        if (exception is null)
        {
            throw new ResultAssertException(message ?? "Expected failure to contain an Exception, but none was found.");
        }

        return exception;
    }

    /// <summary>
    /// Asserts that the specified result contains an exception of the specified type in its failure dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The exception cast to the specified type.</returns>
    /// <exception cref="ResultAssertException">Thrown when the result does not contain an exception of the specified type.</exception>
    public static TException HasFailureException<T, TException>(Result<T> result, string? message = null)
        where T : notnull
        where TException : Exception
    {
        var exception = HasFailureException(result, message);

        if (exception is not TException typedException)
        {
            throw new ResultAssertException(message ?? $"Expected failure exception of type '{typeof(TException).Name}', but was '{exception.GetType().Name}'.");
        }

        return typedException;
    }
}

/// <summary>
/// Exception thrown when a result assertion fails.
/// </summary>
public class ResultAssertException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultAssertException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the assertion failure.</param>
    public ResultAssertException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultAssertException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the assertion failure.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ResultAssertException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Provides extension methods for fluent result assertions in tests.
/// </summary>
public static class ResultAssertExtensions
{
    /// <summary>
    /// Asserts that the result represents a successful operation.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static Result ShouldBeSuccess(this Result result, string? message = null)
    {
        ResultAssert.IsSuccess(result, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result represents a failed operation.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static Result ShouldBeFailure(this Result result, string? message = null)
    {
        ResultAssert.IsFailure(result, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result has an associated exception.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static Result ShouldHaveException(this Result result, string? message = null)
    {
        ResultAssert.HasException(result, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result has an associated exception of the specified type.
    /// </summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The exception cast to the specified type.</returns>
    public static TException ShouldHaveException<TException>(this Result result, string? message = null) where TException : Exception
    {
        return ResultAssert.HasException<TException>(result, message);
    }

    /// <summary>
    /// Asserts that the result represents a successful operation.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static Result<T> ShouldBeSuccess<T>(this Result<T> result, string? message = null) where T : notnull
    {
        ResultAssert.IsSuccess(result, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result represents a failed operation.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static Result<T> ShouldBeFailure<T>(this Result<T> result, string? message = null) where T : notnull
    {
        ResultAssert.IsFailure(result, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result is successful and contains the expected value.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="expectedValue">The expected value contained in the result.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static Result<T> ShouldHaveValue<T>(this Result<T> result, T expectedValue, string? message = null) where T : notnull
    {
        ResultAssert.HasValue(result, expectedValue, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result is successful and the value satisfies the given predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="predicate">A function to test the result value.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static Result<T> ShouldHaveValueMatching<T>(this Result<T> result, Func<T, bool> predicate, string? message = null) where T : notnull
    {
        ResultAssert.HasValueMatching(result, predicate, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result contains a failure with the given key.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="failureKey">The key to look for in the failure dictionary.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static Result<T> ShouldHaveFailureKey<T>(this Result<T> result, string failureKey, string? message = null) where T : notnull
    {
        ResultAssert.HasFailureKey(result, failureKey, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result contains a failure with the given key and value.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="failureKey">The key to look for in the failure dictionary.</param>
    /// <param name="expectedValue">The expected value associated with the failure key.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static Result<T> ShouldHaveFailure<T>(this Result<T> result, string failureKey, object expectedValue, string? message = null) where T : notnull
    {
        ResultAssert.HasFailure(result, failureKey, expectedValue, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result contains an exception in its failure dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The exception from the failure dictionary.</returns>
    public static Exception ShouldHaveFailureException<T>(this Result<T> result, string? message = null) where T : notnull
    {
        return ResultAssert.HasFailureException(result, message);
    }

    /// <summary>
    /// Asserts that the result contains an exception of the specified type in its failure dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The exception cast to the specified type.</returns>
    public static TException ShouldHaveFailureException<T, TException>(this Result<T> result, string? message = null)
        where T : notnull
        where TException : Exception
    {
        return ResultAssert.HasFailureException<T, TException>(result, message);
    }
}
