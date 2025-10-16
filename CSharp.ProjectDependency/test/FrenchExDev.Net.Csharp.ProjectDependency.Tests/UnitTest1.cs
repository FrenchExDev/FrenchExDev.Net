using FrenchExDev.Net.CSharp.ProjectDependency;
using FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

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

    [Fact]
    public async Task Test_SolutionAnalysisGenerator()
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

        // ensure projects loaded
        solution.LoadProjects(defaultProjectCollection);

        var solutionAnalysisGenerator = new SolutionAnalysisGenerator();
        var solutionAnalysis = solutionAnalysisGenerator.Generate(solution, defaultProjectCollection);

        Assert.NotNull(solutionAnalysis);
        Assert.True(solutionAnalysis.TotalProjects > 0);
        Assert.Equal(solutionAnalysis.TotalProjects, solutionAnalysis.Projects.Count);
        Assert.NotNull(solutionAnalysis.ProjectMetricsMap);

        // each analyzed project should have metrics
        foreach (var p in solutionAnalysis.Projects)
        {
            Assert.True(solutionAnalysis.ProjectMetricsMap.ContainsKey(p.FilePath));
        }

        // basic consistency checks
        Assert.Equal(solutionAnalysis.TotalPackageReferences, solutionAnalysis.Projects.Sum(pr => pr.PackageReferences?.Count ?? 0));

        // generate markdown for first project
        var p0 = solutionAnalysis.Projects.First();
        var pmg = new ProjectMarkdownGenerator();
        var md = pmg.Generate(p0);
        Assert.False(string.IsNullOrWhiteSpace(md));

        var smg = new SolutionMarkdownGenerator();
        var smd = smg.Generate(solutionAnalysis);
        Assert.False(string.IsNullOrWhiteSpace(smd));
    }

    [Fact]
    public async Task Test_ProjectsMarkdownGenerator()
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

        var generator = new SolutionAnalysisGenerator();
        var analysis = generator.Generate(solution, defaultProjectCollection);

        var projectsGen = new ProjectsMarkdownGenerator();
        var allMd = projectsGen.GenerateWithTableOfContents(analysis);

        Assert.False(string.IsNullOrWhiteSpace(allMd));
        // should contain at least one project header
        Assert.Contains("## ", allMd);

        await File.WriteAllTextAsync(@"C:\code\FrenchExDev.Net\FrenchExDev.Net_i2\FrenchExDev.Net\FrenchExDev.Net.md", allMd);
    }
}
