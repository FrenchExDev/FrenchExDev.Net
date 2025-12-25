namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents a collection of builder instances that uses a factory function for creating new builders,
/// enabling dependency injection and custom builder instantiation scenarios.
/// </summary>
/// <typeparam name="TClass">The type of object that each builder in the list will construct. Must be a reference type.</typeparam>
/// <typeparam name="TBuilder">The type of builder used to construct <typeparamref name="TClass"/> instances. 
/// Must implement <see cref="IBuilder{TClass}"/>.</typeparam>
/// <remarks>
/// <para>
/// Unlike <see cref="BuilderList{TClass, TBuilder}"/> which requires a parameterless constructor,
/// <see cref="BuilderListWithFactory{TClass, TBuilder}"/> uses a factory function to create builder instances.
/// This enables:
/// </para>
/// <list type="bullet">
///   <item><description>Dependency injection - builders can be resolved from a DI container</description></item>
///   <item><description>Custom initialization - builders can be pre-configured by the factory</description></item>
///   <item><description>Builder pooling - factories can reuse builder instances</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Using with dependency injection
/// var list = new BuilderListWithFactory&lt;Person, PersonBuilder&gt;(
///     () => serviceProvider.GetRequiredService&lt;PersonBuilder&gt;());
/// 
/// list.New(b => b.WithName("Alice").WithAge(25));
/// list.New(b => b.WithName("Bob").WithAge(30));
/// 
/// var people = list.BuildSuccess();
/// </code>
/// </example>
/// <seealso cref="BuilderList{TClass, TBuilder}"/>
/// <seealso cref="BuilderListExtensions"/>
public class BuilderListWithFactory<TClass, TBuilder> : List<TBuilder>
    where TClass : class
    where TBuilder : IBuilder<TClass>
{
    private readonly Func<TBuilder> _builderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuilderListWithFactory{TClass, TBuilder}"/> class 
    /// using the specified factory function for creating builder instances.
    /// </summary>
    /// <param name="builderFactory">
    /// A function that creates new <typeparamref name="TBuilder"/> instances.
    /// This function is called each time <see cref="New"/> is invoked.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builderFactory"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// // Simple factory
    /// var list = new BuilderListWithFactory&lt;Person, PersonBuilder&gt;(() => new PersonBuilder());
    /// 
    /// // DI factory
    /// var list = new BuilderListWithFactory&lt;Person, PersonBuilder&gt;(
    ///     () => services.GetRequiredService&lt;PersonBuilder&gt;());
    /// </code>
    /// </example>
    public BuilderListWithFactory(Func<TBuilder> builderFactory)
    {
        _builderFactory = builderFactory ?? throw new ArgumentNullException(nameof(builderFactory));
    }

    /// <summary>
    /// Converts this builder list to a <see cref="ReferenceList{TClass}"/> containing references to each builder's target object.
    /// </summary>
    /// <returns>
    /// A new <see cref="ReferenceList{TClass}"/> containing <see cref="Reference{TClass}"/> objects for each builder.
    /// </returns>
    /// <remarks>
    /// The returned references will be unresolved until the corresponding builders are built.
    /// </remarks>
    public ReferenceList<TClass> AsReferenceList()
    {
        var references = this.Select(x => x.Reference());
        return new(references);
    }

    /// <summary>
    /// Creates a new builder instance using the factory, applies the specified configuration, and adds it to the list.
    /// </summary>
    /// <param name="body">An action that configures the newly created builder instance.</param>
    /// <returns>The current <see cref="BuilderListWithFactory{TClass, TBuilder}"/> instance for method chaining.</returns>
    /// <remarks>
    /// The factory function provided in the constructor is invoked to create each new builder instance.
    /// </remarks>
    /// <example>
    /// <code>
    /// list
    ///     .New(b => b.WithName("Alice").WithAge(25))
    ///     .New(b => b.WithName("Bob").WithAge(30))
    ///     .New(b => b.WithName("Charlie").WithAge(35));
    /// </code>
    /// </example>
    public BuilderListWithFactory<TClass, TBuilder> New(Action<TBuilder> body)
    {
        var builder = _builderFactory();
        body(builder);
        Add(builder);
        return this;
    }

    /// <summary>
    /// Builds all builders in the list and returns a list of successfully built instances.
    /// </summary>
    /// <returns>A list containing all successfully built <typeparamref name="TClass"/> instances.</returns>
    /// <exception cref="ReferenceNotResolvedException">
    /// Thrown if any builder fails validation and its reference cannot be resolved.
    /// </exception>
    public List<TClass> BuildSuccess() => [.. this.Select(x => x.Build().Value.Resolved())];

    /// <summary>
    /// Validates all builders in the list and returns failure collectors for builders that have validation errors.
    /// </summary>
    /// <returns>
    /// A list of <see cref="IFailureCollector"/> objects, one for each builder that has validation failures.
    /// Builders that pass validation are not included in the result.
    /// </returns>
    public List<IFailureCollector> ValidateFailures()
    {
        var visited = new VisitedObjectDictionary();

        return [.. this.Select(x =>
        {
            IFailureCollector failures = new FailuresDictionary();
            x.Validate(visited, failures);
            return failures;
        }).Where(f => f.HasFailures)];
    }
}
