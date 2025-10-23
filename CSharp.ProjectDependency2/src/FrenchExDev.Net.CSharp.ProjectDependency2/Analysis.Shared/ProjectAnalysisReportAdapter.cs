using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

/// <summary>
/// Adapts an IProjectAnalysisResult into an IProjectAnalysisReportResult.
/// </summary>
public sealed record ProjectAnalysisReportAdapter(IProjectAnalysisResult InnerResult) : IProjectAnalysisReportResult;
