namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for creating <see cref="Reference{TClass}"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IReferenceFactory"/> follows the Factory pattern and allows customization
/// of how references are created. This is useful for:
/// </para>
/// <list type="bullet">
///   <item><description>Dependency injection scenarios</description></item>
///   <item><description>Custom reference tracking or pooling</description></item>
///   <item><description>Testing with mock references</description></item>
/// </list>
/// </remarks>
/// <seealso cref="DefaultReferenceFactory"/>
/// <seealso cref="Reference{TClass}"/>
public interface IReferenceFactory
{
    /// <summary>
    /// Creates a new unresolved reference.
    /// </summary>
    /// <typeparam name="TClass">The type of object to reference.</typeparam>
    /// <returns>A new unresolved <see cref="Reference{TClass}"/>.</returns>
    Reference<TClass> Create<TClass>() where TClass : class;

    /// <summary>
    /// Creates a new reference, optionally pre-resolved with an existing instance.
    /// </summary>
    /// <typeparam name="TClass">The type of object to reference.</typeparam>
    /// <param name="existing">The existing instance to wrap, or <see langword="null"/> for an unresolved reference.</param>
    /// <returns>A new <see cref="Reference{TClass}"/>, resolved if <paramref name="existing"/> is not <see langword="null"/>.</returns>
    Reference<TClass> Create<TClass>(TClass? existing) where TClass : class;
}
