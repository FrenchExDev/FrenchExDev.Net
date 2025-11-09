using FrenchExDev.Net.CSharp.Aspire.Dev;

await DevAppHost.Default(
        builder: () => DistributedApplication.CreateBuilder(args),
        environment: "Development")
    .EnsureSetup()
    .WithProjectInstance(
        resourceBuilder: (builder) => builder.AddProject<Projects.FrenchExDev_Net_CSharp_ProjectDependency5_Api>("api"),
        name: "api"
    )
    .Build()
    .RunAsync();
