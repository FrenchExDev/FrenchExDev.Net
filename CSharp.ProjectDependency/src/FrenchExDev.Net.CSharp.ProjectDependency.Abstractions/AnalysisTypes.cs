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

/// <summary>
/// Core KPIs for a project.
/// </summary>
public record ProjectCoreKpis(
    int TimesUsed,
    int OutgoingProjectReferences,
    int NuGetReferences
);

/// <summary>
/// Code-level metrics for a project.
/// </summary>
public record ProjectCodeMetrics(
    int SourceFileCount,
    int TotalLinesOfCode,
    int CommentLines,
    double CommentDensity
);

/// <summary>
/// Quality and complexity metrics.
/// </summary>
public record ProjectQualityMetrics(
    int DiagnosticsCount,
    int CyclomaticComplexity
);

/// <summary>
/// Churn / git metrics for a project.
/// </summary>
public record ProjectChurnMetrics(
    int CommitCount,
    DateTimeOffset? LastCommitDate
);

/// <summary>
/// Derived indicators computed from basic metrics.
/// </summary>
public record DerivedProjectIndicators(
    double MaintainabilityIndex,
    double TestabilityIndex,
    double HotspotScore
);

/// <summary>
/// Aggregated solution-level indicators.
/// </summary>
public record SolutionIndicators(
    int TotalSourceFiles,
    int TotalLinesOfCode,
    int TotalDiagnostics,
    double AverageCyclomaticComplexity,
    double AverageMaintainabilityIndex
);