using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public sealed record InterfaceDesignDetailedReport(
    string ProjectName,
    IReadOnlyList<InterfaceDesignItem> Items
) : IProjectAnalysisReportResult;
