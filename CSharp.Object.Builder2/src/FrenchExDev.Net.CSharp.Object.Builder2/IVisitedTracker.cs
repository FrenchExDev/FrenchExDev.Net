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
