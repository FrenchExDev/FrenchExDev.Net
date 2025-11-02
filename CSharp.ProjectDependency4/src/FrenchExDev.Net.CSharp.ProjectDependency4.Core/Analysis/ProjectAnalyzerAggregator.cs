namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core.Analysis;

/// <summary>
/// Aggregates multiple project analyzers and executes them collectively on a solution.
/// </summary>
/// <remarks>Use this class to combine several implementations of <see cref="IProjectAnalyzer"/> and run their
/// analysis in a single operation. The analyzers are executed in the order they are added. This class is useful for
/// scenarios where multiple analysis results are needed from a single solution. Thread safety is not guaranteed; ensure
/// external synchronization if accessed concurrently.</remarks>
public class ProjectAnalyzerAggregator
{
    private readonly List<IProjectAnalyzer> _analyzers = new();

    /// <summary>
    /// Adds the specified project analyzer to the aggregator.
    /// </summary>
    /// <param name="analyzer">The project analyzer to add. Cannot be null.</param>
    /// <returns>The current instance of <see cref="ProjectAnalyzerAggregator"/> to allow method chaining.</returns>
    public ProjectAnalyzerAggregator Add(IProjectAnalyzer analyzer)
    {
        _analyzers.Add(analyzer);
        return this;
    }

    /// <summary>
    /// Runs all registered analyzers asynchronously on the specified solution and returns their results.
    /// </summary>
    /// <remarks>The returned dictionary uses case-insensitive keys based on analyzer names. If the operation
    /// is canceled, an incomplete set of results may be returned.</remarks>
    /// <param name="solution">The solution to be analyzed by each registered analyzer. Must not be null.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the analysis operation.</param>
    /// <returns>A dictionary containing the results from each analyzer, keyed by analyzer name. Each value represents the result
    /// produced by the corresponding analyzer.</returns>
    public async Task<Dictionary<string, object>> RunAsync(Solution solution, CancellationToken ct = default)
    {
        var results = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var analyzer in _analyzers)
        {
            results[analyzer.Name] = await analyzer.AnalyzeAsync(solution, ct);
        }
        return results;
    }
}
