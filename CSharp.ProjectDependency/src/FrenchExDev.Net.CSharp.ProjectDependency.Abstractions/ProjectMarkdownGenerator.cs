using System;
using System.Linq;
using System.Text;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

/// <summary>
/// Generates markdown for a single project analysis.
/// </summary>
public class ProjectMarkdownGenerator : IMarkdownGenerator<ProjectAnalysis>
{
    public string Generate(ProjectAnalysis p)
    {
        if (p == null) throw new ArgumentNullException(nameof(p));

        var sb = new StringBuilder();
        sb.AppendLine($"# {p.Name}");
        sb.AppendLine();
        sb.AppendLine($"`Path:` {p.FilePath}");
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
