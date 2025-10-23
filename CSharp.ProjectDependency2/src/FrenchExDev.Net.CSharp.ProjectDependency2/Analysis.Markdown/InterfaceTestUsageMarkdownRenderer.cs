using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Tests;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Markdown;

public sealed class InterfaceTestUsageMarkdownRenderer : IMarkdownReportRenderer
{
    public bool CanRender(IProjectAnalysisReportResult report) => report is InterfaceTestUsageDetailedReport;

    public MarkdownSection Render(IProjectAnalysisReportResult report)
    {
        var r = (InterfaceTestUsageDetailedReport)report;
        var sb = new StringBuilder();
        foreach (var i in r.Items)
        {
            var locs = i.SampleLocations != null && i.SampleLocations.Count > 0 ? " (" + string.Join(", ", i.SampleLocations) + ")" : string.Empty;
            sb.AppendLine($"- {i.InterfaceName}: {(i.UsedInTests ? "used" : "not used")}{locs}");
        }
        return new MarkdownSection("Interface test usage", "interface-test-usage", sb.ToString().TrimEnd());
    }
}
