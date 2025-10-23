using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.CodeQuality;

public record AsyncConventionsItem(string Method, bool HasAsyncSuffix, bool BlocksOnTask);
public record AsyncConventionsReport(IReadOnlyList<AsyncConventionsItem> Items) : IProjectAnalysisResult;

public class AsyncConventionsAnalyzer : IProjectAnalyzer
{
    public Result<IProjectAnalysisResult> AnalyzeProject(Project project, Solution solution)
    {
        try
        {
            var items = new List<AsyncConventionsItem>();
            var comp = project.Code.GetCompilationAsync().GetAwaiter().GetResult();
            if (comp == null) return Result<IProjectAnalysisResult>.Success(new AsyncConventionsReport(items));

            foreach (var tree in comp.SyntaxTrees)
            {
                var root = tree.GetRoot();
                var model = comp.GetSemanticModel(tree);

                foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    var symbol = model.GetDeclaredSymbol(method) as IMethodSymbol;
                    if (symbol == null) continue;

                    var returnsTask = symbol.ReturnType.ToDisplayString().StartsWith("System.Threading.Tasks.Task");
                    if (!returnsTask) continue;

                    bool hasSuffix = symbol.Name.EndsWith("Async", StringComparison.Ordinal);

                    bool blocksOnTask = method.Body?.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                        .Any(ma => ma.Name.Identifier.Text is "Result" or "Wait") == true;

                    items.Add(new AsyncConventionsItem(symbol.ToDisplayString(), hasSuffix, blocksOnTask));
                }
            }

            return Result<IProjectAnalysisResult>.Success(new AsyncConventionsReport(items));
        }
        catch (Exception ex)
        {
            return Result<IProjectAnalysisResult>.Failure(ex);
        }
    }
}
