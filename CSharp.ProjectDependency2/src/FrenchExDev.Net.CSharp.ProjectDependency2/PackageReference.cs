namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Represents a reference to a NuGet package within a project.
/// </summary>
/// <param name="Package">The package dependency referenced by the project. Cannot be null.</param>
public record PackageReference(PackageDependency Package);
