namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Services;

public interface ISessionManager
{
 string GetOrCreateSessionId();
 Task SetLastSolutionPathAsync(string? slnPath);
 Task<string?> GetLastSolutionPathAsync();
 Task SetAgentUrlAsync(string? url);
 Task<string?> GetAgentUrlAsync();
 Task SetSessionIdAsync(string? id);
 Task<string?> GetSessionIdAsync();
}
