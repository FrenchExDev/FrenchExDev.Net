using Microsoft.Build.Evaluation;

namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Provides a default implementation of the <see cref="IProjectCollection"/> interface that uses the global MSBuild
/// project collection.
/// </summary>
/// <remarks>This class wraps the <see cref="ProjectCollection.GlobalProjectCollection"/> to facilitate loading
/// projects using the global collection. It is suitable for scenarios where a shared project collection is preferred,
/// such as in single-process applications or tools that do not require isolated project collections.</remarks>
public class DefaultProjectCollection : IProjectCollection
{
    private readonly ProjectCollection _pc;
    public DefaultProjectCollection()
    {
        _pc = ProjectCollection.GlobalProjectCollection;
    }
    public Microsoft.Build.Evaluation.Project LoadProject(string path)
    {
        return _pc.LoadProject(path);
    }
}
