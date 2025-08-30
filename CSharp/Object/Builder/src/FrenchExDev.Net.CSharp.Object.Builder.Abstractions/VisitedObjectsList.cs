namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents a collection that maps objects to their associated values, with support for retrieving values by type.
/// </summary>
/// <remarks>This class extends <see cref="Dictionary{TKey, TValue}"/> to provide additional functionality for
/// storing and retrieving objects with type safety. It is particularly useful in scenarios where objects need to be
/// tracked or associated with metadata during processing.</remarks>
public class VisitedObjectsList : Dictionary<object, object>
{
    /// <summary>
    /// Gets the value associated with the specified key, cast to the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public T Get<T>(object key)
    {
        if (TryGetValue(key, out var value) && value is T tValue)
        {
            return tValue;
        }
        throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary or is not of type {typeof(T).FullName}.");
    }

    /// <summary>
    /// Sets the value associated with the specified key in the collection.
    /// </summary>
    /// <param name="key">The key to associate with the specified value. Cannot be <see langword="null"/>.</param>
    /// <param name="value">The value to associate with the specified key. Can be <see langword="null"/>.</param>
    public void Set(object key, object value)
    {
        this[key] = value;
    }
}
