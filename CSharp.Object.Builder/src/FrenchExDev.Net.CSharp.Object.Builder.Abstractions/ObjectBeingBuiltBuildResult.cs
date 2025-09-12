namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

public class ObjectBeingBuiltBuildResult<TClass> : IObjectBuildResult<TClass>
{
    public TClass ObjectBeingBuilt { get; init; }
    public ExceptionBuildDictionary Exceptions { get; init; }
    public VisitedObjectsList Visited { get; init; }
    public ObjectBeingBuiltBuildResult(TClass objectBeingBuilt, ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        ObjectBeingBuilt = objectBeingBuilt;
        Exceptions = exceptions;
        Visited = visited;
    }
}