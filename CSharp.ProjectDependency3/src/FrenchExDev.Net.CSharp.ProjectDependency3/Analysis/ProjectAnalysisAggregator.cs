namespace FrenchExDev.Net.CSharp.ProjectDependency3;

public class ProjectAnalysisAggregator
{
 private readonly List<IProjectAnalyzer> _analyzers = new();

 public ProjectAnalysisAggregator Add(IProjectAnalyzer analyzer)
 {
 _analyzers.Add(analyzer);
 return this;
 }

 public async Task<Dictionary<string, object>> RunAsync(Solution solution, CancellationToken ct = default)
 {
 var results = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
 foreach (var analyzer in _analyzers)
 {
 results[analyzer.Name] = await analyzer.AnalyzeAsync(solution, ct);
 }
 return results;
 }
}
