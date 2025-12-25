namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Default implementation of ISynchronizationStrategy using lock statements.
/// </summary>
public sealed class LockSynchronizationStrategy : ISynchronizationStrategy
{
    public static readonly LockSynchronizationStrategy Instance = new();

    private LockSynchronizationStrategy() { }

    public void Execute(object lockObject, Action action)
    {
        lock (lockObject) { action(); }
    }

    public TResult Execute<TResult>(object lockObject, Func<TResult> func)
    {
        lock (lockObject) { return func(); }
    }
}
