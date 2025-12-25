namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Default synchronization strategy that uses <c>lock</c> statements for thread-safe operations.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="LockSynchronizationStrategy"/> is the default strategy used by <see cref="AbstractBuilder{TClass}"/>.
/// It provides simple, reliable thread safety using the standard C# <c>lock</c> keyword.
/// </para>
/// <para>
/// This strategy is suitable for most scenarios. For single-threaded applications where
/// synchronization overhead is unnecessary, use <see cref="NoSynchronizationStrategy"/> instead.
/// For read-heavy scenarios, consider <see cref="ReaderWriterSynchronizationStrategy"/>.
/// </para>
/// <para>
/// This class is implemented as a singleton with thread-safe access via <see cref="Instance"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Using explicitly (rarely needed, as it's the default)
/// public class MyBuilder : AbstractBuilder&lt;MyClass&gt;
/// {
///     public MyBuilder() 
///         : base(DefaultReferenceFactory.Instance, LockSynchronizationStrategy.Instance)
///     { }
/// }
/// </code>
/// </example>
/// <seealso cref="ISynchronizationStrategy"/>
/// <seealso cref="NoSynchronizationStrategy"/>
/// <seealso cref="ReaderWriterSynchronizationStrategy"/>
public sealed class LockSynchronizationStrategy : ISynchronizationStrategy
{
    /// <summary>
    /// Gets the singleton instance of <see cref="LockSynchronizationStrategy"/>.
    /// </summary>
    public static readonly LockSynchronizationStrategy Instance = new();

    private LockSynchronizationStrategy() { }

    /// <summary>
    /// Executes an action within a <c>lock</c> block using the specified lock object.
    /// </summary>
    /// <param name="lockObject">The object to lock on.</param>
    /// <param name="action">The action to execute.</param>
    public void Execute(object lockObject, Action action)
    {
        lock (lockObject) { action(); }
    }

    /// <summary>
    /// Executes a function within a <c>lock</c> block using the specified lock object and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <param name="lockObject">The object to lock on.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>The result of the function.</returns>
    public TResult Execute<TResult>(object lockObject, Func<TResult> func)
    {
        lock (lockObject) { return func(); }
    }
}
