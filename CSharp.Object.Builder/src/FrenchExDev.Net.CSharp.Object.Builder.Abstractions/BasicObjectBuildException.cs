namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents an exception that occurs during a build operation, providing context about the builder and visited
/// objects.
/// </summary>
/// <remarks>This exception is typically used to capture and propagate errors that occur during a build process.
/// It provides access to the builder instance and an optional dictionary of visited objects for additional
/// context.</remarks>
/// <typeparam name="TBuilder">The type of the builder associated with the exception.</typeparam>
public class BasicObjectBuildException<TClass, TBuilder> : Exception where TBuilder : IObjectBuilder<TClass>
{
    private Dictionary<object, object>? _visited;

    private TBuilder? _builder;

    public Dictionary<object, object> Visited => _visited ?? throw new InvalidOperationException("Visited is not set.");

    public TBuilder Builder => _builder ?? throw new InvalidOperationException("Builder is not set.");

    public BasicObjectBuildException(string message, TBuilder builder, VisitedObjectsList? visited = null) : base(message)
    {
        _builder = builder;
        _visited = visited;
    }
}
