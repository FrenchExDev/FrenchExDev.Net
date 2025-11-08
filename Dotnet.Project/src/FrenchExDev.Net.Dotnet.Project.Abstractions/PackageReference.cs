namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Represents a reference to a NuGet package by name.
/// </summary>
/// <param name="Name">The name of the NuGet package being referenced. Cannot be null.</param>
public record PackageReference(PackageName Name) : IPackageReference;
