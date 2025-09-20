namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Represents a specific version of a package, identified by its version string.
/// </summary>
/// <param name="Version">The version string that uniquely identifies the package version. This value should follow the versioning scheme used
/// by the package source (for example, semantic versioning).</param>
public record PackageVersion(string Version) : IPackageVersion;
