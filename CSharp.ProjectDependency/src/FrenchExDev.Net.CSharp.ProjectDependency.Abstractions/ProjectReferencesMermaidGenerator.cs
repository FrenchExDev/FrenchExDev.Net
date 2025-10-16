using System.Text;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public class ProjectReferencesMermaidGenerator : IProjectReferencesMermaidGenerator
{
    public string Generate(ProjectAnalysis project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        var sb = new StringBuilder();
        sb.AppendLine("```mermaid");
        sb.AppendLine("graph LR");
        var rootId = "proj";
        sb.AppendLine($"    {rootId}[\"{Escape(project.Name)}\"]");
        foreach (var pref in project.ProjectReferences ?? Array.Empty<ProjectReference>())
        {
            var to = pref.Project?.FilePath ?? pref.Project?.Name ?? "unknown";
            var id = Sanitize(to);
            var lbl = pref.Project?.Name ?? System.IO.Path.GetFileName(to);
            sb.AppendLine($"    {id}[\"{Escape(lbl)}\"]");
            sb.AppendLine($"    {rootId} --> {id}");
        }
        sb.AppendLine("```");
        return sb.ToString();
    }

    private static string Sanitize(string s) => System.Text.RegularExpressions.Regex.Replace(s ?? "", "[^a-zA-Z0-9_]", "_");
    private static string Escape(string s) => (s ?? string.Empty).Replace("\"", "\\\"");
}
