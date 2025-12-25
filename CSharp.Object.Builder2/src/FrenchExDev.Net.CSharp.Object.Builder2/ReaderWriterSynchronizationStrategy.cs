namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Synchronization strategy using <see cref="ReaderWriterLockSlim"/> for scenarios with many reads and few writes.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ReaderWriterSynchronizationStrategy"/> provides optimized concurrency for read-heavy workloads
/// by allowing multiple concurrent reads while ensuring exclusive access for writes.
/// </para>
/// <para>
/// Unlike <see cref="LockSynchronizationStrategy"/>, this strategy maintains its own internal lock
/// rather than using the provided lock object. The <c>lockObject</c> parameter in 
/// <see cref="Execute(object, Action)"/> and <see cref="Execute{TResult}(object, Func{TResult})"/> is ignored.
/// </para>
/// <para>
/// The strategy is configured with <see cref="LockRecursionPolicy.SupportsRecursion"/> to handle
/// nested lock acquisitions that may occur during complex build operations.
/// </para>
/// <para>
/// <strong>Note:</strong> This class is not a singleton. Each instance maintains its own lock,
/// so ensure the same instance is used across related operations.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Create a strategy instance for a builder
/// var strategy = new ReaderWriterSynchronizationStrategy();
/// 
/// public class MyBuilder : AbstractBuilder&lt;MyClass&gt;
/// {
///     private static readonly ReaderWriterSynchronizationStrategy _syncStrategy = new();
///     
///     public MyBuilder() 
///         : base(DefaultReferenceFactory.Instance, _syncStrategy)
///     { }
/// }
/// </code>
/// </example>
/// <seealso cref="ISynchronizationStrategy"/>
/// <seealso cref="LockSynchronizationStrategy"/>
/// <seealso cref="NoSynchronizationStrategy"/>
/// <seealso cref="ReaderWriterLockSlim"/>
public sealed class ReaderWriterSynchronizationStrategy : ISynchronizationStrategy
{
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

    /// <summary>
    /// Executes an action within a write lock.
    /// </summary>
    /// <param name="lockObject">Ignored. The internal <see cref="ReaderWriterLockSlim"/> is used instead.</param>
    /// <param name="action">The action to execute with exclusive write access.</param>
    public void Execute(object lockObject, Action action)
    {
        _lock.EnterWriteLock();
        try { action(); }
        finally { _lock.ExitWriteLock(); }
    }

    /// <summary>
    /// Executes a function within a write lock and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <param name="lockObject">Ignored. The internal <see cref="ReaderWriterLockSlim"/> is used instead.</param>
    /// <param name="func">The function to execute with exclusive write access.</param>
    /// <returns>The result of the function.</returns>
    public TResult Execute<TResult>(object lockObject, Func<TResult> func)
    {
        _lock.EnterWriteLock();
        try { return func(); }
        finally { _lock.ExitWriteLock(); }
    }

    /// <summary>
    /// Executes a read operation with a shared read lock, allowing concurrent reads.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <param name="func">The function to execute with shared read access.</param>
    /// <returns>The result of the function.</returns>
    /// <remarks>
    /// This method is specific to <see cref="ReaderWriterSynchronizationStrategy"/> and provides
    /// optimized access for read operations that don't modify state.
    /// </remarks>
    public TResult ExecuteRead<TResult>(Func<TResult> func)
    {
        _lock.EnterReadLock();
        try { return func(); }
        finally { _lock.ExitReadLock(); }
    }
}
