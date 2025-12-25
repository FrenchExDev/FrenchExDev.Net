namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents a validation or build failure with strong typing.
/// </summary>
public abstract record Failure
{
    public static Failure FromException(Exception exception) => new ExceptionFailure(exception);
    public static Failure FromMessage(string message) => new MessageFailure(message);
    public static Failure FromNested(IFailureCollector failures) => new NestedFailure(failures);

    public static implicit operator Failure(Exception ex) => new ExceptionFailure(ex);
    public static implicit operator Failure(string message) => new MessageFailure(message);

    public abstract TResult Match<TResult>(
        Func<Exception, TResult> onException,
        Func<string, TResult> onMessage,
        Func<IFailureCollector, TResult> onNested);

    public abstract void Match(
        Action<Exception> onException,
        Action<string> onMessage,
        Action<IFailureCollector> onNested);

    public bool IsException => this is ExceptionFailure;
    public bool IsMessage => this is MessageFailure;
    public bool IsNested => this is NestedFailure;

    public bool TryGetException(out Exception? exception)
    {
        if (this is ExceptionFailure ef) { exception = ef.Exception; return true; }
        exception = null; return false;
    }

    public bool TryGetMessage(out string? message)
    {
        if (this is MessageFailure mf) { message = mf.Message; return true; }
        message = null; return false;
    }

    public bool TryGetNested(out IFailureCollector? failures)
    {
        if (this is NestedFailure nf) { failures = nf.Failures; return true; }
        failures = null; return false;
    }
}

public sealed record ExceptionFailure(Exception Exception) : Failure
{
    public override TResult Match<TResult>(Func<Exception, TResult> onException, Func<string, TResult> onMessage, Func<IFailureCollector, TResult> onNested) => onException(Exception);
    public override void Match(Action<Exception> onException, Action<string> onMessage, Action<IFailureCollector> onNested) => onException(Exception);
}

public sealed record MessageFailure(string Message) : Failure
{
    public override TResult Match<TResult>(Func<Exception, TResult> onException, Func<string, TResult> onMessage, Func<IFailureCollector, TResult> onNested) => onMessage(Message);
    public override void Match(Action<Exception> onException, Action<string> onMessage, Action<IFailureCollector> onNested) => onMessage(Message);
}

public sealed record NestedFailure(IFailureCollector Failures) : Failure
{
    public override TResult Match<TResult>(Func<Exception, TResult> onException, Func<string, TResult> onMessage, Func<IFailureCollector, TResult> onNested) => onNested(Failures);
    public override void Match(Action<Exception> onException, Action<string> onMessage, Action<IFailureCollector> onNested) => onNested(Failures);
}
