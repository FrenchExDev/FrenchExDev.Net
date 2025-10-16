using FrenchExDev.Net.CSharp.ProjectDependency;
using FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;

namespace FrenchExDev.Net.Csharp.ProjectDependency.Tests;

public class UnitTest1
{

    [Fact]
    public async Task Test1()
    {
        // Try to find a solution file by walking up the directory tree from the current working directory.
        var rootSln = @"C:\code\FrenchExDev.Net\FrenchExDev.Net_i2\FrenchExDev.Net\FrenchExDev.Net.sln";

        var msBuildRegisteringService = new MsBuildRegisteringService();
        msBuildRegisteringService.Register();

        var defaultProjectCollection = new DefaultProjectCollection();

        var msBuildWorkspace = new MsBuildWorkspace(defaultProjectCollection);
        msBuildWorkspace.Initialize();

        var solutionLoader = new SolutionLoader(msBuildRegisteringService, msBuildWorkspace);

        var solutionR = await solutionLoader.OpenSolutionAsync(rootSln);

        Assert.True(solutionR.IsSuccess);

        // additional quick use of analyzer
        var solution = solutionR.ObjectOrThrow();
        var scans = solution.ScanProjects(defaultProjectCollection);
        Assert.NotNull(scans);
        Assert.True(scans.Count() > 0);
    }

    [Fact]
    public async Task Test_AnalyzeAllProjects()
    {
        var rootSln = @"C:\code\FrenchExDev.Net\FrenchExDev.Net_i2\FrenchExDev.Net\FrenchExDev.Net.sln";

        var msBuildRegisteringService = new MsBuildRegisteringService();
        msBuildRegisteringService.Register();

        var defaultProjectCollection = new DefaultProjectCollection();

        var msBuildWorkspace = new MsBuildWorkspace(defaultProjectCollection);
        msBuildWorkspace.Initialize();

        var solutionLoader = new SolutionLoader(msBuildRegisteringService, msBuildWorkspace);

        var solutionR = await solutionLoader.OpenSolutionAsync(rootSln);

        Assert.True(solutionR.IsSuccess);

        var solution = solutionR.ObjectOrThrow();

        solution.LoadProjects(defaultProjectCollection);

        var scans = solution.ScanProjects(defaultProjectCollection).ToList();

        Assert.NotEmpty(scans);


    }
}
