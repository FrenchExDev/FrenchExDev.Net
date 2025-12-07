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
