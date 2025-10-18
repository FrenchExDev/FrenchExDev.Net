namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Represents a dependency relationship between two projects, indicating that one project depends on another within a
/// solution or build context.
/// </summary>
/// <remarks>Use this type to model project-to-project dependencies, such as when a project references another for
/// compilation or runtime purposes. Both the owner and dependent project must be specified to establish the
/// relationship.</remarks>
public record ProjectDependency : IProjectDependency
{
    public required Project Owner { get; init; }
    public required Project Project { get; init; }
}
