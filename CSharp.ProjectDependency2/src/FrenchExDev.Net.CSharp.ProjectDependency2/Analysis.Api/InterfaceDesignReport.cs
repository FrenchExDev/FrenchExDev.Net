using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public record InterfaceDesignReport(IReadOnlyList<InterfaceDesignItem> Interfaces) : IProjectAnalysisResult;
