using FrenchExDev.Net.CSharp.ProjectDependency3.AppHost;
using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var dnsConfig = builder.Configuration.GetSection("DnsConfiguration").Get<DnsConfiguration>() ?? new DnsConfiguration();
builder.EnsureSetup(dnsConfig);

var dashboardUrl = dnsConfig.GetDashboardUrl();
Console.WriteLine($"Aspire Dashboard URL: {dashboardUrl}");

var orchestratorUrl = dnsConfig.GetOrchestratorUrl();
var orchestrator = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Worker_Orchestrator>("orchestrator")
    .WithEnvironment("ASPNETCORE_URLS", orchestratorUrl)
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
    ;

Console.WriteLine($"Using Orchestrator URL: {orchestratorUrl}");

var vizUrl = dnsConfig.GetVizUrl();
var viz = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz>("viz")
    .WithEnvironment("ASPNETCORE_URLS", vizUrl)
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
    .WithEnvironment("OrchestratorUrl", orchestratorUrl)
    .WithReference(orchestrator);

Console.WriteLine($"Using Viz URL: {vizUrl}");

// Add worker agents
var workers = new List<IResourceBuilder<ProjectResource>>();

for (int i = 1; i <= dnsConfig.WorkerCount; i++)
{
    var workerUrl = dnsConfig.GetWorkerUrl(i);
    var workerPort = dnsConfig.Ports.WorkerBase + i - 1;

    var worker = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Worker_Agent>($"worker-agent-{i}")
        .WithEnvironment("ASPNETCORE_URLS", workerUrl)
        .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
        .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
        ;

    Console.WriteLine($"Using Worker {i} URL: {workerUrl}");
    workers.Add(worker);
}

builder.Build().Run();

