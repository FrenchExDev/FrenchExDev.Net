namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents a dictionary that allows storing and retrieving objects with support for type-safe retrieval.
/// </summary>
/// <remarks>This class extends <see cref="Dictionary{object, object}"/> and provides additional methods for 
/// setting and retrieving values with type safety. The <see cref="Get{T}(object)"/> method ensures that  the retrieved
/// value matches the expected type, throwing an exception if the type does not match.</remarks>
public class IntermediateObjectDictionary : Dictionary<object, object>
{
    /// <summary>
    /// Retrieves the value associated with the specified key and casts it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be cast.</typeparam>
    /// <param name="key">The key whose associated value is to be retrieved. Cannot be <see langword="null"/>.</param>
    /// <returns>The value associated with the specified key, cast to type <typeparamref name="T"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified key does not exist in the dictionary or if the associated value cannot be cast to type
    /// <typeparamref name="T"/>.</exception>
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
