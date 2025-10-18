namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public interface IProjectMarkdownFileGenerator
{
    /// <summary>
    /// Generate a markdown file for a single project under the specified output directory.
    /// Returns the path to the created file.
    /// </summary>
    string Generate(ProjectAnalysis project, string outputDirectory);
}
