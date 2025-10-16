namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public interface IPackageMermaidGenerator
{
    string Generate(ProjectAnalysis project);
}
