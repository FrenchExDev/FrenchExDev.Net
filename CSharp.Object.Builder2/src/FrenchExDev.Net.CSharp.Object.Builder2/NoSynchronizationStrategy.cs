namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// A no-operation synchronization strategy for single-threaded scenarios where thread safety is not required.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="NoSynchronizationStrategy"/> executes actions and functions directly without any locking,
/// providing better performance when thread safety is guaranteed by external means or not needed.
/// </para>
/// <para>
/// <strong>Warning:</strong> Use this strategy only when you are certain that:
/// </para>
/// <list type="bullet">
///   <item><description>The builder will only be accessed from a single thread</description></item>
///   <item><description>External synchronization is already in place</description></item>
///   <item><description>The application is inherently single-threaded</description></item>
/// </list>
/// <para>
/// This class is implemented as a singleton with thread-safe access via <see cref="Instance"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Use for single-threaded performance optimization
/// public class MyBuilder : AbstractBuilder&lt;MyClass&gt;
/// {
///     public MyBuilder() 
///         : base(DefaultReferenceFactory.Instance, NoSynchronizationStrategy.Instance)
///     { }
/// }
/// </code>
/// </example>
/// <seealso cref="ISynchronizationStrategy"/>
/// <seealso cref="LockSynchronizationStrategy"/>
/// <seealso cref="ReaderWriterSynchronizationStrategy"/>
public sealed class NoSynchronizationStrategy : ISynchronizationStrategy
{
    /// <summary>
    /// Gets the singleton instance of <see cref="NoSynchronizationStrategy"/>.
    /// </summary>
    public static readonly NoSynchronizationStrategy Instance = new();

    private NoSynchronizationStrategy() { }

    /// <summary>
    /// Executes an action directly without any synchronization.
    /// </summary>
    /// <param name="lockObject">Ignored. No locking is performed.</param>
    /// <param name="action">The action to execute.</param>
    public void Execute(object lockObject, Action action) => action();

    /// <summary>
    /// Executes a function directly without any synchronization and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <param name="lockObject">Ignored. No locking is performed.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>The result of the function.</returns>
    public TResult Execute<TResult>(object lockObject, Func<TResult> func) => func();
}
