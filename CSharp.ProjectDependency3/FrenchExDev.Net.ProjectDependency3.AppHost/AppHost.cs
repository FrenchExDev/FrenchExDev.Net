using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var viz = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz>("viz");

builder.Build().Run();
