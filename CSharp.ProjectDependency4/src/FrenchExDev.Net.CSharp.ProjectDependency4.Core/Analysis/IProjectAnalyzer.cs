namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core.Analysis;

public interface IProjectAnalyzer
{
 string Name { get; }
 Task<IProjectAnalysisResult> AnalyzeAsync(Solution solution, CancellationToken cancellationToken = default);
}
