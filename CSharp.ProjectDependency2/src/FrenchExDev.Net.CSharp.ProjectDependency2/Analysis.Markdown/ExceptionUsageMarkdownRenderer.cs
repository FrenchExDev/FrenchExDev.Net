using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.CodeQuality;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Markdown;

public sealed class ExceptionUsageMarkdownRenderer : IMarkdownReportRenderer
{
    public bool CanRender(IProjectAnalysisReportResult report) => report is ExceptionUsageDetailedReport;

    public MarkdownSection Render(IProjectAnalysisReportResult report)
    {
        var r = (ExceptionUsageDetailedReport)report;
        var sb = new StringBuilder();
        foreach (var i in r.Items)
        {
            sb.AppendLine($"- {i.File}:{i.Line} nakedCatch={(i.NakedCatch ? "yes" : "no")}, broadCatch={(i.BroadCatch ? "yes" : "no")}, rethrowWrong={(i.RethrowWrong ? "yes" : "no")}");
        }
        return new MarkdownSection("Exception usage", "exception-usage", sb.ToString().TrimEnd());
    }
}
