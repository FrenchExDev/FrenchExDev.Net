using FrenchExDev.Net.CSharp.ManagedList;
using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace FrenchExDev.Net.CSharp.ProjectDependency;

/// <summary>
/// Provides functionality to register MSBuild assemblies for use within the current application domain.
/// </summary>
/// <remarks>This service ensures that MSBuild is registered only once per application domain. Registering MSBuild
/// enables APIs that depend on MSBuild assemblies to function correctly. Typically, registration should occur before
/// invoking any MSBuild-dependent operations.</remarks>
public class MsBuildRegisteringService : IMsBuildRegisteringService
{
    private bool _isRegistered = false;
    public void Register()
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

    public MSBuildWorkspace Workspace => _workspace ?? throw new InvalidOperationException();
    public ICollection<Microsoft.CodeAnalysis.Project> Projects => _projects;

    public MsBuildWorkspace()
    {
        _projects = new();
    }

    public void Initialize()
    {
        _workspace = MSBuildWorkspace.Create();
        _workspace.WorkspaceFailed += (s, e) => Console.WriteLine($"Workspace failed: {e.Diagnostic.Message}");

    }

    public void Dispose()
    {
        _workspace?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<Result<Abstractions.Project>> OpenProjectAsync(string csprojPath, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<Abstractions.Project>.Success(new Abstractions.Project(await Workspace.OpenProjectAsync(csprojPath, progress, cancellationToken), csprojPath));
        }
        catch (Exception ex)
        {
            return Result<Abstractions.Project>.Failure(d => d.Add("Exception", ex));
        }
    }

    public async Task<Result<Abstractions.Solution>> OpenSolutionAsync(string solutionSln, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<Abstractions.Solution>.Success(new Abstractions.Solution(await Workspace.OpenSolutionAsync(solutionSln, progress, cancellationToken)));
        }
        catch (Exception ex)
        {
            return Result<Abstractions.Solution>.Failure(d => d.Add("Exception", ex));
        }
    }
}