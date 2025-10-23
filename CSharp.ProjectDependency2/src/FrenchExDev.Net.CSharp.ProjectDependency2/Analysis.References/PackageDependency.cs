namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.References;

/// <summary>
/// Represents a dependency on a package within a project.
/// </summary>
/// <remarks>This abstract record serves as a base type for describing package dependencies in project systems.
/// Implementations typically specify additional metadata or constraints relevant to the dependency. Use this type to
/// model relationships between a project and its required packages.</remarks>
public abstract record PackageDependency : IProjectDependency
{
    public required string Name { get; init; }
}
