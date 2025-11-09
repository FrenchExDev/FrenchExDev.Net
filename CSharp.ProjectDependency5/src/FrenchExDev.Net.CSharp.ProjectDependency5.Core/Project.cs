using FrenchExDev.Net.CSharp.ManagedList;

namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core;

/// <summary>
/// Represents a project reference with name and path information.
/// </summary>
public class Project
{
    public string FilePath { get; init; }
    public Microsoft.Build.Evaluation.Project Msproj { get; }

    private Microsoft.CodeAnalysis.Project _code;

    public Project(Microsoft.CodeAnalysis.Project code, string path, Microsoft.Build.Evaluation.Project msproj)
    {
        _code = code;
        FilePath = path;
        Msproj = msproj;
    }

    /// <summary>
    /// Gets the Roslyn project associated with this instance.
    /// </summary>
    public Microsoft.CodeAnalysis.Project Code => _code;

    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public string Name => Code.Name;

    /// <summary>
    /// Gets or sets the path to the project file (.csproj).
    /// </summary>
    public string Path => Code.FilePath ?? string.Empty;

    private readonly OpenManagedList<IProjectDependency> _dependencies = new();
    public ICollection<IProjectDependency> Dependencies => _dependencies;
}

