using FrenchExDev.Net.CSharp.ManagedList;
using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;

namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Provides functionality for loading and managing C# projects using MSBuild within a Roslyn workspace.
/// </summary>
/// <remarks>This class encapsulates an MSBuild-based Roslyn workspace, enabling project loading and management
/// for code analysis scenarios. It implements <see cref="IDisposable"/> to ensure proper release of resources
/// associated with the underlying workspace. Thread safety is not guaranteed; callers should synchronize access if used
/// concurrently.</remarks>
public class MsBuildWorkspace : IMsBuildWorkspace, IDisposable
{
    private MSBuildWorkspace? _workspace;
    private readonly OpenManagedList<Microsoft.CodeAnalysis.Project> _projects;
    private readonly ILogger<IMsBuildWorkspace> _logger;

    public MSBuildWorkspace Workspace => _workspace ?? throw new InvalidOperationException();
    public ICollection<Microsoft.CodeAnalysis.Project> Projects => _projects;

    public IProjectCollection _projectCollection { get; }

    public MsBuildWorkspace(IProjectCollection projectCollection, ILogger<IMsBuildWorkspace> logger)
    {
        _logger = logger;
        _projects = new();
        _projectCollection = projectCollection;
    }

    /// <summary>
    /// Initializes the workspace if it has not already been created.
    /// </summary>
    /// <remarks>This method sets up the MSBuild workspace and attaches an error handler to log workspace
    /// failures. Call this method before performing any operations that require a workspace to ensure it is properly
    /// initialized. This method is safe to call multiple times; subsequent calls will have no effect if the workspace
    /// is already initialized.</remarks>
    public void Initialize()
    {
        _workspace = MSBuildWorkspace.Create();
        _workspace.WorkspaceFailed += (s, e) => _logger.LogError(e.Diagnostic.Message);

    }

    public void Dispose()
    {
        _workspace?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<Result<Project>> OpenProjectAsync(string csprojPath, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<Project>.Success(new Project(await Workspace.OpenProjectAsync(csprojPath, progress, cancellationToken), csprojPath, _projectCollection.LoadProject(csprojPath)));
        }
        catch (Exception ex)
        {
            return Result<Project>.Failure(ex);
        }
    }

    public async Task<Result<Solution>> OpenSolutionAsync(string solutionSln, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<Solution>.Success(new Solution(await Workspace.OpenSolutionAsync(solutionSln, progress, cancellationToken)));
        }
        catch (Exception ex)
        {
            return Result<Solution>.Failure(ex);
        }
    }
}
