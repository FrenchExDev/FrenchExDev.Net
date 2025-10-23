namespace FrenchExDev.Net.CSharp.ProjectDependency3;

public record ProjectDependency : IProjectDependency
{
    public required Project Owner { get; init; }
    public required Project Project { get; init; }
}

