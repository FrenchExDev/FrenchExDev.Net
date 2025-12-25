namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for tracking visited objects during validation/build to prevent cycles.
/// Follows Dependency Inversion Principle.
/// </summary>
public interface IVisitedTracker
{
    /// <summary>
    /// Checks if an object with the given ID has been visited.
    /// </summary>
    bool IsVisited(Guid id);

    /// <summary>
    /// Tries to get a visited object by ID.
    /// </summary>
    bool TryGet(Guid id, out object? value);

    /// <summary>
    /// Marks an object as visited.
    /// </summary>
    void MarkVisited(Guid id, object value);

    /// <summary>
    /// Gets the number of visited objects.
    /// </summary>
    int Count { get; }
}

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

/// <summary>
/// Defines a contract for validation strategies.
/// Follows Open/Closed Principle - new validation strategies can be added without modifying existing code.
/// </summary>
/// <typeparam name="TBuilder">The type of builder being validated.</typeparam>
public interface IValidationStrategy<in TBuilder>
{
    /// <summary>
    /// Validates the builder and collects failures.
    /// </summary>
    void Validate(TBuilder builder, IVisitedTracker visited, IFailureCollector failures);
}

/// <summary>
/// Defines a contract for build orchestration.
/// Follows Single Responsibility Principle - separates orchestration from builder logic.
/// </summary>
/// <typeparam name="TClass">The type of object being built.</typeparam>
public interface IBuildOrchestrator<TClass> where TClass : class
{
    /// <summary>
    /// Orchestrates the build process.
    /// </summary>
    Result2.Result<Reference<TClass>> Build(IBuilder<TClass> builder, IVisitedTracker? visited = null);
}
