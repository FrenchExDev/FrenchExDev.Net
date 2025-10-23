using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Dependencies;

public sealed record ProjectReferencesMarkdownReport(string Markdown) : IProjectAnalysisReportResult;
