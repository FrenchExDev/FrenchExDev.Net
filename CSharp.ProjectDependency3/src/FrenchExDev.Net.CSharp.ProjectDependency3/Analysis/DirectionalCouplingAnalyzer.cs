using Microsoft.CodeAnalysis;

namespace FrenchExDev.Net.CSharp.ProjectDependency3;

// Directional coupling intensity: per A->B, count used public types and members from B used in A
public class DirectionalCouplingAnalyzer : IProjectAnalyzer
{
 public string Name => "DirectionalCoupling";

 public async Task<object> AnalyzeAsync(Solution solution, CancellationToken cancellationToken = default)
 {
 var result = new Dictionary<string, Dictionary<string, (int uniqueTypes, int memberUses)>>(StringComparer.OrdinalIgnoreCase);
 var roslynSolution = solution.Code;
 var projects = roslynSolution.Projects.ToList();

 foreach (var pa in projects)
 {
 var compA = await pa.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
 if (compA is null) continue;
 var aEntry = result[pa.Name] = new(StringComparer.OrdinalIgnoreCase);
 var projectAssemblyNames = projects.ToDictionary(p => p.Id, p => p.AssemblyName, EqualityComparer<ProjectId>.Default);
 var idByName = projects.ToDictionary(p => p.AssemblyName ?? p.Name, p => p.Id, StringComparer.OrdinalIgnoreCase);
 var trees = compA.SyntaxTrees.ToList();
 foreach (var pb in projects)
 {
 if (pb.Id == pa.Id) continue;
 // quick structural filter: A references B assembly?
 var hasRef = pa.ProjectReferences.Any(r => r.ProjectId == pb.Id)
 || pa.MetadataReferences.Any(m => (m.Display ?? string.Empty).Contains(pb.AssemblyName ?? pb.Name, StringComparison.OrdinalIgnoreCase));
 if (!hasRef) continue;
 var targetAsm = pb.AssemblyName ?? pb.Name;
 var types = new HashSet<string>(StringComparer.Ordinal);
 var memberUses =0;
 foreach (var tree in trees)
 {
 var model = compA.GetSemanticModel(tree);
 var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);
 foreach (var node in root.DescendantNodes())
 {
 if (cancellationToken.IsCancellationRequested) break;
 ISymbol? sym = node switch
 {
 Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax inv => model.GetSymbolInfo(inv.Expression, cancellationToken).Symbol,
 Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax oc => model.GetSymbolInfo(oc.Type, cancellationToken).Symbol,
 Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax at => model.GetSymbolInfo(at, cancellationToken).Symbol,
 _ => null
 };
 if (sym?.ContainingAssembly?.Name == targetAsm)
 {
 memberUses++;
 var t = sym.ContainingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
 if (!string.IsNullOrEmpty(t)) types.Add(t!);
 }
 }
 }
 aEntry[pb.Name] = (types.Count, memberUses);
 }
 }
 return result;
 }
}
