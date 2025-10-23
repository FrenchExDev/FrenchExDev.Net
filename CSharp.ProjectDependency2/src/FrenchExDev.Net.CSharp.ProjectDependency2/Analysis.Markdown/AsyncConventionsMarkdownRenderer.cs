using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.CodeQuality;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Markdown;

public sealed class AsyncConventionsMarkdownRenderer : IMarkdownReportRenderer
{
    public bool CanRender(IProjectAnalysisReportResult report) => report is AsyncConventionsDetailedReport;

    public MarkdownSection Render(IProjectAnalysisReportResult report)
    {
        var r = (AsyncConventionsDetailedReport)report;
        var sb = new StringBuilder();
        foreach (var i in r.Items)
        {
            sb.AppendLine($"- {i.Method}: suffix={(i.HasAsyncSuffix ? "ok" : "missing")}, blocksOnTask={(i.BlocksOnTask ? "yes" : "no")}");
        }
        return new MarkdownSection("Async conventions", "async-conventions", sb.ToString().TrimEnd());
    }
}
