using FrenchExDev.Net.CSharp.Object.Result2.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Result2.Tests;

public class Tests
{
    #region Success Tests

    [Fact]
    public void Success_WithValue_ShouldBeSuccess()
    {
        // Arrange & Act
        var result = Result<int>.Success(42);

        // Assert
        ResultTesting.ShouldBeSuccess(result);
    }

    [Fact]
    public void Success_WithValue_ShouldReturnCorrectValue()
    {
        // Arrange & Act
        var result = Result<int>.Success(42);

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(result, 42);
    }

    [Fact]
    public void Success_ShouldBeSuccessWithValue_ReturnsValue()
    {
        // Arrange
        var result = Result<string>.Success("test value");

        // Act
        var value = ResultTesting.ShouldBeSuccessWithValue(result);

        // Assert
        value.ShouldBe("test value");
    }

    [Fact]
    public void Success_WithNullValue_ShouldBeSuccess()
    {
        // Arrange & Act
        var result = Result<string?>.Success(null);

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(result, null);
    }

    #endregion

    #region Failure Tests

    [Fact]
    public void Failure_WithException_ShouldBeFailure()
    {
        // Arrange & Act
        var result = Result<int>.Failure(new InvalidOperationException("An error occurred"));

        // Assert
        ResultTesting.ShouldBeFailure(result);
    }

    [Fact]
    public void Failure_WithException_ShouldReturnCorrectException()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("An error occurred"));

        // Act
        var exception = ResultTesting.ShouldBeFailureWithException<int, InvalidOperationException>(result);

        // Assert
        exception.ShouldNotBeNull();
        exception.Message.ShouldBe("An error occurred");
    }

    [Fact]
    public void Failure_WithException_ShouldHaveCorrectMessage()
    {
        // Arrange
        var result = Result<int>.Failure(new ArgumentException("Invalid argument"));

        // Assert
        ResultTesting.ShouldBeFailureWithMessage<int, ArgumentException>(result, "Invalid argument");
    }

    [Fact]
    public void Failure_WithResultException_ShouldBeFailure()
    {
        // Arrange & Act
        var result = Result<string>.Failure(new ResultException("Result error"));

        // Assert
        ResultTesting.ShouldBeFailureWithMessage<string, ResultException>(result, "Result error");
    }

    [Fact]
    public void Failure_WithInvalidResultAccessOperationException_ShouldBeFailure()
    {
        // Arrange & Act
        var result = Result<int>.Failure(new InvalidResultAccessOperationException("Invalid access"));

        // Assert
        var exception = ResultTesting.ShouldBeFailureWithException<int, InvalidResultAccessOperationException>(result);
        exception.Message.ShouldBe("Invalid access");
    }

    #endregion

    #region Value Property Tests

    [Fact]
    public void Value_OnFailure_ShouldThrowInvalidResultAccessOperationException()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("error"));

        // Act & Assert
        var ex = Should.Throw<InvalidResultAccessOperationException>(() => _ = result.Value);
        ex.Message.ShouldBe("Cannot access Value when the result is a failure.");
    }

    #endregion

    #region Exception<T> Method Tests

    [Fact]
    public void Exception_OnSuccess_ShouldThrowInvalidResultAccessOperationException()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act & Assert
        var ex = Should.Throw<InvalidResultAccessOperationException>(() => result.Exception<Exception>());
        ex.Message.ShouldBe("Cannot access Exception when the result is a success.");
    }

    [Fact]
    public void Exception_WithWrongType_ShouldThrowInvalidResultAccessOperationException()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("error"));

        // Act & Assert
        var ex = Should.Throw<InvalidResultAccessOperationException>(() => result.Exception<ArgumentException>());
        ex.Message.ShouldBe("Exception is null after cast to TException");
    }

    #endregion

    #region TryGetException Tests

    [Fact]
    public void TryGetException_OnSuccess_ShouldReturnFalse()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var success = result.TryGetException<Exception>(out var exception);

        // Assert
        success.ShouldBeFalse();
        exception.ShouldBeNull();
    }

    [Fact]
    public void TryGetException_OnFailure_WithCorrectType_ShouldReturnTrue()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("error"));

        // Act
        var success = result.TryGetException<InvalidOperationException>(out var exception);

        // Assert
        success.ShouldBeTrue();
        exception.ShouldNotBeNull();
        exception.Message.ShouldBe("error");
    }

    [Fact]
    public void TryGetException_OnFailure_WithWrongType_ShouldReturnFalse()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("error"));

        // Act
        var success = result.TryGetException<ArgumentException>(out var exception);

        // Assert
        success.ShouldBeFalse();
        exception.ShouldBeNull();
    }

    #endregion

    #region TryGetSuccessValue Tests

    [Fact]
    public void TryGetSuccessValue_OnSuccess_ShouldReturnTrueAndValue()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var success = result.TryGetSuccessValue(out var value);

        // Assert
        success.ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void TryGetSuccessValue_OnFailure_ShouldReturnFalseAndDefault()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("error"));

        // Act
        var success = result.TryGetSuccessValue(out var value);

        // Assert
        success.ShouldBeFalse();
        value.ShouldBe(default);
    }

    #endregion

    #region Match Tests

    [Fact]
    public void Match_OnSuccess_ShouldInvokeOnSuccessAction()
    {
        // Arrange
        var result = Result<int>.Success(42);
        int? capturedValue = null;

        // Act
        var returnedResult = result.Match(
            onSuccess: v => capturedValue = v,
            onFailure: _ => { });

        // Assert
        capturedValue.ShouldBe(42);
        returnedResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Match_OnFailure_ShouldInvokeOnFailureAction()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("error"));
        Exception? capturedException = null;

        // Act
        var returnedResult = result.Match(
            onSuccess: _ => { },
            onFailure: ex => capturedException = ex);

        // Assert
        capturedException.ShouldNotBeNull();
        capturedException.Message.ShouldBe("error");
        returnedResult.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void MatchWithTypedException_OnSuccess_ShouldInvokeOnSuccessAction()
    {
        // Arrange
        var result = Result<string>.Success("test");
        string? capturedValue = null;

        // Act
        var returnedResult = result.Match<InvalidOperationException>(
            onSuccess: v => capturedValue = v,
            onFailure: _ => { });

        // Assert
        capturedValue.ShouldBe("test");
        returnedResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void MatchWithTypedException_OnFailure_ShouldInvokeOnFailureAction()
    {
        // Arrange
        var result = Result<string>.Failure(new ArgumentException("arg error"));
        ArgumentException? capturedException = null;

        // Act
        var returnedResult = result.Match<ArgumentException>(
            onSuccess: _ => { },
            onFailure: ex => capturedException = ex);

        // Assert
        capturedException.ShouldNotBeNull();
        capturedException.Message.ShouldBe("arg error");
        returnedResult.IsSuccess.ShouldBeFalse();
    }

    #endregion

    #region MatchAsync Tests

    [Fact]
    public async Task MatchAsync_OnSuccess_ShouldInvokeOnSuccessAction()
    {
        // Arrange
        var result = Result<int>.Success(42);
        int? capturedValue = null;

        // Act
        var returnedResult = await result.MatchAsync(
            onSuccess: async v =>
            {
                await Task.Delay(1);
                capturedValue = v;
            },
            onFailure: async _ => await Task.Delay(1));

        // Assert
        capturedValue.ShouldBe(42);
        returnedResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task MatchAsync_OnFailure_ShouldInvokeOnFailureAction()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("async error"));
        Exception? capturedException = null;

        // Act
        var returnedResult = await result.MatchAsync(
            onSuccess: async _ => await Task.Delay(1),
            onFailure: async ex =>
            {
                await Task.Delay(1);
                capturedException = ex;
            });

        // Assert
        capturedException.ShouldNotBeNull();
        capturedException.Message.ShouldBe("async error");
        returnedResult.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task MatchAsyncWithTypedException_OnSuccess_ShouldInvokeOnSuccessAction()
    {
        // Arrange
        var result = Result<string>.Success("async test");
        string? capturedValue = null;

        // Act
        var returnedResult = await result.MatchAsync<InvalidOperationException>(
            onSuccess: async v =>
            {
                await Task.Delay(1);
                capturedValue = v;
            },
            onFailure: async _ => await Task.Delay(1));

        // Assert
        capturedValue.ShouldBe("async test");
        returnedResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task MatchAsyncWithTypedException_OnFailure_ShouldInvokeOnFailureAction()
    {
        // Arrange
        var result = Result<string>.Failure(new ArgumentException("async arg error"));
        ArgumentException? capturedException = null;

        // Act
        var returnedResult = await result.MatchAsync<ArgumentException>(
            onSuccess: async _ => await Task.Delay(1),
            onFailure: async ex =>
            {
                await Task.Delay(1);
                capturedException = ex;
            });

        // Assert
        capturedException.ShouldNotBeNull();
        capturedException.Message.ShouldBe("async arg error");
        returnedResult.IsSuccess.ShouldBeFalse();
    }

    #endregion

    #region IfSuccess Tests

    [Fact]
    public void IfSuccess_OnSuccess_ShouldInvokeAction()
    {
        // Arrange
        var result = Result<int>.Success(100);
        int? capturedValue = null;

        // Act
        var returnedResult = result.IfSuccess(v => capturedValue = v);

        // Assert
        capturedValue.ShouldBe(100);
        returnedResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void IfSuccess_OnFailure_ShouldNotInvokeAction()
    {
        // Arrange
        var result = Result<int>.Failure(new Exception("error"));
        bool wasCalled = false;

        // Act
        var returnedResult = result.IfSuccess(_ => wasCalled = true);

        // Assert
        wasCalled.ShouldBeFalse();
        returnedResult.IsSuccess.ShouldBeFalse();
    }

    #endregion

    #region IfSuccessAsync Tests

    [Fact]
    public async Task IfSuccessAsync_OnSuccess_ShouldInvokeAsyncAction()
    {
        // Arrange
        var result = Result<int>.Success(200);
        int? capturedValue = null;

        // Act
        var returnedResult = await result.IfSuccessAsync(async v =>
        {
            await Task.Delay(1);
            capturedValue = v;
        });

        // Assert
        capturedValue.ShouldBe(200);
        returnedResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task IfSuccessAsync_OnFailure_ShouldNotInvokeAsyncAction()
    {
        // Arrange
        var result = Result<int>.Failure(new Exception("error"));
        bool wasCalled = false;

        // Act
        var returnedResult = await result.IfSuccessAsync(async _ =>
        {
            await Task.Delay(1);
            wasCalled = true;
        });

        // Assert
        wasCalled.ShouldBeFalse();
        returnedResult.IsSuccess.ShouldBeFalse();
    }

    #endregion

    #region IfException Tests

    [Fact]
    public void IfException_OnFailure_ShouldInvokeAction()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("failure"));
        Exception? capturedException = null;

        // Act
        var returnedResult = result.IfException(ex => capturedException = ex);

        // Assert
        capturedException.ShouldNotBeNull();
        capturedException.Message.ShouldBe("failure");
        returnedResult.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void IfException_OnSuccess_ShouldNotInvokeAction()
    {
        // Arrange
        var result = Result<int>.Success(42);
        bool wasCalled = false;

        // Act
        var returnedResult = result.IfException(_ => wasCalled = true);

        // Assert
        wasCalled.ShouldBeFalse();
        returnedResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void IfExceptionWithType_OnFailure_ShouldInvokeAction()
    {
        // Arrange
        var result = Result<int>.Failure(new ArgumentException("arg failure"));
        ArgumentException? capturedException = null;

        // Act
        var returnedResult = result.IfException<ArgumentException>(ex => capturedException = ex);

        // Assert
        capturedException.ShouldNotBeNull();
        capturedException.Message.ShouldBe("arg failure");
        returnedResult.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void IfExceptionWithType_OnSuccess_ShouldNotInvokeAction()
    {
        // Arrange
        var result = Result<int>.Success(42);
        bool wasCalled = false;

        // Act
        var returnedResult = result.IfException<ArgumentException>(_ => wasCalled = true);

        // Assert
        wasCalled.ShouldBeFalse();
        returnedResult.IsSuccess.ShouldBeTrue();
    }

    #endregion

    #region IfExceptionAsync Tests

    [Fact]
    public async Task IfExceptionAsync_OnFailure_ShouldInvokeAsyncAction()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("async failure"));
        Exception? capturedException = null;

        // Act
        var returnedResult = await result.IfExceptionAsync(async ex =>
        {
            await Task.Delay(1);
            capturedException = ex;
        });

        // Assert
        capturedException.ShouldNotBeNull();
        capturedException.Message.ShouldBe("async failure");
        returnedResult.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task IfExceptionAsync_OnSuccess_ShouldNotInvokeAsyncAction()
    {
        // Arrange
        var result = Result<int>.Success(42);
        bool wasCalled = false;

        // Act
        var returnedResult = await result.IfExceptionAsync(async _ =>
        {
            await Task.Delay(1);
            wasCalled = true;
        });

        // Assert
        wasCalled.ShouldBeFalse();
        returnedResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task IfExceptionAsyncWithType_OnFailure_ShouldInvokeAsyncAction()
    {
        // Arrange
        var result = Result<int>.Failure(new ArgumentException("typed async failure"));
        ArgumentException? capturedException = null;

        // Act
        var returnedResult = await result.IfExceptionAsync<ArgumentException>(async ex =>
        {
            await Task.Delay(1);
            capturedException = ex;
        });

        // Assert
        capturedException.ShouldNotBeNull();
        capturedException.Message.ShouldBe("typed async failure");
        returnedResult.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task IfExceptionAsyncWithType_OnSuccess_ShouldNotInvokeAsyncAction()
    {
        // Arrange
        var result = Result<int>.Success(42);
        bool wasCalled = false;

        // Act
        var returnedResult = await result.IfExceptionAsync<ArgumentException>(async _ =>
        {
            await Task.Delay(1);
            wasCalled = true;
        });

        // Assert
        wasCalled.ShouldBeFalse();
        returnedResult.IsSuccess.ShouldBeTrue();
    }

    #endregion

    #region Exception Class Tests

    [Fact]
    public void ResultException_DefaultConstructor_ShouldCreateInstance()
    {
        // Act
        var exception = new ResultException();

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public void ResultException_WithMessage_ShouldSetMessage()
    {
        // Act
        var exception = new ResultException("test message");

        // Assert
        exception.Message.ShouldBe("test message");
    }

    [Fact]
    public void ResultException_WithMessageAndInnerException_ShouldSetBoth()
    {
        // Arrange
        var inner = new InvalidOperationException("inner");

        // Act
        var exception = new ResultException("outer message", inner);

        // Assert
        exception.Message.ShouldBe("outer message");
        exception.InnerException.ShouldBeSameAs(inner);
    }

    [Fact]
    public void InvalidResultAccessOperationException_DefaultConstructor_ShouldCreateInstance()
    {
        // Act
        var exception = new InvalidResultAccessOperationException();

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public void InvalidResultAccessOperationException_WithMessage_ShouldSetMessage()
    {
        // Act
        var exception = new InvalidResultAccessOperationException("access error");

        // Assert
        exception.Message.ShouldBe("access error");
    }

    [Fact]
    public void InvalidResultAccessOperationException_WithMessageAndInnerException_ShouldSetBoth()
    {
        // Arrange
        var inner = new ArgumentException("inner arg");

        // Act
        var exception = new InvalidResultAccessOperationException("outer access", inner);

        // Assert
        exception.Message.ShouldBe("outer access");
        exception.InnerException.ShouldBeSameAs(inner);
    }

    #endregion

    #region ResultTesting Validation Tests

    [Fact]
    public void ResultTesting_ShouldBeSuccess_OnFailure_ShouldThrow()
    {
        // Arrange
        var result = Result<int>.Failure(new Exception("error"));

        // Act & Assert
        Should.Throw<ResultTestingException>(() => ResultTesting.ShouldBeSuccess(result));
    }

    [Fact]
    public void ResultTesting_ShouldBeFailure_OnSuccess_ShouldThrow()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act & Assert
        Should.Throw<ResultTestingException>(() => ResultTesting.ShouldBeFailure(result));
    }

    [Fact]
    public void ResultTesting_ShouldBeSuccessWithValue_WrongValue_ShouldThrow()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act & Assert
        Should.Throw<ResultTestingException>(() => ResultTesting.ShouldBeSuccessWithValue(result, 99));
    }

    [Fact]
    public void ResultTesting_ShouldBeFailureWithException_WrongType_ShouldThrow()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("error"));

        // Act & Assert
        Should.Throw<ResultTestingException>(() => 
            ResultTesting.ShouldBeFailureWithException<int, ArgumentException>(result));
    }

    [Fact]
    public void ResultTesting_ShouldBeFailureWithMessage_WrongMessage_ShouldThrow()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("actual message"));

        // Act & Assert
        Should.Throw<ResultTestingException>(() => 
            ResultTesting.ShouldBeFailureWithMessage<int, InvalidOperationException>(result, "expected message"));
    }

    [Fact]
    public void ResultTesting_ShouldBeFailureWithMessageContaining_MatchingSubstring_ShouldPass()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("This is an error message"));

        // Act & Assert (should not throw)
        ResultTesting.ShouldBeFailureWithMessageContaining<int, InvalidOperationException>(result, "error");
    }

    [Fact]
    public void ResultTesting_ShouldBeFailureWithMessageContaining_NonMatchingSubstring_ShouldThrow()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("This is an error message"));

        // Act & Assert
        Should.Throw<ResultTestingException>(() => 
            ResultTesting.ShouldBeFailureWithMessageContaining<int, InvalidOperationException>(result, "not found"));
    }

    #endregion

    #region ResultTestingException Tests

    [Fact]
    public void ResultTestingException_WithMessage_ShouldSetMessage()
    {
        // Act
        var exception = new ResultTestingException("testing exception");

        // Assert
        exception.Message.ShouldBe("testing exception");
    }

    #endregion
}
