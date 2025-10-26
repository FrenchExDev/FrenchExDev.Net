using FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Models;
using FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Services;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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

// Add services to the container
builder.Services.AddOpenApi();

// Add registry services and WebSocket manager
builder.Services.AddSingleton<IOrchestratorRegistry, OrchestratorRegistry>();
builder.Services.AddSingleton<IAgentRegistry, AgentRegistry>();
builder.Services.AddSingleton<RegistryWebSocketManager>();

// Enable WebSockets
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
  policy.AllowAnyOrigin()
      .AllowAnyMethod()
 .AllowAnyHeader();
  });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseWebSockets();

// Wire up registry events to WebSocket broadcasts
var orchestratorRegistry = app.Services.GetRequiredService<IOrchestratorRegistry>() as OrchestratorRegistry;
var agentRegistry = app.Services.GetRequiredService<IAgentRegistry>() as AgentRegistry;
var wsManager = app.Services.GetRequiredService<RegistryWebSocketManager>();

if (orchestratorRegistry != null)
{
    orchestratorRegistry.OrchestratorRegistered += async (_, reg) => await wsManager.BroadcastAsync(new { type = "orchestrator_registered", data = reg });
    orchestratorRegistry.OrchestratorUnregistered += async (_, reg) => await wsManager.BroadcastAsync(new { type = "orchestrator_unregistered", data = reg });
 orchestratorRegistry.OrchestratorStatusChanged += async (_, reg) => await wsManager.BroadcastAsync(new { type = "orchestrator_status_changed", data = reg });
}

if (agentRegistry != null)
{
    agentRegistry.AgentRegistered += async (_, reg) => await wsManager.BroadcastAsync(new { type = "agent_registered", data = reg });
    agentRegistry.AgentUnregistered += async (_, reg) => await wsManager.BroadcastAsync(new { type = "agent_unregistered", data = reg });
    agentRegistry.AgentStatusChanged += async (_, reg) => await wsManager.BroadcastAsync(new { type = "agent_status_changed", data = reg });
}

// Orchestrator API endpoints
app.MapGet("/api/orchestrators", (IOrchestratorRegistry registry) => Results.Ok(registry.GetAll()))
   .WithName("GetOrchestrators")
   .WithTags("Orchestrators");

app.MapPost("/api/orchestrators/register", (RegisterOrchestratorRequest request, IOrchestratorRegistry registry) =>
{
    var id = registry.Register(request.Url);
    return Results.Ok(new { id });
})
.WithName("RegisterOrchestrator")
.WithTags("Orchestrators");

app.MapPost("/api/orchestrators/{id}/unregister", (string id, IOrchestratorRegistry registry) =>
{
    var success = registry.Unregister(id);
    return success ? Results.Ok() : Results.NotFound();
})
.WithName("UnregisterOrchestrator")
.WithTags("Orchestrators");

app.MapPost("/api/orchestrators/{id}/heartbeat", (string id, IOrchestratorRegistry registry) =>
{
    registry.UpdateHeartbeat(id);
    return Results.Ok();
})
.WithName("OrchestratorHeartbeat")
.WithTags("Orchestrators");

app.MapPost("/api/orchestrators/{id}/status", (string id, OrchestratorStatus status, IOrchestratorRegistry registry) =>
{
    registry.UpdateStatus(id, status);
    return Results.Ok();
})
.WithName("UpdateOrchestratorStatus")
.WithTags("Orchestrators");

// Agent API endpoints
app.MapGet("/api/agents", (IAgentRegistry registry) => Results.Ok(registry.GetAll()))
   .WithName("GetAgents")
   .WithTags("Agents");

app.MapGet("/api/agents/orchestrator/{orchestratorId}", (string orchestratorId, IAgentRegistry registry) =>
    Results.Ok(registry.GetByOrchestrator(orchestratorId)))
   .WithName("GetAgentsByOrchestrator")
   .WithTags("Agents");

app.MapPost("/api/agents/register", (RegisterAgentRequest request, IAgentRegistry registry) =>
{
    var id = registry.Register(request.OrchestratorId, request.Url);
    return Results.Ok(new { id });
})
.WithName("RegisterAgent")
.WithTags("Agents");

app.MapPost("/api/agents/{id}/unregister", (string id, IAgentRegistry registry) =>
{
    var success = registry.Unregister(id);
    return success ? Results.Ok() : Results.NotFound();
})
.WithName("UnregisterAgent")
.WithTags("Agents");

app.MapPost("/api/agents/{id}/heartbeat", (string id, IAgentRegistry registry) =>
{
    registry.UpdateHeartbeat(id);
    return Results.Ok();
})
.WithName("AgentHeartbeat")
.WithTags("Agents");

app.MapPost("/api/agents/{id}/status", (string id, AgentStatus status, IAgentRegistry registry) =>
{
    registry.UpdateStatus(id, status);
    return Results.Ok();
})
.WithName("UpdateAgentStatus")
.WithTags("Agents");

// WebSocket endpoint
app.MapGet("/ws", async (HttpContext context, RegistryWebSocketManager wsManager) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
   var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await wsManager.HandleWebSocketAsync(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
})
.WithName("WebSocketEndpoint")
.WithTags("WebSocket");

app.Run();
