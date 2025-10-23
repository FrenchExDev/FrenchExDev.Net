using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.CodeQuality;

public sealed class ExceptionUsageReportGenerator : IProjectAnalysisReportGenerator
{
    private readonly ExceptionUsageAnalyzer _analyzer = new();

    public Result<IProjectAnalysisReportResult> GenerateReport<T>(Project project, Solution solution)
    {
        var analysis = _analyzer.AnalyzeProject(project, solution);
        if (!analysis.IsSuccess) return Result<IProjectAnalysisReportResult>.Failure(analysis.FailuresOrThrow());
        var report = (ExceptionUsageReport)analysis.Object;
        return Result<IProjectAnalysisReportResult>.Success(new ExceptionUsageDetailedReport(project.Name, report.Items));
    }
}
