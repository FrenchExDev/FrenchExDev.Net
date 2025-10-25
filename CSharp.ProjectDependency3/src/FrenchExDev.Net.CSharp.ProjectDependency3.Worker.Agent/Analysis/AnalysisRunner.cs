using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency3.Analysis;
using FrenchExDev.Net.CSharp.ProjectDependency3.Markdown;
using FrenchExDev.Net.CSharp.ProjectDependency3.Reporting;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Agent.Analysis;

/// <summary>
/// Service that orchestrates analysis execution using registered analyzers and report generators.
/// </summary>
public interface IAnalysisRunner
{
    Task<Result<AnalysisRunnerResult>> RunAsync(string solutionPath, IProgress<AnalysisProgress>? progress = null, CancellationToken ct = default);
}

public record AnalysisProgress(string Phase, int ProgressPercent, string Message);

public record AnalysisRunnerResult(string MarkdownContent);

public class AnalysisRunner : IAnalysisRunner
{
    private readonly IMsBuildRegisteringService _msBuildRegistering;
    private readonly IProjectCollection _projectCollection;
    private readonly ILogger<IMsBuildWorkspace> _workspaceLogger;

    public AnalysisRunner(
        IMsBuildRegisteringService msBuildRegistering,
        IProjectCollection projectCollection,
        ILogger<IMsBuildWorkspace> workspaceLogger)
    {
        _msBuildRegistering = msBuildRegistering;
        _projectCollection = projectCollection;
        _workspaceLogger = workspaceLogger;
    }

    public async Task<Result<AnalysisRunnerResult>> RunAsync(string solutionPath, IProgress<AnalysisProgress>? progress = null, CancellationToken ct = default)
    {
        try
        {
            progress?.Report(new AnalysisProgress("init", 0, "Initializing MSBuild workspace"));
            _msBuildRegistering.RegisterIfNeeded();

            progress?.Report(new AnalysisProgress("load", 10, "Loading solution"));
            var workspace = new MsBuildWorkspace(_projectCollection, _workspaceLogger);
            workspace.Initialize();

            var solutionResult = await workspace.OpenSolutionAsync(solutionPath, cancellationToken: ct);
            if (solutionResult.IsFailure)
            {
                return Result<AnalysisRunnerResult>.Failure(b => b.Add("Solution", "Failed to load solution: " + solutionResult.FailuresOrThrow().ToString()));
            }

            var solution = solutionResult.ObjectOrThrow();

            progress?.Report(new AnalysisProgress("analyze", 30, "Running analyzers"));

            // Create and configure analyzers
            var aggregator = new ProjectAnalyzerAggregator()
                .Add(new StructuralCouplingAnalyzer())
                .Add(new ClassicalCouplingAnalyzer())
                .Add(new DirectionalCouplingAnalyzer())
                ;

            var analysisResults = await aggregator.RunAsync(solution, ct);

            progress?.Report(new AnalysisProgress("generate", 70, "Generating reports"));

            // Generate markdown from all results
            var doc = new MarkdownDocument();

            doc.AddSection(new MarkdownSection("Solution Analysis")
            .AddContent($"**Solution:** {solutionPath}")
            .AddContent($"**Generated:** {DateTimeOffset.UtcNow:u}"));

            // Apply report generators for each analysis result
            foreach (var kv in analysisResults)
            {
                var result = kv.Value;

                switch (result)
                {
                    case StructuralCouplingResult scr:
                        {
                            var gen = new StructuralCouplingReportGenerator();
                            foreach (var section in gen.Generate(scr))
                            {
                                doc.AddSection(section);
                            }

                            break;
                        }

                    case ClassicalCouplingResult ccr:
                        {
                            var gen = new ClassicalCouplingReportGenerator();
                            foreach (var section in gen.Generate(ccr))
                            {
                                doc.AddSection(section);
                            }

                            break;
                        }

                    case DirectionalCouplingResult dcr:
                        {
                            var gen = new DirectionalCouplingReportGenerator();
                            foreach (var section in gen.Generate(dcr))
                            {
                                doc.AddSection(section);
                            }

                            break;
                        }
                    default: throw new NotSupportedException($"No report generator for analysis result type {result.GetType().FullName}");
                }
            }

            var markdown = doc.Render();

            progress?.Report(new AnalysisProgress("complete", 100, "Analysis complete"));

            workspace.Dispose();

            return Result<AnalysisRunnerResult>.Success(new AnalysisRunnerResult(markdown));
        }
        catch (Exception ex)
        {
            return Result<AnalysisRunnerResult>.Failure(ex);
        }
    }
}
