using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Tests;

public record InterfaceTestUsageItem(string InterfaceName, bool UsedInTests, IReadOnlyList<string> SampleLocations);
public record InterfaceTestUsageReport(IReadOnlyList<InterfaceTestUsageItem> Items) : IProjectAnalysisResult;

public class InterfaceTestUsageAnalyzer : IProjectAnalyzer
{
    private static bool IsTestProject(string name) => name.Contains("Test", StringComparison.OrdinalIgnoreCase) || name.Contains("Tests", StringComparison.OrdinalIgnoreCase);

    public Result<IProjectAnalysisResult> AnalyzeProject(Project project, Solution solution)
    {
        try
        {
            var code = project.Code;
            var comp = code.GetCompilationAsync().GetAwaiter().GetResult();
            if (comp == null) return Result<IProjectAnalysisResult>.Success(new InterfaceTestUsageReport(new List<InterfaceTestUsageItem>()));

            // interfaces in current project
            var interfaces = new List<INamedTypeSymbol>();
            foreach (var tree in comp.SyntaxTrees)
            {
                var model = comp.GetSemanticModel(tree);
                var root = tree.GetRoot();
                var decl = root.DescendantNodes()
                    .Select(n => model.GetDeclaredSymbol(n))
                    .OfType<INamedTypeSymbol>()
                    .Where(s => s.TypeKind == TypeKind.Interface && s.DeclaredAccessibility == Accessibility.Public);
                interfaces.AddRange(decl);
            }

            var items = new List<InterfaceTestUsageItem>();

            foreach (var iface in interfaces)
            {
                var used = false;
                var locations = new List<string>();

                foreach (var other in solution.Projects)
                {
                    if (!IsTestProject(other.Name)) continue;

                    foreach (var doc in other.Documents)
                    {
                        var model = other.GetCompilationAsync().GetAwaiter().GetResult()?.GetSemanticModel(doc.GetSyntaxTreeAsync().GetAwaiter().GetResult()!);
                        if (model == null) continue;
                        var root = doc.GetSyntaxRootAsync().GetAwaiter().GetResult();
                        if (root == null) continue;

                        var ids = root.DescendantNodes().OfType<IdentifierNameSyntax>();
                        foreach (var id in ids)
                        {
                            var sym = model.GetSymbolInfo(id).Symbol as INamedTypeSymbol;
                            if (sym == null) continue;
                            if (SymbolEqualityComparer.Default.Equals(sym.OriginalDefinition, iface) || string.Equals(sym.ToDisplayString(), iface.ToDisplayString(), StringComparison.Ordinal))
                            {
                                used = true;
                                var loc = id.GetLocation().GetLineSpan();
                                locations.Add($"{doc.FilePath}:{loc.StartLinePosition.Line + 1}");
                                if (locations.Count >= 3) break;
                            }
                        }
                        if (used) break;
                    }
                    if (used) break;
                }

                items.Add(new InterfaceTestUsageItem(iface.ToDisplayString(), used, locations));
            }

            return Result<IProjectAnalysisResult>.Success(new InterfaceTestUsageReport(items));
        }
        catch (Exception ex)
        {
            return Result<IProjectAnalysisResult>.Failure(ex);
        }
    }
}
