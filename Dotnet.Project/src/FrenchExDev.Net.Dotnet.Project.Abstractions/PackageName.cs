namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Represents the name of a NuGet package or similar software package.
/// </summary>
/// <param name="Name">The name of the package. Cannot be null.</param>
public record PackageName(string Name);
