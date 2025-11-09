namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core;

public abstract record PackageDependency : IProjectDependency
{
    public required string Name { get; init; }
}

