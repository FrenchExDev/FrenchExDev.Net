using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.CodeQuality;

public sealed class AsyncConventionsReportGenerator : IProjectAnalysisReportGenerator
{
    private readonly AsyncConventionsAnalyzer _analyzer = new();

    public Result<IProjectAnalysisReportResult> GenerateReport<T>(Project project, Solution solution)
    {
        var analysis = _analyzer.AnalyzeProject(project, solution);
        if (!analysis.IsSuccess) return Result<IProjectAnalysisReportResult>.Failure(analysis.FailuresOrThrow());
        var report = (AsyncConventionsReport)analysis.Object;
        return Result<IProjectAnalysisReportResult>.Success(new AsyncConventionsDetailedReport(project.Name, report.Items));
    }
}
