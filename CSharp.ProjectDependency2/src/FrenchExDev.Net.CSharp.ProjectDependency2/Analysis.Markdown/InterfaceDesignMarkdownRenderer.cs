using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Markdown;

public sealed class InterfaceDesignMarkdownRenderer : IMarkdownReportRenderer
{
    public bool CanRender(IProjectAnalysisReportResult report) => report is InterfaceDesignDetailedReport;

    public MarkdownSection Render(IProjectAnalysisReportResult report)
    {
        var r = (InterfaceDesignDetailedReport)report;
        var sb = new StringBuilder();
        foreach (var i in r.Items)
        {
            sb.AppendLine($"- {i.Name}: methods={i.MethodCount}, props={i.PropertyCount}, events={i.EventCount}, arity={i.GenericArity}, depth={i.InheritanceDepth}, startsWithI={i.NameStartsWithI}, tooLarge={i.TooLarge}, onlyProps={i.OnlyProperties}");
        }
        return new MarkdownSection("Interface design", "interface-design", sb.ToString().TrimEnd());
    }
}
