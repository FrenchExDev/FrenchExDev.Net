using System.Collections.Concurrent;
using System.Text;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Orchestrator;

public static class OrchestratorUI
{
    public static string GenerateAgentStatusUI(ConcurrentDictionary<string, AgentInfo> agents, ConcurrentDictionary<string, (string agentId, string? solution)> sessions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html><head><title>Orchestrator - Agent Status</title>");
        sb.AppendLine("<meta http-equiv='refresh' content='5'>");
        sb.AppendLine("<style>");
   sb.AppendLine("body { font-family: system-ui, sans-serif; margin: 20px; background: #f5f5f5; }");
    sb.AppendLine("h1 { color: #333; }");
        sb.AppendLine("table { border-collapse: collapse; width: 100%; background: white; box-shadow: 0 1px 3px rgba(0,0,0,0.1); margin-bottom: 20px; }");
        sb.AppendLine("th, td { text-align: left; padding: 12px; border-bottom: 1px solid #ddd; }");
        sb.AppendLine("th { background: #007bff; color: white; font-weight: 600; }");
        sb.AppendLine(".status-active { color: #28a745; font-weight: bold; }");
    sb.AppendLine(".status-stopped { color: #dc3545; font-weight: bold; }");
        sb.AppendLine(".refresh-btn { margin: 10px 0; padding: 8px 16px; background: #007bff; color: white; border: none; border-radius: 4px; cursor: pointer; }");
        sb.AppendLine(".refresh-btn:hover { background: #0056b3; }");
        sb.AppendLine("</style>");
        sb.AppendLine("<script>");
        sb.AppendLine("function refreshPage() { location.reload(); }");
        sb.AppendLine("</script>");
      sb.AppendLine("</head><body>");
        sb.AppendLine("<h1>Orchestrator Dashboard</h1>");
        sb.AppendLine($"<p>Last updated: {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss} UTC | Auto-refresh: 5s</p>");
        sb.AppendLine("<button class='refresh-btn' onclick='refreshPage()'>Refresh Now</button>");

 sb.AppendLine("<h2>Registered Agent Workers</h2>");
        sb.AppendLine("<table>");
    sb.AppendLine("<tr><th>Agent ID</th><th>URL</th><th>Status</th><th>Registered At</th><th>Last Heartbeat</th></tr>");

        if (agents.IsEmpty)
        {
       sb.AppendLine("<tr><td colspan='5' style='text-align:center; color:#999;'>No agents registered</td></tr>");
        }
        else
        {
      foreach (var agent in agents.Values.OrderBy(a => a.RegisteredAt))
            {
     var statusClass = agent.Status == "active" ? "status-active" : "status-stopped";
 sb.AppendLine($"<tr>");
    sb.AppendLine($"<td><code>{agent.Id}</code></td>");
           sb.AppendLine($"<td><a href='{agent.Url}' target='_blank'>{agent.Url}</a></td>");
         sb.AppendLine($"<td class='{statusClass}'>{agent.Status.ToUpper()}</td>");
    sb.AppendLine($"<td>{agent.RegisteredAt:yyyy-MM-dd HH:mm:ss}</td>");
       sb.AppendLine($"<td>{agent.LastHeartbeat:yyyy-MM-dd HH:mm:ss}</td>");
                sb.AppendLine($"</tr>");
            }
        }

   sb.AppendLine("</table>");

        sb.AppendLine("<h2>Active Sessions</h2>");
        sb.AppendLine("<table>");
        sb.AppendLine("<tr><th>Session ID</th><th>Agent ID</th><th>Solution</th></tr>");

   if (sessions.IsEmpty)
        {
 sb.AppendLine("<tr><td colspan='3' style='text-align:center; color:#999;'>No active sessions</td></tr>");
        }
        else
        {
      foreach (var sess in sessions)
     {
  sb.AppendLine($"<tr>");
              sb.AppendLine($"<td><code>{sess.Key}</code></td>");
       sb.AppendLine($"<td><code>{sess.Value.agentId}</code></td>");
       sb.AppendLine($"<td>{sess.Value.solution ?? "<em>(not started)</em>"}</td>");
  sb.AppendLine($"</tr>");
 }
        }

        sb.AppendLine("</table>");
        sb.AppendLine("</body></html>");

        return sb.ToString();
    }
}
