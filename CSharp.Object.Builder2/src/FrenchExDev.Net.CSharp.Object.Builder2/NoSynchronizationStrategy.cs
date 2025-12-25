namespace FrenchExDev.Net.CSharp.Object.Builder2;

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
