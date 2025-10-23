using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Tests;

public sealed record InterfaceTestUsageDetailedReport(
    string ProjectName,
    IReadOnlyList<InterfaceTestUsageItem> Items
) : IProjectAnalysisReportResult;
