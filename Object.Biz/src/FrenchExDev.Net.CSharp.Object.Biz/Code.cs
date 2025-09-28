using FrenchExDev.Net.CSharp.Object.Result;

namespace FrenchExDev.Net.CSharp.Object.Biz;

/// <summary>
/// Defines a contract for a facet, representing a discrete aspect or feature that can be associated with an object.
/// </summary>
/// <remarks>Implementations of this interface are typically used to provide additional metadata or behavior to
/// objects in a composable manner. Facets are commonly used in extensibility scenarios to enable flexible object
/// augmentation.</remarks>
public interface IFacet { }

/// <summary>
/// Defines a contract for retrieving a strongly-typed facet from an instance.
/// </summary>
/// <remarks>Implementations of this interface typically provide a mechanism for accessing additional behaviors or
/// data associated with an object, without requiring direct knowledge of its concrete type. This is commonly used in
/// extensibility scenarios where objects may expose optional capabilities via facets.</remarks>
public interface ISpecificFacet
{
    /// <summary>
    /// Returns an instance of the specified type parameter.
    /// </summary>
    /// <typeparam name="T">The type of object to retrieve. Must be a non-nullable reference or value type.</typeparam>
    /// <returns>An instance of type <typeparamref name="T"/>.</returns>
    T OfInstance<T>() where T : notnull;
}

/// <summary>
/// Defines a facet that is associated with a specific instance of type <typeparamref name="TClass"/>.
/// </summary>
/// <typeparam name="TClass">The type of the instance to which the facet is bound. Must be a non-nullable reference type.</typeparam>
public interface IGenericFacet<TClass> : IFacet where TClass : notnull
{
    TClass OfInstance();
}

/// <summary>
/// Represents a collection of named facets, where each facet is identified by a string key and associated with an
/// object value.
/// </summary>
/// <remarks>FacetDictionary is commonly used to store arbitrary metadata or attributes for entities, allowing
/// flexible extension of data models. Keys should be unique within the dictionary. Values can be of any type, but
/// consumers are responsible for casting them to the expected type when retrieving values.</remarks>
public class FacetDictionary : Dictionary<string, object>
{

}

/// <summary>
/// Represents an exception that is thrown when a requested facet does not exist.
/// </summary>
/// <remarks>Use this exception to indicate that an operation attempted to access a facet that is not defined or
/// available. This exception is typically thrown by APIs that manage or query facets in a data model or metadata
/// system.</remarks>
public class FacetDoesNotExistException : Exception
{
    /// <summary>
    /// Initializes a new instance of the FacetDoesNotExistException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public FacetDoesNotExistException(string message) : base(message) { }
}

/// <summary>
/// Represents an object that associates a facet of type <typeparamref name="TFacet"/> with a class of type
/// <typeparamref name="TClass"/>. Provides methods to retrieve, assign, and check for the presence of the facet.
/// </summary>
/// <remarks>Use this type to manage facets for a given class instance, enabling flexible extension or annotation
/// of objects without modifying their original structure. Facets are stored and retrieved by their type, allowing for
/// type-safe access and assignment.</remarks>
/// <typeparam name="TClass">The type of the class to which the facet is associated. Must be non-null.</typeparam>
/// <typeparam name="TFacet">The type of the facet to associate with the class. Must implement <see cref="IGenericFacet{TClass}"/> and be
/// non-null.</typeparam>
public class FacetObject<TClass, TFacet> where TFacet : notnull, IFacet
{
    /// <summary>
    /// Stores the Facet dictionary.
    /// </summary>
    private FacetDictionary _facets = [];

    /// <summary>
    /// Retrieves the facet of type <typeparamref name="TFacet"/> from the current context, if available.
    /// </summary>
    /// <remarks>Use this method to access additional metadata or features associated with the current
    /// context. If the requested facet is not present, the returned result will indicate failure.</remarks>
    /// <returns>A <see cref="Result{TFacet}"/> containing the facet of type <typeparamref name="TFacet"/> if it exists;
    /// otherwise, a failure result.</returns>
    public Result<TFacet> Facet()
    {
        if (_facets.TryGetValue(typeof(TFacet).Name, out var result))
        {
            return Result<TFacet>.Success((TFacet)(object)result);
        }

        return Result<TFacet>.Failure((b) => b.Add(nameof(_facets), new FacetDoesNotExistException("requested facet does not exist")));
    }

    /// <summary>
    /// Associates the specified facet with this object and returns the updated instance.
    /// </summary>
    /// <remarks>If a facet of the same type has already been associated, it will be replaced by the new
    /// value.</remarks>
    /// <param name="aspect">The facet to associate with this object. Cannot be null.</param>
    /// <returns>The current instance with the specified facet applied.</returns>
    public FacetObject<TClass, TFacet> Facet(TFacet aspect)
    {
        _facets[typeof(TFacet).Name] = aspect;
        return this;
    }

    public bool Has()
    {
        return _facets.ContainsKey(typeof(TFacet).Name);
    }
}

public class FacetObject
{
    private FacetDictionary _facets = [];

    public Result<TFacet> Facet<TFacet>() where TFacet : notnull, IFacet
    {
        if (_facets.TryGetValue(typeof(TFacet).Name, out var result))
        {
            return Result<TFacet>.Success((TFacet)(object)result);
        }

        return Result<TFacet>.Failure();
    }

    public FacetObject Facet<TFacet>(TFacet aspect) where TFacet : notnull, IFacet
    {
        _facets[typeof(TFacet).Name] = aspect;
        return this;
    }

    public bool Has<TFacet>() where TFacet : notnull, IFacet
    {
        return _facets.ContainsKey(typeof(TFacet).Name);
    }
}

public abstract class Facet
{
    private object _instance;

    public Facet(object ofInstance)
    {
        _instance = ofInstance;
    }

    public object OfInstance()
    {
        return _instance;
    }
}
