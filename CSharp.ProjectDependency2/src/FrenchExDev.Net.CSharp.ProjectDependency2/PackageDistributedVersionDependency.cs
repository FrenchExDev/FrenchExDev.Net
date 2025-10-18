namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Represents a package dependency that is distributed with a specific version constraint.
/// </summary>
/// <remarks>Use this type to specify dependencies on packages where the required version must be explicitly
/// defined and distributed. This is commonly used in scenarios where package versioning is critical for compatibility
/// or deployment.</remarks>
public record PackageDistributedVersionDependency : PackageDependency
{

}
