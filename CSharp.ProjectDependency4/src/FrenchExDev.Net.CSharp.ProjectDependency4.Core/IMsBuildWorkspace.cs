using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.CodeAnalysis.MSBuild;

namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core;

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

