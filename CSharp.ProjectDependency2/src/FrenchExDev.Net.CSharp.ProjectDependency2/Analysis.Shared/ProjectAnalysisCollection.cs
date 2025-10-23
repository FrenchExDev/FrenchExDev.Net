using FrenchExDev.Net.CSharp.Object.Result;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

public class ProjectAnalysisCollection : IProjectAnalysisCollection
{
    private readonly List<IProjectAnalyzer> _analyzers = new();

    public IProjectAnalysisCollection AddAnalyzer(IProjectAnalyzer analyzer)
    {
        if (analyzer != null) _analyzers.Add(analyzer);
        return this;
    }

    public Result<List<Result<IProjectAnalysisResult>>> GenerateAnalysis(Project project, Solution solution)
    {
        var results = new List<Result<IProjectAnalysisResult>>();
        foreach (var analyzer in _analyzers)
        {
            results.Add(analyzer.AnalyzeProject(project, solution));
        }
        return Result<List<Result<IProjectAnalysisResult>>>.Success(results);
    }
}
