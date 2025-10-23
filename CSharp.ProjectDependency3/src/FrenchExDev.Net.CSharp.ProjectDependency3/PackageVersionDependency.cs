namespace FrenchExDev.Net.CSharp.ProjectDependency3;

public record PackageVersionDependency : PackageDependency
{
    public required Version Version { get; init; }
}

