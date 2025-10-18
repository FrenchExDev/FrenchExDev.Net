using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Tests;

namespace FrenchExDev.Net.CSharp.ProjectDependency2;

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
                    PackageDependency dep;
                    if (!string.IsNullOrWhiteSpace(versionText) && Version.TryParse(versionText, out var ver))
                    {
                        dep = new PackageVersionDependency { Name = name, Version = ver };
                    }
                    else
                    {
                        dep = new PackageDistributedVersionDependency { Name = name };
                    }
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
}
