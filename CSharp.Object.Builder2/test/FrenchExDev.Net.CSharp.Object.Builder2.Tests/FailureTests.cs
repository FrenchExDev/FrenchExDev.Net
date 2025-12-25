using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class FailureTests
{
    [Fact]
    public void FromException_ShouldCreateExceptionFailure()
    {
        var ex = new InvalidOperationException("test");

        var failure = Failure.FromException(ex);

        failure.IsException.ShouldBeTrue();
        failure.TryGetException(out var result).ShouldBeTrue();
        result.ShouldBe(ex);
    }

    [Fact]
    public void FromMessage_ShouldCreateMessageFailure()
    {
        var failure = Failure.FromMessage("test message");

        failure.IsMessage.ShouldBeTrue();
        failure.TryGetMessage(out var result).ShouldBeTrue();
        result.ShouldBe("test message");
    }

    [Fact]
    public void FromNested_ShouldCreateNestedFailure()
    {
        var nested = new FailuresDictionary();
        nested.AddFailure("field", Failure.FromMessage("error"));

        var failure = Failure.FromNested(nested);

        failure.IsNested.ShouldBeTrue();
        failure.TryGetNested(out var result).ShouldBeTrue();
        result.ShouldBe(nested);
    }

    [Fact]
    public void Match_WithExceptionFailure_ShouldCallOnException()
    {
        var failure = Failure.FromException(new InvalidOperationException("test"));

        var result = failure.Match(
            onException: ex => "exception: " + ex.Message,
            onMessage: msg => "message: " + msg,
            onNested: _ => "nested"
        );

        result.ShouldBe("exception: test");
    }

    [Fact]
    public void Match_WithMessageFailure_ShouldCallOnMessage()
    {
        var failure = Failure.FromMessage("test message");

        var result = failure.Match(
            onException: ex => "exception",
            onMessage: msg => "message: " + msg,
            onNested: _ => "nested"
        );

        result.ShouldBe("message: test message");
    }

    [Fact]
    public void MatchAction_WithExceptionFailure_ShouldCallOnException()
    {
        var failure = Failure.FromException(new InvalidOperationException("test"));
        Exception? captured = null;

        failure.Match(
            onException: ex => captured = ex,
            onMessage: _ => { },
            onNested: _ => { }
        );

        captured.ShouldNotBeNull();
        captured!.Message.ShouldBe("test");
    }

    [Fact]
    public void MatchAction_WithMessageFailure_ShouldCallOnMessage()
    {
        var failure = Failure.FromMessage("test message");
        string? captured = null;

        failure.Match(
            onException: _ => { },
            onMessage: msg => captured = msg,
            onNested: _ => { }
        );

        captured.ShouldBe("test message");
    }

    [Fact]
    public void MatchAction_WithNestedFailure_ShouldCallOnNested()
    {
        var nested = new FailuresDictionary();
        var failure = Failure.FromNested(nested);
        IFailureCollector? captured = null;

        failure.Match(
            onException: _ => { },
            onMessage: _ => { },
            onNested: n => captured = n
        );

        captured.ShouldBe(nested);
    }

    [Fact]
    public void ImplicitConversion_FromException_ShouldWork()
    {
        Failure failure = new InvalidOperationException("test");

        failure.IsException.ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldWork()
    {
        Failure failure = "test message";

        failure.IsMessage.ShouldBeTrue();
    }
}

