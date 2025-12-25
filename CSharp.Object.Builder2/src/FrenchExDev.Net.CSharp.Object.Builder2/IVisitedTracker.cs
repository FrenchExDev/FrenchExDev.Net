namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for tracking visited objects during build or validation operations to prevent cycles.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IVisitedTracker"/> is essential for handling circular references in object graphs.
/// Implementations track which objects (typically builders) have already been processed during
/// a build or validation operation.
/// </para>
/// <para>
/// This interface follows the Dependency Inversion Principle, allowing different tracking
/// implementations to be used without modifying the builder code.
/// </para>
/// </remarks>
/// <seealso cref="VisitedObjectDictionary"/>
public interface IVisitedTracker
{
    /// <summary>
    /// Determines whether an object with the specified identifier has been visited.
    /// </summary>
    /// <param name="id">The unique identifier of the object to check.</param>
    /// <returns><see langword="true"/> if the object has been visited; otherwise, <see langword="false"/>.</returns>
    bool IsVisited(Guid id);

    /// <summary>
    /// Attempts to retrieve a visited object by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the object to retrieve.</param>
    /// <param name="value">When this method returns, contains the visited object if found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if an object with the specified identifier was found; otherwise, <see langword="false"/>.</returns>
    bool TryGet(Guid id, out object? value);

    /// <summary>
    /// Marks an object as visited.
    /// </summary>
    /// <param name="id">The unique identifier of the object being marked as visited.</param>
    /// <param name="value">The object to associate with the identifier (typically the builder instance).</param>
    void MarkVisited(Guid id, object value);

    /// <summary>
    /// Gets the number of objects that have been marked as visited.
    /// </summary>
    int Count { get; }
}
