using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.CodeQuality;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Dependencies;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Markdown;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Tests;
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

        // Prepare report generators (one per analyzer)
        var reportGenerators = new IProjectAnalysisReportGenerator[]
        {
            new ProjectReferencesReportGenerator(),
            new PublicApiReportGenerator(),
            new InterfaceDesignReportGenerator(),
            new XmlDocCoverageReportGenerator(),
            new InterfaceTestUsageReportGenerator(),
            new AsyncConventionsReportGenerator(),
            new ExceptionUsageReportGenerator()
        };

        var outDir = Path.Combine(Path.GetTempPath(), "projdep2-md", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outDir);

        var mdGen = new ProjectMarkdownFileGenerator();

        int generated = 0;
        foreach (var projectKv in projectCollectionR.Object)
        {
            var project = projectKv.Value.ObjectOrNull();
            if (project is null) continue;

            var reports = new List<IProjectAnalysisReportResult>();
            foreach (var gen in reportGenerators)
            {
                var r = gen.GenerateReport<object>(project, solutionR.Object);
                if (r.IsSuccess && r.Object != null)
                {
                    reports.Add(r.Object);
                }
            }

            var mdR = mdGen.Generate(project.Name, reports);
            mdR.IsSuccess.ShouldBeTrue();
            var file = Path.Combine(outDir, $"{project.Name}.md");
            File.WriteAllText(file, mdR.ObjectOrThrow());
            generated++;
        }

        generated.ShouldBeGreaterThan(0);
    }
}
