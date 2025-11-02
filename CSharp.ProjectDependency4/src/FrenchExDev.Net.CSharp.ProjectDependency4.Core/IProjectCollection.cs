namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core;

/// <summary>
/// Represents a collection of projects and provides functionality to load projects from specified file paths.
/// </summary>
/// <remarks>Implementations of this interface typically manage the lifecycle and access to multiple project
/// instances. Use this interface to load and interact with projects in a structured manner.</remarks>
public interface IProjectCollection
{
    /// <summary>
    /// Loads a project from the specified file path and returns a corresponding Project instance.
    /// </summary>
    /// <remarks>If the project file contains invalid XML or is not a supported MSBuild format, an exception
    /// may be thrown. The returned Project instance reflects the state of the project as defined in the file at the
    /// time of loading.</remarks>
    /// <param name="path">The full path to the project file to load. The file must exist and be a valid MSBuild project file.</param>
    /// <returns>A Project object representing the loaded MSBuild project.</returns>
    Microsoft.Build.Evaluation.Project LoadProject(string path);
}

