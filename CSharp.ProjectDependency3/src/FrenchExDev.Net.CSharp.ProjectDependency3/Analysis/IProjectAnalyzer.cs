namespace FrenchExDev.Net.CSharp.ProjectDependency3;

public interface IProjectAnalyzer
{
 string Name { get; }
 Task<FrenchExDev.Net.CSharp.ProjectDependency3.Analysis.IProjectAnalysisResult> AnalyzeAsync(Solution solution, CancellationToken cancellationToken = default);
}
