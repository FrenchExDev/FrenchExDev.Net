using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.References;

/// <summary>
/// Represents the result of analyzing a project, including counts of its package and project references.
/// </summary>
/// <param name="ProjectName">The name of the project being analyzed.</param>
/// <param name="PackageReferences">The number of NuGet package references defined in the project.</param>
/// <param name="ProjectReferences">The number of other project references included in the project.</param>
public record ProjectReferenceAnalysis(string ProjectName, List<PackageReference> PackageReferences, List<ProjectReference> ProjectReferences) : IProjectAnalysisResult;
