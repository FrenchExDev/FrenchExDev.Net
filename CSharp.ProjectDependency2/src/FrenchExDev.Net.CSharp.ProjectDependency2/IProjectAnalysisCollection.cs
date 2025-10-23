using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Represents a collection of project analyzers that can be used to perform analysis on projects within a solution.
/// </summary>
/// <remarks>This interface allows for the composition of multiple project analyzers and provides a method to
/// execute all registered analyzers against a specified project and solution. Implementations may vary in how analyzers
/// are managed or invoked. Thread safety depends on the specific implementation.</remarks>
public interface IProjectAnalysisCollection
{
    /// <summary>
    /// Adds the specified project analyzer to the collection.
    /// </summary>
    /// <param name="analyzer">The project analyzer to add. Cannot be null.</param>
    /// <returns>The updated project analysis collection that includes the specified analyzer.</returns>
    IProjectAnalysisCollection AddAnalyzer(IProjectAnalyzer analyzer);

    /// <summary>
    /// Analyzes the specified project within the context of the given solution and returns the results for each
    /// analysis performed.
    /// </summary>
    /// <param name="project">The project to analyze. Cannot be null.</param>
    /// <param name="solution">The solution that contains the project. Cannot be null.</param>
    /// <returns>A result object containing a list of analysis results for the project. Each item in the list represents the
    /// outcome of an individual analysis and may indicate success or failure.</returns>
    Result<List<Result<IProjectAnalysisResult>>> GenerateAnalysis(Project project, Solution solution);
}
