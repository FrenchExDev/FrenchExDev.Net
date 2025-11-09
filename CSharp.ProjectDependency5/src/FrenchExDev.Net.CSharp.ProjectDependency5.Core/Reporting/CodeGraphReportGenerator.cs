using FrenchExDev.Net.CSharp.ProjectDependency5.Core.Analysis;
using FrenchExDev.Net.CSharp.ProjectDependency5.Core.Markdown;

namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core.Reporting;

/// <summary>
/// Generates a markdown report for CodeGraph analysis results.
/// </summary>
public class CodeGraphReportGenerator : IReportGenerator<CodeGraphResult>
{
    public string Name => "CodeGraphReport";

    public MarkdownSection[] Generate(CodeGraphResult result)
    {
var sections = new List<MarkdownSection>();
        var section = new MarkdownSection("Code Graph");

        section.AddContent($"Total nodes: **{result.Model.Nodes.Count}**");
        section.AddContent($"Total links: **{result.Model.Links.Count}**");

     // Summary by node kind
        var nodeKinds = result.Model.Nodes.GroupBy(n => n.Kind).OrderBy(g => g.Key);
    var kindSummary = new System.Text.StringBuilder();
kindSummary.AppendLine("### Node Summary");
  kindSummary.AppendLine("| Kind | Count |");
        kindSummary.AppendLine("|---|---:|");
  foreach (var g in nodeKinds)
        {
      kindSummary.AppendLine($"| {g.Key} | {g.Count()} |");
      }
        section.AddContent(kindSummary.ToString());

    // Summary by link kind
        var linkKinds = result.Model.Links.GroupBy(l => l.Kind).OrderBy(g => g.Key);
   var linkSummary = new System.Text.StringBuilder();
        linkSummary.AppendLine("### Link Summary");
   linkSummary.AppendLine("| Kind | Count |");
    linkSummary.AppendLine("|---|---:|");
   foreach (var g in linkKinds)
 {
          linkSummary.AppendLine($"| {g.Key} | {g.Count()} |");
        }
        section.AddContent(linkSummary.ToString());

   section.AddContent("Refer to the Graph Viewer for interactive visualization.");

        sections.Add(section);
 return sections.ToArray();
    }
}
