using System.Text;
using System.Text.RegularExpressions;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

/// <summary>
/// Generates markdown that concatenates per-project markdown for a collection of projects.
/// </summary>
public class ProjectsMarkdownGenerator : IMarkdownGenerator<IEnumerable<ProjectAnalysis>>
{
    private readonly ProjectMarkdownGenerator _projectGenerator = new ProjectMarkdownGenerator();
    private readonly MermaidGenerator _mermaid = new MermaidGenerator();

    public string Generate(IEnumerable<ProjectAnalysis> projects)
    {
        if (projects == null) throw new ArgumentNullException(nameof(projects));

        var sb = new StringBuilder();

        foreach (var p in projects.OrderBy(p => p.Name))
        {
            sb.AppendLine(_projectGenerator.Generate(p));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Generates output based on the provided solution analysis.
    /// </summary>
    /// <param name="analysis">The solution analysis containing project information to use for generation. Cannot be null.</param>
    /// <returns>A string containing the generated output based on the analysis.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the analysis parameter is null.</exception>
    public string Generate(SolutionAnalysis analysis)
    {
        if (analysis == null) throw new ArgumentNullException(nameof(analysis));
        return Generate(analysis.Projects);
    }

    /// <summary>
    /// Generates a Markdown-formatted table of contents listing all projects in the specified solution analysis.
    /// </summary>
    /// <param name="analysis">The solution analysis containing the collection of projects to include in the table of contents. Cannot be null.</param>
    /// <returns>A string containing the Markdown table of contents for the projects in the solution. The string will be empty if
    /// there are no projects.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the analysis parameter is null.</exception>
    public string GenerateProjectsTableOfContents(SolutionAnalysis analysis)
    {
        if (analysis == null) throw new ArgumentNullException(nameof(analysis));

        return GenerateProjectsTableOfContents(analysis.Projects);
    }

    /// <summary>
    /// Generates a Markdown-formatted table of contents for a collection of projects.
    /// </summary>
    /// <remarks>Projects are listed in alphabetical order by name. If a project's name is null, the file name
    /// (without extension) is used instead. Each entry links to an anchor based on the project name.</remarks>
    /// <param name="projects">The collection of projects to include in the table of contents. Each project should provide a name and file
    /// path.</param>
    /// <returns>A string containing the Markdown table of contents, with each project listed as a link anchored by its name.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the projects parameter is null.</exception>
    public string GenerateProjectsTableOfContents(IEnumerable<ProjectAnalysis> projects)
    {
        if (projects == null) throw new ArgumentNullException(nameof(projects));

        var sb = new StringBuilder();
        sb.AppendLine("# Projects Table of Contents");
        sb.AppendLine();

        foreach (var p in projects.OrderBy(p => p.Name))
        {
            var name = p.Name ?? Path.GetFileNameWithoutExtension(p.FilePath ?? string.Empty) ?? "Unknown";
            var anchor = ToAnchor(name);
            sb.AppendLine($"- [{name}](#{anchor})");
        }

        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        return sb.ToString();
    }

    private static string ToAnchor(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        // normalize: lower, remove accents, replace non-alnum with hyphen, collapse hyphens
        var lower = text.ToLowerInvariant();
        // remove characters except letters, digits, space
        var cleaned = Regex.Replace(lower, "[^a-z0-9 _-]", string.Empty);
        // replace spaces/underscores with hyphen
        cleaned = Regex.Replace(cleaned, "[ _]+", "-");
        // collapse multiple hyphens
        cleaned = Regex.Replace(cleaned, "-+", "-");
        // trim
        cleaned = cleaned.Trim('-');
        return cleaned;
    }

    /// <summary>
    /// Generate projects markdown with TOC inserted before project contents and KPI mermaid charts.
    /// </summary>
    public string GenerateWithTableOfContents(SolutionAnalysis analysis)
    {
        if (analysis == null) throw new ArgumentNullException(nameof(analysis));

        var sb = new StringBuilder();
        sb.AppendLine(GenerateProjectsTableOfContents(analysis));

        // KPI summary table
        sb.AppendLine(GenerateKpiSummaryTable(analysis));

        // include KPI mermaid charts and dependency graph
        sb.AppendLine(GenerateKpiMermaidCharts(analysis));

        sb.AppendLine(Generate(analysis));
        return sb.ToString();
    }

    public string GenerateWithTableOfContents(IEnumerable<ProjectAnalysis> projects)
    {
        if (projects == null) throw new ArgumentNullException(nameof(projects));
        var sb = new StringBuilder();
        sb.AppendLine(GenerateProjectsTableOfContents(projects));
        sb.AppendLine(Generate(projects));
        return sb.ToString();
    }

    /// <summary>
    /// Generate a compact KPI summary table for all projects (one row per project).
    /// Columns: Project | Times used | Source files | LOC | Diagnostics | Cyclomatic | Commits | Last commit | Maintainability
    /// </summary>
    private string GenerateKpiSummaryTable(SolutionAnalysis analysis)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Projects KPI summary");
        sb.AppendLine();
        sb.AppendLine("Project | Times used | Source files | LOC | Diagnostics | Cyclomatic | Commits | Last commit | Maintainability");
        sb.AppendLine("--- | ---: | ---: | ---: | ---: | ---: | ---: | --- | ---: ");

        foreach (var p in analysis.Projects.OrderBy(p => p.Name))
        {
            var key = p.FilePath ?? string.Empty;
            if (analysis.ProjectMetricsMap != null && analysis.ProjectMetricsMap.TryGetValue(key, out var m))
            {
                var last = m.LastCommitDate?.ToString("u") ?? "-";
                sb.AppendLine($"{p.Name} | {m.TimesUsed} | {m.SourceFileCount} | {m.TotalLinesOfCode} | {m.DiagnosticsCount} | {m.CyclomaticComplexity} | {m.CommitCount} | {last} | {m.MaintainabilityIndex:F1}");
            }
            else
            {
                sb.AppendLine($"{p.Name} | - | - | - | - | - | - | - | -");
            }
        }

        sb.AppendLine();
        return sb.ToString();
    }

    /// <summary>
    /// Generate mermaid charts for selected KPIs: dependency graph, times-used distribution and top packages.
    /// </summary>
    private string GenerateKpiMermaidCharts(SolutionAnalysis analysis)
    {
        var sb = new StringBuilder();

        // dependency graph using existing mermaid generator
        sb.AppendLine("## Dependency graph");
        sb.AppendLine(_mermaid.Generate(analysis));

        // times-used pie (projects most used by others)
        sb.AppendLine("## Project usage distribution");
        sb.AppendLine("```mermaid");
        sb.AppendLine("pie title Projects times-used distribution");
        var metrics = analysis.ProjectMetricsMap ?? new Dictionary<string, ProjectMetrics>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in metrics.OrderByDescending(kv => kv.Value.TimesUsed).Take(10))
        {
            var name = Path.GetFileNameWithoutExtension(kv.Value.FilePath) ?? kv.Key;
            sb.AppendLine($"    \"{Escape(name)}\" : {Math.Max(0, kv.Value.TimesUsed)}");
        }
        sb.AppendLine("```\n");

        // package distribution pie
        sb.AppendLine("## Top NuGet packages (by project references)");
        sb.AppendLine("```mermaid");
        sb.AppendLine("pie title Top NuGet packages");
        foreach (var kv in analysis.PackageReferenceCounts.OrderByDescending(kv => kv.Value).Take(10))
        {
            sb.AppendLine($"    \"{Escape(kv.Key)}\" : {kv.Value}");
        }
        sb.AppendLine("```\n");

        return sb.ToString();
    }

    private static string Escape(string s) => s?.Replace("\"", "\\\"") ?? string.Empty;
}
