namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents a list of builder instances that uses a factory for creating new builders,
/// enabling dependency injection scenarios.
/// </summary>
/// <typeparam name="TClass">The type of object to be built.</typeparam>
/// <typeparam name="TBuilder">The type of builder.</typeparam>
public class BuilderListWithFactory<TClass, TBuilder> : List<TBuilder>
    where TClass : class
    where TBuilder : IBuilder<TClass>
{
    private readonly Func<TBuilder> _builderFactory;

    /// <summary>
    /// Creates a new BuilderListWithFactory using the specified factory function.
    /// </summary>
    /// <param name="builderFactory">A function that creates new builder instances.</param>
    public BuilderListWithFactory(Func<TBuilder> builderFactory)
    {
        _builderFactory = builderFactory ?? throw new ArgumentNullException(nameof(builderFactory));
    }

    /// <summary>
    /// Creates a new reference list containing references to each item in the collection.
    /// </summary>
    public ReferenceList<TClass> AsReferenceList()
    {
        var references = this.Select(x => x.Reference());
        return new(references);
    }

    /// <summary>
    /// Creates a new builder instance using the factory, applies the specified configuration action, and adds the builder to the list.
    /// </summary>
    /// <param name="body">An action that configures the newly created builder instance.</param>
    /// <returns>The current list containing the newly added and configured builder instance.</returns>
    public BuilderListWithFactory<TClass, TBuilder> New(Action<TBuilder> body)
    {
        var builder = _builderFactory();
        body(builder);
        Add(builder);
        return this;
    }

    /// <summary>
    /// Builds and returns a list of successful results of type <typeparamref name="TClass"/>.
    /// </summary>
    public List<TClass> BuildSuccess() => [.. this.Select(x => x.Build().Value.Resolved())];

    /// <summary>
    /// Returns a list of failure collectors resulting from validating each builder in the collection.
    /// </summary>
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

/// <summary>
/// Provides extension methods for creating BuilderListWithFactory instances.
/// </summary>
public static class BuilderListExtensions
{
    /// <summary>
    /// Creates a new BuilderListWithFactory with the specified factory function.
    /// </summary>
    /// <typeparam name="TClass">The type of object to be built.</typeparam>
    /// <typeparam name="TBuilder">The type of builder.</typeparam>
    /// <param name="factory">A function that creates new builder instances.</param>
    /// <returns>A new BuilderListWithFactory instance.</returns>
    public static BuilderListWithFactory<TClass, TBuilder> CreateBuilderList<TClass, TBuilder>(Func<TBuilder> factory)
        where TClass : class
        where TBuilder : IBuilder<TClass>
    {
        return new BuilderListWithFactory<TClass, TBuilder>(factory);
    }
}
