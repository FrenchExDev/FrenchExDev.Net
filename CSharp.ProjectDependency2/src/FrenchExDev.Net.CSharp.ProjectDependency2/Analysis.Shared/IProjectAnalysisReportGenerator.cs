using FrenchExDev.Net.CSharp.Object.Result;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

public interface IProjectAnalysisReportGenerator
{
    Result<IProjectAnalysisReportResult> GenerateReport<T>(Project project, Solution solution);
}
