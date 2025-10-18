using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public record ProjectAnalysisGenerationResult(
    IReadOnlyList<ProjectAnalysis> Projects,
    IReadOnlyDictionary<string, ProjectMetrics> Metrics
);

public interface IProjectAnalysisGenerator
{
    Task<ProjectAnalysisGenerationResult> GenerateAsync(Solution solution, IProjectCollection projectCollection);
}

public class ProjectAnalysisGenerator : IProjectAnalysisGenerator
{
    private readonly IGitRepository _git;

    public ProjectAnalysisGenerator(IGitRepository? git = null)
    {
        _git = git ?? new GitCliRepository();
    }

    public async Task<ProjectAnalysisGenerationResult> GenerateAsync(Solution solution, IProjectCollection projectCollection)
    {
        if (solution is null) throw new ArgumentNullException(nameof(solution));
        if (projectCollection is null) throw new ArgumentNullException(nameof(projectCollection));

        // ensure internal project map is populated
        solution.LoadProjects(projectCollection);

        // base project analyses
        var projectAnalyses = new List<ProjectAnalysis>();
        await foreach (var item in solution.ScanProjectsAsync(projectCollection)) { projectAnalyses.Add(item); }

        // build graph from currently loaded projects
        var graph = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in solution.Projects)
        {
            var projPath = kv.Key;
            var proj = kv.Value;
            var refs = new List<string>();

            try
            {
                var msproj = proj.Msproj;
                foreach (var item in msproj.GetItems("ProjectReference"))
                {
                    var include = item.EvaluatedInclude ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(include)) continue;
                    var resolved = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projPath) ?? string.Empty, include));
                    if (!refs.Contains(resolved, StringComparer.OrdinalIgnoreCase)) refs.Add(resolved);
                }
            }
            catch
            {
                // ignore load errors for graph building
            }

            graph[projPath] = refs;
        }

        // compute times-used (in-degree) from graph
        var allNodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in graph)
        {
            allNodes.Add(kv.Key);
            foreach (var v in kv.Value)
                allNodes.Add(v);
        }

        var inDegree = allNodes.ToDictionary(k => k, k => 0, StringComparer.OrdinalIgnoreCase);
        foreach (var kv in graph)
        {
            foreach (var to in kv.Value)
            {
                if (inDegree.ContainsKey(to)) inDegree[to]++;
            }
        }

        // compute per-project metrics
        var metrics = new Dictionary<string, ProjectMetrics>(StringComparer.OrdinalIgnoreCase);

        // prepare map of which projects are test projects
        var testProjectPaths = new HashSet<string>(solution.Projects.Where(p => (p.Value.Name?.IndexOf("test", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 || (p.Value.Name?.IndexOf("tests", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0).Select(p => p.Key), StringComparer.OrdinalIgnoreCase);

        foreach (var projKv in solution.Projects)
        {
            var filePath = projKv.Key;
            var project = projKv.Value;

            int sourceFiles = 0;
            int totalLines = 0;
            int diagnosticsCount = 0;
            int cyclomatic = 0;
            int testRefs = 0;
            int outgoingRefs = 0;
            int nugetRefs = 0;
            int commentLines = 0;
            double commentDensity = 0.0;
            int commitCount = 0;
            DateTimeOffset? lastCommit = null;
            double maintainability = 0.0;
            double testability = 0.0;
            double hotspot = 0.0;

            try
            {
                var codeProject = project.Code;
                if (codeProject != null)
                {
                    var compilation = codeProject.GetCompilationAsync().GetAwaiter().GetResult();

                    // diagnostics
                    diagnosticsCount = compilation.GetDiagnostics().Length;

                    // documents
                    foreach (var doc in codeProject.Documents)
                    {
                        sourceFiles++;
                        try
                        {
                            var text = doc.GetTextAsync().GetAwaiter().GetResult();
                            totalLines += text.Lines.Count;
                            // count comment lines simple heuristic
                            foreach (var line in text.Lines)
                            {
                                var s = line.ToString().Trim();
                                if (s.StartsWith("//") || s.StartsWith("/*") || s.StartsWith("*")) commentLines++;
                            }
                        }
                        catch
                        {
                            // ignore
                        }

                        try
                        {
                            var tree = doc.GetSyntaxTreeAsync().GetAwaiter().GetResult();
                            if (tree == null) continue;
                            var root = tree.GetRoot();

                            // improved cyclomatic complexity: count decision & branch constructs in method bodies
                            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
                            foreach (var md in methods)
                            {
                                int methodComplexity = 1; // baseline

                                methodComplexity += md.DescendantNodes().Count(n =>
                                    n is IfStatementSyntax
                                    || n is ForStatementSyntax
                                    || n is ForEachStatementSyntax
                                    || n is WhileStatementSyntax
                                    || n is DoStatementSyntax
                                    || n is SwitchStatementSyntax
                                    || n is CaseSwitchLabelSyntax
                                    || n is ConditionalExpressionSyntax
                                );

                                cyclomatic += methodComplexity;
                            }
                        }
                        catch
                        {
                        }
                    }

                    // outgoing project refs and nuget refs from msproj
                    try
                    {
                        var msproj = project.Msproj;
                        outgoingRefs = msproj.GetItems("ProjectReference").Count;
                        nugetRefs = msproj.GetItems("PackageReference").Count;
                    }
                    catch { }

                    // compute how many test projects reference this project
                    foreach (var kv in graph)
                    {
                        var from = kv.Key;
                        var targets = kv.Value;
                        if (targets.Contains(filePath, StringComparer.OrdinalIgnoreCase) && testProjectPaths.Contains(from))
                        {
                            testRefs++;
                        }
                    }

                    // comment density
                    commentDensity = totalLines == 0 ? 0.0 : (double)commentLines / totalLines;
                }
            }
            catch
            {
                // ignore per-project analysis errors
            }

            var timesUsed = inDegree.TryGetValue(filePath, out var v) ? v : 0;

            // derive indicators using constructs data from projectAnalyses when available
            var pa = projectAnalyses.FirstOrDefault(x => string.Equals(x.FilePath, filePath, StringComparison.OrdinalIgnoreCase));
            var constructs = pa?.Constructs;

            maintainability = Math.Max(0.0, 100.0 - cyclomatic - (diagnosticsCount * 0.5));
            testability = (double)(constructs?.Interfaces ?? 0) - (constructs?.Classes ?? 0) * 0.1;
            hotspot = timesUsed * (1.0 + cyclomatic / Math.Max(1, sourceFiles) + diagnosticsCount * 0.1);

            // git churn
            try
            {
                var repoRoot = _git.GetRepositoryRoot(filePath);
                if (repoRoot != null)
                {
                    var rel = Path.GetRelativePath(repoRoot, filePath).Replace(Path.DirectorySeparatorChar, '/');
                    var commitR = _git.GetCommitCount(repoRoot, rel);
                    if (commitR.IsSuccess) commitCount = commitR.ObjectOrThrow();
                    var lastR = _git.GetLastCommitDate(repoRoot, rel);
                    if (lastR.IsSuccess) lastCommit = lastR.ObjectOrThrow();
                }
            }
            catch
            {
                // ignore git errors
            }

            metrics[filePath] = new ProjectMetrics(filePath, timesUsed, sourceFiles, totalLines, diagnosticsCount, cyclomatic, testRefs,
                outgoingRefs, nugetRefs, commentLines, commentDensity, commitCount, lastCommit, maintainability, testability, hotspot);
        }

        // enrich project analyses with per-project metrics so generators can print KPIs
        var enrichedAnalyses = new List<ProjectAnalysis>();
        foreach (var p in projectAnalyses)
        {
            var key = p.FilePath ?? string.Empty;
            if (metrics.TryGetValue(key, out var m))
            {
                var core = new ProjectCoreKpis(m.TimesUsed, m.OutgoingProjectReferences, m.NuGetReferences);
                var code = new ProjectCodeMetrics(m.SourceFileCount, m.TotalLinesOfCode, m.CommentLines, m.CommentDensity);
                var quality = new ProjectQualityMetrics(m.DiagnosticsCount, m.CyclomaticComplexity);
                var churn = new ProjectChurnMetrics(m.CommitCount, m.LastCommitDate);
                var derived = new DerivedProjectIndicators(m.MaintainabilityIndex, m.TestabilityIndex, m.HotspotScore);

                var enriched = new ProjectAnalysis(p.Name, p.FilePath, p.PackageReferences, p.ProjectReferences, p.ReferenceCouplings, p.Constructs, core, code, quality, churn, derived);
                enrichedAnalyses.Add(enriched);
            }
            else
            {
                enrichedAnalyses.Add(p);
            }
        }

        return new ProjectAnalysisGenerationResult(enrichedAnalyses, metrics);
    }
}
