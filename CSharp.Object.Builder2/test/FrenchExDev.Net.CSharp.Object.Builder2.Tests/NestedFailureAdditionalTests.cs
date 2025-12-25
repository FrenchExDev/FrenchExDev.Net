using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class NestedFailureAdditionalTests
{
    [Fact]
    public void NestedFailure_Match_ShouldCallOnNested()
    {
        var nested = new FailuresDictionary();
        nested.AddFailure("inner", Failure.FromMessage("inner error"));
        var failure = Failure.FromNested(nested);

        var result = failure.Match(
            onException: ex => "exception",
            onMessage: msg => "message",
            onNested: n => "nested: " + n.FailureCount
        );

        result.ShouldBe("nested: 1");
    }

    [Fact]
    public void NestedFailure_TryGetNested_ShouldReturnFailures()
    {
        var nested = new FailuresDictionary();
        nested.AddFailure("field", Failure.FromMessage("error"));
        var failure = Failure.FromNested(nested);

        failure.TryGetNested(out var result).ShouldBeTrue();
        result.ShouldBe(nested);
    }

    [Fact]
    public void NestedFailure_TryGetException_ShouldReturnFalse()
    {
        var failure = Failure.FromNested(new FailuresDictionary());
        failure.TryGetException(out var result).ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Fact]
    public void NestedFailure_TryGetMessage_ShouldReturnFalse()
    {
        var failure = Failure.FromNested(new FailuresDictionary());
        failure.TryGetMessage(out var result).ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Fact]
    public void NestedFailure_MatchAction_ShouldCallOnNested()
    {
        var nested = new FailuresDictionary();
        nested.AddFailure("field", Failure.FromMessage("error"));
        var failure = Failure.FromNested(nested);
        IFailureCollector? captured = null;

        failure.Match(
            onException: _ => { },
            onMessage: _ => { },
            onNested: n => captured = n
        );

        captured.ShouldBe(nested);
    }
}

