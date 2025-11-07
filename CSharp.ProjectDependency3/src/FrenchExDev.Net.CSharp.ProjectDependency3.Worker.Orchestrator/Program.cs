using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Orchestrator;
using FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Orchestrator.Services;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use mkcert certificate if provided
var certPath = builder.Configuration["Kestrel:Certificates:Default:Path"];
var keyPath = builder.Configuration["Kestrel:Certificates:Default:KeyPath"];

if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(keyPath) && File.Exists(certPath) && File.Exists(keyPath))
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.ServerCertificate = X509Certificate2.CreateFromPemFile(certPath, keyPath);
            httpsOptions.ClientCertificateMode = ClientCertificateMode.NoCertificate;
        });
    });
}

builder.Services.AddHealthChecks()
    .AddCheck("orchestrator_ready", () => HealthCheckResult.Healthy("Orchestrator is ready"))
    .AddCheck("agents_available", () =>
    {
        // This will be populated after app starts, for now just return healthy
        return HealthCheckResult.Healthy("Checking agent availability");
    });

builder.Services.AddHttpClient();

// Register auto-registration service
builder.Services.AddHostedService<OrchestratorRegistrationService>();

var app = builder.Build();

var solutionsRoot = app.Configuration.GetValue<string>("SolutionsRoot") ?? "c:/code/";

app.UseWebSockets();

var agents = new ConcurrentDictionary<string, AgentInfo>();
var sessions = new ConcurrentDictionary<string, (string agentId, string? solution)>();

app.MapPost("/agents/register", async (HttpContext ctx) =>
{
    using var sr = new StreamReader(ctx.Request.Body);
    var url = await sr.ReadToEndAsync();
    var id = Guid.NewGuid().ToString("N");
    var now = DateTimeOffset.UtcNow;
    agents[id] = new AgentInfo(id, new Uri(url), now, now, "active");
    return Results.Ok(new { id, url });
});

app.MapGet("/agents", () => Results.Ok(agents.Select(kv => new
{
    id = kv.Key,
    url = kv.Value.Url,
    registeredAt = kv.Value.RegisteredAt,
    lastHeartbeat = kv.Value.LastHeartbeat,
    status = kv.Value.Status
})));

// Web UI to show agent workers
app.MapGet("/ui", async (HttpContext ctx) =>
{
    ctx.Response.ContentType = "text/html";
    var html = OrchestratorUI.GenerateAgentStatusUI(agents, sessions);
    await ctx.Response.WriteAsync(html);
});

// sessions
app.MapPost("/sessions", (HttpContext ctx) =>
{
    if (agents.IsEmpty) return Results.Problem("No agents available");
    var agentId = agents.Keys.First();
    var agentUrl = agents[agentId].Url.ToString();
    var sid = Guid.NewGuid().ToString("N");
    sessions[sid] = (agentId, null);
    return Results.Ok(new { sessionId = sid, agentId, agentUrl });
});

app.MapDelete("/sessions/{sid}", async (string sid, IHttpClientFactory httpFactory) =>
{
    if (!sessions.TryRemove(sid, out var sess)) return Results.NotFound();
    if (!agents.TryGetValue(sess.agentId, out var agentInfo)) return Results.Ok();
    try
    {
        var http = httpFactory.CreateClient();
        await http.PostAsync(new Uri(agentInfo.Url, "/shutdown"), new StringContent(""));
        agents[sess.agentId] = agentInfo with { Status = "stopped" };
    }
    catch { }
    return Results.Ok();
});

// list solutions from configured root (kept for convenience; UI should prefer agent's list)
app.MapGet("/solutions", () =>
{
    try
    {
        var slns = Directory.EnumerateFiles(solutionsRoot, "*.sln", SearchOption.AllDirectories)
        .Take(200)
        .Select(p => new { name = Path.GetFileName(p), path = p })
        .ToArray();
        return Results.Ok(slns);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/orchestrate/analyze", async (HttpContext ctx, IHttpClientFactory httpFactory) =>
{
    if (agents.IsEmpty) return Results.Problem("No agents available");
    var agent = agents.First().Value.Url; // naive pick
    using var sr = new StreamReader(ctx.Request.Body);
    var sln = await sr.ReadToEndAsync();
    var http = httpFactory.CreateClient();
    var resp = await http.PostAsync(new Uri(agent, "/analyze"), new StringContent(sln));
    if ((int)resp.StatusCode is >= 200 and < 300)
    {
        return Results.Ok(new { solution = sln, agent = agent.ToString() });
    }
    return Results.Problem($"Agent error: {(int)resp.StatusCode}");
});

app.MapGet("/orchestrate/artifacts/{type}", async (string type, string sln, IHttpClientFactory httpFactory) =>
{
    if (agents.IsEmpty) return Results.NotFound();
    var agent = agents.First().Value.Url; // naive
    var http = httpFactory.CreateClient();
    var path = type.Equals("graph", StringComparison.OrdinalIgnoreCase) ? "/artifacts/graph" : "/artifacts/markdown";
    var uri = new Uri(agent, $"{path}?sln={Uri.EscapeDataString(sln)}");
    var resp = await http.GetAsync(uri);
    if (!resp.IsSuccessStatusCode) return Results.StatusCode((int)resp.StatusCode);
    var content = await resp.Content.ReadAsStringAsync();
    var contentType = type.Equals("graph", StringComparison.OrdinalIgnoreCase) ? "application/json" : "text/markdown";
    return Results.Text(content, contentType);
});

app.Map("/ws/progress", async (HttpContext ctx, IHttpClientFactory factory) =>
{
    if (!ctx.WebSockets.IsWebSocketRequest) { ctx.Response.StatusCode = 400; return; }
    var sid = ctx.Request.Query["sid"].ToString();
    if (string.IsNullOrWhiteSpace(sid) || !sessions.TryGetValue(sid, out var sess)) { ctx.Response.StatusCode = 400; return; }
    if (!agents.TryGetValue(sess.agentId, out var agentInfo)) { ctx.Response.StatusCode = 404; return; }

    using var wsClient = new ClientWebSocket();
    await wsClient.ConnectAsync(new Uri(agentInfo.Url, "/ws"), ctx.RequestAborted);
    using var ws = await ctx.WebSockets.AcceptWebSocketAsync();

    var forwardFromAgent = Task.Run(async () =>
    {
        var buff = new byte[4096];
        while (wsClient.State == WebSocketState.Open && ws.State == WebSocketState.Open)
        {
            var res = await wsClient.ReceiveAsync(buff, ctx.RequestAborted);
            if (res.MessageType == WebSocketMessageType.Close) break;
            await ws.SendAsync(buff.AsMemory(0, res.Count), WebSocketMessageType.Text, res.EndOfMessage, ctx.RequestAborted);
        }
    }, ctx.RequestAborted);

    var forwardFromUi = Task.Run(async () =>
    {
        var buff = new byte[4096];
        while (ws.State == WebSocketState.Open && wsClient.State == WebSocketState.Open)
        {
            var res = await ws.ReceiveAsync(buff, ctx.RequestAborted);
            if (res.MessageType == WebSocketMessageType.Close) break;
            await wsClient.SendAsync(buff.AsMemory(0, res.Count), WebSocketMessageType.Text, res.EndOfMessage, ctx.RequestAborted);
        }
    }, ctx.RequestAborted);

    await Task.WhenAny(forwardFromAgent, forwardFromUi);
    try { await wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", ctx.RequestAborted); } catch { }
    try { await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", ctx.RequestAborted); } catch { }
});

app.MapHealthChecks("/health");
app.UseHealthChecks("/health");

app.Run();
