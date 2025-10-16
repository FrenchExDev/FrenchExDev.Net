using System.Text;
using System.Text.RegularExpressions;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

/// <summary>
/// Generates markdown that concatenates per-project markdown for a collection of projects.
/// </summary>
public class ProjectsMarkdownGenerator : IMarkdownGenerator<IEnumerable<ProjectAnalysis>>
{
    private readonly ProjectMarkdownGenerator _projectGenerator = new ProjectMarkdownGenerator();

    public string Generate(IEnumerable<ProjectAnalysis> projects)
    {
        if (projects == null) throw new ArgumentNullException(nameof(projects));

        var sb = new StringBuilder();

        foreach (var p in projects.OrderBy(p => p.Name))
        {
            sb.AppendLine(_projectGenerator.Generate(p));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Generates output based on the provided solution analysis.
    /// </summary>
    /// <param name="analysis">The solution analysis containing project information to use for generation. Cannot be null.</param>
    /// <returns>A string containing the generated output based on the analysis.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the analysis parameter is null.</exception>
    public string Generate(SolutionAnalysis analysis)
    {
        if (analysis == null) throw new ArgumentNullException(nameof(analysis));
        return Generate(analysis.Projects);
    }

    /// <summary>
    /// Generates a Markdown-formatted table of contents listing all projects in the specified solution analysis.
    /// </summary>
    /// <param name="analysis">The solution analysis containing the collection of projects to include in the table of contents. Cannot be null.</param>
    /// <returns>A string containing the Markdown table of contents for the projects in the solution. The string will be empty if
    /// there are no projects.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the analysis parameter is null.</exception>
    public string GenerateProjectsTableOfContents(SolutionAnalysis analysis)
    {
        if (analysis == null) throw new ArgumentNullException(nameof(analysis));

        return GenerateProjectsTableOfContents(analysis.Projects);
    }

    /// <summary>
    /// Generates a Markdown-formatted table of contents for a collection of projects.
    /// </summary>
    /// <remarks>Projects are listed in alphabetical order by name. If a project's name is null, the file name
    /// (without extension) is used instead. Each entry links to an anchor based on the project name.</remarks>
    /// <param name="projects">The collection of projects to include in the table of contents. Each project should provide a name and file
    /// path.</param>
    /// <returns>A string containing the Markdown table of contents, with each project listed as a link anchored by its name.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the projects parameter is null.</exception>
    public string GenerateProjectsTableOfContents(IEnumerable<ProjectAnalysis> projects)
    {
        if (projects == null) throw new ArgumentNullException(nameof(projects));

        var sb = new StringBuilder();
        sb.AppendLine("# Projects Table of Contents");
        sb.AppendLine();

        foreach (var p in projects.OrderBy(p => p.Name))
        {
            var name = p.Name ?? Path.GetFileNameWithoutExtension(p.FilePath ?? string.Empty) ?? "Unknown";
            var anchor = ToAnchor(name);
            sb.AppendLine($"- [{name}](#{anchor})");
        }

        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        return sb.ToString();
    }

    private static string ToAnchor(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        // normalize: lower, remove accents, replace non-alnum with hyphen, collapse hyphens
        var lower = text.ToLowerInvariant();
        // remove characters except letters, digits, space
        var cleaned = Regex.Replace(lower, "[^a-z0-9 _-]", string.Empty);
        // replace spaces/underscores with hyphen
        cleaned = Regex.Replace(cleaned, "[ _]+", "-");
        // collapse multiple hyphens
        cleaned = Regex.Replace(cleaned, "-+", "-");
        // trim
        cleaned = cleaned.Trim('-');
        return cleaned;
    }

    /// <summary>
    /// Generate projects markdown with TOC inserted before project contents.
    /// </summary>
    public string GenerateWithTableOfContents(SolutionAnalysis analysis)
    {
        if (analysis == null) throw new ArgumentNullException(nameof(analysis));

        var sb = new StringBuilder();
        sb.AppendLine(GenerateProjectsTableOfContents(analysis));
        sb.AppendLine(Generate(analysis));
        return sb.ToString();
    }

    public string GenerateWithTableOfContents(IEnumerable<ProjectAnalysis> projects)
    {
        if (projects == null) throw new ArgumentNullException(nameof(projects));
        var sb = new StringBuilder();
        sb.AppendLine(GenerateProjectsTableOfContents(projects));
        sb.AppendLine(Generate(projects));
        return sb.ToString();
    }
}
