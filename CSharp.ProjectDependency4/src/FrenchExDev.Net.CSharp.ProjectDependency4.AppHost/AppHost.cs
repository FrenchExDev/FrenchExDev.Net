using FrenchExDev.Net.Aspire.DevAppHost;
using Microsoft.Extensions.Logging;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var devAppHost = DevAppHost2.Default(builder);

if (!devAppHost.EnsureSetup2().TryGetSuccess(out DnsConfiguration2? dnsConfiguration)) 
{ 
    throw new InvalidOperationException("Could not setup DevAppHost2"); 
}

ArgumentNullException.ThrowIfNull(dnsConfiguration);

var port = dnsConfiguration.GetPortOfDns("Viz.Api").ObjectOrThrow();
var domain = dnsConfiguration.GetDomain("Viz.Api", 1, 2, "https").ObjectOrThrow();

devAppHost.Logger.LogInformation($"Configuring Viz.Api on port {port} with domain {domain}");

var vizApi = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency4_Viz_Api>("viz-api-01")
    .WithHttpsEndpoint(port: port, name: "https")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfiguration.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfiguration.KeyPathOrDie())
    .WithEnvironment("ASPNETCORE_URLS", $"{domain}:{port.ToString()}")
    ;

builder.Build().Run();
