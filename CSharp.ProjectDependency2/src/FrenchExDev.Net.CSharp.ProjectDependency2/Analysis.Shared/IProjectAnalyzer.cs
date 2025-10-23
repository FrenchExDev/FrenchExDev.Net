using FrenchExDev.Net.CSharp.Object.Result;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

public interface IProjectAnalyzer
{
    Result<IProjectAnalysisResult> AnalyzeProject(Project project, Solution solution);
}
