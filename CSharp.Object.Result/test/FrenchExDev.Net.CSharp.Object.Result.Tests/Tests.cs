using FrenchExDev.Net.CSharp.Object.Result.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Result.Tests;


/// <summary>
/// Contains unit tests for the Result and Result<T> types, verifying their success and failure behaviors, exception
/// handling, and related extension methods.
/// </summary>
/// <remarks>These tests ensure that the Result API correctly represents operation outcomes, handles exceptions as
/// failures, and supports conditional execution based on result state. The class covers both generic and non-generic
/// result scenarios, including extension methods for converting values and exceptions to results.</remarks>
public class ResultTests
{
    [Fact]
    public void NonGeneric_Result_Success_and_Failure_flags()
    {
        var success = Result.Success();
        success.ShouldBeSuccess();

        var failure = Result.Failure();
        failure.ShouldBeFailure();
    }

    [Fact]
    public void NonGeneric_Result_Failure_with_exception_is_stored()
    {
        var ex = new InvalidOperationException("boom");
        var r = Result.Failure(ex);
        r.ShouldBeFailure()
         .ShouldHaveException();
        
        var storedEx = r.ShouldHaveException<InvalidOperationException>();
        storedEx.ShouldBeSameAs(ex);
    }

    [Fact]
    public void NonGeneric_IfSuccess_and_IfFailure_actions_are_conditionally_invoked()
    {
        var invoked = false;
        Result.Success().IfSuccess(() => invoked = true);
        invoked.ShouldBeTrue();

        invoked = false;
        Result.Failure().IfSuccess(() => invoked = true);
        invoked.ShouldBeFalse();

        Exception? captured = null;
        Result.Failure(new Exception("err")).IfFailure(e => captured = e);
        captured.ShouldNotBeNull();
        captured!.Message.ShouldBe("err");
    }

    [Fact]
    public async Task NonGeneric_IfSuccessAsync_and_IfFailureAsync_work()
    {
        var called = false;
        var res = await Result.Success().IfSuccessAsync(async () =>
        {
            await Task.Delay(1);
            called = true;
        });
        called.ShouldBeTrue();
        res.ShouldBeSuccess();

        called = false;
        await Result.Failure(new Exception()).IfFailureAsync(async ex =>
        {
            await Task.Delay(1);
            called = true;
        });
        called.ShouldBeTrue();
    }

    [Fact]
    public void Generic_Result_Success_and_accessors()
    {
        var res = Result<string>.Success("hello");
        res.ShouldBeSuccess()
           .ShouldHaveValue("hello");
        
        res.ObjectOrNull().ShouldBe("hello");
        res.TryGetSuccess(out var value).ShouldBeTrue();
        value.ShouldBe("hello");
    }

    [Fact]
    public void Generic_Result_Failure_behaviour_and_exceptions()
    {
        var failure = Result<string>.Failure();
        failure.ShouldBeFailure();
        
        failure.ObjectOrNull().ShouldBeNull();
        failure.TryGetSuccess(out var _).ShouldBeFalse();

        Should.Throw<InvalidResultAccessException>(() => { var _ = failure.Object; });
        Should.Throw<InvalidOperationException>(() => failure.ObjectOrThrow());
        Should.Throw<InvalidResultAccessException>(() => failure.FailuresOrThrow());
    }

    [Fact]
    public void Generic_Result_Failure_with_exception_extension_sets_exception_in_failures()
    {
        var ex = new ArgumentException("bad");
        var fail = ex.ToFailure<string>();
        fail.ShouldBeFailure()
            .ShouldHaveFailureKey("Exception");
    }

    [Fact]
    public void TryCatch_converts_exceptions_to_failure_and_returns_success_on_ok()
    {
        var ok = Result<int>.TryCatch(() => 42.ToSuccess());
        ok.ShouldBeSuccess()
          .ShouldHaveValue(42);

        var fail = Result<int>.TryCatch<int, InvalidOperationException>(() => throw new InvalidOperationException("boom"));
        fail.ShouldBeFailure()
            .ShouldHaveFailureKey("Exception");
    }

    [Fact]
    public void ToResult_and_ToSuccess_extensions()
    {
        var r1 = true.ToResult();
        r1.ShouldBeSuccess();

        var s = "x".ToSuccess();
        s.ShouldBeSuccess()
         .ShouldHaveValue("x");
    }

    [Fact]
    public void Failure_with_builder_and_dictionary()
    {
        var res = Result<string>.Failure(d => d.Add("a", "b"));
        res.ShouldBeFailure()
           .ShouldHaveFailureKey("a");
    }

    [Fact]
    public void Failure_to_failure_with_subject()
    {
        var subj = "subject";
        var r = subj.ToFailure<object>("reason");
        r.ShouldBeFailure()
         .ShouldHaveFailureKey("Failure")
         .ShouldHaveFailureKey("Subject");
    }

    [Fact]
    public void IResult_interface_is_implemented()
    {
        IResult r1 = (IResult)Result.Success();
        r1.IsSuccess.ShouldBeTrue();

        IResult r2 = (IResult)Result<string>.Success("ok");
        r2.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void FailureDictionary_adds_and_accumulates_values()
    {
        var fd = new FailureDictionary();
        fd.Add("k", "v1");
        fd.ContainsKey("k").ShouldBeTrue();
        fd["k"].Count.ShouldBe(1);
        fd["k"][0].ShouldBe("v1");

        fd.Add("k", "v2");
        fd["k"].Count.ShouldBe(2);
        fd["k"].ShouldContain("v1");
        fd["k"].ShouldContain("v2");
    }

    [Fact]
    public void FailureDictionaryBuilder_builds_and_is_independent()
    {
        var b = new FailureDictionaryBuilder();
        b.Add("a", "first");
        var built = b.Build();
        built.ContainsKey("a").ShouldBeTrue();
        built["a"].Count.ShouldBe(1);
        built["a"][0].ShouldBe("first");

        // modify builder after build
        b.Add("a", "second");
        // built should remain unchanged
        built["a"].Count.ShouldBe(1);
        built["a"][0].ShouldBe("first");
    }

    [Fact]
    public void TryCatch_with_mismatched_exception_propagates()
    {
        Should.Throw<ArgumentException>(() =>
        {
            // TException is InvalidOperationException but action throws ArgumentException -> should propagate
            _ = Result<int>.TryCatch<int, InvalidOperationException>(() => throw new ArgumentException("bad"));
        });
    }

    [Fact]
    public void TryCatch_non_typed_captures_any_exception()
    {
        var ex = new InvalidOperationException("boom");
        var res = Result<int>.TryCatch<int>(() => throw ex);
        res.ShouldBeFailure()
           .ShouldHaveFailureKey("Exception");
        
        var storedEx = res.ShouldHaveFailureException<int, InvalidOperationException>();
        storedEx.ShouldBeSameAs(ex);
    }

    [Fact]
    public async Task Generic_IfSuccessAsync_and_IfFailureAsync_behave_correctly()
    {
        var called = false;
        var ok = Result<int>.Success(5);
        var returned = await ok.IfSuccessAsync(async v =>
        {
            await Task.Yield();
            called = true;
            v.ShouldBe(5);
        });

        called.ShouldBeTrue();
        returned.ShouldBeSuccess()
                .ShouldHaveValue(5);

        called = false;
        var fail = Result<int>.Failure(d => d.Add("x", "y"));
        var returnedFail = await fail.IfFailureAsync(async dict =>
        {
            await Task.Yield();
            called = dict != null && dict.ContainsKey("x");
        });
        called.ShouldBeTrue();
        returnedFail.ShouldBeFailure();
    }

    [Fact]
    public void ObjectOrThrow_and_TryGetSuccess_for_value_types()
    {
        var r = Result<int>.Success(10);
        r.ShouldBeSuccess()
         .ShouldHaveValue(10);
        r.ObjectOrThrow().ShouldBe(10);

        var f = Result<int>.Failure();
        f.ShouldBeFailure();
        f.TryGetSuccess(out var val).ShouldBeFalse();
        val.ShouldBe(default(int));
    }

    [Fact]
    public async Task Chaining_methods_return_equivalent_result()
    {
        var res = Result<int>.Success(1);
        var after = res.IfSuccess(v => { /* no-op */ });
        after.ShouldBeSuccess()
             .ShouldHaveValue(1);

        var afterAsync = await after.IfSuccessAsync(async v => await Task.Yield());
        afterAsync.ShouldBeSuccess()
                  .ShouldHaveValue(1);
    }

    [Fact]
    public void Failure_contents_store_expected_subject_and_failure_values()
    {
        var subj = "me";
        var res = subj.ToFailure<object>("reason");
        res.ShouldBeFailure()
           .ShouldHaveFailure("Failure", "reason")
           .ShouldHaveFailure("Subject", subj);
    }

    [Fact]
    public void Generic_Result_Failure_with_exception_static_method()
    {
        var ex = new InvalidOperationException("static-fail");
        var res = Result<string>.Failure(ex);
        res.ShouldBeFailure()
           .ShouldHaveFailureKey("Exception");
        
        var storedEx = res.ShouldHaveFailureException<string, InvalidOperationException>();
        storedEx.ShouldBeSameAs(ex);
    }

    [Fact]
    public void Generic_Result_Exception_method_stores_exception_in_failures()
    {
        var ex = new InvalidOperationException("ex-method");
        var res = Result<string>.Exception(ex);
        res.ShouldBeFailure()
           .ShouldHaveFailureKey("Exception");
        
        var storedEx = res.ShouldHaveFailureException<string, InvalidOperationException>();
        storedEx.ShouldBeSameAs(ex);
    }

    [Fact]
    public void Generic_IfFailure_invokes_action_with_nonnull_failures()
    {
        var invoked = false;
        var res = Result<int>.Failure(d => d.Add("k", "v"));
        res.ShouldBeFailure()
           .ShouldHaveFailureKey("k");
        
        res.IfFailure(f =>
        {
            invoked = true;
            f.ShouldNotBeNull();
            f!.ContainsKey("k").ShouldBeTrue();
        });
        invoked.ShouldBeTrue();
    }
}

#region ResultAssert Tests

/// <summary>
/// Tests for ResultAssert static assertion methods - specifically testing failure scenarios.
/// </summary>
public class ResultAssertTests
{
    [Fact]
    public void IsSuccess_NonGeneric_Throws_When_Result_Is_Failure()
    {
        var failure = Result.Failure();
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.IsSuccess(failure));
        ex.Message.ShouldContain("failure");
    }

    [Fact]
    public void IsSuccess_NonGeneric_Throws_With_Custom_Message()
    {
        var failure = Result.Failure();
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.IsSuccess(failure, "Custom message"));
        ex.Message.ShouldBe("Custom message");
    }

    [Fact]
    public void IsFailure_NonGeneric_Throws_When_Result_Is_Success()
    {
        var success = Result.Success();
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.IsFailure(success));
        ex.Message.ShouldContain("successful");
    }

    [Fact]
    public void IsFailure_NonGeneric_Throws_With_Custom_Message()
    {
        var success = Result.Success();
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.IsFailure(success, "Custom failure message"));
        ex.Message.ShouldBe("Custom failure message");
    }

    [Fact]
    public void IsSuccess_Generic_Throws_When_Result_Is_Failure()
    {
        var failure = Result<string>.Failure();
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.IsSuccess(failure));
        ex.Message.ShouldContain("failure");
        ex.Message.ShouldContain("String");
    }

    [Fact]
    public void IsSuccess_Generic_Throws_With_Custom_Message()
    {
        var failure = Result<string>.Failure();
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.IsSuccess(failure, "Custom generic message"));
        ex.Message.ShouldBe("Custom generic message");
    }

    [Fact]
    public void IsFailure_Generic_Throws_When_Result_Is_Success()
    {
        var success = Result<string>.Success("value");
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.IsFailure(success));
        ex.Message.ShouldContain("successful");
        ex.Message.ShouldContain("String");
    }

    [Fact]
    public void IsFailure_Generic_Throws_With_Custom_Message()
    {
        var success = Result<string>.Success("value");
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.IsFailure(success, "Custom failure generic"));
        ex.Message.ShouldBe("Custom failure generic");
    }

    [Fact]
    public void HasValue_Throws_When_Value_Does_Not_Match()
    {
        var result = Result<string>.Success("actual");
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasValue(result, "expected"));
        ex.Message.ShouldContain("actual");
        ex.Message.ShouldContain("expected");
    }

    [Fact]
    public void HasValue_Throws_With_Custom_Message()
    {
        var result = Result<string>.Success("actual");
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasValue(result, "expected", "Custom value message"));
        ex.Message.ShouldBe("Custom value message");
    }

    [Fact]
    public void HasValueMatching_Throws_When_Predicate_Fails()
    {
        var result = Result<int>.Success(5);
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasValueMatching(result, v => v > 10));
        ex.Message.ShouldContain("5");
        ex.Message.ShouldContain("predicate");
    }

    [Fact]
    public void HasValueMatching_Throws_With_Custom_Message()
    {
        var result = Result<int>.Success(5);
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasValueMatching(result, v => v > 10, "Custom predicate message"));
        ex.Message.ShouldBe("Custom predicate message");
    }

    [Fact]
    public void HasException_Throws_When_No_Exception()
    {
        var result = Result.Failure();
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasException(result));
        ex.Message.ShouldContain("null");
    }

    [Fact]
    public void HasException_Throws_With_Custom_Message()
    {
        var result = Result.Failure();
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasException(result, "Custom no exception message"));
        ex.Message.ShouldBe("Custom no exception message");
    }

    [Fact]
    public void HasException_Generic_Throws_When_Wrong_Type()
    {
        var result = Result.Failure(new InvalidOperationException("test"));
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasException<ArgumentException>(result));
        ex.Message.ShouldContain("ArgumentException");
        ex.Message.ShouldContain("InvalidOperationException");
    }

    [Fact]
    public void HasException_Generic_Throws_With_Custom_Message()
    {
        var result = Result.Failure(new InvalidOperationException("test"));
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasException<ArgumentException>(result, "Custom exception type message"));
        ex.Message.ShouldBe("Custom exception type message");
    }

    [Fact]
    public void HasFailureKey_Throws_When_Key_Not_Found()
    {
        var result = Result<string>.Failure(d => d.Add("OtherKey", "value"));
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasFailureKey(result, "MissingKey"));
        ex.Message.ShouldContain("MissingKey");
    }

    [Fact]
    public void HasFailureKey_Throws_With_Custom_Message()
    {
        var result = Result<string>.Failure(d => d.Add("OtherKey", "value"));
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasFailureKey(result, "MissingKey", "Custom key message"));
        ex.Message.ShouldBe("Custom key message");
    }

    [Fact]
    public void HasFailure_Throws_When_Value_Not_Found()
    {
        var result = Result<string>.Failure(d => d.Add("Key", "actual"));
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasFailure(result, "Key", "expected"));
        ex.Message.ShouldContain("Key");
        ex.Message.ShouldContain("expected");
    }

    [Fact]
    public void HasFailure_Throws_With_Custom_Message()
    {
        var result = Result<string>.Failure(d => d.Add("Key", "actual"));
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasFailure(result, "Key", "expected", "Custom failure value message"));
        ex.Message.ShouldBe("Custom failure value message");
    }

    [Fact]
    public void HasFailureException_Throws_When_No_Exception_In_Failures()
    {
        var result = Result<string>.Failure(d => d.Add("Exception", "not-an-exception"));
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasFailureException(result));
        ex.Message.ShouldContain("Exception");
    }

    [Fact]
    public void HasFailureException_Throws_With_Custom_Message()
    {
        var result = Result<string>.Failure(d => d.Add("Exception", "not-an-exception"));
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasFailureException(result, "Custom no failure exception message"));
        ex.Message.ShouldBe("Custom no failure exception message");
    }

    [Fact]
    public void HasFailureException_Generic_Throws_When_Wrong_Type()
    {
        var result = Result<string>.Failure(new InvalidOperationException("test"));
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasFailureException<string, ArgumentException>(result));
        ex.Message.ShouldContain("ArgumentException");
        ex.Message.ShouldContain("InvalidOperationException");
    }

    [Fact]
    public void HasFailureException_Generic_Throws_With_Custom_Message()
    {
        var result = Result<string>.Failure(new InvalidOperationException("test"));
        
        var ex = Should.Throw<ResultAssertException>(() => ResultAssert.HasFailureException<string, ArgumentException>(result, "Custom typed failure exception message"));
        ex.Message.ShouldBe("Custom typed failure exception message");
    }
}

#endregion

#region ResultAssertException Tests

/// <summary>
/// Tests for ResultAssertException constructors.
/// </summary>
public class ResultAssertExceptionTests
{
    [Fact]
    public void Constructor_With_Message_Sets_Message()
    {
        var ex = new ResultAssertException("Test message");
        
        ex.Message.ShouldBe("Test message");
    }

    [Fact]
    public void Constructor_With_Message_And_InnerException_Sets_Both()
    {
        var inner = new InvalidOperationException("Inner");
        var ex = new ResultAssertException("Outer message", inner);
        
        ex.Message.ShouldBe("Outer message");
        ex.InnerException.ShouldBeSameAs(inner);
    }

    [Fact]
    public void Exception_Can_Be_Thrown_And_Caught()
    {
        var inner = new ArgumentException("inner arg");
        
        var caught = Should.Throw<ResultAssertException>(() =>
        {
            throw new ResultAssertException("Test throw", inner);
        });
        
        caught.Message.ShouldBe("Test throw");
        caught.InnerException.ShouldBeSameAs(inner);
    }
}

#endregion

#region ResultAssertExtensions Failure Path Tests

/// <summary>
/// Tests for ResultAssertExtensions - specifically testing failure scenarios to ensure exceptions propagate correctly.
/// </summary>
public class ResultAssertExtensionsTests
{
    [Fact]
    public void ShouldBeSuccess_NonGeneric_Throws_When_Failure()
    {
        var failure = Result.Failure();
        
        Should.Throw<ResultAssertException>(() => failure.ShouldBeSuccess());
    }

    [Fact]
    public void ShouldBeFailure_NonGeneric_Throws_When_Success()
    {
        var success = Result.Success();
        
        Should.Throw<ResultAssertException>(() => success.ShouldBeFailure());
    }

    [Fact]
    public void ShouldHaveException_NonGeneric_Throws_When_No_Exception()
    {
        var failure = Result.Failure();
        
        Should.Throw<ResultAssertException>(() => failure.ShouldHaveException());
    }

    [Fact]
    public void ShouldHaveException_Generic_Throws_When_Wrong_Type()
    {
        var failure = Result.Failure(new InvalidOperationException());
        
        Should.Throw<ResultAssertException>(() => failure.ShouldHaveException<ArgumentException>());
    }

    [Fact]
    public void ShouldBeSuccess_Generic_Throws_When_Failure()
    {
        var failure = Result<int>.Failure();
        
        Should.Throw<ResultAssertException>(() => failure.ShouldBeSuccess());
    }

    [Fact]
    public void ShouldBeFailure_Generic_Throws_When_Success()
    {
        var success = Result<int>.Success(42);
        
        Should.Throw<ResultAssertException>(() => success.ShouldBeFailure());
    }

    [Fact]
    public void ShouldHaveValue_Throws_When_Value_Differs()
    {
        var result = Result<string>.Success("actual");
        
        Should.Throw<ResultAssertException>(() => result.ShouldHaveValue("expected"));
    }

    [Fact]
    public void ShouldHaveValueMatching_Throws_When_Predicate_Fails()
    {
        var result = Result<int>.Success(5);
        
        Should.Throw<ResultAssertException>(() => result.ShouldHaveValueMatching(v => v > 100));
    }

    [Fact]
    public void ShouldHaveFailureKey_Throws_When_Key_Missing()
    {
        var result = Result<string>.Failure(d => d.Add("A", "value"));
        
        Should.Throw<ResultAssertException>(() => result.ShouldHaveFailureKey("B"));
    }

    [Fact]
    public void ShouldHaveFailure_Throws_When_Value_Missing()
    {
        var result = Result<string>.Failure(d => d.Add("Key", "actual"));
        
        Should.Throw<ResultAssertException>(() => result.ShouldHaveFailure("Key", "expected"));
    }

    [Fact]
    public void ShouldHaveFailureException_NonTyped_Throws_When_No_Exception()
    {
        var result = Result<string>.Failure(d => d.Add("Exception", "not-exception"));
        
        Should.Throw<ResultAssertException>(() => result.ShouldHaveFailureException());
    }

    [Fact]
    public void ShouldHaveFailureException_Typed_Throws_When_Wrong_Type()
    {
        var result = Result<string>.Failure(new InvalidOperationException());
        
        Should.Throw<ResultAssertException>(() => result.ShouldHaveFailureException<string, ArgumentException>());
    }

    [Fact]
    public void ShouldHaveValueMatching_Returns_Result_When_Predicate_Matches()
    {
        var result = Result<int>.Success(10);
        
        var returned = result.ShouldHaveValueMatching(v => v == 10);
        returned.IsSuccess.ShouldBeTrue();
        returned.Object.ShouldBe(10);
    }

    [Fact]
    public void ShouldHaveFailureException_Returns_Exception_When_Found()
    {
        var ex = new InvalidOperationException("stored");
        var result = Result<string>.Failure(ex);
        
        var returnedException = result.ShouldHaveFailureException();
        returnedException.ShouldBeSameAs(ex);
    }
}

#endregion
