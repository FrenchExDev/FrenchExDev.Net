namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public interface IProjectReferencesMermaidGenerator
{
    string Generate(ProjectAnalysis project);
}
