using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public sealed record PublicApiDetailedReport(
    string ProjectName,
    int PublicTypeCount,
    int PublicMemberCount,
    IReadOnlyList<PublicTypeInfo> Types
) : IProjectAnalysisReportResult;
