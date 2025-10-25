using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Services;

/// <summary>
/// Client-side placeholder orchestrator. In WASM we cannot run Roslyn/MSBuild. This service
/// keeps track of graph/markdown artifacts the user places under wwwroot/Reporting or uploads,
/// and provides links to view them. Server/desktop runner is responsible for generating graph.json and md.
/// </summary>
public class AnalysisService : IAnalysisService
{
    private readonly ISessionManager _sessions;
    private readonly ILocalStorageService _local;
    private readonly string _orchestratorUrl;
    private const string GraphFileName = "graph.json";
    private const string MarkdownFileName = "output.md";
    private const string ReportingBase = "Reporting"; // static folder mounted by the app

    public AnalysisService(ISessionManager sessions, ILocalStorageService local, string orchestratorUrl)
    {
        _sessions = sessions;
        _local = local;
        _orchestratorUrl = orchestratorUrl;
    }

    public async Task<AnalysisRunResult> AnalyzeSolutionAsync(string solutionPath, CancellationToken ct = default)
    {
        // Persist last used path for UX; generation is expected to be done externally
        await _sessions.SetLastSolutionPathAsync(solutionPath);
        var sid = _sessions.GetOrCreateSessionId();

        // Point to static reporting directory. The viewer uses `/Reporting/index.html` and expects graph.json nearby
        var graphUrl = $"{ReportingBase}/{GraphFileName}"; // user must copy graph.json here

        // Try to read embedded markdown if shipped; otherwise, point to instructions
        var md = await LoadBundledMarkdownAsync() ?? BuildHelpMarkdown();

        return new AnalysisRunResult(sid, solutionPath, graphUrl, md);
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

    private string BuildHelpMarkdown()
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Project Dependency Analysis");
        sb.AppendLine();
        sb.AppendLine($"**Orchestrator URL:** {_orchestratorUrl}");
        sb.AppendLine();
        sb.AppendLine("## Getting Started");
        sb.AppendLine();
        sb.AppendLine("1. Create a session with the orchestrator");
        sb.AppendLine("2. Select a solution from the list provided by the agent");
        sb.AppendLine("3. Run analysis and monitor progress");
        sb.AppendLine("4. View results in markdown or graph viewer");
        sb.AppendLine();
        sb.AppendLine("Provide analysis artifacts:");
        sb.AppendLine("- Place `graph.json` under `wwwroot/Reporting/`.");
        sb.AppendLine("- Place `output.md` under `wwwroot/Reporting/` (optional).");
        sb.AppendLine();
        sb.AppendLine("Open Graph Viewer via the left menu or navigate to `/Reporting/index.html`.");
        return sb.ToString();
    }
}
