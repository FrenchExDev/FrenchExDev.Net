namespace FrenchExDev.Net.CSharp.ProjectDependency3;

/// <summary>
/// Analyzes project dependencies to determine structural coupling between projects within a solution.
/// </summary>
/// <remarks>Structural coupling is assessed by examining project references, identifying which projects depend on
/// others. This analyzer can be used to visualize or quantify architectural dependencies, which may inform refactoring
/// or modularization efforts. The analysis is performed asynchronously and returns a mapping of each project to its
/// referenced projects.</remarks>
public class StructuralCouplingAnalyzer : IProjectAnalyzer
{
    public string Name => "StructuralCoupling";

    /// <summary>
    /// Analyzes the specified solution and returns a mapping of each project to its referenced projects.
    /// </summary>
    /// <param name="solution">The solution to analyze. Must contain one or more projects to produce meaningful results.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the analysis operation.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains a dictionary mapping project names to
    /// lists of referenced project paths.</returns>
    public Task<object> AnalyzeAsync(Solution solution, CancellationToken cancellationToken = default)
    {
        var perProject = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var proj in solution.Projects)
        {
            var refs = proj.Msproj.GetItems("ProjectReference")
            .Select(i => NormalizePath(proj.Msproj, i.EvaluatedInclude))
            .Where(p => p is not null)
            .Select(p => p!)
            .ToList();
            perProject[proj.Name] = refs;
        }
        return Task.FromResult<object>(perProject);
    }

    /// <summary>
    /// Resolves the specified relative path against the directory of the given MSBuild project and returns the absolute
    /// path.
    /// </summary>
    /// <remarks>This method returns null if the path resolution fails due to invalid input or file system
    /// errors. The returned path is not validated for existence.</remarks>
    /// <param name="msproj">The MSBuild project whose directory is used as the base for path resolution. Must not be null and must have a
    /// valid FullPath property.</param>
    /// <param name="include">The relative or absolute path to normalize. If relative, it is combined with the project's directory.</param>
    /// <returns>The absolute path corresponding to the specified include path, or null if the path cannot be resolved.</returns>
    private static string? NormalizePath(Microsoft.Build.Evaluation.Project msproj, string include)
    {
        try
        {
            var path = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(msproj.FullPath)!, include));
            return path;
        }
        catch
        {
            return null;
        }
    }
}
