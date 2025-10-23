using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.CodeQuality;

public sealed record ExceptionUsageDetailedReport(
    string ProjectName,
    IReadOnlyList<ExceptionUsageItem> Items
) : IProjectAnalysisReportResult;
