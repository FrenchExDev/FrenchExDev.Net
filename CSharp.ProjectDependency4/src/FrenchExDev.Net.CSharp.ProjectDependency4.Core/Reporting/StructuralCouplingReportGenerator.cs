using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency4.Core.Analysis;
using FrenchExDev.Net.CSharp.ProjectDependency4.Core.Markdown;

namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core.Reporting;

public class StructuralCouplingReportGenerator : IReportGenerator<StructuralCouplingResult>
{
    public string Name => "StructuralCouplingReport";

    public MarkdownSection[] Generate(StructuralCouplingResult result)
    {
        var sections = new List<MarkdownSection>();
        var section = new MarkdownSection("Structural Coupling");

        // Normalized table: one row per direct reference (Project -> Referenced project/path)
        var table = new StringBuilder();
        table.AppendLine("| Project | Referenced Project | Referenced Path |");
        table.AppendLine("|---|---|---|");
        foreach (var kv in result.ProjectReferences.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (kv.Value.Count == 0)
            {
                table.AppendLine($"| {kv.Key} | (none) | | ");
                continue;
            }
            foreach (var refPath in kv.Value)
            {
                var refName = System.IO.Path.GetFileNameWithoutExtension(refPath);
                table.AppendLine($"| {kv.Key} | {refName} | {refPath} |");
            }
        }
        section.AddContent(table.ToString());

        // Mermaid graph (project references)
        var mermaid = new StringBuilder();
        mermaid.AppendLine("```mermaid");
        mermaid.AppendLine("graph LR");
        foreach (var kv in result.ProjectReferences)
        {
            var from = Sanitize(kv.Key);
            foreach (var toPath in kv.Value)
            {
                var to = Sanitize(System.IO.Path.GetFileNameWithoutExtension(toPath));
                mermaid.AppendLine($" {from} --> {to}");
            }
        }
        mermaid.AppendLine("```");
        section.AddContent(mermaid.ToString());

        sections.Add(section);
        return sections.ToArray();
    }

    private static string Sanitize(string name)
    {
        var s = new string(name.Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '-').ToArray());
        return string.IsNullOrEmpty(s) ? "P" : s;
    }
}
