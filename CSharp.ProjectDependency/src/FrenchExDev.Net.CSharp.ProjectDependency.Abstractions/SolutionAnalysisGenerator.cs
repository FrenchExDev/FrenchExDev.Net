using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

/// <summary>
/// Represents aggregated analysis information for a solution: per-project analyses and key indicators.
/// </summary>
public record ProjectMetrics(string FilePath, int TimesUsed, int SourceFileCount, int TotalLinesOfCode, int DiagnosticsCount, int CyclomaticComplexity, int TestProjectsReferencing);

public record SolutionAnalysis(
    IReadOnlyList<ProjectAnalysis> Projects,
    int TotalProjects,
    int TotalPackageReferences,
    int TotalUniquePackages,
    double AveragePackagesPerProject,
    int TotalProjectReferences,
    IReadOnlyDictionary<string, int> PackageReferenceCounts,
    IReadOnlyDictionary<string, ProjectMetrics> ProjectMetricsMap
);

/// <summary>
/// Produces a <see cref="SolutionAnalysis"/> from a loaded <see cref="Solution"/> instance.
/// </summary>
public class SolutionAnalysisGenerator
{
    /// <summary>
    /// Generate analysis for the provided solution. This will ensure the solution's projects are loaded via
    /// <see cref="Solution.LoadProjects(IProjectCollection)"/> before scanning.
    /// </summary>
    public SolutionAnalysis Generate(Solution solution, IProjectCollection projectCollection)
    {
        if (solution is null) throw new ArgumentNullException(nameof(solution));
        if (projectCollection is null) throw new ArgumentNullException(nameof(projectCollection));

        // ensure internal project map is populated
        solution.LoadProjects(projectCollection);

        var projectAnalyses = solution.ScanProjects(projectCollection).ToList();

        var totalProjects = projectAnalyses.Count;

        var allPackageRefs = projectAnalyses
            .SelectMany(p => p.PackageReferences ?? Array.Empty<PackageReference>())
            .Where(p => !string.IsNullOrWhiteSpace(p.Name))
            .ToList();

        var totalPackageReferences = allPackageRefs.Count;
        var totalUniquePackages = allPackageRefs
            .Select(p => p.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        var avgPackagesPerProject = totalProjects == 0 ? 0.0 : (double)totalPackageReferences / totalProjects;

        var totalProjectReferences = projectAnalyses
            .SelectMany(p => p.ProjectReferences ?? Array.Empty<ProjectReference>())
            .Count();

        var packageCounts = allPackageRefs
            .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

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

        // compute per-project metrics: source files, lines of code, diagnostics, cyclomatic complexity, test references
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

                            // approximate cyclomatic complexity: count decision tokens
                            var decisionNodes = root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax>();
                            foreach (var dn in decisionNodes)
                            {
                                var textNode = dn.ToString();
                                cyclomatic += CountDecisionPoints(textNode);
                            }
                        }
                        catch
                        {
                        }
                    }

                    // compute how many test projects reference this project
                    // if any project in solution.Project references this filePath and is a test project
                    foreach (var kv in graph)
                    {
                        var from = kv.Key;
                        var targets = kv.Value;
                        if (targets.Contains(filePath, StringComparer.OrdinalIgnoreCase) && testProjectPaths.Contains(from))
                        {
                            testRefs++;
                        }
                    }
                }
            }
            catch
            {
                // ignore per-project analysis errors
            }

            var timesUsed = inDegree.TryGetValue(filePath, out var v) ? v : 0;
            metrics[filePath] = new ProjectMetrics(filePath, timesUsed, sourceFiles, totalLines, diagnosticsCount, cyclomatic, testRefs);
        }

        return new SolutionAnalysis(
            projectAnalyses,
            totalProjects,
            totalPackageReferences,
            totalUniquePackages,
            avgPackagesPerProject,
            totalProjectReferences,
            packageCounts,
            metrics
        );
    }

    private static int CountDecisionPoints(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return 0;
        int count = 0;
        // simple heuristics
        count += CountOccurrences(code, "if(");
        count += CountOccurrences(code, "if (");
        count += CountOccurrences(code, "for(");
        count += CountOccurrences(code, "for (");
        count += CountOccurrences(code, "while(");
        count += CountOccurrences(code, "while (");
        count += CountOccurrences(code, "case ");
        count += CountOccurrences(code, "&&");
        count += CountOccurrences(code, "||");
        count += CountOccurrences(code, "? ");
        return count;
    }

    private static int CountOccurrences(string src, string token)
    {
        int idx = 0, cnt = 0;
        while ((idx = src.IndexOf(token, idx, StringComparison.Ordinal)) >= 0)
        {
            cnt++; idx += token.Length;
        }
        return cnt;
    }
}
