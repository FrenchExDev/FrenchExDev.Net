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
        var ex = Should.Throw<InvalidOperationException>(() => result.Exception<ArgumentException>());
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

    #region Map Tests

    [Fact]
    public void Map_OnSuccess_ShouldTransformValue()
    {
        // Arrange
        var result = Result<int>.Success(10);

        // Act
        var mappedResult = result.Map(x => x * 2);

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(mappedResult, 20);
    }

    [Fact]
    public void Map_OnSuccess_ShouldTransformToStringType()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var mappedResult = result.Map(x => $"Value is {x}");

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(mappedResult, "Value is 42");
    }

    [Fact]
    public void Map_OnFailure_ShouldPropagateFailure()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("original error"));

        // Act
        var mappedResult = result.Map(x => x * 2);

        // Assert
        ResultTesting.ShouldBeFailure(mappedResult);
        var ex = ResultTesting.ShouldBeFailureWithException<int, InvalidOperationException>(mappedResult);
        ex.Message.ShouldBe("original error");
    }

    #endregion

    #region MapAsync Tests

    [Fact]
    public async Task MapAsync_OnSuccess_ShouldTransformValueAsynchronously()
    {
        // Arrange
        var result = Result<int>.Success(10);

        // Act
        var mappedResult = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 3;
        });

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(mappedResult, 30);
    }

    [Fact]
    public async Task MapAsync_OnFailure_ShouldPropagateFailure()
    {
        // Arrange
        var result = Result<int>.Failure(new ArgumentException("async error"));

        // Act
        var mappedResult = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 3;
        });

        // Assert
        ResultTesting.ShouldBeFailure(mappedResult);
        var ex = ResultTesting.ShouldBeFailureWithException<int, ArgumentException>(mappedResult);
        ex.Message.ShouldBe("async error");
    }

    #endregion

    #region Bind Tests

    [Fact]
    public void Bind_OnSuccess_ShouldChainSuccessfulResult()
    {
        // Arrange
        var result = Result<int>.Success(10);

        // Act
        var boundResult = result.Bind(x => Result<string>.Success($"Number: {x}"));

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(boundResult, "Number: 10");
    }

    [Fact]
    public void Bind_OnSuccess_ShouldChainToFailure()
    {
        // Arrange
        var result = Result<int>.Success(10);

        // Act
        var boundResult = result.Bind<string>(x => 
            Result<string>.Failure(new InvalidOperationException("bind failed")));

        // Assert
        ResultTesting.ShouldBeFailure(boundResult);
        var ex = ResultTesting.ShouldBeFailureWithException<string, InvalidOperationException>(boundResult);
        ex.Message.ShouldBe("bind failed");
    }

    [Fact]
    public void Bind_OnFailure_ShouldPropagateFailure()
    {
        // Arrange
        var result = Result<int>.Failure(new ArgumentException("initial failure"));

        // Act
        var boundResult = result.Bind(x => Result<string>.Success($"Number: {x}"));

        // Assert
        ResultTesting.ShouldBeFailure(boundResult);
        var ex = ResultTesting.ShouldBeFailureWithException<string, ArgumentException>(boundResult);
        ex.Message.ShouldBe("initial failure");
    }

    [Fact]
    public void Bind_ShouldEnableFlatComposition()
    {
        // Arrange - simulate a validation pipeline
        var step1 = Result<int>.Success(100);

        // Act - chain multiple operations without nesting
        var finalResult = step1
            .Bind(value => value > 0 
                ? Result<int>.Success(value * 2) 
                : Result<int>.Failure(new ArgumentException("Must be positive")))
            .Bind(value => Result<string>.Success($"Final: {value}"));

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(finalResult, "Final: 200");
    }

    #endregion

    #region BindAsync Tests

    [Fact]
    public async Task BindAsync_OnSuccess_ShouldChainSuccessfulResult()
    {
        // Arrange
        var result = Result<int>.Success(10);

        // Act
        var boundResult = await result.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Result<string>.Success($"Async Number: {x}");
        });

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(boundResult, "Async Number: 10");
    }

    [Fact]
    public async Task BindAsync_OnSuccess_ShouldChainToFailure()
    {
        // Arrange
        var result = Result<int>.Success(10);

        // Act
        var boundResult = await result.BindAsync<string>(async x =>
        {
            await Task.Delay(1);
            return Result<string>.Failure(new InvalidOperationException("async bind failed"));
        });

        // Assert
        ResultTesting.ShouldBeFailure(boundResult);
        var ex = ResultTesting.ShouldBeFailureWithException<string, InvalidOperationException>(boundResult);
        ex.Message.ShouldBe("async bind failed");
    }

    [Fact]
    public async Task BindAsync_OnFailure_ShouldPropagateFailure()
    {
        // Arrange
        var result = Result<int>.Failure(new ArgumentException("async initial failure"));

        // Act
        var boundResult = await result.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Result<string>.Success($"Async Number: {x}");
        });

        // Assert
        ResultTesting.ShouldBeFailure(boundResult);
        var ex = ResultTesting.ShouldBeFailureWithException<string, ArgumentException>(boundResult);
        ex.Message.ShouldBe("async initial failure");
    }

    [Fact]
    public async Task BindAsync_ShouldEnableFlatAsyncComposition()
    {
        // Arrange - simulate an async validation pipeline
        var step1 = Result<int>.Success(50);

        // Act - chain multiple async operations without nesting
        var finalResult = await step1
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                return value > 0
                    ? Result<int>.Success(value * 2)
                    : Result<int>.Failure(new ArgumentException("Must be positive"));
            })
            .ContinueWith(t => t.Result.Bind(value => Result<string>.Success($"Final: {value}")));

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(finalResult, "Final: 100");
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void Success_WithDefaultValue_ShouldBeSuccess()
    {
        // Arrange & Act
        var result = Result<int>.Success(default);

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(result, 0);
    }

    [Fact]
    public void Success_WithDefaultBoolValue_ShouldBeSuccess()
    {
        // Arrange & Act
        var result = Result<bool>.Success(default);

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(result, false);
    }

    [Fact]
    public void Success_WithEmptyString_ShouldBeSuccess()
    {
        // Arrange & Act
        var result = Result<string>.Success(string.Empty);

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(result, string.Empty);
    }

    [Fact]
    public void Failure_WithInnerException_ShouldPreserveInnerException()
    {
        // Arrange
        var inner = new ArgumentException("inner error");
        var outer = new InvalidOperationException("outer error", inner);

        // Act
        var result = Result<int>.Failure(outer);

        // Assert
        var ex = ResultTesting.ShouldBeFailureWithException<int, InvalidOperationException>(result);
        ex.InnerException.ShouldBeSameAs(inner);
    }

    [Fact]
    public void TryGetException_WithBaseType_ShouldReturnTrueForDerivedType()
    {
        // Arrange - ArgumentException derives from Exception
        var result = Result<int>.Failure(new ArgumentException("error"));

        // Act
        var success = result.TryGetException<Exception>(out var exception);

        // Assert
        success.ShouldBeTrue();
        exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    public void TryGetException_WithSystemException_ShouldReturnTrueForDerivedType()
    {
        // Arrange - InvalidOperationException derives from SystemException
        var result = Result<int>.Failure(new InvalidOperationException("error"));

        // Act
        var success = result.TryGetException<SystemException>(out var exception);

        // Assert
        success.ShouldBeTrue();
        exception.ShouldBeOfType<InvalidOperationException>();
    }

    #endregion

    #region Composition Chain Tests

    [Fact]
    public void Map_ChainMultipleTransformations_ShouldComposeCorrectly()
    {
        // Arrange
        var result = Result<int>.Success(2);

        // Act
        var finalResult = result
            .Map(x => x * 3)      // 6
            .Map(x => x + 4)      // 10
            .Map(x => x.ToString()); // "10"

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(finalResult, "10");
    }

    [Fact]
    public void Map_ChainWithFailureInMiddle_ShouldPropagateFailure()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("initial error"));

        // Act
        var finalResult = result
            .Map(x => x * 2)
            .Map(x => x + 10)
            .Map(x => x.ToString());

        // Assert
        ResultTesting.ShouldBeFailure(finalResult);
        var ex = ResultTesting.ShouldBeFailureWithException<string, InvalidOperationException>(finalResult);
        ex.Message.ShouldBe("initial error");
    }

    [Fact]
    public void Bind_ChainedFailures_ShouldPropagateFirstFailure()
    {
        // Arrange
        var result = Result<int>.Failure(new ArgumentException("first error"));

        // Act - second Bind should never execute
        var boundResult = result
            .Bind(x => Result<int>.Failure(new InvalidOperationException("second error")));

        // Assert - should have the first error
        ResultTesting.ShouldBeFailure(boundResult);
        var ex = ResultTesting.ShouldBeFailureWithException<int, ArgumentException>(boundResult);
        ex.Message.ShouldBe("first error");
    }

    [Fact]
    public void Bind_ValidationPipeline_ShouldStopAtFirstFailure()
    {
        // Arrange - simulate a validation pipeline
        var input = Result<int>.Success(-5);

        // Act
        var finalResult = input
            .Bind(value => value >= 0
                ? Result<int>.Success(value)
                : Result<int>.Failure(new ArgumentException("Must be non-negative")))
            .Bind(value => value <= 100
                ? Result<int>.Success(value)
                : Result<int>.Failure(new ArgumentException("Must be <= 100")))
            .Bind(value => Result<string>.Success($"Valid: {value}"));

        // Assert - should fail at first validation
        ResultTesting.ShouldBeFailure(finalResult);
        ResultTesting.ShouldBeFailureWithMessage<string, ArgumentException>(finalResult, "Must be non-negative");
    }

    [Fact]
    public void Bind_ValidationPipeline_AllValid_ShouldSucceed()
    {
        // Arrange
        var input = Result<int>.Success(50);

        // Act
        var finalResult = input
            .Bind(value => value >= 0
                ? Result<int>.Success(value)
                : Result<int>.Failure(new ArgumentException("Must be non-negative")))
            .Bind(value => value <= 100
                ? Result<int>.Success(value)
                : Result<int>.Failure(new ArgumentException("Must be <= 100")))
            .Bind(value => Result<string>.Success($"Valid: {value}"));

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(finalResult, "Valid: 50");
    }

    [Fact]
    public void Map_AndBind_MixedComposition_ShouldWorkCorrectly()
    {
        // Arrange
        var result = Result<string>.Success("42");

        // Act
        var finalResult = result
            .Map(int.Parse)                    // Result<int> = 42
            .Map(x => x * 2)                   // Result<int> = 84
            .Bind(x => x > 0
                ? Result<int>.Success(x)
                : Result<int>.Failure(new ArgumentException("Must be positive")))
            .Map(x => $"Result: {x}");         // Result<string> = "Result: 84"

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(finalResult, "Result: 84");
    }

    #endregion

    #region Async Composition Tests

    [Fact]
    public async Task MapAsync_ChainMultipleTransformations_ShouldComposeCorrectly()
    {
        // Arrange
        var result = Result<int>.Success(5);

        // Act
        var step1 = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2; // 10
        });

        var step2 = await step1.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x + 5; // 15
        });

        var finalResult = await step2.MapAsync(async x =>
        {
            await Task.Delay(1);
            return $"Value: {x}"; // "Value: 15"
        });

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(finalResult, "Value: 15");
    }

    [Fact]
    public async Task BindAsync_ChainedOperations_ShouldStopAtFirstFailure()
    {
        // Arrange
        var result = Result<int>.Success(10);

        // Act
        var finalResult = await result
            .BindAsync(async x =>
            {
                await Task.Delay(1);
                return Result<int>.Failure(new InvalidOperationException("First async failure"));
            });

        // This would be the second bind, but it should never execute
        var afterSecondBind = await finalResult.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Result<string>.Success($"Should not reach: {x}");
        });

        // Assert
        ResultTesting.ShouldBeFailure(afterSecondBind);
        ResultTesting.ShouldBeFailureWithMessage<string, InvalidOperationException>(
            afterSecondBind, "First async failure");
    }

    #endregion

    #region Struct Behavior Tests

    [Fact]
    public void Result_IsValueType_ShouldBeTrue()
    {
        // Assert - Result<T> should be a struct (value type)
        typeof(Result<int>).IsValueType.ShouldBeTrue();
    }

    [Fact]
    public void Result_DefaultValue_ShouldNotBeSuccess()
    {
        // Arrange - default struct initialization
        Result<int> result = default;

        // Assert - default should not be success (IsSuccess = false by default)
        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void Result_CopySemantics_ShouldBeIndependent()
    {
        // Arrange
        var original = Result<int>.Success(42);

        // Act - copy the struct
        var copy = original;

        // Assert - both should have same value but be independent
        original.Value.ShouldBe(42);
        copy.Value.ShouldBe(42);
    }

    #endregion

    #region Thread Safety Tests

    [Fact]
    public async Task Result_ConcurrentAccess_ShouldBeSafe()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act - concurrent reads
        var tasks = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(() =>
            {
                result.IsSuccess.ShouldBeTrue();
                return result.Value;
            }))
            .ToArray();

        await Task.WhenAll(tasks);

        // Assert - all reads should return same value
        tasks.All(t => t.Result == 42).ShouldBeTrue();
    }

    [Fact]
    public async Task Result_ConcurrentMapOperations_ShouldBeIndependent()
    {
        // Arrange
        var result = Result<int>.Success(10);

        // Act - concurrent map operations (each creates a new Result)
        var tasks = Enumerable.Range(1, 10)
            .Select(multiplier => Task.Run(() => result.Map(x => x * multiplier)))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - each mapped result should be independent
        for (int i = 0; i < results.Length; i++)
        {
            var mappedValue = results[i].Value;
            mappedValue.ShouldBe(10 * (i + 1));
        }
    }

    #endregion

    #region Match Return Value Tests

    [Fact]
    public void Match_ShouldReturnSameResultInstance()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var returnedResult = result.Match(
            onSuccess: _ => { },
            onFailure: _ => { });

        // Assert - struct copy, but same values
        returnedResult.IsSuccess.ShouldBe(result.IsSuccess);
        returnedResult.Value.ShouldBe(result.Value);
    }

    [Fact]
    public void IfSuccess_ShouldReturnSameResultForChaining()
    {
        // Arrange
        var result = Result<int>.Success(42);
        var callCount = 0;

        // Act - chain multiple IfSuccess calls
        var finalResult = result
            .IfSuccess(_ => callCount++)
            .IfSuccess(_ => callCount++)
            .IfSuccess(_ => callCount++);

        // Assert
        callCount.ShouldBe(3);
        finalResult.IsSuccess.ShouldBeTrue();
        finalResult.Value.ShouldBe(42);
    }

    [Fact]
    public void IfException_ShouldReturnSameResultForChaining()
    {
        // Arrange
        var result = Result<int>.Failure(new InvalidOperationException("error"));
        var callCount = 0;

        // Act - chain multiple IfException calls
        var finalResult = result
            .IfException(_ => callCount++)
            .IfException(_ => callCount++)
            .IfException(_ => callCount++);

        // Assert
        callCount.ShouldBe(3);
        finalResult.IsSuccess.ShouldBeFalse();
    }

    #endregion

    #region Complex Type Tests

    [Fact]
    public void Success_WithComplexType_ShouldPreserveReference()
    {
        // Arrange
        var complexObject = new List<string> { "a", "b", "c" };

        // Act
        var result = Result<List<string>>.Success(complexObject);

        // Assert
        result.Value.ShouldBeSameAs(complexObject);
    }

    [Fact]
    public void Map_WithComplexTypeTransformation_ShouldWork()
    {
        // Arrange
        var result = Result<string>.Success("hello,world,test");

        // Act
        var mappedResult = result.Map(s => s.Split(',').ToList());

        // Assert
        ResultTesting.ShouldBeSuccess(mappedResult);
        mappedResult.Value.Count.ShouldBe(3);
        mappedResult.Value.ShouldContain("hello");
        mappedResult.Value.ShouldContain("world");
        mappedResult.Value.ShouldContain("test");
    }

    [Fact]
    public void Bind_WithComplexTypeTransformation_ShouldWork()
    {
        // Arrange
        var result = Result<List<int>>.Success([1, 2, 3, 4, 5]);

        // Act
        var boundResult = result.Bind(list =>
            list.Count > 0
                ? Result<int>.Success(list.Sum())
                : Result<int>.Failure(new InvalidOperationException("Empty list")));

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(boundResult, 15);
    }

    #endregion

    #region Nullable Reference Type Tests

    [Fact]
    public void Success_WithNullableReferenceType_ShouldHandleNull()
    {
        // Arrange & Act
        var result = Result<string?>.Success(null);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void TryGetSuccessValue_WithNullValue_ShouldReturnTrueAndNull()
    {
        // Arrange
        var result = Result<string?>.Success(null);

        // Act
        var success = result.TryGetSuccessValue(out var value);

        // Assert
        success.ShouldBeTrue();
        value.ShouldBeNull();
    }

    [Fact]
    public void Map_WithNullableInput_ShouldHandleNull()
    {
        // Arrange
        var result = Result<string?>.Success(null);

        // Act
        var mappedResult = result.Map(s => s?.Length ?? -1);

        // Assert
        ResultTesting.ShouldBeSuccessWithValue(mappedResult, -1);
    }

    #endregion
}
