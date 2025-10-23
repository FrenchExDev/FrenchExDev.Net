using FrenchExDev.Net.CSharp.ProjectDependency3.Analysis;
using FrenchExDev.Net.CSharp.ProjectDependency3.Markdown;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Reporting;

/// <summary>
/// Defines a contract for generating reports from project analysis results of a specified type.
/// </summary>
/// <typeparam name="T">The type of project analysis result to generate reports from. Must implement <see cref="IProjectAnalysisResult"/>.</typeparam>
public interface IReportGenerator<T> where T : IProjectAnalysisResult
{
    /// <summary>
    /// Gets the name associated with the current instance.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Generates an array of markdown sections based on the specified result.
    /// </summary>
    /// <param name="result">The result object used as input for generating markdown sections. Cannot be null.</param>
    /// <returns>An array of <see cref="MarkdownSection"/> objects representing the generated markdown content. The array will be
    /// empty if no sections are produced.</returns>
    MarkdownSection[] Generate(T result);
}
