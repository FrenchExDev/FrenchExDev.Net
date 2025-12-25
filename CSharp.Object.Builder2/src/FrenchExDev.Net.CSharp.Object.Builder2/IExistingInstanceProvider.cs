namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for builders that can be initialized with an existing instance,
/// bypassing the normal build process.
/// </summary>
/// <typeparam name="TClass">The type of the existing instance.</typeparam>
/// <remarks>
/// <para>
/// <see cref="IExistingInstanceProvider{TClass}"/> is implemented by builders that support
/// the "existing instance" pattern, where a pre-built object is wrapped by the builder
/// rather than being constructed through the normal build process.
/// </para>
/// <para>
/// When an existing instance is set, the builder's <c>Build</c> method should:
/// </para>
/// <list type="bullet">
///   <item><description>Skip validation</description></item>
///   <item><description>Skip the normal instantiation process</description></item>
///   <item><description>Resolve the reference with the existing instance</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// var existingPerson = LoadFromDatabase(id);
/// var builder = new PersonBuilder();
/// 
/// if (builder is IExistingInstanceProvider&lt;Person&gt; provider)
/// {
///     // Set the existing instance
///     ((AbstractBuilder&lt;Person&gt;)builder).Existing(existingPerson);
///     
///     if (provider.HasExisting)
///     {
///         Console.WriteLine($"Using existing: {provider.ExistingInstance?.Name}");
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="AbstractBuilder{TClass}"/>
public interface IExistingInstanceProvider<TClass> where TClass : class
{
    /// <summary>
    /// Gets a value indicating whether an existing instance has been set.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if <see cref="ExistingInstance"/> contains a valid instance;
    /// otherwise, <see langword="false"/>.
    /// </value>
    bool HasExisting { get; }

    /// <summary>
    /// Gets the existing instance that was set on this builder, if any.
    /// </summary>
    /// <value>
    /// The existing <typeparamref name="TClass"/> instance, or <see langword="null"/> if none was set.
    /// </value>
    TClass? ExistingInstance { get; }
}
