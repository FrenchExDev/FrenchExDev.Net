using Microsoft.Build.Evaluation;

namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core;

/// <summary>
/// Provides a default implementation of the <see cref="IProjectCollection"/> interface that uses the global MSBuild
/// project collection.
/// </summary>
/// <remarks>This class enables loading MSBuild projects using the shared global project collection. It is
/// suitable for scenarios where a single, application-wide project collection is desired. Thread safety and resource
/// management are handled by the underlying <see cref="ProjectCollection.GlobalProjectCollection"/>.</remarks>
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
