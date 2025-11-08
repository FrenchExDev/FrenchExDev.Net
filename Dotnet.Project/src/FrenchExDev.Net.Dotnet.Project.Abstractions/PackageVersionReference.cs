using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Represents a reference to a specific version of a package, including its name and version information.
/// </summary>
/// <param name="Name">The name of the package being referenced.</param>
/// <param name="PackageVersion">A reference to the version information of the package.</param>
public record PackageVersionReference(PackageName Name, Reference<IPackageVersion> PackageVersion) : IPackageReference;
