using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.References;

/// <summary>
/// Analyzes project references and package references within a given project.
/// </summary>
/// <remarks>This analyzer provides a summary of the number of project and package references for a specified
/// project. It is typically used to assess project dependencies within a solution.</remarks>
public class ProjectReferencesAnalyzer : IProjectAnalyzer
{
    public Result<IProjectAnalysisResult> AnalyzeProject(Project project, Solution solution)
    {
        try
        {
            var msproj = project.Msproj;

            // Build package references with best-effort version extraction
            var packageRefs = msproj
                .GetItems("PackageReference")
                .Select(item =>
                {
                    var name = item.EvaluatedInclude ?? string.Empty;
                    var versionText = item.GetMetadataValue("Version");
                    PackageDependency dep = !string.IsNullOrWhiteSpace(versionText) && Version.TryParse(versionText, out var ver)
                        ? new PackageVersionDependency { Name = name, Version = ver }
                        : new PackageDistributedVersionDependency { Name = name };

                    return new PackageReference(dep);
                })
                .ToList();

            // Build project references list using resolved absolute paths
            var projectRefs = msproj
                .GetItems("ProjectReference")
                .Select(item => item.EvaluatedInclude)
                .Where(inc => !string.IsNullOrWhiteSpace(inc))
                .Select(inc => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(project.FilePath) ?? string.Empty, inc!)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(full => new ProjectReference(full))
                .ToList();

            var analysis = new ProjectReferenceAnalysis(project.Name, packageRefs, projectRefs);
            return Result<IProjectAnalysisResult>.Success(analysis);
        }
        catch (Exception ex)
        {
            return Result<IProjectAnalysisResult>.Failure(ex);
        }
    }

    public Result<IProjectAnalysisReportResult> GenerateReport(ProjectReferenceAnalysis analysis)
    {
        if (analysis is null)
            return Result<IProjectAnalysisReportResult>.Failure(d => d.Add("ArgumentNull", nameof(analysis)));

        var packageNames = analysis.PackageReferences
            .Select(p => p.Package.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var projectPaths = analysis.ProjectReferences
            .Select(r => r.ProjectPath)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var report = new ProjectReferencesDetailedReport(
            analysis.ProjectName,
            packageNames.Count,
            packageNames,
            projectPaths.Count,
            projectPaths
        );

        return Result<IProjectAnalysisReportResult>.Success(report);
    }
}
