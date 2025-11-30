using FrenchExDev.Net.CSharp.Aspire.Dev;

await DevAppHost.Default(
        builder: () => DistributedApplication.CreateBuilder(args),
        environment: "Development")
    .EnsureSetup()
    //.WithProjectInstance(
    //    resourceBuilder: (builder) => builder.AddProject<Projects.FrenchExDev_Net_CSharp_ProjectDependency5_Api>("api"),
    //    name: "api"
    //)
    //.WithProjectInstance(
    //    resourceBuilder: (builder) => builder.AddProject<Projects.FrenchExDev_Net_CSharp_ProjectDependency5_Viz2>("viz"),
    //    name: "viz"
    //)
    .WithProjectInstance(
        name: "worker",
        resourceBuilder: (builder) => builder.AddProject<Projects.FrenchExDev_Net_CSharp_ProjectDependency5_Worker3>("worker"),
        replicas: 2
    )
    .Build()
    .RunAsync();
