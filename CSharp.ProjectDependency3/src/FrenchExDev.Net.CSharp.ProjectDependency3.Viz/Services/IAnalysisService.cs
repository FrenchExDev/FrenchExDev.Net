namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Services;

public interface IAnalysisService
{
 Task<AnalysisRunResult> AnalyzeSolutionAsync(string solutionPath, CancellationToken ct = default);
}

public record AnalysisRunResult(
 string SessionId,
 string? SolutionPath,
 string GraphFileRelativeUrl,
 string MarkdownContent
);
