using FrenchExDev.Net.CSharp.Aspire.DevAppHost;
using Projects;

await DevAppHost2Builder
    .Defaults(args)
    .EnsureDevSetup2()
    .WithDefaultLogger()
    .WithBuilder("Viz", (builder, str, apps, dns, instance, port) =>
    {
        return builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency5_Viz>("viz-0" + instance)
            .WithHttpEndpoint(port: port, name: "https")
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dns.CertPathOrDie())
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dns.KeyPathOrDie())
            .WithEnvironment("ASPNETCORE_URLS", $"https://{"viz-0" + instance}.pd5i1.com:{port.ToString()}");
    })
    .CreateBuilder(args)
    .Build()
    .RunAsync();
