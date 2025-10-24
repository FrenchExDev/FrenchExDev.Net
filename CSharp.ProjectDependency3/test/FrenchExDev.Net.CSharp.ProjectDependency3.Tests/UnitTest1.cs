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
        //var rootSln = @"C:\\code\\FrenchExDev.Net\\FrenchExDev.Net_i2\\FrenchExDev.Net\\Alpine\\FrenchExDev.Net.Alpine.sln";

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
            .Add(new DirectionalCouplingAnalyzer())
            .Add(new CodeGraphAnalyzer());

        var results = await aggregator.RunAsync(solution);
        results.ShouldNotBeNull();

        // Generate markdown for each known analyzer result
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
                    // write graph.json for HTML viewer
                    var json = JsonSerializer.Serialize(gr.Model, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(@"C:\code\graph.json", json);
                    break;
            }
        }
        var markdown = doc.Render();
        markdown.ShouldNotBeNullOrWhiteSpace();

        await File.WriteAllTextAsync(@"C:\code\output.md", markdown);

        // Copy the viewer HTML next to the graph so it can be opened directly
        var slnDir = Path.GetDirectoryName(rootSln)!;
        var indexHtmlSrc = Path.Combine(slnDir,
        "CSharp.ProjectDependency3", "src", "FrenchExDev.Net.CSharp.ProjectDependency3", "Reporting", "index.html");
        if (File.Exists(indexHtmlSrc))
        {
            File.Copy(indexHtmlSrc, @"C:\code\index.html", overwrite: true);
        }
    }
}
