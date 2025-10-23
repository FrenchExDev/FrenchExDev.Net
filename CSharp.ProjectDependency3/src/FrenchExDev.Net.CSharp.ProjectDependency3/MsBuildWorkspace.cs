using FrenchExDev.Net.CSharp.ManagedList;
using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;

namespace FrenchExDev.Net.CSharp.ProjectDependency3;

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

    /// <summary>
    /// Gets the project collection associated with the current context.
    /// </summary>
    protected IProjectCollection _projectCollection { get; }

    /// <summary>
    /// Gets the current MSBuild workspace instance used for project and solution operations.
    /// </summary>
    /// <remarks>Accessing this property when the workspace has not been initialized will result in an
    /// exception. The workspace provides APIs for loading, analyzing, and manipulating MSBuild-based projects and
    /// solutions.</remarks>
    public MSBuildWorkspace Workspace => _workspace ?? throw new InvalidOperationException();

    /// <summary>
    /// Gets the collection of projects contained in this instance.
    /// </summary>
    /// <remarks>The returned collection provides access to all projects currently managed by this object.
    /// Modifications to the collection may not affect the underlying data unless explicitly supported by the
    /// implementation.</remarks>
    public ICollection<Microsoft.CodeAnalysis.Project> Projects => _projects;


    /// <summary>
    /// Initializes a new instance of the MsBuildWorkspace class using the specified project collection and logger.
    /// </summary>
    /// <param name="projectCollection">The project collection that manages loaded MSBuild projects and global build settings for this workspace. Cannot
    /// be null.</param>
    /// <param name="logger">The logger used to record diagnostic and operational messages for the workspace. Cannot be null.</param>
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

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>Call this method when you are finished using the object to free unmanaged resources and
    /// perform other cleanup operations. After calling Dispose, the object should not be used further.</remarks>
    public void Dispose()
    {
        _workspace?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously opens a project from the specified .csproj file path and returns the result.
    /// </summary>
    /// <remarks>If the project fails to open, the returned result will contain the exception that occurred.
    /// The method wraps any exception in the result rather than throwing it directly.</remarks>
    /// <param name="csprojPath">The full path to the .csproj file to open. Must not be null or empty.</param>
    /// <param name="progress">An optional progress reporter that receives updates about the project load progress. If null, no progress will
    /// be reported.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A result containing the loaded project if successful; otherwise, a result containing the encountered exception.</returns>
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

    /// <summary>
    /// Asynchronously opens a solution file and returns the loaded solution wrapped in a result object.
    /// </summary>
    /// <remarks>If the solution fails to load, the returned result will contain the exception. The method
    /// does not throw on failure, allowing callers to handle errors via the result object. Progress updates are
    /// reported if a progress reporter is provided.</remarks>
    /// <param name="solutionSln">The full path to the solution (.sln) file to open. Must not be null or empty.</param>
    /// <param name="progress">An optional progress reporter that receives updates about project load progress. Can be null if progress
    /// reporting is not required.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A result containing the loaded solution if successful; otherwise, a result containing the exception encountered
    /// during loading.</returns>
    public async Task<Result<Solution>> OpenSolutionAsync(string solutionSln, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var roslynSolution = await Workspace.OpenSolutionAsync(solutionSln, progress, cancellationToken);
            var sol = new Solution(roslynSolution);
            foreach (var p in roslynSolution.Projects)
            {
                // Load matching MSBuild project to pair with Roslyn project
                var csprojPath = p.FilePath ?? string.Empty;
                if (string.IsNullOrWhiteSpace(csprojPath)) continue;
                var msproj = _projectCollection.LoadProject(csprojPath);
                sol.AddProject(new Project(p, csprojPath, msproj));
            }
            return Result<Solution>.Success(sol);
        }
        catch (Exception ex)
        {
            return Result<Solution>.Failure(ex);
        }
    }
}
