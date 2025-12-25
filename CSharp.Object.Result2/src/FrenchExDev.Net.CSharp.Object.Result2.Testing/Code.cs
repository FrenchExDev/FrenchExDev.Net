using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Result2.Testing;

/// <summary>
/// Provides static helper methods for testing Result&lt;TResult&gt; instances.
/// </summary>
public static class ResultTesting
{
    /// <summary>
    /// Asserts that the result represents a successful operation.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <exception cref="ResultTestingException">Thrown if the result is not successful.</exception>
    public static void ShouldBeSuccess<TResult>(Result<TResult> result)
    {
        if (!result.IsSuccess)
        {
            throw new ResultTestingException("Expected result to be a success, but it was a failure.");
        }
    }

    /// <summary>
    /// Asserts that the result represents a successful operation and returns the value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <returns>The value contained in the successful result.</returns>
    /// <exception cref="ResultTestingException">Thrown if the result is not successful.</exception>
    public static TResult ShouldBeSuccessWithValue<TResult>(Result<TResult> result)
    {
        ShouldBeSuccess(result);
        return result.Value;
    }

    /// <summary>
    /// Asserts that the result represents a successful operation with the expected value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="expectedValue">The expected value.</param>
    /// <exception cref="ResultTestingException">Thrown if the result is not successful or the value does not match.</exception>
    public static void ShouldBeSuccessWithValue<TResult>(Result<TResult> result, TResult expectedValue)
    {
        ShouldBeSuccess(result);
        if (!Equals(result.Value, expectedValue))
        {
            throw new ResultTestingException($"Expected result value to be '{expectedValue}', but was '{result.Value}'.");
        }
    }

    /// <summary>
    /// Asserts that the result represents a failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <exception cref="ResultTestingException">Thrown if the result is not a failure.</exception>
    public static void ShouldBeFailure<TResult>(Result<TResult> result)
    {
        if (result.IsSuccess)
        {
            throw new ResultTestingException("Expected result to be a failure, but it was a success.");
        }
    }

    /// <summary>
    /// Asserts that the result represents a failure and returns the exception.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <returns>The exception contained in the failed result.</returns>
    /// <exception cref="ResultTestingException">Thrown if the result is not a failure or the exception type does not match.</exception>
    public static TException ShouldBeFailureWithException<TResult, TException>(Result<TResult> result) where TException : Exception
    {
        ShouldBeFailure(result);
        if (!result.TryGetException<TException>(out var exception) || exception is null)
        {
            throw new ResultTestingException($"Expected exception of type '{typeof(TException).Name}', but it was not found.");
        }
        return exception;
    }

    /// <summary>
    /// Asserts that the result represents a failure with an exception containing the expected message.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="expectedMessage">The expected exception message.</param>
    /// <exception cref="ResultTestingException">Thrown if the result is not a failure, the exception type does not match, or the message does not match.</exception>
    public static void ShouldBeFailureWithMessage<TResult, TException>(Result<TResult> result, string expectedMessage) where TException : Exception
    {
        var exception = ShouldBeFailureWithException<TResult, TException>(result);
        if (exception.Message != expectedMessage)
        {
            throw new ResultTestingException($"Expected exception message '{expectedMessage}', but was '{exception.Message}'.");
        }
    }

    /// <summary>
    /// Asserts that the result represents a failure with an exception message containing the specified substring.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="messageSubstring">The substring that should be contained in the exception message.</param>
    /// <exception cref="ResultTestingException">Thrown if the result is not a failure, the exception type does not match, or the message does not contain the substring.</exception>
    public static void ShouldBeFailureWithMessageContaining<TResult, TException>(Result<TResult> result, string messageSubstring) where TException : Exception
    {
        var exception = ShouldBeFailureWithException<TResult, TException>(result);
        if (!exception.Message.Contains(messageSubstring, StringComparison.Ordinal))
        {
            throw new ResultTestingException($"Expected exception message to contain '{messageSubstring}', but was '{exception.Message}'.");
        }
    }
}

/// <summary>
/// Exception thrown when a result testing assertion fails.
/// </summary>
public class ResultTestingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ResultTestingException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the assertion failure.</param>
    public ResultTestingException(string message) : base(message)
    {
    }
}
