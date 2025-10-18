using FrenchExDev.Net.CSharp.ProjectDependency;
using FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

namespace FrenchExDev.Net.Csharp.ProjectDependency.Tests;

public class ProjectMarkdownFileGeneratorTests
{
    [Fact]
    public async Task GenerateFiles_ForAllProjects()
    {
        // same solution used by other tests in this project
        var rootSln = @"C:\code\FrenchExDev.Net\FrenchExDev.Net_i2\FrenchExDev.Net\Alpine\FrenchExDev.Net.Alpine.sln";

        var msBuildRegisteringService = new MsBuildRegisteringService();
        msBuildRegisteringService.Register();

        var defaultProjectCollection = new DefaultProjectCollection();

        var msBuildWorkspace = new MsBuildWorkspace(defaultProjectCollection);
        msBuildWorkspace.Initialize();

        var solutionLoader = new SolutionLoader(msBuildRegisteringService, msBuildWorkspace);

        var solutionR = await solutionLoader.OpenSolutionAsync(rootSln);
        Assert.True(solutionR.IsSuccess, "Failed to open solution - integration environment may be missing");

        var solution = solutionR.ObjectOrThrow();

        // ensure projects are loaded
        solution.LoadProjects(defaultProjectCollection);

        var sag = new SolutionAnalysisGenerator();
        var analysis = await sag.GenerateAsync(solution, defaultProjectCollection);

        Assert.NotNull(analysis);
        Assert.True(analysis.TotalProjects > 0);

        var rootDir = @"C:\code\FrenchExDev.Net\FrenchExDev.Net_i2\FrenchExDev.Net\doc\gen";
        var outDir = Path.Combine(rootDir, "project-markdown", Guid.NewGuid().ToString());
        if (Directory.Exists(outDir)) Directory.Delete(outDir, true);

        Directory.CreateDirectory(outDir);

        var gen = new ProjectMarkdownFileGenerator();

        // run generation in parallel, create a generator per task to avoid shared-state
        var tasks = analysis.Projects.Select(project => Task.Run(async () =>
        {
            var localGen = new ProjectMarkdownFileGenerator();
            var fileContent = localGen.Generate(project, outDir);
            var filePath = Path.Combine(outDir, $"{project.Name}.md");
            await File.WriteAllTextAsync(filePath, fileContent);
        }));

        await Task.WhenAll(tasks);
    }
}
