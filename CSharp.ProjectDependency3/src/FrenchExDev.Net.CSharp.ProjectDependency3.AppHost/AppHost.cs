using FrenchExDev.Net.CSharp.ProjectDependency3.AppHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger("apphost");

var dnsConfig = builder.Configuration.GetSection("DnsConfiguration").Get<DnsConfiguration>() ?? new DnsConfiguration();
builder.EnsureSetup(dnsConfig, logger);

var dashboardUrl = dnsConfig.GetDashboardUrl();
logger.LogInformation($"Aspire Dashboard URL: {dashboardUrl}");

var apiUrl = dnsConfig.GetApiHostUrl();
logger.LogInformation($"Registry API URL: {apiUrl}");

// Add the Registry API service
var api = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz_Api>("registry-api")
    .WithHttpsEndpoint(port: dnsConfig.Ports.Api, name: "https")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
    .WithUrl(apiUrl)
    ;

logger.LogInformation($"Using Registry API URL: {apiUrl}");

var orchestratorUrl = dnsConfig.GetOrchestratorUrl();
var orchestrator = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Worker_Orchestrator>("orchestrator")
    .WithHttpsEndpoint(port: dnsConfig.Ports.Orchestrator, name: "https")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
    .WithEnvironment("RegistryApiUrl", apiUrl)
    .WithReference(api)
    .WithUrl(orchestratorUrl)
    ;

logger.LogInformation($"Using Orchestrator URL: {orchestratorUrl}");

var vizUrl = dnsConfig.GetVizUrl();
var viz = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz>("viz")
    .WithHttpsEndpoint(port: dnsConfig.Ports.Viz, name: "https")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
    .WithEnvironment("RegistryApiUrl", apiUrl)
    .WithReference(orchestrator)
    .WithReference(api)
    .WithUrl(vizUrl)
    ;

logger.LogInformation($"Using Viz URL: {vizUrl}");

// Add worker agents
var workers = new List<IResourceBuilder<ProjectResource>>();

for (int i = 1; i <= dnsConfig.WorkerCount; i++)
{
    var workerUrl = dnsConfig.GetWorkerUrl(i);
    var workerPort = dnsConfig.Ports.WorkerBase + i - 1;

    var worker = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Worker_Agent>($"worker-agent-{i}")
        .WithHttpsEndpoint(port: workerPort, name: "https")
        .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
        .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
        .WithEnvironment("OrchestratorUrl", orchestratorUrl)
        .WithEnvironment("RegistryApiUrl", apiUrl)
        .WithReference(orchestrator)
        .WithReference(api)
        .WithUrl(workerUrl)
   ;

    logger.LogInformation($"Using Worker {i} URL: {workerUrl}");
    workers.Add(worker);
}

builder.Build().Run();

