using FrenchExDev.Net.CSharp.ManagedList;
using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

/// <summary>
/// Defines a service for asynchronously loading a solution from a specified .sln file.
/// </summary>
/// <remarks>Implementations of this interface may support reporting project load progress and cancellation. The
/// returned result indicates whether the solution was loaded successfully, and provides details about any errors
/// encountered during the process.</remarks>
public interface ISolutionLoader
{
    Task<Result<Solution>> OpenSolutionAsync(string solutionSln, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Loads a <see cref="Solution"/>.
/// </summary>
public class SolutionLoader : ISolutionLoader
{
    private readonly IMsBuildRegisteringService _msBuildRegisteringService;
    private readonly IMsBuildWorkspace _msBuildWorkspace;

    public SolutionLoader(
        IMsBuildRegisteringService msBuildRegisteringService,
        IMsBuildWorkspace msBuildWorkspace
    )
    {
        _msBuildRegisteringService = msBuildRegisteringService;
        _msBuildWorkspace = msBuildWorkspace;
    }

    public async Task<Result<Solution>> OpenSolutionAsync(string solutionSln, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        _msBuildRegisteringService.Register();
        _msBuildWorkspace.Initialize();

        var solution = await _msBuildWorkspace.OpenSolutionAsync(solutionSln, progress, cancellationToken);

        return solution;
    }
}

public record PackageReference(string Name, string? Version = null);

public record ProjectReference(Project Owner, Project Project);

// ProjectAnalysis extended to include reference coupling and construct metrics
public record ProjectAnalysis(
    string Name,
    string FilePath,
    IReadOnlyCollection<PackageReference> PackageReferences,
    IReadOnlyCollection<ProjectReference> ProjectReferences,
    IReadOnlyCollection<ReferenceCoupling>? ReferenceCouplings = null,
    ProjectConstructMetrics? Constructs = null
);

public interface IProjectCollection
{
    Microsoft.Build.Evaluation.Project LoadProject(string path);
}

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
/// Provides static methods for loading C# project files (.csproj) into Roslyn project representations or extracting
/// minimal project information.
/// </summary>
/// <remarks>ProjectLoader enables integration with Roslyn by loading .csproj files using MSBuildWorkspace, which
/// requires the Microsoft.CodeAnalysis.Workspaces.MSBuild package at runtime. It also offers a fallback mechanism to
/// extract basic project details if Roslyn loading fails. All methods are static and thread-safe. This class is
/// intended for scenarios where programmatic access to project metadata or compilation is required.</remarks>
public class ProjectLoader
{
    /// <summary>
    /// Asynchronously loads a Roslyn project from the specified .csproj file using the provided MSBuild workspace.
    /// </summary>
    /// <remarks>If the project fails to load, the returned result will contain error information describing
    /// the failure. This method does not throw on load errors; instead, errors are captured in the result
    /// object.</remarks>
    /// <param name="workspace">The MSBuild workspace used to open and load the project.</param>
    /// <param name="csprojPath">The full path to the .csproj file representing the project to load. Cannot be null, empty, or whitespace.</param>
    /// <param name="progress">An optional progress reporter that receives updates about the project load process. If null, no progress will be
    /// reported.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the project load operation.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains the loaded project if successful;
    /// otherwise, a failure result with error details.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="csprojPath"/> is null, empty, or consists only of whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file specified by <paramref name="csprojPath"/> does not exist.</exception>
    public async Task<Result<Abstractions.Project>> LoadRoslynProjectAsync(IMsBuildWorkspace workspace, string csprojPath, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(csprojPath)) throw new System.ArgumentNullException(nameof(csprojPath));
        if (!File.Exists(csprojPath)) throw new FileNotFoundException("csproj file not found", csprojPath);

        try
        {
            var roslynProject = await workspace.OpenProjectAsync(csprojPath, progress, cancellationToken).ConfigureAwait(false);

            if (roslynProject.IsFailure)
            {
                return Result<Abstractions.Project>.Failure(roslynProject.FailuresOrThrow());
            }

            return Result<Abstractions.Project>.Success(roslynProject.ObjectOrThrow());
        }
        catch (Exception ex)
        {
            return Result<Abstractions.Project>.Failure((d) => d.Add("Exception", ex));
        }
    }
}

/// <summary>
/// Provides a service for registering MSBuild components or functionality within the host environment.
/// </summary>
/// <remarks>Implementations of this interface should define the registration logic required to enable MSBuild
/// integration. This service is typically used to ensure that MSBuild-related features are available and properly
/// configured before use.</remarks>
public interface IMsBuildRegisteringService
{
    void Register();
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

    private readonly OpenManagedList<IDependency> _dependencies = new();
    public ICollection<IDependency> Dependencies => _dependencies;
}

public interface IDependency
{

}

public record ProjectDependency : IDependency
{
    public required Project Owner { get; init; }
    public required Project Project { get; init; }
}

public abstract record PackageDependency : IDependency
{
    public required string Name { get; init; }
}

public record PackageVersionDependency : PackageDependency
{
    public required Version Version { get; init; }
}

public record PackageDistributedVersionDependency : PackageDependency
{

}