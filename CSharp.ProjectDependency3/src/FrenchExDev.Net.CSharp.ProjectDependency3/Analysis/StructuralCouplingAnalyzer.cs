using Microsoft.Build.Evaluation;
using FrenchExDev.Net.CSharp.ProjectDependency3.Analysis;

namespace FrenchExDev.Net.CSharp.ProjectDependency3;

// Structural coupling: graph of direct project references
public class StructuralCouplingAnalyzer : IProjectAnalyzer
{
    public string Name => "StructuralCoupling";

    public Task<IProjectAnalysisResult> AnalyzeAsync(Solution solution, CancellationToken cancellationToken = default)
    {
        var perProject = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var solutionDir = System.IO.Path.GetDirectoryName(solution.Code.FilePath ?? string.Empty) ?? System.Environment.CurrentDirectory;
        foreach (var proj in solution.Projects)
        {
            var refs = proj.Msproj.GetItems("ProjectReference")
            .Select(i => MakeRelativeProjectPath(proj.Msproj, i.EvaluatedInclude, solutionDir))
            .Where(p => p is not null)
            .Select(p => p!)
            .ToList();
            perProject[proj.Name] = refs;
        }
        IProjectAnalysisResult res = new StructuralCouplingResult(Name, perProject.ToDictionary(kv => kv.Key, kv => (IReadOnlyList<string>)kv.Value));
        return Task.FromResult(res);
    }

    private static string? MakeRelativeProjectPath(Microsoft.Build.Evaluation.Project msproj, string include, string solutionDir)
    {
        try
        {
            var full = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(msproj.FullPath)!, include));
            var rel = System.IO.Path.GetRelativePath(solutionDir, full);
            return rel;
        }
        catch
        {
            return null;
        }
    }
}
