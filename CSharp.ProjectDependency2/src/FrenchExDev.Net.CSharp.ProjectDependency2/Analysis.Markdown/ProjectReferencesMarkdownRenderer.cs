using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.References;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Markdown;

public sealed class ProjectReferencesMarkdownRenderer : IMarkdownReportRenderer
{
    public bool CanRender(IProjectAnalysisReportResult report) => report is ProjectReferencesDetailedReport;

    public MarkdownSection Render(IProjectAnalysisReportResult report)
    {
        var r = (ProjectReferencesDetailedReport)report;
        var sb = new StringBuilder();
        sb.AppendLine($"Packages ({r.PackageCount})");
        foreach (var p in r.PackageNames)
        {
            sb.AppendLine($"- {p}");
        }
        sb.AppendLine();
        sb.AppendLine($"Projects ({r.ProjectReferenceCount})");
        foreach (var p in r.ProjectPaths)
        {
            sb.AppendLine($"- {p}");
        }
        return new MarkdownSection("Project references", "project-references", sb.ToString().TrimEnd());
    }
}
