using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Tests;

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

        var projectCollectionLoader = new ProjectCollectionLoader();
        var projectCollectionR = projectCollectionLoader.LoadProjects(defaultProjectCollection, solutionR.Object.Projects);

        projectCollectionR.IsSuccess.ShouldBeTrue();
        projectCollectionR.Object.Count.ShouldBeGreaterThan(0);

        var generator = new ProjectAnalysisCollection()
            .AddAnalyzer(new ProjectReferencesAnalyzer());

        var dic = new Dictionary<string, List<Result<IProjectAnalysisResult>>>();

        foreach (var projectKv in projectCollectionR.Object)
        {
            Result<List<Result<IProjectAnalysisResult>>> analysisR = generator.GenerateAnalysis(projectKv.Value.Object, solutionR.Object);
            analysisR.IsSuccess.ShouldBeTrue();
            if (!dic.ContainsKey(projectKv.Key))
            {
                dic[projectKv.Key] = new List<Result<IProjectAnalysisResult>>();
            }
            dic[projectKv.Key].AddRange(analysisR.ObjectOrThrow());
        }

        dic.ShouldNotBeEmpty();
    }
}
