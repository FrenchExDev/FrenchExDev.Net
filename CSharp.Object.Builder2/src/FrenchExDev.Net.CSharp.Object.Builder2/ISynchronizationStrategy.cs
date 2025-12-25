namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for synchronization strategies used during build and validation operations.
/// Follows the Strategy pattern for flexible concurrency control.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ISynchronizationStrategy"/> allows builders to be configured with different
/// synchronization approaches depending on the use case:
/// </para>
/// <list type="bullet">
///   <item><description><see cref="LockSynchronizationStrategy"/> - Default, uses <c>lock</c> statements</description></item>
///   <item><description><see cref="NoSynchronizationStrategy"/> - No locking, for single-threaded scenarios</description></item>
///   <item><description><see cref="ReaderWriterSynchronizationStrategy"/> - Uses <see cref="ReaderWriterLockSlim"/> for read-heavy scenarios</description></item>
/// </list>
/// <para>
/// This follows the Dependency Inversion Principle, allowing high-level builder code to depend
/// on this abstraction rather than concrete synchronization implementations.
/// </para>
/// </remarks>
/// <seealso cref="LockSynchronizationStrategy"/>
/// <seealso cref="NoSynchronizationStrategy"/>
/// <seealso cref="ReaderWriterSynchronizationStrategy"/>
public interface ISynchronizationStrategy
{
    /// <summary>
    /// Executes an action within a synchronized context.
    /// </summary>
    /// <param name="lockObject">The object to use for synchronization.</param>
    /// <param name="action">The action to execute within the synchronized context.</param>
    void Execute(object lockObject, Action action);

    /// <summary>
    /// Executes a function within a synchronized context and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="lockObject">The object to use for synchronization.</param>
    /// <param name="func">The function to execute within the synchronized context.</param>
    /// <returns>The result of the function.</returns>
    TResult Execute<TResult>(object lockObject, Func<TResult> func);
}
