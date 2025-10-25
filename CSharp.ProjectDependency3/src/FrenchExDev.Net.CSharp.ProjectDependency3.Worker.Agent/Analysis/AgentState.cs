using System.Collections.Concurrent;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Agent.Analysis;

public record AnalysisArtifacts(string SolutionPath, string Markdown, DateTimeOffset CreatedAt);

public record AnalysisJob(string JobId, string SolutionPath, string Status, int Progress, DateTimeOffset StartedAt, DateTimeOffset? CompletedAt = null, string? Error = null);

public class AgentState
{
    private readonly ConcurrentDictionary<string, AnalysisArtifacts> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, AnalysisJob> _jobs = new(StringComparer.OrdinalIgnoreCase);

    public AnalysisArtifacts Upsert(string solutionPath, string markdown)
    {
        var artifacts = new AnalysisArtifacts(solutionPath, markdown, DateTimeOffset.UtcNow);
        _cache[solutionPath] = artifacts;
        return artifacts;
    }

    public bool TryGet(string solutionPath, out AnalysisArtifacts? value) => _cache.TryGetValue(solutionPath, out value);

    public string CreateJob(string solutionPath)
    {
        var jobId = Guid.NewGuid().ToString("N");
        var job = new AnalysisJob(jobId, solutionPath, "pending", 0, DateTimeOffset.UtcNow);
        _jobs[jobId] = job;
        return jobId;
    }

    public void UpdateJob(string jobId, string status, int progress, string? error = null)
    {
        if (!_jobs.TryGetValue(jobId, out var existing))
        {
            return;
        }

        var updated = existing with { Status = status, Progress = progress, Error = error, CompletedAt = status is "completed" or "failed" ? DateTimeOffset.UtcNow : existing.CompletedAt };
        _jobs[jobId] = updated;
    }

    public bool TryGetJob(string jobId, out AnalysisJob? job) => _jobs.TryGetValue(jobId, out job);
}
