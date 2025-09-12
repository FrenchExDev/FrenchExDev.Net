namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents an exception that is thrown when an error occurs during asynchronous object construction using a
/// specified builder.
/// </summary>
/// <remarks>This exception provides access to the member name where the error occurred, the builder instance
/// involved in the construction, and the set of visited objects during the build process. It is typically thrown to
/// indicate a failure in the asynchronous object building workflow, allowing callers to inspect relevant context for
/// diagnostics.</remarks>
/// <typeparam name="TClass">The type of object being constructed asynchronously.</typeparam>
/// <typeparam name="TBuilder">The type of builder used to construct the object asynchronously. Must implement <see
/// cref="IAsyncObjectBuilder{TClass}"/>.</typeparam>
public class BasicAsyncObjectBuildException<TClass, TBuilder> : Exception where TBuilder : IAsyncObjectBuilder<TClass>
{
    private Dictionary<object, object>? _visited;
    private readonly MemberName _memberName;
    private TBuilder? _builder;

    public MemberName MemberName => _memberName;
    public Dictionary<object, object> Visited => _visited ?? throw new InvalidOperationException("Visited is not set.");

    public TBuilder Builder => _builder ?? throw new InvalidOperationException("Builder is not set.");

    public BasicAsyncObjectBuildException(MemberName memberName, string message, TBuilder builder, VisitedObjectsList? visited = null) : base(message)
    {
        _memberName = memberName;
        _builder = builder;
        _visited = visited;
    }
}