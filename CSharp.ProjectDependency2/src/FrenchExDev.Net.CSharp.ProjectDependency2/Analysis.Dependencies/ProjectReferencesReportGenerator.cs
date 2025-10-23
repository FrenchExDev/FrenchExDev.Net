using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.References;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Dependencies;

public sealed class ProjectReferencesReportGenerator : IProjectAnalysisReportGenerator
{
    private readonly ProjectReferencesAnalyzer _analyzer = new();

    public Result<IProjectAnalysisReportResult> GenerateReport<T>(Project project, Solution solution)
    {
        var analysis = _analyzer.AnalyzeProject(project, solution);
        if (!analysis.IsSuccess) return Result<IProjectAnalysisReportResult>.Failure(analysis.FailuresOrThrow());
        var report = (ProjectReferenceAnalysis)analysis.Object;
        var header = $"# References for {project.Name}\n\n";
        var pkgs = report.PackageReferences.Select(p => $"- {p.Package.Name}");
        var projs = report.ProjectReferences.Select(r => $"- {r.ProjectPath}");
        var md = header +
                 $"## Packages ({report.PackageReferences.Count})\n" + string.Join("\n", pkgs) + "\n\n" +
                 $"## Projects ({report.ProjectReferences.Count})\n" + string.Join("\n", projs);
        return Result<IProjectAnalysisReportResult>.Success(new ProjectReferencesMarkdownReport(md));
    }
}
