using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared; // Updated namespace for clarity
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public record PublicTypeInfo(string Kind, string Name, IReadOnlyList<string> Members);
public record PublicApiReport(IReadOnlyList<PublicTypeInfo> Types) : IProjectAnalysisResult
{
    public int PublicTypeCount => Types.Count;
    public int PublicMemberCount => Types.Sum(t => t.Members.Count);
}

public class PublicApiAnalyzer : IProjectAnalyzer
{
    public Result<IProjectAnalysisResult> AnalyzeProject(Project project, Solution solution)
    {
        try
        {
            var code = project.Code;
            var types = new List<PublicTypeInfo>();

            foreach (var document in code.Documents)
            {
                var model = code.GetCompilationAsync().GetAwaiter().GetResult()?.GetSemanticModel(document.GetSyntaxTreeAsync().GetAwaiter().GetResult()!);
                if (model == null) continue;
                var root = document.GetSyntaxRootAsync().GetAwaiter().GetResult();
                if (root == null) continue;

                var declared = root.DescendantNodes().Select(n => model.GetDeclaredSymbol(n)).OfType<INamedTypeSymbol>()
                    .Where(s => s.DeclaredAccessibility == Accessibility.Public)
                    .ToImmutableArray();

                foreach (var s in declared)
                {
                    var members = s.GetMembers()
                        .Where(m => m.DeclaredAccessibility == Accessibility.Public)
                        .Select(m => m.ToDisplayString())
                        .ToList();

                    types.Add(new PublicTypeInfo(s.TypeKind.ToString(), s.ToDisplayString(), members));
                }
            }

            return Result<IProjectAnalysisResult>.Success(new PublicApiReport(types));
        }
        catch (Exception ex)
        {
            return Result<IProjectAnalysisResult>.Failure(ex);
        }
    }
}
