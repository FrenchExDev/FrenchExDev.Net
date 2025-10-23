using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Markdown;

public sealed class PublicApiMarkdownRenderer : IMarkdownReportRenderer
{
    public bool CanRender(IProjectAnalysisReportResult report) => report is PublicApiDetailedReport;

    public MarkdownSection Render(IProjectAnalysisReportResult report)
    {
        var r = (PublicApiDetailedReport)report;
        var sb = new StringBuilder();
        sb.AppendLine($"- Public types: {r.PublicTypeCount}");
        sb.AppendLine($"- Public members: {r.PublicMemberCount}");
        sb.AppendLine();
        foreach (var t in r.Types)
        {
            sb.AppendLine($"### {t.Kind} {t.Name}");
            foreach (var m in t.Members)
            {
                sb.AppendLine($"- {m}");
            }
            sb.AppendLine();
        }
        return new MarkdownSection("Public API", "public-api", sb.ToString().TrimEnd());
    }
}
