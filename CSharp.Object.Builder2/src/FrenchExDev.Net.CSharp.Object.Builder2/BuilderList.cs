namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents a collection of builder instances that can be configured, validated, and built as a group.
/// </summary>
/// <typeparam name="TClass">The type of object that each builder in the list will construct. Must be a reference type.</typeparam>
/// <typeparam name="TBuilder">The type of builder used to construct <typeparamref name="TClass"/> instances. 
/// Must implement <see cref="IBuilder{TClass}"/> and have a parameterless constructor.</typeparam>
/// <remarks>
/// <para>
/// <see cref="BuilderList{TClass, TBuilder}"/> extends <see cref="List{T}"/> to provide specialized functionality 
/// for managing collections of builders in the Builder pattern implementation.
/// </para>
/// <para>
/// This class is particularly useful when building object graphs that contain one-to-many relationships,
/// such as a <c>Person</c> with multiple <c>Friends</c> or a <c>Department</c> with multiple <c>Employees</c>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Define a list of person builders
/// public BuilderList&lt;Person, PersonBuilder&gt; Friends { get; } = [];
/// 
/// // Add configured builders using the fluent API
/// builder.Friends
///     .New(f => f.WithName("Alice").WithAge(25))
///     .New(f => f.WithName("Bob").WithAge(30));
/// 
/// // Build all and get the instances
/// var friends = builder.Friends.BuildSuccess();
/// </code>
/// </example>
/// <seealso cref="BuilderListWithFactory{TClass, TBuilder}"/>
/// <seealso cref="ReferenceList{TClass}"/>
/// <seealso cref="IBuilder{TClass}"/>
public class BuilderList<TClass, TBuilder> : List<TBuilder>
    where TClass : class
    where TBuilder : IBuilder<TClass>, new()
{
    /// <summary>
    /// Converts this builder list to a <see cref="ReferenceList{TClass}"/> containing references to each builder's target object.
    /// </summary>
    /// <returns>
    /// A new <see cref="ReferenceList{TClass}"/> containing <see cref="Reference{TClass}"/> objects for each builder.
    /// The references will be unresolved until the corresponding builders are built.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is typically used in the <c>Instantiate()</c> method of a parent builder to collect
    /// references to child objects that may not yet be built.
    /// </para>
    /// <para>
    /// Use <see cref="ReferenceList{TClass}.AsEnumerable()"/> to iterate over resolved instances only.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override Person Instantiate() => new()
    /// {
    ///     Name = Name!,
    ///     Friends = [.. Friends.AsReferenceList().AsEnumerable()]
    /// };
    /// </code>
    /// </example>
    public ReferenceList<TClass> AsReferenceList() => new(this.Select(x => x.Reference()));

    /// <summary>
    /// Creates a new builder instance, applies the specified configuration, and adds it to the list.
    /// </summary>
    /// <param name="body">An action that configures the newly created builder instance.</param>
    /// <returns>The current <see cref="BuilderList{TClass, TBuilder}"/> instance for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method uses the parameterless constructor of <typeparamref name="TBuilder"/> to create new instances.
    /// For dependency injection scenarios where builders require constructor parameters, 
    /// use <see cref="BuilderListWithFactory{TClass, TBuilder}"/> instead.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builders = new BuilderList&lt;Person, PersonBuilder&gt;();
    /// builders
    ///     .New(p => p.WithName("Alice").WithAge(25))
    ///     .New(p => p.WithName("Bob").WithAge(30))
    ///     .New(p => p.WithName("Charlie").WithAge(35));
    /// </code>
    /// </example>
    public BuilderList<TClass, TBuilder> New(Action<TBuilder> body)
    {
        var builder = new TBuilder(); 
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
    /// <remarks>
    /// <para>
    /// This method calls <see cref="IBuilder{TClass}.Build"/> on each builder and resolves the resulting references.
    /// If any builder fails validation, accessing its resolved instance will throw an exception.
    /// </para>
    /// <para>
    /// For scenarios where you need to handle validation failures gracefully, 
    /// use <see cref="ValidateFailures"/> first to check for errors.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builders = new BuilderList&lt;Person, PersonBuilder&gt;();
    /// builders.New(p => p.WithName("Alice").WithAge(25));
    /// builders.New(p => p.WithName("Bob").WithAge(30));
    /// 
    /// List&lt;Person&gt; people = builders.BuildSuccess();
    /// // people contains two Person instances
    /// </code>
    /// </example>
    public List<TClass> BuildSuccess() => [.. this.Select(x => x.Build().Value.Resolved())];

    /// <summary>
    /// Validates all builders in the list and returns a list of failure dictionaries for builders that have validation errors.
    /// </summary>
    /// <returns>
    /// A list of <see cref="FailuresDictionary"/> objects, one for each builder that has validation failures.
    /// Builders that pass validation are not included in the result.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is useful for validating all builders before attempting to build them,
    /// allowing you to collect and report all validation errors at once.
    /// </para>
    /// <para>
    /// A shared <see cref="VisitedObjectDictionary"/> is used across all validations to handle circular references.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builders = new BuilderList&lt;Person, PersonBuilder&gt;();
    /// builders.New(p => p.WithAge(25)); // Missing name - will fail
    /// builders.New(p => p.WithName("Bob").WithAge(30)); // Valid
    /// builders.New(p => p.WithAge(-5)); // Missing name and negative age - will fail
    /// 
    /// var failures = builders.ValidateFailures();
    /// // failures.Count == 2 (two builders have validation errors)
    /// </code>
    /// </example>
    public List<FailuresDictionary> ValidateFailures()
    {
        var visited = new VisitedObjectDictionary();
        return [.. this.Select(x => {
            var failures = new FailuresDictionary();
            x.Validate(visited, failures);
            return failures;
        }).Where(f => f.HasFailures)];
    }
}
