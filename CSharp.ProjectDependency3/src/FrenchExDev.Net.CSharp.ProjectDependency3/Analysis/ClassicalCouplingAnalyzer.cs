namespace FrenchExDev.Net.CSharp.ProjectDependency3;

// Afferent (Ca) and Efferent (Ce) and Instability I = Ce/(Ca+Ce)
public class ClassicalCouplingAnalyzer : IProjectAnalyzer
{
 public string Name => "ClassicalCoupling";

 public Task<object> AnalyzeAsync(Solution solution, CancellationToken cancellationToken = default)
 {
 // Map project by path for resolution
 var byPath = solution.Projects.ToDictionary(p => p.Path, p => p, StringComparer.OrdinalIgnoreCase);
 var byName = solution.Projects.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

 // Build adjacency list using structural references
 var outgoing = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
 var incoming = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
 foreach (var p in solution.Projects)
 {
 var outSet = outgoing[p.Name] = new(StringComparer.OrdinalIgnoreCase);
 foreach (var item in p.Msproj.GetItems("ProjectReference"))
 {
 var full = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(p.Msproj.FullPath)!, item.EvaluatedInclude));
 if (byPath.TryGetValue(full, out var target))
 {
 outSet.Add(target.Name);
 if (!incoming.TryGetValue(target.Name, out var inSet))
 {
 inSet = new(StringComparer.OrdinalIgnoreCase);
 incoming[target.Name] = inSet;
 }
 inSet.Add(p.Name);
 }
 }
 if (!incoming.ContainsKey(p.Name)) incoming[p.Name] = new(StringComparer.OrdinalIgnoreCase);
 }

 var metrics = new Dictionary<string, (int Ca, int Ce, double Instability)>(StringComparer.OrdinalIgnoreCase);
 foreach (var p in solution.Projects)
 {
 var ce = outgoing.TryGetValue(p.Name, out var outs) ? outs.Count :0;
 var ca = incoming.TryGetValue(p.Name, out var ins) ? ins.Count :0;
 var denom = (ca + ce);
 var instability = denom ==0 ?0d : (double)ce / denom;
 metrics[p.Name] = (ca, ce, instability);
 }

 return Task.FromResult<object>(metrics);
 }
}
