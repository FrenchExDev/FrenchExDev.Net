namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Represents a collection of projects and provides functionality to load projects from specified file paths.
/// </summary>
/// <remarks>Implementations of this interface typically manage the lifecycle and access to multiple project
/// instances. Thread safety and project caching behavior may vary depending on the implementation.</remarks>
public interface IProjectCollection
{
    Microsoft.Build.Evaluation.Project LoadProject(string path);
}
