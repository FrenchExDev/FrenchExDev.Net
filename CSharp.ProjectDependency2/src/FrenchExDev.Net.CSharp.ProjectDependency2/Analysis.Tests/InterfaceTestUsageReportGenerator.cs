using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Tests;

public sealed class InterfaceTestUsageReportGenerator : IProjectAnalysisReportGenerator
{
    private readonly InterfaceTestUsageAnalyzer _analyzer = new();

    public Result<IProjectAnalysisReportResult> GenerateReport<T>(Project project, Solution solution)
    {
        var analysis = _analyzer.AnalyzeProject(project, solution);
        if (!analysis.IsSuccess) return Result<IProjectAnalysisReportResult>.Failure(analysis.FailuresOrThrow());
        var report = (InterfaceTestUsageReport)analysis.Object;
        return Result<IProjectAnalysisReportResult>.Success(new InterfaceTestUsageDetailedReport(project.Name, report.Items));
    }
}
