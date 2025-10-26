using Blazored.LocalStorage;
using FrenchExDev.Net.CSharp.Object.Result;
using System.Text;
using System.Text.Json;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Services;

/// <summary>
/// Client-side service for project dependency analysis. Communicates with orchestrator
/// via Registry API to obtain orchestrator URLs dynamically.
/// </summary>
public class AnalysisService : IAnalysisService
{
    private readonly ISessionManager _sessions;
    private readonly ILocalStorageService _local;
    private readonly string _registryApiUrl;
    private const string GraphFileName = "graph.json";
    private const string MarkdownFileName = "output.md";
    private const string ReportingBase = "Reporting";

    public AnalysisService(ISessionManager sessions, ILocalStorageService local, string registryApiUrl)
    {
        _sessions = sessions;
        _local = local;
        _registryApiUrl = registryApiUrl;
    }

    public async Task<AnalysisRunResult> AnalyzeSolutionAsync(string solutionPath, CancellationToken ct = default)
    {
        // Persist last used path for UX
        await _sessions.SetLastSolutionPathAsync(solutionPath);
        var sid = _sessions.GetOrCreateSessionId();

        // Get orchestrator URL from Registry API
        var orchestratorResult = await GetOrchestratorUrlAsync(ct);

        if (orchestratorResult.IsFailure)
        {
            var errorMessage = orchestratorResult.FailuresOrThrow().FirstOrDefault().Value?.ToString()
                              ?? "No orchestrator available";
            var md = BuildErrorMarkdown(errorMessage);
            return new AnalysisRunResult(sid, solutionPath, "", md);
        }

        var orchestratorUrl = orchestratorResult.ObjectOrThrow();

        // Point to static reporting directory
        var graphUrl = $"{ReportingBase}/{GraphFileName}";

        // Try to read embedded markdown if shipped; otherwise, point to instructions
        var markdown = await LoadBundledMarkdownAsync() ?? BuildHelpMarkdown(orchestratorUrl);

        return new AnalysisRunResult(sid, solutionPath, graphUrl, markdown);
    }

    private async Task<Result<string>> GetOrchestratorUrlAsync(CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrEmpty(_registryApiUrl))
            {
                return Result<string>.Failure(b => b.Add("Configuration", "RegistryApiUrl is not configured"));
            }

            using var http = new HttpClient { BaseAddress = new Uri(_registryApiUrl) };
            http.Timeout = TimeSpan.FromSeconds(5);

            HttpResponseMessage? response;
            try
            {
                response = await http.GetAsync("/api/orchestrators", ct);
            }
            catch (HttpRequestException ex)
            {
                return Result<string>.Failure(b => b.Add("Network", $"Failed to connect to Registry API: {ex.Message}"));
            }
            catch (TaskCanceledException ex)
            {
                return Result<string>.Failure(b => b.Add("Timeout", $"Request to Registry API timed out: {ex.Message}"));
            }

            if (!response.IsSuccessStatusCode)
            {
                return Result<string>.Failure(b => b.Add("HttpError", $"Registry API returned {response.StatusCode}"));
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var orchestrators = JsonSerializer.Deserialize<List<OrchestratorInfo>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (orchestrators == null || orchestrators.Count == 0)
            {
                return Result<string>.Failure(b => b.Add("NoOrchestrators", "No orchestrators are currently registered. Please ensure at least one orchestrator is running."));
            }

            // Return the first running orchestrator, or the first available if none are running
            var running = orchestrators.FirstOrDefault(o => o.Status == "Running");
            var selectedUrl = running?.Url ?? orchestrators.First().Url;

            if (string.IsNullOrEmpty(selectedUrl))
            {
                return Result<string>.Failure(b => b.Add("InvalidUrl", "Orchestrator URL is empty or null"));
            }

            return Result<string>.Success(selectedUrl);
        }
        catch (JsonException ex)
        {
            return Result<string>.Failure(b => b.Add("Deserialization", $"Failed to parse orchestrators response: {ex.Message}"));
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(b => b.Add("Unexpected", $"Unexpected error fetching orchestrator URL: {ex.Message}"));
        }
    }

    private static async Task<string?> LoadBundledMarkdownAsync()
    {
        try
        {
            // Try fetch /Reporting/output.md via browser fetch
            using var http = new HttpClient { BaseAddress = new Uri("/", UriKind.Relative) };
            var text = await http.GetStringAsync("Reporting/" + MarkdownFileName);
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }
        catch
        {
            return null;
        }
    }

    private string BuildHelpMarkdown(string orchestratorUrl)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Project Dependency Analysis");
        sb.AppendLine();
        sb.AppendLine($"**Registry API URL:** {_registryApiUrl}");
        sb.AppendLine($"**Orchestrator URL:** {orchestratorUrl}");
        sb.AppendLine();
        sb.AppendLine("## Getting Started");
        sb.AppendLine();
        sb.AppendLine("1. The system automatically discovered the orchestrator from the Registry API");
        sb.AppendLine("2. Navigate to `/lifecycle` to monitor orchestrators and agents in real-time");
        sb.AppendLine("3. Create a session with the orchestrator");
        sb.AppendLine("4. Select a solution from the list provided by the agent");
        sb.AppendLine("5. Run analysis and monitor progress");
        sb.AppendLine("6. View results in markdown or graph viewer");
        sb.AppendLine();
        sb.AppendLine("## Artifacts");
        sb.AppendLine();
        sb.AppendLine("Provide analysis artifacts:");
        sb.AppendLine("- Place `graph.json` under `wwwroot/Reporting/`.");
        sb.AppendLine("- Place `output.md` under `wwwroot/Reporting/` (optional).");
        sb.AppendLine();
        sb.AppendLine("Open Graph Viewer via the left menu or navigate to `/Reporting/index.html`.");
        sb.AppendLine();
        sb.AppendLine("## Service Discovery");
        sb.AppendLine();
        sb.AppendLine("This application uses a centralized Registry API for service discovery:");
        sb.AppendLine("- Orchestrators register themselves on startup");
        sb.AppendLine("- Agents register themselves and associate with an orchestrator");
        sb.AppendLine("- The UI queries the Registry API to find available services");
        sb.AppendLine("- Real-time updates via WebSocket on `/ws` endpoint");
        return sb.ToString();
    }

    private string BuildErrorMarkdown(string error)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Project Dependency Analysis - Error");
        sb.AppendLine();
        sb.AppendLine($"**Registry API URL:** {_registryApiUrl}");
        sb.AppendLine();
        sb.AppendLine("## Error");
        sb.AppendLine();
        sb.AppendLine(error);
        sb.AppendLine();
        sb.AppendLine("## Troubleshooting");
        sb.AppendLine();
        sb.AppendLine("### Common Issues");
        sb.AppendLine();
        sb.AppendLine("#### Configuration Error");
        sb.AppendLine("- Verify `RegistryApiUrl` is set in `appsettings.json`");
        sb.AppendLine("- Default value: `https://api.pd3i1.com:5060`");
        sb.AppendLine();
        sb.AppendLine("#### Network Error");
        sb.AppendLine("- Ensure the Registry API is running");
        sb.AppendLine("- Check network connectivity");
        sb.AppendLine("- Verify firewall rules allow port 5060");
        sb.AppendLine();
        sb.AppendLine("#### No Orchestrators Available");
        sb.AppendLine("- Ensure at least one orchestrator is running and registered");
        sb.AppendLine("- Check the `/lifecycle` page to see registered services");
        sb.AppendLine("- Verify orchestrator's `RegistryApiUrl` configuration");
        sb.AppendLine();
        sb.AppendLine("#### Timeout Error");
        sb.AppendLine("- Registry API may be overloaded or slow");
        sb.AppendLine("- Check Registry API logs for errors");
        sb.AppendLine("- Verify API is responding: `curl https://api.pd3i1.com:5060/api/orchestrators`");
        sb.AppendLine();
        sb.AppendLine("### Next Steps");
        sb.AppendLine();
        sb.AppendLine("1. Check the Registry API is running at the configured URL");
        sb.AppendLine("2. Verify at least one orchestrator is registered and in 'Running' status");
        sb.AppendLine("3. Navigate to `/lifecycle` to monitor service health");
        sb.AppendLine("4. Review browser console (F12) for additional error details");
        return sb.ToString();
    }

    private class OrchestratorInfo
    {
        public string Id { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime RegisteredAt { get; set; }
        public DateTime LastHeartbeat { get; set; }
        public string Status { get; set; } = "";
    }
}
