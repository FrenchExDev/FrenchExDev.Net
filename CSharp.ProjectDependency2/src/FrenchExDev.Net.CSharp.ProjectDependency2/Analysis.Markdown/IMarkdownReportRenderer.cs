using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Markdown;

public record MarkdownSection(string Title, string Anchor, string Content);

public interface IMarkdownReportRenderer
{
    bool CanRender(IProjectAnalysisReportResult report);
    MarkdownSection Render(IProjectAnalysisReportResult report);
}
