using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.CodeQuality;

public sealed record ExceptionUsageMarkdownReport(string Markdown) : IProjectAnalysisReportResult;
