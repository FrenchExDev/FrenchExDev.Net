using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.CodeQuality;

public record ExceptionUsageItem(string File, int Line, bool NakedCatch, bool BroadCatch, bool RethrowWrong);
public record ExceptionUsageReport(IReadOnlyList<ExceptionUsageItem> Items) : IProjectAnalysisResult;

public class ExceptionUsageAnalyzer : IProjectAnalyzer
{
    public Result<IProjectAnalysisResult> AnalyzeProject(Project project, Solution solution)
    {
        try
        {
            var items = new List<ExceptionUsageItem>();
            var comp = project.Code.GetCompilationAsync().GetAwaiter().GetResult();
            if (comp == null) return Result<IProjectAnalysisResult>.Success(new ExceptionUsageReport(items));

            foreach (var tree in comp.SyntaxTrees)
            {
                var root = tree.GetRoot();
                foreach (var tryStmt in root.DescendantNodes().OfType<TryStatementSyntax>())
                {
                    foreach (var catchClause in tryStmt.Catches)
                    {
                        bool naked = catchClause.Declaration == null;
                        bool broad = catchClause.Declaration?.Type is { } t && t.ToString() == "Exception";
                        bool rethrowWrong = catchClause.Block.DescendantNodes().OfType<ThrowStatementSyntax>().Any(ts => ts.Expression != null);
                        var loc = catchClause.GetLocation().GetLineSpan();
                        items.Add(new ExceptionUsageItem(tree.FilePath ?? string.Empty, loc.StartLinePosition.Line + 1, naked, broad, rethrowWrong));
                    }
                }
            }

            return Result<IProjectAnalysisResult>.Success(new ExceptionUsageReport(items));
        }
        catch (Exception ex)
        {
            return Result<IProjectAnalysisResult>.Failure(ex);
        }
    }
}
