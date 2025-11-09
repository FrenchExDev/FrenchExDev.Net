namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core;

public record PackageVersionDependency : PackageDependency
{
    public required Version Version { get; init; }
}

