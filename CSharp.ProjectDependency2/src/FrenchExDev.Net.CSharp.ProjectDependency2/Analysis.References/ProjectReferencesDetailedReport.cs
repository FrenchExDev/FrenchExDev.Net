using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.References;

/// <summary>
/// Structured report for project references to be used by markdown generators.
/// </summary>
public sealed record ProjectReferencesDetailedReport(
    string ProjectName,
    int PackageCount,
    IReadOnlyList<string> PackageNames,
    int ProjectReferenceCount,
    IReadOnlyList<string> ProjectPaths
) : IProjectAnalysisReportResult;
