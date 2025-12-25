namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for synchronization during build/validation operations.
/// Follows Dependency Inversion Principle - allows different concurrency strategies.
/// </summary>
public interface ISynchronizationStrategy
{
    /// <summary>
    /// Executes an action within a synchronized context.
    /// </summary>
    void Execute(object lockObject, Action action);

    /// <summary>
    /// Executes a function within a synchronized context.
    /// </summary>
    TResult Execute<TResult>(object lockObject, Func<TResult> func);
}
