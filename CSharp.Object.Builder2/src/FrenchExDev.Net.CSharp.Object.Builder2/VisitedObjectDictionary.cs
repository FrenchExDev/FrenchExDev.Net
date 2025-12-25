namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Dictionary to track visited objects. Implements IVisitedTracker.
/// </summary>
public class VisitedObjectDictionary : Dictionary<Guid, object>, IVisitedTracker
{
    public bool IsVisited(Guid id) => ContainsKey(id);

    public bool TryGet(Guid id, out object? value) => TryGetValue(id, out value);

    public void MarkVisited(Guid id, object value) => this[id] = value;
}
