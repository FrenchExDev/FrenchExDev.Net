using FrenchExDev.Net.CSharp.ProjectDependency3.Analysis;
using FrenchExDev.Net.CSharp.ProjectDependency3.Markdown;
using FrenchExDev.Net.CSharp.ProjectDependency3.Reporting;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using System.Text.Json;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        // same solution used by other tests in this project
        var rootSln = @"C:\code\FrenchExDev.Net\FrenchExDev.Net_i2\FrenchExDev.Net\FrenchExDev.Net.sln";
        var dep3Sln = @"C:\code\FrenchExDev.Net\FrenchExDev.Net_i2\FrenchExDev.Net\CSharp.ProjectDependency3\CSharp.ProjectDependency3.sln";
        //var rootSln = @"C:\\code\\FrenchExDev.Net\\FrenchExDev.Net_i2\\FrenchExDev.Net\\Alpine\\FrenchExDev.Net.Alpine.sln";
        var playground = @"C:\code\FrenchExDev.Net\FrenchExDev.Net_i2\FrenchExDev.Net\CSharp.ProjectDependency3\test\playground";
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
            /* .Add(new StructuralCouplingAnalyzer())
             .Add(new ClassicalCouplingAnalyzer())
             .Add(new DirectionalCouplingAnalyzer())*/
            .Add(new CodeGraphAnalyzer());

        var results = await aggregator.RunAsync(solution);
        results.ShouldNotBeNull();

        //// Generate markdown for each known analyzer result
        var doc = new MarkdownDocument();
        foreach (var kv in results)
        {
            switch (kv.Value)
            {
                case StructuralCouplingResult sr:
                    var sSections = new StructuralCouplingReportGenerator().Generate(sr);
                    foreach (var s in sSections) doc.AddSection(s);
                    break;
                case ClassicalCouplingResult cr:
                    var cSections = new ClassicalCouplingReportGenerator().Generate(cr);
                    foreach (var s in cSections) doc.AddSection(s);
                    break;
                case DirectionalCouplingResult dr:
                    var dSections = new DirectionalCouplingReportGenerator().Generate(dr);
                    foreach (var s in dSections) doc.AddSection(s);
                    break;
                case CodeGraphResult gr:
                    var json = JsonSerializer.Serialize(gr.Model, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(Path.Combine(playground, "graph.json"), json);
                    break;
            }
        }
        //var markdown = doc.Render();

        //await File.WriteAllTextAsync(Path.Combine(playground, "output.md"), markdown);

        // Copy the viewer HTML next to the graph so it can be opened directly
        var slnDir = Path.GetDirectoryName(dep3Sln)!;
        var indexHtmlSrc = Path.Combine(slnDir, "src", "FrenchExDev.Net.CSharp.ProjectDependency3", "Reporting", "index.html");
        if (File.Exists(indexHtmlSrc))
        {
            File.Copy(indexHtmlSrc, Path.Combine(playground, "index.html"), overwrite: true);
        }
    }
}
