using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public sealed record InterfaceDesignMarkdownReport(string Markdown) : IProjectAnalysisReportResult;
