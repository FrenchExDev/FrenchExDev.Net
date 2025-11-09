using FrenchExDev.Net.CSharp.Aspire.Dev;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger("apphost");

var devAppHost = DevAppHost.Default(
    builder: () => DistributedApplication.CreateBuilder(args),
    environment: "Development");

devAppHost.EnsureSetup();

devAppHost.SetupProject(
    resourceBuilder: (builder) => builder.AddProject<Projects.FrenchExDev_Net_CSharp_ProjectDependency5_Api>("api"),
    name: "api"
);

var project = devAppHost.Build();

await project.RunAsync();
