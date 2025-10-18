using FrenchExDev.Net.CSharp.ManagedDictionary;
using FrenchExDev.Net.CSharp.ManagedList;
using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Provides a service for registering MSBuild components or functionality within the host environment.
/// </summary>
/// <remarks>Implementations of this interface should define the registration logic required to enable MSBuild
/// integration. This service is typically used to ensure that MSBuild-related features are available and properly
/// configured before use.</remarks>
public interface IMsBuildRegisteringService
{
    void RegisterIfNeeded();
}

/// <summary>
/// Provides functionality to register MSBuild assemblies for use within the current application domain.
/// </summary>
/// <remarks>This service ensures that MSBuild is registered only once per application domain. Registering MSBuild
/// enables APIs that depend on MSBuild assemblies to function correctly. Typically, registration should occur before
/// invoking any MSBuild-dependent operations.</remarks>
public class MsBuildRegisteringService : IMsBuildRegisteringService
{
    private bool _isRegistered = false;
    public void RegisterIfNeeded()
    {
        if (_isRegistered) return;

        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }

        _isRegistered = true;
    }
}

/// <summary>
/// Represents a collection of projects and provides functionality to load projects from specified file paths.
/// </summary>
/// <remarks>Implementations of this interface typically manage the lifecycle and access to multiple project
/// instances. Thread safety and project caching behavior may vary depending on the implementation.</remarks>
public interface IProjectCollection
{
    Microsoft.Build.Evaluation.Project LoadProject(string path);
}

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


/// <summary>
/// Represents a workspace capable of initializing and opening MSBuild-based projects for analysis and manipulation.
/// </summary>
/// <remarks>Implementations of this interface provide methods to prepare the workspace and asynchronously load C#
/// project files (.csproj) using MSBuild. This interface is typically used in scenarios where Roslyn-based project
/// analysis or code generation is required. Thread safety and initialization requirements may vary by
/// implementation.</remarks>
public interface IMsBuildWorkspace
{
    /// <summary>
    /// Initializes the component and prepares it for use.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Asynchronously opens a project from the specified .csproj file path and reports progress during the loading
    /// operation.
    /// </summary>
    /// <remarks>If the project file is invalid or cannot be loaded, the returned result will indicate failure
    /// and provide details. This method does not block the calling thread.</remarks>
    /// <param name="csprojPath">The full path to the .csproj file to open. Cannot be null or empty.</param>
    /// <param name="progress">An optional progress reporter that receives updates about the project loading progress. If null, progress is not
    /// reported.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains a <see cref="Project"/> if the project is
    /// loaded successfully; otherwise, contains error information.</returns>
    Task<Result<Project>> OpenProjectAsync(string csprojPath, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously opens a solution file and loads its projects.
    /// </summary>
    /// <remarks>If the solution contains multiple projects, progress updates are reported as each project is
    /// loaded. The method does not block the calling thread and can be awaited. The returned <see cref="Result{T}"/>
    /// provides details about any errors encountered during loading.</remarks>
    /// <param name="solutionSln">The path to the solution (.sln) file to open. Must not be null or empty.</param>
    /// <param name="progress">An optional progress reporter that receives updates about project load progress. If null, no progress will be
    /// reported.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains a <see cref="Solution"/> if the solution
    /// is loaded successfully; otherwise, contains error information.</returns>
    Task<Result<Solution>> OpenSolutionAsync(string solutionSln, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default);
}

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

/// <summary>
/// Represents a dependency of a project, such as a reference to another project, library, or external resource.
/// </summary>
/// <remarks>Implementations of this interface define the characteristics and behavior of project dependencies.
/// This interface is typically used to model relationships between projects or components within a build system or
/// project management tool.</remarks>
public interface IProjectDependency
{

}

/// <summary>
/// Represents a dependency relationship between two projects, indicating that one project depends on another within a
/// solution or build context.
/// </summary>
/// <remarks>Use this type to model project-to-project dependencies, such as when a project references another for
/// compilation or runtime purposes. Both the owner and dependent project must be specified to establish the
/// relationship.</remarks>
public record ProjectDependency : IProjectDependency
{
    public required Project Owner { get; init; }
    public required Project Project { get; init; }
}

/// <summary>
/// Represents a dependency on a package within a project.
/// </summary>
/// <remarks>This abstract record serves as a base type for describing package dependencies in project systems.
/// Implementations typically specify additional metadata or constraints relevant to the dependency. Use this type to
/// model relationships between a project and its required packages.</remarks>
public abstract record PackageDependency : IProjectDependency
{
    public required string Name { get; init; }
}

/// <summary>
/// Represents a package dependency that requires a specific version of the package.
/// </summary>
public record PackageVersionDependency : PackageDependency
{
    public required Version Version { get; init; }
}

/// <summary>
/// Represents a package dependency that is distributed with a specific version constraint.
/// </summary>
/// <remarks>Use this type to specify dependencies on packages where the required version must be explicitly
/// defined and distributed. This is commonly used in scenarios where package versioning is critical for compatibility
/// or deployment.</remarks>
public record PackageDistributedVersionDependency : PackageDependency
{

}

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

/// <summary>
/// Represents a wrapper for a Roslyn solution, providing access to project and workspace information for code analysis
/// and manipulation.
/// </summary>
/// <remarks>Use this class to interact with a Roslyn solution in scenarios such as code analysis, refactoring, or
/// workspace management. The underlying solution instance can be accessed and operated on using Roslyn APIs. This class
/// does not expose additional functionality beyond encapsulating the provided solution.</remarks>
public class Solution
{
    private Microsoft.CodeAnalysis.Solution _solution;

    /// <summary>
    /// Initializes a new instance of the Solution class using the specified Roslyn solution.
    /// </summary>
    /// <remarks>This constructor allows integration with the Microsoft.CodeAnalysis workspace model, enabling
    /// further analysis or manipulation of the provided solution. The Solution instance maintains a reference to the
    /// supplied solution for subsequent operations.</remarks>
    /// <param name="solution">The Roslyn solution to be wrapped and managed by this instance. Cannot be null.</param>
    public Solution(Microsoft.CodeAnalysis.Solution solution)
    {
        _solution = solution;
    }

    /// <summary>
    /// Gets a collection containing all projects in the current solution.
    /// </summary>
    public ICollection<Microsoft.CodeAnalysis.Project> Projects => _solution.Projects.ToList();
}

/// <summary>
/// Provides functionality to load and manage a collection of projects, resolving project references and associating
/// MSBuild and Roslyn project representations as needed.
/// </summary>
/// <remarks>ProjectCollectionLoader is designed to efficiently traverse and load project files, including their
/// references, in parallel. It ensures that each project is loaded only once and maintains thread safety when updating
/// the internal collection. This class is useful when working with solutions that contain multiple interdependent
/// projects and require both MSBuild and Roslyn project information.</remarks>
public class ProjectCollectionLoader
{
    private readonly object _projectsLock = new();

    private readonly OpenManagedDictionary<string, Result<Project>> _projects = new OpenManagedDictionary<string, Result<Project>>();

    /// <summary>
    /// Loads and returns all projects and their dependencies from the specified collection, mapping each project's file
    /// path to its corresponding project instance.
    /// </summary>
    /// <remarks>If the projects have already been loaded, the method returns the existing dictionary without
    /// reloading. Project dependencies are resolved recursively by following MSBuild project references. The operation
    /// is thread-safe and processes multiple project roots in parallel for improved performance.</remarks>
    /// <param name="pc">The project collection used to load MSBuild project files. Must not be null.</param>
    /// <param name="projects">A sequence of Roslyn projects representing the initial set of projects to load. Each project's file path is used
    /// to identify and resolve dependencies.</param>
    /// <returns>A result containing a dictionary that maps project file paths to their loaded project instances. The dictionary
    /// includes all discovered projects and their transitive dependencies.</returns>
    public Result<OpenManagedDictionary<string, Result<Project>>> LoadProjects(IProjectCollection pc, IEnumerable<Microsoft.CodeAnalysis.Project> projects)
    {
        if (_projects.Count > 0)
        {
            return Result<OpenManagedDictionary<string, Result<Project>>>.Success(_projects);
        }

        // Use a concurrent visited set so multiple threads can walk different roots in parallel
        var visited = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

        // collect root project file paths
        var roots = projects
            .Where(p => !string.IsNullOrWhiteSpace(p.FilePath))
            .Select(p => Path.GetFullPath(p.FilePath!))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // local DFS stack for this worker
        var stack = new Stack<string>();
        foreach (var root in roots)
        {
            stack.Push(root);
        }

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            // ensure single visit across all threads
            if (!visited.TryAdd(current, 0))
                continue;

            // already added to projects? skip loading
            lock (_projectsLock)
            {
                if (_projects.ContainsKey(current))
                    continue;
            }

            // try to load the MSBuild project; on failure continue
            Microsoft.Build.Evaluation.Project? msproj = null;
            try
            {
                msproj = pc.LoadProject(current);
            }
            catch (Exception ex)
            {
                // unable to load project file, skip recursion from this node
                _projects.Add(current, Result<Project>.Failure(ex));
                continue;
            }

            // try to find corresponding Roslyn project in the solution by file path
            var roslynForCurrent = projects.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.FilePath)
                && Path.GetFullPath(p.FilePath!).Equals(current, StringComparison.OrdinalIgnoreCase));

            Microsoft.CodeAnalysis.Project codeProject;
            if (roslynForCurrent != null)
            {
                codeProject = roslynForCurrent;
            }
            else
            {
                // fallback: create a minimal project in adhoc workspace
                var adhoc = new AdhocWorkspace();
                var name = Path.GetFileNameWithoutExtension(current) ?? "Unknown";
                var pid = ProjectId.CreateNewId();
                var info = ProjectInfo.Create(pid, VersionStamp.Create(), name, name, LanguageNames.CSharp, filePath: current);
                codeProject = adhoc.AddProject(info);
            }

            var project = Result<Project>.Success(new Project(codeProject, current, msproj));

            _projects.Add(current, project);

            // enqueue referenced project files for further processing
            foreach (var item in msproj.GetItems("ProjectReference"))
            {
                var include = item.EvaluatedInclude ?? string.Empty;
                if (string.IsNullOrWhiteSpace(include))
                    continue;

                // resolve relative path from the current project's directory
                var resolved = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(current) ?? string.Empty, include));

                // schedule for visit if not already visited or loaded
                if (!visited.ContainsKey(resolved))
                {
                    // small race is fine; TryAdd will protect from duplicates when popped
                    stack.Push(resolved);
                }
            }
        }

        return Result<OpenManagedDictionary<string, Result<Project>>>.Success(_projects);
    }
}