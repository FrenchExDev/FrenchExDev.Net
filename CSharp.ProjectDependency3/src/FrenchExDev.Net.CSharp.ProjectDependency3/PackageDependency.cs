namespace FrenchExDev.Net.CSharp.ProjectDependency3;

public abstract record PackageDependency : IProjectDependency
{
    public required string Name { get; init; }
}

