using FrenchExDev.Net.CSharp.Object.Result;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Tests;

/// <summary>
/// Defines a contract for analyzing a project within a solution and producing a result of a specified type.
/// </summary>
/// <remarks>Implementations of this interface are responsible for performing analysis on a given project in the
/// context of its containing solution. The analysis result type allows for flexibility in the kind of information
/// returned, such as diagnostics, metrics, or custom data relevant to the project.</remarks>
public interface IProjectAnalyzer
{
    /// <summary>
    /// Analyzes the specified project within the context of the given solution and produces an analysis result.
    /// </summary>
    /// <param name="project">The project to analyze. Cannot be null.</param>
    /// <param name="solution">The solution that contains the project. Cannot be null.</param>
    /// <returns>A result object containing the analysis outcome for the specified project. The result indicates success or
    /// failure and includes the analysis data if successful.</returns>
    Result<IProjectAnalysisResult> AnalyzeProject(Project project, Solution solution);
}
