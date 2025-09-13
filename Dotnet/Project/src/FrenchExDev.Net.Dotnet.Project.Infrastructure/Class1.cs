using FrenchExDev.Net.Dotnet.Project.Abstractions;
using Microsoft.Build.Construction;

namespace FrenchExDev.Net.Dotnet.Project.Infrastructure;

public static class AbstractProjectModelExtensions
{
    public static readonly string TargetFramework = "TargetFramework";
    public static readonly string ImplicitUsings = "ImplicitUsings";
    public static readonly string Nullable = "Nullable";
    public static readonly string Version = "Version";
    public static readonly string GeneratePackageOnBuild = "GeneratePackageOnBuild";
    public static readonly string Description = "Description";
    public static readonly string Copyright = "Copyright";
    public static readonly string PackageProjectUrl = "PackageProjectUrl";
    public static readonly string RepositoryUrl = "RepositoryUrl";
    public static readonly string RepositoryType = "RepositoryType";
    public static readonly string Authors = "Authors";
    public static readonly string PackageTags = "PackageTags";
    public static readonly string IncludeSymbols = "IncludeSymbols";
    public static readonly string EnforeCodeStyleInBuild = "EnforeCodeStyleInBuild";

    public static readonly string Enable = "enable";
    public static readonly string Disable = "disable";

    public static ProjectRootElement ToProjectRootElement<T>(this IProjectModel projectModel) where T : IProjectModel
    {
        ProjectRootElement root = ProjectRootElement.Create();

        var propertyGroup = root.AddPropertyGroup();

        if (!string.IsNullOrEmpty(projectModel.TargetFramework))
        {
            propertyGroup.AddProperty(TargetFramework, projectModel.TargetFramework);
        }

        if (projectModel.ImplicitUsings.HasValue)
        {
            propertyGroup.AddProperty(ImplicitUsings, projectModel.ImplicitUsings.Value ? Enable : Disable);
        }

        if (projectModel.Nullable.HasValue)
        {
            propertyGroup.AddProperty(Nullable, projectModel.Nullable.Value ? Enable : Disable);
        }

        if (!string.IsNullOrEmpty(projectModel.Version))
        {
            propertyGroup.AddProperty(Version, projectModel.Version);
        }

        if (!string.IsNullOrEmpty(projectModel.Description))
        {
            propertyGroup.AddProperty(Description, projectModel.Description);
        }

        if (!string.IsNullOrEmpty(projectModel.Copyright))
        {
            propertyGroup.AddProperty(Copyright, projectModel.Copyright);
        }

        if (!string.IsNullOrEmpty(projectModel.PackageProjectUrl))
        {
            propertyGroup.AddProperty(PackageProjectUrl, projectModel.PackageProjectUrl);
        }

        if (!string.IsNullOrEmpty(projectModel.PackageProjectUrl))
        {
            propertyGroup.AddProperty(RepositoryUrl, projectModel.RepositoryUrl);
        }

        if (!string.IsNullOrEmpty(projectModel.PackageProjectUrl))
        {
            propertyGroup.AddProperty(RepositoryType, projectModel.RepositoryType);
        }

        if (!string.IsNullOrEmpty(projectModel.PackageProjectUrl))
        {
            propertyGroup.AddProperty(Authors, projectModel.Authors);
        }

        if (!string.IsNullOrEmpty(projectModel.PackageProjectUrl))
        {
            propertyGroup.AddProperty(PackageTags, projectModel.PackageTags != null ? string.Join(",", projectModel.PackageTags) : null);
        }

        if (!string.IsNullOrEmpty(projectModel.PackageProjectUrl))
        {
            propertyGroup.AddProperty(IncludeSymbols, projectModel.IncludeSymbols.HasValue && projectModel.IncludeSymbols.Value ? "true" : "false");
        }

        if (!string.IsNullOrEmpty(projectModel.PackageProjectUrl))
        {
            propertyGroup.AddProperty(EnforeCodeStyleInBuild, "true");
        }

        return root;
    }
}
