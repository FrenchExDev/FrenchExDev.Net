using System.Text;
using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Markdown;

public sealed class ProjectMarkdownFileGenerator
{
    private readonly List<IMarkdownReportRenderer> _renderers = new()
    {
        new ProjectReferencesMarkdownRenderer(),
        new PublicApiMarkdownRenderer(),
        new InterfaceDesignMarkdownRenderer(),
        new XmlDocCoverageMarkdownRenderer(),
        new InterfaceTestUsageMarkdownRenderer(),
        new AsyncConventionsMarkdownRenderer(),
        new ExceptionUsageMarkdownRenderer()
    };

    public Result<string> Generate(string projectName, IEnumerable<IProjectAnalysisReportResult> reports)
    {
        if (string.IsNullOrWhiteSpace(projectName))
            return Result<string>.Failure(d => d.Add("ArgumentNull", nameof(projectName)));
        if (reports is null)
            return Result<string>.Failure(d => d.Add("ArgumentNull", nameof(reports)));

        var sections = new List<MarkdownSection>();
        foreach (var r in reports)
        {
            var renderer = _renderers.FirstOrDefault(x => x.CanRender(r));
            if (renderer == null) continue;
            sections.Add(renderer.Render(r));
        }

        var sb = new StringBuilder();
        sb.AppendLine($"# {projectName}");
        sb.AppendLine();
        if (sections.Count > 0)
        {
            sb.AppendLine("## Table of contents");
            foreach (var s in sections)
            {
                sb.AppendLine($"- [{s.Title}](#{s.Anchor})");
            }
            sb.AppendLine();
        }

        foreach (var s in sections)
        {
            sb.AppendLine($"## {s.Title}");
            sb.AppendLine();
            sb.AppendLine(s.Content);
            sb.AppendLine();
        }

        return Result<string>.Success(sb.ToString());
    }
}
