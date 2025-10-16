using System.Text;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public class PackageMermaidGenerator : IPackageMermaidGenerator
{
    public string Generate(ProjectAnalysis project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        var sb = new StringBuilder();
        sb.AppendLine("```mermaid");
        sb.AppendLine("graph LR");
        var rootId = "proj";
        sb.AppendLine($"    {rootId}[\"{Escape(project.Name)}\"]");
        foreach (var pkg in project.PackageReferences ?? Array.Empty<PackageReference>())
        {
            var pid = Sanitize(pkg.Name);
            sb.AppendLine($"    {pid}[\"{Escape(pkg.Name)}\"]");
            sb.AppendLine($"    {rootId} --> {pid}");
        }
        sb.AppendLine("```");
        return sb.ToString();
    }

    private static string Sanitize(string s) => System.Text.RegularExpressions.Regex.Replace(s ?? "", "[^a-zA-Z0-9_]", "_");
    private static string Escape(string s) => (s ?? string.Empty).Replace("\"", "\\\"");
}
