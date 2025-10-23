using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Markdown;

public sealed class XmlDocCoverageMarkdownRenderer : IMarkdownReportRenderer
{
    public bool CanRender(IProjectAnalysisReportResult report) => report is XmlDocCoverageDetailedReport;

    public MarkdownSection Render(IProjectAnalysisReportResult report)
    {
        var r = (XmlDocCoverageDetailedReport)report;
        var percent = r.TotalCount == 0 ? 0 : (int)Math.Round(r.Coverage * 100);
        var sb = new StringBuilder();
        sb.AppendLine($"Coverage: {percent}% ({r.DocumentedCount}/{r.TotalCount})");
        sb.AppendLine();
        sb.AppendLine("#### Missing docs");
        foreach (var i in r.Items.Where(i => !i.HasDocs))
        {
            sb.AppendLine($"- {i.Symbol}");
        }
        return new MarkdownSection("XML doc coverage", "xml-doc-coverage", sb.ToString().TrimEnd());
    }
}
