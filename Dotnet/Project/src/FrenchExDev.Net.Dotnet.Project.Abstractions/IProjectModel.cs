namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

public interface IProjectModel
{
    string? TargetFramework { get; }
    bool? ImplicitUsings { get; }
    bool? Nullable { get; }
    string? Version { get; }
    string? Description { get; }
    string? Copyright { get; }
    string? PackageProjectUrl { get; }
    string? RepositoryUrl { get; }
    string? RepositoryType { get; }
    string? Authors { get; }
    List<string> PackageTags { get; }
    bool? IncludeSymbols { get; }
}
