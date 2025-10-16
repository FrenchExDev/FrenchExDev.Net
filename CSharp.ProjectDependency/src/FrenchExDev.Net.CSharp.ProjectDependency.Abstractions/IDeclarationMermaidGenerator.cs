namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public interface IDeclarationMermaidGenerator
{
    string Generate(ProjectAnalysis project);
}
