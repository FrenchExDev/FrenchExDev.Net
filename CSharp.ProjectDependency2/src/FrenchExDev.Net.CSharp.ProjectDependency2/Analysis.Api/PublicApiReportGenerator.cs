using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public sealed class PublicApiReportGenerator : IProjectAnalysisReportGenerator
{
    private readonly PublicApiAnalyzer _analyzer = new();

    public Result<IProjectAnalysisReportResult> GenerateReport<T>(Project project, Solution solution)
    {
        var analysis = _analyzer.AnalyzeProject(project, solution);
        if (!analysis.IsSuccess) return Result<IProjectAnalysisReportResult>.Failure(analysis.FailuresOrThrow());
        var report = (PublicApiReport)analysis.Object;
        return Result<IProjectAnalysisReportResult>.Success(
            new PublicApiDetailedReport(project.Name, report.PublicTypeCount, report.PublicMemberCount, report.Types)
        );
    }
}
