namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents a collection that maps objects to their associated values, with support for retrieving values by type.
/// </summary>
/// <remarks>This class extends <see cref="Dictionary{TKey, TValue}"/> to provide additional functionality for
/// storing and retrieving objects with type safety. It is particularly useful in scenarios where objects need to be
/// tracked or associated with metadata during processing.</remarks>
public class VisitedObjectsList : Dictionary<object, object>
{
   
}
