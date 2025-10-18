using System.Text;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public class ProjectMarkdownFileGenerator : IProjectMarkdownFileGenerator
{
    private readonly IProjectMarkdownGenerator _projectGenerator = new ProjectMarkdownGenerator();
    private readonly IDeclarationMermaidGenerator _declarationMermaid;
    private readonly IPackageMermaidGenerator _packageMermaid;
    private readonly IProjectReferencesMermaidGenerator _projectRefsMermaid;

    public ProjectMarkdownFileGenerator(
        IProjectMarkdownGenerator? projectMarkdownGenerator = null,
        IDeclarationMermaidGenerator? declarationMermaid = null,
        IPackageMermaidGenerator? packageMermaid = null,
        IProjectReferencesMermaidGenerator? projectRefsMermaid = null
    )
    {
        _projectGenerator = projectMarkdownGenerator ?? new ProjectMarkdownGenerator();
        _declarationMermaid = declarationMermaid ?? new DeclarationMermaidGenerator();
        _packageMermaid = packageMermaid ?? new PackageMermaidGenerator();
        _projectRefsMermaid = projectRefsMermaid ?? new ProjectReferencesMermaidGenerator();
    }

    public string Generate(ProjectAnalysis project, string outputDirectory)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        if (string.IsNullOrWhiteSpace(outputDirectory)) throw new ArgumentNullException(nameof(outputDirectory));

        var fileName = SanitizeFileName(project.Name ?? Path.GetFileNameWithoutExtension(project.FilePath ?? "project")) + ".md";
        var outPath = Path.Combine(outputDirectory, fileName);

        Directory.CreateDirectory(outputDirectory);

        var sb = new StringBuilder();
        sb.AppendLine(_projectGenerator.Generate(project));
        sb.AppendLine("## Declarations");
        sb.AppendLine(_declarationMermaid.Generate(project));
        sb.AppendLine("## Packages");
        sb.AppendLine(_packageMermaid.Generate(project));
        sb.AppendLine("## Project references");
        sb.AppendLine(_projectRefsMermaid.Generate(project));

        return sb.ToString();
    }

    private static string SanitizeFileName(string s)
    {
        var name = string.IsNullOrWhiteSpace(s) ? "project" : s;
        return System.Text.RegularExpressions.Regex.Replace(name, "[^a-zA-Z0-9_.-]", "_");
    }
}
