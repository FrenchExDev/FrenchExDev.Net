using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Dependencies;

public record ProjectReferenceAnalysis(string ProjectName, List<PackageReference> PackageReferences, List<ProjectReference> ProjectReferences) : IProjectAnalysisResult;
