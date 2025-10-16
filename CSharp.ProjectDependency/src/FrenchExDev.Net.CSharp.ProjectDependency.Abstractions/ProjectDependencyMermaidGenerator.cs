using System.Text;
using System.Text.RegularExpressions;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

/// <summary>
/// Generates a mermaid graph representing project-to-project references.
/// </summary>
public class ProjectDependencyMermaidGenerator : IProjectDependencyMermaidGenerator
{
    // sanitize node ids for mermaid: keep letters, digits and replace others with underscore
    private static string SanitizeId(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return "unknown";
        var id = Regex.Replace(path, "[^a-zA-Z0-9]", "_");
        // ensure starts with letter
        if (!Regex.IsMatch(id, "^[A-Za-z]")) id = "p_" + id;
        return id;
    }

    public string Generate(SolutionAnalysis analysis)
    {
        if (analysis == null) throw new ArgumentNullException(nameof(analysis));

        var sb = new StringBuilder();
        sb.AppendLine("```mermaid");
        sb.AppendLine("graph LR");

        // nodes
        var projects = analysis.Projects;
        var fileToId = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in projects)
        {
            var id = SanitizeId(p.FilePath ?? p.Name ?? Guid.NewGuid().ToString());
            fileToId[p.FilePath ?? p.Name ?? string.Empty] = id;
            var label = !string.IsNullOrWhiteSpace(p.Name) ? p.Name : System.IO.Path.GetFileName(p.FilePath ?? string.Empty);
            sb.AppendLine($"    {id}[\"{EscapeLabel(label)}\"]");
        }

        // edges
        foreach (var p in projects)
        {
            var fromKey = p.FilePath ?? p.Name ?? string.Empty;
            if (!fileToId.TryGetValue(fromKey, out var fromId)) continue;

            foreach (var pref in p.ProjectReferences ?? Array.Empty<ProjectReference>())
            {
                var toPath = pref.Project?.FilePath ?? string.Empty;
                if (string.IsNullOrWhiteSpace(toPath)) continue;
                if (!fileToId.TryGetValue(toPath, out var toId))
                {
                    toId = SanitizeId(toPath);
                    fileToId[toPath] = toId;
                    var lbl = System.IO.Path.GetFileName(toPath);
                    sb.AppendLine($"    {toId}[\"{EscapeLabel(lbl)}\"]");
                }

                sb.AppendLine($"    {fromId} --> {toId}");
            }
        }

        sb.AppendLine("```");
        return sb.ToString();
    }

    private static string EscapeLabel(string label)
    {
        if (label == null) return string.Empty;
        return label.Replace("\"", "\\\"");
    }
}
