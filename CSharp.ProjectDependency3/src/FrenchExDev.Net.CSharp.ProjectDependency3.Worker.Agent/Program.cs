using FrenchExDev.Net.CSharp.ProjectDependency3;
using FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Agent.Analysis;
using FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Agent.Services;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use mkcert certificate if provided
var certPath = builder.Configuration["ASPNETCORE_Kestrel__Certificates__Default__Path"];
var keyPath = builder.Configuration["ASPNETCORE_Kestrel__Certificates__Default__KeyPath"];

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

builder.Services.AddRouting();
builder.Services.AddSingleton<AgentState>();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("agent_ready", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Agent is ready"));

// Register analysis dependencies
builder.Services.AddSingleton<IMsBuildRegisteringService, MsBuildRegisteringService>();
builder.Services.AddSingleton<IProjectCollection, DefaultProjectCollection>();
builder.Services.AddScoped<IAnalysisRunner, AnalysisRunner>();

// Register HttpClient and auto-registration service
builder.Services.AddHttpClient();
builder.Services.AddHostedService<AgentRegistrationService>();

var app = builder.Build();

var solutionsRoot = app.Configuration.GetValue<string>("SolutionsRoot") ?? "c:/code/";

// Track active WebSocket connections per job
var wsConnections = new ConcurrentDictionary<string, List<WebSocket>>();

app.UseCors();

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/alive");
app.UseHealthChecks("/health");

app.UseWebSockets();

// WebSocket for streaming progress per job
app.Map("/ws/progress", async ctx =>
{
    if (!ctx.WebSockets.IsWebSocketRequest)
    {
        ctx.Response.StatusCode = 400;
    return;
    }
    var jobId = ctx.Request.Query["jobId"].ToString();
  if (string.IsNullOrWhiteSpace(jobId))
    {
        ctx.Response.StatusCode = 400;
        return;
    }
    using var ws = await ctx.WebSockets.AcceptWebSocketAsync();
    var list = wsConnections.GetOrAdd(jobId, _ => new List<WebSocket>());
    lock (list) list.Add(ws);
    try
    {
  var hello = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { type = "hello", message = "progress-connected", jobId }));
        await ws.SendAsync(hello, WebSocketMessageType.Text, true, ctx.RequestAborted);
        var buff = new byte[4096];
        while (ws.State == WebSocketState.Open)
    {
            var res = await ws.ReceiveAsync(buff, ctx.RequestAborted);
      if (res.MessageType == WebSocketMessageType.Close) break;
        }
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", ctx.RequestAborted);
    }
    finally
    {
    lock (list) list.Remove(ws);
    }
});

// List solutions from agent side
app.MapGet("/solutions", () =>
{
    try
    {
    var slns = Directory.EnumerateFiles(solutionsRoot, "*.sln", SearchOption.AllDirectories)
            .Select(p => new { name = Path.GetFileName(p), path = p })
   .ToArray();

        return Results.Ok(slns);
    }
    catch (Exception ex)
    {
 return Results.Problem(ex.Message);
    }
});

// Start an async analysis job
app.MapPost("/analyze", async (HttpContext http, AgentState state, IServiceProvider sp) =>
{
    using var sr = new StreamReader(http.Request.Body);
    var sln = (await sr.ReadToEndAsync()).Trim();
    if (string.IsNullOrWhiteSpace(sln)) return Results.BadRequest("empty solution path");
    var jobId = state.CreateJob(sln);
    _ = Task.Run(async () => await RunAnalysisAsync(jobId, sln, state, wsConnections, sp));
    return Results.Ok(new { jobId, solution = sln });
});

// Get job status
app.MapGet("/jobs/{jobId}", (string jobId, AgentState state) =>
{
    if (!state.TryGetJob(jobId, out var job) || job is null) return Results.NotFound();
 return Results.Ok(job);
});

app.MapGet("/artifacts/markdown", (HttpContext http, AgentState state, string sln) =>
{
    if (!state.TryGet(sln, out var art) || art is null) return Results.NotFound();
    return Results.Text(art.Markdown, "text/markdown", Encoding.UTF8);
});

app.MapPost("/shutdown", (IHostApplicationLifetime lifetime) =>
{
    lifetime.StopApplication();
    return Results.Ok();
});

app.Run();

async Task RunAnalysisAsync(string jobId, string sln, AgentState state, ConcurrentDictionary<string, List<WebSocket>> wsConns, IServiceProvider sp)
{
    try
    {
        state.UpdateJob(jobId, "running", 0);
      await BroadcastProgress(jobId, "running", 0, wsConns);

        using var scope = sp.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IAnalysisRunner>();

        var progressReporter = new Progress<AnalysisProgress>(p =>
  {
     // Map phases to progress percentage ranges
            var percent = p.Phase switch
          {
         "init" => 5,
              "load" => 10 + p.ProgressPercent / 5,
       "analyze" => 30 + (p.ProgressPercent * 40 / 100),
 "generate" => 70 + (p.ProgressPercent * 25 / 100),
      "complete" => 100,
    _ => p.ProgressPercent
   };
            state.UpdateJob(jobId, "running", percent);
            _ = BroadcastProgress(jobId, "running", percent, wsConns, message: p.Message);
 });

        var result = await runner.RunAsync(sln, progressReporter);

        if (result.IsSuccess)
        {
  state.Upsert(sln, result.ObjectOrThrow().MarkdownContent);
         state.UpdateJob(jobId, "completed", 100);
            await BroadcastProgress(jobId, "completed", 100, wsConns, message: "Analysis completed successfully");
    }
        else
        {
        state.UpdateJob(jobId, "failed", 0, result.FailuresOrThrow().First().Value.ToString());
            await BroadcastProgress(jobId, "failed", 0, wsConns, error: result.FailuresOrThrow().First().Value.ToString());
        }
  }
    catch (Exception ex)
    {
        state.UpdateJob(jobId, "failed", 0, ex.Message);
        await BroadcastProgress(jobId, "failed", 0, wsConns, error: ex.Message);
    }
}

async Task BroadcastProgress(string jobId, string status, int progress, ConcurrentDictionary<string, List<WebSocket>> wsConns, string? error = null, string? message = null)
{
    if (!wsConns.TryGetValue(jobId, out var list)) return;
    var msg = JsonSerializer.Serialize(new { type = "progress", jobId, status, progress, error, message });
    var bytes = Encoding.UTF8.GetBytes(msg);
    List<WebSocket> snapshot;
    lock (list) snapshot = new List<WebSocket>(list);
    foreach (var ws in snapshot)
    {
        try
        {
  if (ws.State == WebSocketState.Open)
      await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
     }
      catch { }
    }
}
