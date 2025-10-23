using Microsoft.Build.Evaluation;

namespace FrenchExDev.Net.CSharp.ProjectDependency3;

// Structural coupling: graph of direct project references
public class StructuralCouplingAnalyzer : IProjectAnalyzer
{
 public string Name => "StructuralCoupling";

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
