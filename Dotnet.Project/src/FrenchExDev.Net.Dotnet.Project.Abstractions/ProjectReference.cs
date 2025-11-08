namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Represents a reference to another project within a .NET solution.
/// </summary>
/// <param name="ReferencedProject"></param>
public record ProjectReference(IProjectModel ReferencedProject);
