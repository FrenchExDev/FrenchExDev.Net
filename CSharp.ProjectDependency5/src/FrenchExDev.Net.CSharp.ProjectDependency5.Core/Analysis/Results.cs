namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core.Analysis;

public interface IProjectAnalysisResult
{
 string AnalyzerName { get; }
}

public record StructuralCouplingResult(string AnalyzerName, IReadOnlyDictionary<string, IReadOnlyList<string>> ProjectReferences) : IProjectAnalysisResult;

public record ClassicalCouplingResult(string AnalyzerName, IReadOnlyDictionary<string, (int Ca, int Ce, double Instability)> Metrics,
 IReadOnlyDictionary<string, IReadOnlyList<string>> Incoming,
 IReadOnlyDictionary<string, IReadOnlyList<string>> Outgoing) : IProjectAnalysisResult;

public record DirectionalCouplingResult(string AnalyzerName, IReadOnlyDictionary<string, IReadOnlyDictionary<string, (int UniqueTypes, int MemberUses)>> Coupling) : IProjectAnalysisResult;

public record CodeGraphResult(string AnalyzerName, GraphModel Model) : IProjectAnalysisResult;
