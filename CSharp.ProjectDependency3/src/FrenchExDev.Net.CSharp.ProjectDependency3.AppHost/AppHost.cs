using FrenchExDev.Net.CSharp.ProjectDependency3.AppHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Projects;

var rootConfig = new ConfigurationBuilder()
    .AddJsonFile("devapphost.json")
    .AddEnvironmentVariables()
    .Build();

var dnsConfig = rootConfig.GetSection("DnsConfiguration").Get<DnsConfiguration>() ?? throw new InvalidOperationException("missing DnsConfiguration");

System.Environment.SetEnvironmentVariable("ASPIRE_ENVIRONMENT", rootConfig["Environment"]);
System.Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", rootConfig["Environment"]);
System.Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", dnsConfig.GetDashboardUrl(21190));
System.Environment.SetEnvironmentVariable("ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL", dnsConfig.GetDashboardUrl(22187));
System.Environment.SetEnvironmentVariable("ASPNETCORE_URLS", dnsConfig.GetDashboardUrl());

var builder = DistributedApplication.CreateBuilder(args);
var logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger("apphost");

builder.EnsureSetup(dnsConfig, logger);

// Backend services (internal, localhost only)
var api = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz_Api>("viz-api-00")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
    .WithEnvironment("ASPNETCORE_URLS", $"http://0.0.0.0:{dnsConfig.Ports.Api.ToString()}")
    ;

var orchestratorUrl = dnsConfig.GetOrchestratorUrl();
var orchestrator = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Worker_Orchestrator>("orchestrator")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
    .WithEnvironment("ASPNETCORE_URLS", $"http://0.0.0.0:{dnsConfig.Ports.Orchestrator.ToString()}")
    .WithEnvironment("RegistryApiUrl", dnsConfig.GetApiHostUrl())
    ;

logger.LogInformation($"Using Orchestrator URL: {orchestratorUrl}");

var viz = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz>("viz")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
    .WithEnvironment("ASPNETCORE_URLS", $"http://0.0.0.0:{dnsConfig.Ports.Viz.ToString()}")
    .WithEnvironment("RegistryApiUrl", dnsConfig.GetApiHostUrl())
    ;

// Add worker agents
var workers = new List<IResourceBuilder<ProjectResource>>();

for (int i = 1; i <= dnsConfig.WorkerCount; i++)
{
    var workerPort = dnsConfig.Ports.WorkerBase + i - 1;

    var worker = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Worker_Agent>($"worker-agent-{i}")
        .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
        .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
        .WithEnvironment("ASPNETCORE_URLS", $"http://0.0.0.0:{workerPort.ToString()}")
   ;

    workers.Add(worker);
}

await builder.Build().RunAsync();

