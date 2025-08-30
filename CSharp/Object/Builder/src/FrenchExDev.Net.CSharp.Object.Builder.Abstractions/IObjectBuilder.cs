namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

public interface IObjectBuilder<TClass>
{
    IObjectBuildResult<TClass> Build(VisitedObjectsList? visited = null);
}
