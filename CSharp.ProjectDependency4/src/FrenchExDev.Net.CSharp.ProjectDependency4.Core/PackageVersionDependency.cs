namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core;

public record PackageVersionDependency : PackageDependency
{
    public required Version Version { get; init; }
}

