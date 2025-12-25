namespace FrenchExDev.Net.CSharp.Object.Builder2;

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
