namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core;

public abstract record PackageDependency : IProjectDependency
{
    public required string Name { get; init; }
}

