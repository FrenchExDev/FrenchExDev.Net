using System.Text;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public class DeclarationMermaidGenerator : IDeclarationMermaidGenerator
{
    public string Generate(ProjectAnalysis project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));

        var sb = new StringBuilder();
        sb.AppendLine("```mermaid");
        sb.AppendLine("classDiagram");

        var decls = (project.Declarations ?? Array.Empty<DeclarationDescriptor>())
            .Where(d => d != null)
            .Select(d => new { Name = d.Name ?? string.Empty, Kind = (d.Kind ?? string.Empty).ToLowerInvariant(), d.IsAbstract })
            .Where(d => d.Kind == "class" || d.Kind == "interface" || d.Kind == "struct" || d.Kind == "enum" || d.Kind == "record")
            .OrderBy(d => d.Kind)
            .ThenBy(d => d.Name)
            .ToList();

        var usedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var d in decls)
        {
            var idBase = Sanitize(d.Name);
            var id = idBase;
            var suffix = 1;
            while (usedIds.Contains(id))
            {
                id = idBase + "_" + suffix++;
            }
            usedIds.Add(id);

            // declare the node according to kind
            switch (d.Kind)
            {
                case "interface":
                    sb.AppendLine($"    interface {id}");
                    break;
                case "enum":
                    sb.AppendLine($"    class {id} <<enum>>");
                    break;
                case "struct":
                    sb.AppendLine($"    class {id} <<struct>>");
                    break;
                case "record":
                    sb.AppendLine($"    class {id} <<record>>");
                    break;
                case "class":
                default:
                    if (d.IsAbstract)
                        sb.AppendLine($"    abstract class {id}");
                    else
                        sb.AppendLine($"    class {id}");
                    break;
            }

            // add a small note to display the full original name (namespace qualified)
            var safeName = Escape(d.Name);
            if (!string.IsNullOrWhiteSpace(safeName))
            {
                // place note to the right for readability
                sb.AppendLine($"    note right of {id} : {safeName}");
            }
        }

        if (!decls.Any())
        {
            var n = Sanitize(project.Name ?? System.IO.Path.GetFileName(project.FilePath ?? "project"));
            sb.AppendLine($"    class {n} {{");
            sb.AppendLine("        // no public declarations detected");
            sb.AppendLine("    }");
        }

        sb.AppendLine("```");
        return sb.ToString();
    }

    private static string Sanitize(string s) => string.IsNullOrWhiteSpace(s) ? "project" : System.Text.RegularExpressions.Regex.Replace(s, "[^a-zA-Z0-9_]", "_");
    private static string Escape(string s) => (s ?? string.Empty).Replace("\"", "\\\"");
}
