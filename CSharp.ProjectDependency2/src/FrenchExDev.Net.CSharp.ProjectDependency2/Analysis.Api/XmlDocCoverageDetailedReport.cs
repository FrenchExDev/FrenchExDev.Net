using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public sealed record XmlDocCoverageDetailedReport(
    string ProjectName,
    double Coverage,
    int DocumentedCount,
    int TotalCount,
    IReadOnlyList<XmlDocCoverageItem> Items
) : IProjectAnalysisReportResult;
