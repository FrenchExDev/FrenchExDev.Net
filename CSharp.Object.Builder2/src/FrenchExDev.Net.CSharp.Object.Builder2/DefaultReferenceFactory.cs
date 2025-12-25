namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Default implementation of <see cref="IReferenceFactory"/> that creates standard <see cref="Reference{TClass}"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="DefaultReferenceFactory"/> is a singleton factory used by <see cref="AbstractBuilder{TClass}"/>
/// when no custom factory is provided. It creates plain <see cref="Reference{TClass}"/> instances without
/// any additional tracking or pooling.
/// </para>
/// <para>
/// For custom reference creation logic (e.g., tracking, pooling, or special initialization),
/// implement <see cref="IReferenceFactory"/> and pass it to the builder constructor.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Using the default factory explicitly
/// var reference = DefaultReferenceFactory.Instance.Create&lt;Person&gt;();
/// 
/// // Creating with an existing instance
/// var person = new Person { Name = "Alice" };
/// var resolvedRef = DefaultReferenceFactory.Instance.Create(person);
/// </code>
/// </example>
/// <seealso cref="IReferenceFactory"/>
/// <seealso cref="Reference{TClass}"/>
public class DefaultReferenceFactory : IReferenceFactory
{
    /// <summary>
    /// Gets the singleton instance of <see cref="DefaultReferenceFactory"/>.
    /// </summary>
    public static readonly DefaultReferenceFactory Instance = new();

    /// <summary>
    /// Creates a new unresolved <see cref="Reference{TClass}"/>.
    /// </summary>
    /// <typeparam name="TClass">The type of object to reference.</typeparam>
    /// <returns>A new unresolved reference.</returns>
    public Reference<TClass> Create<TClass>() where TClass : class => new();

    /// <summary>
    /// Creates a new <see cref="Reference{TClass}"/>, optionally pre-resolved with an existing instance.
    /// </summary>
    /// <typeparam name="TClass">The type of object to reference.</typeparam>
    /// <param name="existing">The existing instance to wrap, or <see langword="null"/> for an unresolved reference.</param>
    /// <returns>A new reference, resolved if <paramref name="existing"/> is not <see langword="null"/>.</returns>
    public Reference<TClass> Create<TClass>(TClass? existing) where TClass : class => new(existing);
}
