namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents a builder that can construct instances of type <typeparamref name="TClass"/>.
/// This is the primary interface for all builder implementations.
/// </summary>
/// <typeparam name="TClass">The type of object this builder constructs. Must be a reference type.</typeparam>
/// <remarks>
/// <para>
/// <see cref="IBuilder{TClass}"/> is a composite interface that combines:
/// </para>
/// <list type="bullet">
///   <item><description><see cref="IBuildable{TClass}"/> - ability to build objects</description></item>
///   <item><description><see cref="IValidatable"/> - ability to validate before building</description></item>
///   <item><description><see cref="IReferenceable{TClass}"/> - ability to provide references for deferred resolution</description></item>
///   <item><description><see cref="IIdentifiable"/> - unique identification for cycle detection</description></item>
/// </list>
/// <para>
/// Implement this interface by inheriting from <see cref="AbstractBuilder{TClass}"/> which provides
/// the standard implementation of all these capabilities.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public class PersonBuilder : AbstractBuilder&lt;Person&gt;
/// {
///     // Implementation
/// }
/// 
/// IBuilder&lt;Person&gt; builder = new PersonBuilder();
/// var result = builder.Build();
/// </code>
/// </example>
/// <seealso cref="AbstractBuilder{TClass}"/>
/// <seealso cref="IBuildable{TClass}"/>
/// <seealso cref="IValidatable"/>
public interface IBuilder<TClass> : IBuildable<TClass>, IValidatable, IReferenceable<TClass>, IIdentifiable
    where TClass : class
{
    /// <summary>
    /// Gets the built result value, or <see langword="null"/> if the build has not completed or failed.
    /// </summary>
    /// <value>The built <typeparamref name="TClass"/> instance, or <see langword="null"/>.</value>
    /// <exception cref="InvalidOperationException">May be thrown if accessed before building is complete.</exception>
    /// <remarks>
    /// For safer access, use the result returned by <see cref="IBuildable{TClass}.Build"/> or call
    /// <see cref="Reference{TClass}.Resolved"/> on the builder's reference.
    /// </remarks>
    TClass? Result { get; }
}
