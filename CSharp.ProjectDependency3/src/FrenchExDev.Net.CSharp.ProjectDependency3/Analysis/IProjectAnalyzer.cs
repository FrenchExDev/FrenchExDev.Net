namespace FrenchExDev.Net.CSharp.ProjectDependency3;

public interface IProjectAnalyzer
{
 string Name { get; }
 Task<object> AnalyzeAsync(Solution solution, CancellationToken cancellationToken = default);
}
