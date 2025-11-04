using FrenchExDev.Net.Aspire.DevAppHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(c =>
{
    c.SetMinimumLevel(LogLevel.Debug);
    c.AddConsole(options =>
    {
        options.FormatterName = "simple";
    });
    c.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.IncludeScopes = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
}).CreateLogger("apphost");


var apps = new Dictionary<string, IResourceBuilder<ProjectResource>>();

var dnsConfig = builder.Configuration.GetSection("DnsConfiguration").Get<DnsConfiguration>() ?? DnsConfiguration.Default();

var workers = 0;
var orchestrators = 0;

foreach (var dnsRecord in dnsConfig.Subdomains)
{
    logger.LogInformation($"DNS Record: {dnsRecord.Key} -> {dnsRecord.Value.Domain}:{dnsRecord.Value.Port}");
    switch (dnsRecord.Key)
    {
        case "dashboard": { } break;
        case "orchestrator":
            {
                var dns = dnsConfig.GetDomain(dnsRecord.Value.Domain).ObjectOrThrow();
                var instance = $"orchestrator-{orchestrators++}";
                apps[instance] = builder
                    .AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Worker_Orchestrator>(instance)
                    .WithReference(apps["api"])
                ;
            }
            break;
        case "viz":
            {
                apps["viz"] = builder
                    .AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz>("viz")
                    .WithEnvironment("ApiUrl", dnsConfig.GetDomain("api").ObjectOrThrow())
                    .WithReference(apps["api"])
                ;
            }
            break;
        case "api":
            {
                apps["api"] = builder
                   .AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz_Api>("api")
                   .WithReference(apps["orchestrator"])
                   ;
            }
            break;
        case "worker":
            {
                var dns = dnsConfig.GetDomain(dnsRecord.Value.Domain).ObjectOrThrow();
                var instance = $"worker-{workers++}";
                var project = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Worker_Agent>(instance)
                    .WithEnvironment("ApiUrl", dnsConfig.GetDomain("api").ObjectOrThrow())
                    .WithReference(apps["api"])
                    ;
                apps[instance] = project;
                logger.LogInformation($"Using Worker {instance} URL: {dns}");
            }
            break;
        default:
            logger.LogWarning($"Unknown DNS record key: {dnsRecord.Key}");
            break;
    }
}

var app = builder.Build();

await app.RunAsync();

