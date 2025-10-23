using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        // same solution used by other tests in this project
        var rootSln = @"C:\code\FrenchExDev.Net\FrenchExDev.Net_i2\FrenchExDev.Net\Alpine\FrenchExDev.Net.Alpine.sln";

        var msBuildRegisteringService = new MsBuildRegisteringService();
        msBuildRegisteringService.RegisterIfNeeded();

        var defaultProjectCollection = new DefaultProjectCollection();

        var msBuildWorkspace = new MsBuildWorkspace(defaultProjectCollection, new NullLogger<IMsBuildWorkspace>());
        msBuildWorkspace.Initialize();

        var solutionR = await msBuildWorkspace.OpenSolutionAsync(rootSln);
        solutionR.IsSuccess.ShouldBeTrue();

        var solution = solutionR.ObjectOrThrow();

        // Run analyzers
        var aggregator = new ProjectAnalyzerAggregator()
            .Add(new StructuralCouplingAnalyzer())
            .Add(new ClassicalCouplingAnalyzer())
            .Add(new DirectionalCouplingAnalyzer());

        var results = await aggregator.RunAsync(solution);
        results.ShouldNotBeNull();
        results.ContainsKey("StructuralCoupling").ShouldBeTrue();
        results.ContainsKey("ClassicalCoupling").ShouldBeTrue();
        results.ContainsKey("DirectionalCoupling").ShouldBeTrue();
    }
}
