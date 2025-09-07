namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TClass"></typeparam>
/// <typeparam name="TBuilder"></typeparam>
public class BasicAsyncObjectBuildException<TClass, TBuilder> : Exception where TBuilder : IAsyncObjectBuilder<TClass>
{
    private Dictionary<object, object>? _visited;

    private TBuilder? _builder;

    public Dictionary<object, object> Visited => _visited ?? throw new InvalidOperationException("Visited is not set.");

    public TBuilder Builder => _builder ?? throw new InvalidOperationException("Builder is not set.");

    public BasicAsyncObjectBuildException(string message, TBuilder builder, VisitedObjectsList? visited = null) : base(message)
    {
        _builder = builder;
        _visited = visited;
    }
}