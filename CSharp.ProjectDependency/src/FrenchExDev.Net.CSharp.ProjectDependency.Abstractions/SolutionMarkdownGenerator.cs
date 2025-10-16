using System.Text;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

/// <summary>
/// Generates a markdown report for the whole solution, including per-project sections and a mermaid diagram.
/// </summary>
public class SolutionMarkdownGenerator : IMarkdownGenerator<SolutionAnalysis>
{
    private readonly IMermaidGenerator _mermaid;
    private readonly ProjectMarkdownGenerator _projectGenerator = new ProjectMarkdownGenerator();

    public SolutionMarkdownGenerator(IMermaidGenerator? mermaid = null)
    {
        _mermaid = mermaid ?? new MermaidGenerator();
    }

    public string Generate(SolutionAnalysis analysis)
    {
        if (analysis == null) throw new ArgumentNullException(nameof(analysis));

        var sb = new StringBuilder();
        sb.AppendLine($"# Solution analysis report");
        sb.AppendLine();

        sb.AppendLine("## Key indicators");
        sb.AppendLine();
        sb.AppendLine($"- Total projects: **{analysis.TotalProjects}**");
        sb.AppendLine($"- Total package references: **{analysis.TotalPackageReferences}**");
        sb.AppendLine($"- Total unique packages: **{analysis.TotalUniquePackages}**");
        sb.AppendLine($"- Average packages per project: **{analysis.AveragePackagesPerProject:F2}**");
        sb.AppendLine($"- Total project references: **{analysis.TotalProjectReferences}**");
        sb.AppendLine();

        sb.AppendLine("## Dependency graph");
        sb.AppendLine();
        sb.AppendLine(_mermaid.Generate(analysis));
        sb.AppendLine();

        sb.AppendLine("## Projects");
        sb.AppendLine();

        foreach (var p in analysis.Projects.OrderBy(p => p.Name))
        {
            sb.AppendLine(_projectGenerator.Generate(p));
        }

        // package popularity table
        sb.AppendLine("## Package popularity");
        sb.AppendLine();

        if (analysis.PackageReferenceCounts == null || analysis.PackageReferenceCounts.Count == 0)
        {
            sb.AppendLine("_No package references found._");
        }
        else
        {
            sb.AppendLine("Package | Count");
            sb.AppendLine("--- | ---");
            foreach (var kv in analysis.PackageReferenceCounts.OrderByDescending(kv => kv.Value))
            {
                sb.AppendLine($"{kv.Key} | {kv.Value}");
            }
        }

        return sb.ToString();
    }
}
