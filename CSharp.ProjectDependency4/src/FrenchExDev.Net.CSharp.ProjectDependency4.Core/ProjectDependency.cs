namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core;

public record ProjectDependency : IProjectDependency
{
    public required Project Owner { get; init; }
    public required Project Project { get; init; }
}

