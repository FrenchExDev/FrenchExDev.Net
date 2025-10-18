using System;
using System.Linq;
using System.Text;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public interface IProjectMarkdownGenerator : IMarkdownGenerator<ProjectAnalysis>
{
}

/// <summary>
/// Generates markdown for a single project analysis.
/// </summary>
public class ProjectMarkdownGenerator : IProjectMarkdownGenerator
{
    public string Generate(ProjectAnalysis p)
    {
        if (p == null) throw new ArgumentNullException(nameof(p));

        var sb = new StringBuilder();
        sb.AppendLine($"# {p.Name}");
        sb.AppendLine();
        sb.AppendLine($"`Path:` {p.FilePath}");
        sb.AppendLine();

        // Key metrics if available on the ProjectAnalysis
        sb.AppendLine("### Key metrics");
        var printed = false;
        var hasDetailed = p.CoreKpis != null || p.CodeMetrics != null || p.QualityMetrics != null || p.ChurnMetrics != null || p.DerivedIndicators != null;
        var hasFallback = (p.PackageReferences != null && p.PackageReferences.Any()) || (p.ProjectReferences != null && p.ProjectReferences.Any()) || p.Constructs != null || (p.ReferenceCouplings != null && p.ReferenceCouplings.Any());

        if (hasDetailed || hasFallback)
        {
            sb.AppendLine("Metric | Value");
            sb.AppendLine("--- | ---");

            if (p.CoreKpis != null)
            {
                sb.AppendLine($"Times used | {p.CoreKpis.TimesUsed}");
                sb.AppendLine($"Outgoing project refs | {p.CoreKpis.OutgoingProjectReferences}");
                sb.AppendLine($"NuGet references | {p.CoreKpis.NuGetReferences}");
            }
            else
            {
                // fallback minimal core metrics
                var outgoing = p.ProjectReferences?.Count ?? 0;
                var nuget = p.PackageReferences?.Count ?? 0;
                sb.AppendLine($"Times used | -");
                sb.AppendLine($"Outgoing project refs | {outgoing}");
                sb.AppendLine($"NuGet references | {nuget}");
            }

            if (p.CodeMetrics != null)
            {
                sb.AppendLine($"Source files | {p.CodeMetrics.SourceFileCount}");
                sb.AppendLine($"Lines of code | {p.CodeMetrics.TotalLinesOfCode}");
                sb.AppendLine($"Comment lines | {p.CodeMetrics.CommentLines}");
                sb.AppendLine($"Comment density | {p.CodeMetrics.CommentDensity:P1}");
            }
            else
            {
                // if constructs exist, show counts as proxy
                if (p.Constructs != null)
                {
                    sb.AppendLine($"Exported records | {p.Constructs.Records}");
                    sb.AppendLine($"Exported classes | {p.Constructs.Classes}");
                    sb.AppendLine($"Exported interfaces | {p.Constructs.Interfaces}");
                }
            }

            if (p.QualityMetrics != null)
            {
                sb.AppendLine($"Diagnostics | {p.QualityMetrics.DiagnosticsCount}");
                sb.AppendLine($"Cyclomatic complexity | {p.QualityMetrics.CyclomaticComplexity}");
            }

            if (p.ChurnMetrics != null)
            {
                sb.AppendLine($"Commit count | {p.ChurnMetrics.CommitCount}");
                sb.AppendLine($"Last commit | {p.ChurnMetrics.LastCommitDate?.ToString("u") ?? "-"}");
            }

            if (p.DerivedIndicators != null)
            {
                sb.AppendLine($"Maintainability index | {p.DerivedIndicators.MaintainabilityIndex:F1}");
                sb.AppendLine($"Testability index | {p.DerivedIndicators.TestabilityIndex:F1}");
                sb.AppendLine($"Hotspot score | {p.DerivedIndicators.HotspotScore:F2}");
            }

            printed = true;
        }

        if (!printed)
        {
            sb.AppendLine("_No KPI data available._");
        }

        sb.AppendLine();

        sb.AppendLine("### Exported constructs");
        if (p.Constructs == null)
        {
            sb.AppendLine("_No construct analysis available._");
        }
        else
        {
            sb.AppendLine($"- Records: **{p.Constructs.Records}**");
            sb.AppendLine($"- Enums: **{p.Constructs.Enums}**");
            sb.AppendLine($"- Classes: **{p.Constructs.Classes}**");
            sb.AppendLine($"- Interfaces: **{p.Constructs.Interfaces}**");
            sb.AppendLine($"- Structs: **{p.Constructs.Structs}**");
            sb.AppendLine($"- Extension methods: **{p.Constructs.ExtensionMethods}**");
            sb.AppendLine($"- Public members: **{p.Constructs.PublicMembersCount}**");
        }

        sb.AppendLine();

        // declarations mermaid diagram
        sb.AppendLine("### Declarations diagram");
        sb.AppendLine(new DeclarationMermaidGenerator().Generate(p));
        sb.AppendLine();

        sb.AppendLine("### Package references");
        if (p.PackageReferences == null || !p.PackageReferences.Any())
        {
            sb.AppendLine("_Aucune dépendance NuGet._");
        }
        else
        {
            foreach (var pkg in p.PackageReferences)
            {
                sb.AppendLine($"- {pkg.Name}{(string.IsNullOrWhiteSpace(pkg.Version) ? string.Empty : " (" + pkg.Version + ")")} ");
            }
        }
        sb.AppendLine();

        // package mermaid diagram
        sb.AppendLine("### Package dependency diagram");
        sb.AppendLine(new PackageMermaidGenerator().Generate(p));
        sb.AppendLine();

        sb.AppendLine("### Project references");
        if (p.ProjectReferences == null || !p.ProjectReferences.Any())
        {
            sb.AppendLine("_Aucune référence de projet._");
        }
        else
        {
            foreach (var pref in p.ProjectReferences)
            {
                var refPath = pref.Project?.FilePath ?? "unknown";
                var refName = pref.Project?.Name ?? System.IO.Path.GetFileName(refPath);
                sb.AppendLine($"- {refName} — `{refPath}`");
            }
        }

        sb.AppendLine();

        // project references mermaid diagram
        sb.AppendLine("### Project references diagram");
        sb.AppendLine(new ProjectReferencesMermaidGenerator().Generate(p));
        sb.AppendLine();

        sb.AppendLine("### Reference coupling");
        if (p.ReferenceCouplings == null || !p.ReferenceCouplings.Any())
        {
            sb.AppendLine("_No coupling data._");
        }
        else
        {
            sb.AppendLine("Reference | Coupling | TotalUses | InterfaceUses | ClassUses");
            sb.AppendLine("--- | --- | ---: | ---: | ---:");
            foreach (var rc in p.ReferenceCouplings)
            {
                var name = System.IO.Path.GetFileName(rc.ReferencedProjectPath) ?? rc.ReferencedProjectPath;
                sb.AppendLine($"{name} | {rc.Level} | {rc.TotalUsages} | {rc.InterfaceUsages} | {rc.ClassUsages}");
            }
        }

        sb.AppendLine();

        return sb.ToString();
    }
}
