using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency5.Core.Analysis;
using FrenchExDev.Net.CSharp.ProjectDependency5.Core.Markdown;

namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core.Reporting;

public class DirectionalCouplingReportGenerator : IReportGenerator<DirectionalCouplingResult>
{
    public string Name => "DirectionalCouplingReport";

    public MarkdownSection[] Generate(DirectionalCouplingResult result)
    {
        var sections = new List<MarkdownSection>();
        var section = new MarkdownSection("Directional Coupling (A -> B: unique types, member uses)");

        // Table per A
        foreach (var a in result.Coupling.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
        {
            var sub = new MarkdownSection(a.Key);
            sub.AddContent("Tip: A -> B means A uses B.");
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
