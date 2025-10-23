using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public sealed class XmlDocCoverageReportGenerator : IProjectAnalysisReportGenerator
{
    private readonly XmlDocCoverageAnalyzer _analyzer = new();

    public Result<IProjectAnalysisReportResult> GenerateReport<T>(Project project, Solution solution)
    {
        var analysis = _analyzer.AnalyzeProject(project, solution);
        if (!analysis.IsSuccess) return Result<IProjectAnalysisReportResult>.Failure(analysis.FailuresOrThrow());
        var report = (XmlDocCoverageReport)analysis.Object;
        var documented = report.Items.Count(i => i.HasDocs);
        return Result<IProjectAnalysisReportResult>.Success(
            new XmlDocCoverageDetailedReport(project.Name, report.Coverage, documented, report.Items.Count, report.Items)
        );
    }
}
