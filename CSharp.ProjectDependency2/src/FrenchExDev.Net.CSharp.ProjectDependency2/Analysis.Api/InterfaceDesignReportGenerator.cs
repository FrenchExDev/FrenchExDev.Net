using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public sealed class InterfaceDesignReportGenerator : IProjectAnalysisReportGenerator
{
    private readonly InterfaceDesignAnalyzer _analyzer = new();

    public Result<IProjectAnalysisReportResult> GenerateReport<T>(Project project, Solution solution)
    {
        var analysis = _analyzer.AnalyzeProject(project, solution);
        if (!analysis.IsSuccess) return Result<IProjectAnalysisReportResult>.Failure(analysis.FailuresOrThrow());
        var report = (InterfaceDesignReport)analysis.Object;
        return Result<IProjectAnalysisReportResult>.Success(new InterfaceDesignDetailedReport(project.Name, report.Interfaces));
    }
}
