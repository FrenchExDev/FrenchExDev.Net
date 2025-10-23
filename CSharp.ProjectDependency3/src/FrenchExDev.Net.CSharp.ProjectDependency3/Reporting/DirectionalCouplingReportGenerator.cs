using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency3.Analysis;
using FrenchExDev.Net.CSharp.ProjectDependency3.Markdown;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Reporting;

/// <summary>
/// Generates markdown reports that visualize directional coupling between code entities, highlighting unique type
/// dependencies and member usage from source to target entities.
/// </summary>
/// <remarks>This report generator produces a summary of directional coupling, presenting tables and Mermaid
/// diagrams for each source entity to illustrate how it depends on target entities. The generated report helps identify
/// architectural dependencies and potential areas for refactoring by showing the number of unique types and member uses
/// from one entity to another. The class implements the IReportGenerator interface for DirectionalCouplingResult,
/// making it suitable for integration into reporting workflows that analyze code structure.</remarks>
public class DirectionalCouplingReportGenerator : IReportGenerator<DirectionalCouplingResult>
{
    public string Name => "DirectionalCouplingReport";

    /// <summary>
    /// Generates a collection of markdown sections that summarize directional coupling information between types,
    /// including unique type and member usage statistics.
    /// </summary>
    /// <remarks>Each section includes a table of unique types and member uses, as well as a Mermaid diagram
    /// visualizing the coupling relationships. The output is suitable for documentation or reporting
    /// purposes.</remarks>
    /// <param name="result">The directional coupling analysis result containing type relationships and usage metrics to be represented in
    /// markdown format.</param>
    /// <returns>An array of markdown sections, each detailing directional coupling data for the analyzed types. The array will
    /// be empty if no coupling information is present.</returns>
    public MarkdownSection[] Generate(DirectionalCouplingResult result)
    {
        var sections = new List<MarkdownSection>();
        var section = new MarkdownSection("Directional Coupling (A -> B: unique types, member uses)");

        // Table per A
        foreach (var a in result.Coupling.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
        {
            var sub = new MarkdownSection(a.Key);
            var sb = new StringBuilder();
            sb.AppendLine("| Target B | Unique Types | Member Uses |");
            sb.AppendLine("|---|---:|---:|");
            foreach (var b in a.Value.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
            {
                sb.AppendLine($"| {b.Key} | {b.Value.UniqueTypes} | {b.Value.MemberUses} |");
            }
            sub.AddContent(sb.ToString());

            // Mermaid weighted edges by label
            var mermaid = new StringBuilder();
            mermaid.AppendLine("```mermaid");
            mermaid.AppendLine("graph LR");
            foreach (var b in a.Value)
            {
                var label = $"{b.Value.UniqueTypes}t/{b.Value.MemberUses}m";
                mermaid.AppendLine($" {Sanitize(a.Key)} -->|{label}| {Sanitize(b.Key)}");
            }
            mermaid.AppendLine("```");
            sub.AddContent(mermaid.ToString());
            section.AddSubSection(sub);
        }

        sections.Add(section);
        return sections.ToArray();
    }

    private static string Sanitize(string name)
    {
        var s = new string(name.Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '-').ToArray());
        return string.IsNullOrEmpty(s) ? "P" : s;
    }
}
