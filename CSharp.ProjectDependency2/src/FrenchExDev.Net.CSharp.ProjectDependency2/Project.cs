using FrenchExDev.Net.CSharp.ManagedList;

namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Represents a project that encapsulates both Roslyn and MSBuild project information, including source code, project
/// file path, and dependencies.
/// </summary>
/// <remarks>The Project class provides access to both the code analysis model via Roslyn and the build
/// configuration via MSBuild. It exposes key project metadata and dependencies, allowing consumers to interact with
/// project files and related components in a unified manner. This type is typically used in scenarios where both code
/// analysis and build system integration are required.</remarks>
public class Project
{
    private Microsoft.CodeAnalysis.Project _code;

    private readonly OpenManagedList<IProjectDependency> _dependencies = new();

    /// <summary>
    /// Gets the full path to the file associated with this instance.
    /// </summary>
    public string FilePath { get; init; }
    public Microsoft.Build.Evaluation.Project Msproj { get; }

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

    /// <summary>
    /// Gets the collection of project dependencies associated with this project.
    /// </summary>
    /// <remarks>The returned collection contains all direct dependencies that this project references.
    /// Modifications to the collection may affect the project's dependency graph.</remarks>
    public ICollection<IProjectDependency> Dependencies => _dependencies;

    /// <summary>
    /// Initializes a new instance of the Project class using the specified Roslyn project, file path, and MSBuild
    /// project.
    /// </summary>
    /// <param name="code">The Roslyn project representing the code model for this project. Cannot be null.</param>
    /// <param name="path">The file system path to the project file. Cannot be null or empty.</param>
    /// <param name="msproj">The MSBuild project instance associated with this project. Cannot be null.</param>
    public Project(Microsoft.CodeAnalysis.Project code, string path, Microsoft.Build.Evaluation.Project msproj)
    {
        _code = code;
        FilePath = path;
        Msproj = msproj;
    }
}
