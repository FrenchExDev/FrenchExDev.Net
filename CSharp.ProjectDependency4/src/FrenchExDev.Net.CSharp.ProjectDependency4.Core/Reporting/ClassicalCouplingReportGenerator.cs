using FrenchExDev.Net.CSharp.ProjectDependency4.Core.Analysis;
using FrenchExDev.Net.CSharp.ProjectDependency4.Core.Markdown;
using System.Text;

namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core.Reporting;

/// <summary>
/// Generates a Markdown report summarizing classical coupling metrics, including afferent coupling (Ca), efferent
/// coupling (Ce), and instability for a set of projects.
/// </summary>
/// <remarks>The report includes a reference to the package principles, a table of coupling metrics for each
/// project, and a Mermaid diagram visualizing project dependencies. This generator is intended for use with results
/// produced by classical coupling analysis tools. For more information on the underlying metrics, see the Wikipedia
/// article on package principles.</remarks>
public class ClassicalCouplingReportGenerator : IReportGenerator<ClassicalCouplingResult>
{
    public string Name => "ClassicalCouplingReport";

    /// <summary>
    /// Generates a set of markdown sections summarizing classical coupling metrics and relationships for the analyzed
    /// projects.
    /// </summary>
    /// <remarks>The generated markdown includes a reference to the package principles from Wikipedia, a table
    /// of coupling metrics (Ca, Ce, Instability), and a mermaid diagram illustrating project dependencies. The output
    /// is suitable for inclusion in documentation or reports to visualize architectural coupling.</remarks>
    /// <param name="result">The classical coupling analysis result containing metrics and dependency information for each project.</param>
    /// <returns>An array of markdown sections that include a summary table of afferent and efferent coupling metrics,
    /// instability values, and a mermaid graph visualizing project dependencies.</returns>
    public MarkdownSection[] Generate(ClassicalCouplingResult result)
    {
        var sections = new List<MarkdownSection>();
        var section = new MarkdownSection("Classical Coupling (Ca, Ce, Instability)");

        // Info / reference to wiki
        section.AddContent("Reference: Afferent coupling (Ca), Efferent coupling (Ce) and Instability (I = Ce/(Ca+Ce)). See Wikipedia: [Package principles](https://en.wikipedia.org/wiki/Package_principles).");

        // Table
        var table = new StringBuilder();
        table.AppendLine("| Project | Ca | Ce | Instability |");
        table.AppendLine("|---:|---:|---:|---:|");
        foreach (var kv in result.Metrics.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
        {
            var (ca, ce, inst) = kv.Value;
            table.AppendLine($"| {kv.Key} | {ca} | {ce} | {inst:0.###} |");
        }
        section.AddContent(table.ToString());

        // Mermaid graph using incoming/outgoing
        var graph = new StringBuilder();
        graph.AppendLine("```mermaid");
        graph.AppendLine("graph LR");
        foreach (var from in result.Outgoing)
        {
            foreach (var to in from.Value)
            {
                graph.AppendLine($" {Sanitize(from.Key)} --> {Sanitize(to)}");
            }
        }
        graph.AppendLine("```");
        section.AddContent(graph.ToString());

        sections.Add(section);
        return sections.ToArray();
    }

    /// <summary>
    /// Removes all characters from the specified string except letters, digits, underscores, and hyphens.
    /// </summary>
    /// <param name="name">The input string to be sanitized. Only letters, digits, underscores ('_'), and hyphens ('-') are retained.</param>
    /// <returns>A sanitized string containing only allowed characters. Returns "P" if the input contains no valid characters.</returns>
    private static string Sanitize(string name)
    {
        var s = new string(name.Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '-').ToArray());
        return string.IsNullOrEmpty(s) ? "P" : s;
    }
}
