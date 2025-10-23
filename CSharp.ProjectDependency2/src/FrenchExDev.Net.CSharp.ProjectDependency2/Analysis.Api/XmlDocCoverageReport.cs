using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;
using Microsoft.CodeAnalysis;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public record XmlDocCoverageItem(string Symbol, bool HasDocs);
public record XmlDocCoverageReport(IReadOnlyList<XmlDocCoverageItem> Items, double Coverage) : IProjectAnalysisResult;

public class XmlDocCoverageAnalyzer : IProjectAnalyzer
{
    public Result<IProjectAnalysisResult> AnalyzeProject(Project project, Solution solution)
    {
        try
        {
            var items = new List<XmlDocCoverageItem>();
            var code = project.Code;
            var compilation = code.GetCompilationAsync().GetAwaiter().GetResult();
            if (compilation == null) return Result<IProjectAnalysisResult>.Success(new XmlDocCoverageReport(items, 0));

            foreach (var tree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(tree);
                var root = tree.GetRoot();
                var symbols = root.DescendantNodes()
                    .Select(n => model.GetDeclaredSymbol(n))
                    .OfType<ISymbol>()
                    .Where(s => s.DeclaredAccessibility == Accessibility.Public);

                foreach (var s in symbols)
                {
                    var hasDocs = !string.IsNullOrWhiteSpace(s.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: default));
                    items.Add(new XmlDocCoverageItem(s.ToDisplayString(), hasDocs));
                }
            }

            double coverage = items.Count == 0 ? 0 : items.Count(i => i.HasDocs) / (double)items.Count;
            return Result<IProjectAnalysisResult>.Success(new XmlDocCoverageReport(items, coverage));
        }
        catch (Exception ex)
        {
            return Result<IProjectAnalysisResult>.Failure(ex);
        }
    }
}
