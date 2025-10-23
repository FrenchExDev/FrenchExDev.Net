namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.References;

/// <summary>
/// Represents a package dependency that requires a specific version of the package.
/// </summary>
public record PackageVersionDependency : PackageDependency
{
    public required Version Version { get; init; }
}
