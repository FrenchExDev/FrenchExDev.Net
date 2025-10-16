using FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;
using Xunit;

namespace FrenchExDev.Net.Csharp.ProjectDependency.Tests;

public class ProjectsMarkdownGeneratorTests
{
    [Fact]
    public void GenerateProjectsTableOfContents_Basic()
    {
        var projects = new[] {
            new ProjectAnalysis("A", "C:/proj/A/A.csproj", new PackageReference[0], new ProjectReference[0]),
            new ProjectAnalysis("B", "C:/proj/B/B.csproj", new PackageReference[0], new ProjectReference[0])
        };

        var gen = new ProjectsMarkdownGenerator();
        var toc = gen.GenerateProjectsTableOfContents(projects);
        Assert.Contains("# Projects Table of Contents", toc);
        Assert.Contains("[A](#a)", toc);
        Assert.Contains("[B](#b)", toc);
    }

    [Fact]
    public void GenerateWithTableOfContents_IncludesMermaidAndKpis()
    {
        var pa = new ProjectAnalysis("P1", "C:/proj/P1/P1.csproj", new[] { new PackageReference("Newtonsoft.Json", "13.0.1") }, new ProjectReference[0]);
        var analysis = new SolutionAnalysis(new[] { pa }, 1, 1, 1, 1.0, 0, new System.Collections.Generic.Dictionary<string,int>(), new System.Collections.Generic.Dictionary<string, ProjectMetrics>(), new SolutionIndicators(0,0,0,0,0));

        var gen = new ProjectsMarkdownGenerator();
        var md = gen.GenerateWithTableOfContents(analysis);

        Assert.Contains("# Projects Table of Contents", md);
        Assert.Contains("```mermaid", md);
        // ensure per-project diagrams present
        Assert.Contains("### Declarations", md);
        Assert.Contains("### Packages", md);
        Assert.Contains("### Project references", md);
    }

    [Fact]
    public void ConstructorInjection_AllowsCustomGenerators()
    {
        var pa = new ProjectAnalysis("P1", "C:/proj/P1/P1.csproj", new[] { new PackageReference("Newtonsoft.Json", "13.0.1") }, new ProjectReference[0]);
        var gen = new ProjectsMarkdownGenerator(new ProjectDependencyMermaidGenerator(), new DeclarationMermaidGenerator(), new PackageMermaidGenerator(), new ProjectReferencesMermaidGenerator());
        var md = gen.Generate(new[] { pa });
        Assert.Contains("### Declarations", md);
        Assert.Contains("### Packages", md);
        Assert.Contains("### Project references", md);
    }
}
