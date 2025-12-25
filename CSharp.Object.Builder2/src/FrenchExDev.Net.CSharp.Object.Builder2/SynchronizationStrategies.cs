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

/// <summary>
/// No-op synchronization strategy for single-threaded scenarios.
/// Improves performance when thread-safety is not required.
/// </summary>
public sealed class NoSynchronizationStrategy : ISynchronizationStrategy
{
    public static readonly NoSynchronizationStrategy Instance = new();

    private NoSynchronizationStrategy() { }

    public void Execute(object lockObject, Action action) => action();
    public TResult Execute<TResult>(object lockObject, Func<TResult> func) => func();
}

/// <summary>
/// Synchronization strategy using ReaderWriterLockSlim for read-heavy scenarios.
/// </summary>
public sealed class ReaderWriterSynchronizationStrategy : ISynchronizationStrategy
{
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

    public void Execute(object lockObject, Action action)
    {
        _lock.EnterWriteLock();
        try { action(); }
        finally { _lock.ExitWriteLock(); }
    }

    public TResult Execute<TResult>(object lockObject, Func<TResult> func)
    {
        _lock.EnterWriteLock();
        try { return func(); }
        finally { _lock.ExitWriteLock(); }
    }

    /// <summary>
    /// Executes a read operation with a read lock.
    /// </summary>
    public TResult ExecuteRead<TResult>(Func<TResult> func)
    {
        _lock.EnterReadLock();
        try { return func(); }
        finally { _lock.ExitReadLock(); }
    }
}
