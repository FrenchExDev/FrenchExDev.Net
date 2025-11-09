using FrenchExDev.Net.CSharp.ProjectDependency5.Core;

namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core.Analysis;

public interface IProjectAnalyzer
{
 string Name { get; }
 Task<IProjectAnalysisResult> AnalyzeAsync(Solution solution, CancellationToken cancellationToken = default);
}
