using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class FailureAdditionalBranchCoverageTests
{
    [Fact]
    public void ExceptionFailure_TryGetMessage_ShouldReturnFalse()
    {
        var failure = Failure.FromException(new Exception("test"));
        failure.TryGetMessage(out var result).ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Fact]
    public void ExceptionFailure_TryGetNested_ShouldReturnFalse()
    {
        var failure = Failure.FromException(new Exception("test"));
        failure.TryGetNested(out var result).ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Fact]
    public void MessageFailure_TryGetException_ShouldReturnFalse()
    {
        var failure = Failure.FromMessage("test");
        failure.TryGetException(out var result).ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Fact]
    public void MessageFailure_TryGetNested_ShouldReturnFalse()
    {
        var failure = Failure.FromMessage("test");
        failure.TryGetNested(out var result).ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Fact]
    public void ExceptionFailure_MatchAction_ShouldCallOnException()
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
    public void MessageFailure_MatchAction_ShouldCallOnMessage()
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
}

