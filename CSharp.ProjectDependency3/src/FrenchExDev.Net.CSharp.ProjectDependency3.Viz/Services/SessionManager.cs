using Blazored.SessionStorage;
using Blazored.LocalStorage;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Services;

public class SessionManager(ISessionStorageService session, ILocalStorageService local)
 : ISessionManager
{
 private const string SessionKey = "viz.session.id";
 private const string LastSlnKey = "viz.session.lastsln";
 private const string AgentUrlKey = "viz.session.agenturl";
 private const string SessionIdKey = "viz.session.serverid";

 public string GetOrCreateSessionId()
 {
 var id = session.GetItemAsync<string>(SessionKey).GetAwaiter().GetResult();
 if (string.IsNullOrWhiteSpace(id))
 {
 id = Guid.NewGuid().ToString("N");
 session.SetItemAsync(SessionKey, id).GetAwaiter().GetResult();
 }
 return id;
 }

 public async Task SetLastSolutionPathAsync(string? slnPath)
 {
 if (string.IsNullOrWhiteSpace(slnPath))
 {
 await local.RemoveItemAsync(LastSlnKey);
 return;
 }
 await local.SetItemAsync(LastSlnKey, slnPath);
 }

 public Task<string?> GetLastSolutionPathAsync()
 {
 // Convert ValueTask<string?> to Task<string?>
 var vt = local.GetItemAsync<string?>(LastSlnKey);
 return vt.AsTask();
 }

 public async Task SetAgentUrlAsync(string? url)
 {
 if (string.IsNullOrWhiteSpace(url)) await local.RemoveItemAsync(AgentUrlKey);
 else await local.SetItemAsync(AgentUrlKey, url);
 }

 public Task<string?> GetAgentUrlAsync() => local.GetItemAsync<string?>(AgentUrlKey).AsTask();

 public async Task SetSessionIdAsync(string? id)
 {
 if (string.IsNullOrWhiteSpace(id)) await local.RemoveItemAsync(SessionIdKey);
 else await local.SetItemAsync(SessionIdKey, id);
 }

 public Task<string?> GetSessionIdAsync() => local.GetItemAsync<string?>(SessionIdKey).AsTask();
}
