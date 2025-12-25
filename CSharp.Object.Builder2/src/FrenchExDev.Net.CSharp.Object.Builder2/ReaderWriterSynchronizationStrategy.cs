namespace FrenchExDev.Net.CSharp.Object.Builder2;

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
