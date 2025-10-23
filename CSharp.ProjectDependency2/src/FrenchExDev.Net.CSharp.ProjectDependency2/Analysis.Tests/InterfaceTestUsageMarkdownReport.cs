using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Tests;

public sealed record InterfaceTestUsageMarkdownReport(string Markdown) : IProjectAnalysisReportResult;
