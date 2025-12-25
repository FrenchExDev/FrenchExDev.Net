namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// A dictionary that tracks objects visited during build or validation operations to prevent cycles.
/// Implements <see cref="IVisitedTracker"/> for cycle detection.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="VisitedObjectDictionary"/> is essential for handling circular references in object graphs.
/// When building or validating an object, it's marked as visited. If encountered again during the same
/// operation, the system can detect the cycle and handle it appropriately.
/// </para>
/// <para>
/// Keys are <see cref="Guid"/> identifiers (typically from <see cref="IIdentifiable.Id"/>),
/// and values are the visited objects themselves (usually builders).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var visited = new VisitedObjectDictionary();
/// 
/// // Mark an object as visited
/// visited.MarkVisited(builder.Id, builder);
/// 
/// // Check if already visited
/// if (visited.IsVisited(builder.Id))
/// {
///     // Handle circular reference
///     return existingReference;
/// }
/// </code>
/// </example>
/// <seealso cref="IVisitedTracker"/>
/// <seealso cref="AbstractBuilder{TClass}"/>
public class VisitedObjectDictionary : Dictionary<Guid, object>, IVisitedTracker
{
    /// <summary>
    /// Checks whether an object with the specified identifier has been visited.
    /// </summary>
    /// <param name="id">The unique identifier of the object to check.</param>
    /// <returns><see langword="true"/> if the object has been visited; otherwise, <see langword="false"/>.</returns>
    public bool IsVisited(Guid id) => ContainsKey(id);

    /// <summary>
    /// Attempts to retrieve a visited object by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the object to retrieve.</param>
    /// <param name="value">When this method returns, contains the visited object if found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the object was found; otherwise, <see langword="false"/>.</returns>
    public bool TryGet(Guid id, out object? value) => TryGetValue(id, out value);

    /// <summary>
    /// Marks an object as visited with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the object.</param>
    /// <param name="value">The object being visited (typically a builder instance).</param>
    /// <remarks>
    /// If an object with the same identifier already exists, it will be replaced.
    /// </remarks>
    public void MarkVisited(Guid id, object value) => this[id] = value;
}
