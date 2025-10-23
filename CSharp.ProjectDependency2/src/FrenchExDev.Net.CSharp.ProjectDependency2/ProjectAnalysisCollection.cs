using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Represents a collection of project analyzers that can be configured to analyze a project and generate analysis
/// results using all registered analyzers.
/// </summary>
/// <remarks>Use this class to compose multiple project analyzers and execute them as a group against a given
/// project. The analyzers are invoked in the order they are added. This class enables method chaining for fluent
/// configuration of the analysis pipeline.</remarks>
public class ProjectAnalysisCollection : IProjectAnalysisCollection
{
    private readonly List<IProjectAnalyzer> _analyzers = new();

    /// <summary>
    /// Adds a project analyzer to the analysis pipeline for the specified project analysis result type.
    /// </summary>
    /// <param name="analyzer">The project analyzer to add. Cannot be null. The analyzer must produce results of type TProjectAnalysisResult.</param>
    /// <returns>The current instance of IProjectAnalysisResultGenerator, enabling method chaining.</returns>
    /// <exception cref="InvalidCastException">Thrown if the specified analyzer cannot be cast to IProjectAnalyzer<IProjectAnalysisResult>.</exception>
    public IProjectAnalysisCollection AddAnalyzer(IProjectAnalyzer analyzer)
    {
        if (analyzer != null) _analyzers.Add(analyzer);
        return this;
    }

    /// <summary>
    /// Generates analysis results for the specified project using all configured analyzers.
    /// </summary>
    /// <remarks>Each analyzer is invoked independently. The returned list preserves the order of analyzers as
    /// configured. Callers should inspect each inner result to determine the success or failure of individual
    /// analyses.</remarks>
    /// <param name="project">The project to analyze. Cannot be null.</param>
    /// <param name="solution">The solution that contains the project. Cannot be null.</param>
    /// <returns>A result containing a list of results for each analyzer, where each inner result represents the outcome of
    /// analyzing the project. The list will be empty if there are no analyzers configured.</returns>
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
