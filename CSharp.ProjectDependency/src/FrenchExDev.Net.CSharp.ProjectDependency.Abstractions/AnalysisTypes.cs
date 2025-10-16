namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

/// <summary>
/// Coupling level classification between projects.
/// </summary>
public enum CouplingLevel
{
    Low,
    High
}

/// <summary>
/// Represents coupling details from one project to a referenced project.
/// </summary>
public record ReferenceCoupling(
    string ReferencedProjectPath,
    CouplingLevel Level,
    int TotalUsages,
    int InterfaceUsages,
    int ClassUsages
);

/// <summary>
/// Counts exported constructs for a project (public API surface).
/// </summary>
public record ProjectConstructMetrics(
    int Records,
    int Enums,
    int Classes,
    int Interfaces,
    int Structs,
    int ExtensionMethods,
    int PublicMembersCount
);