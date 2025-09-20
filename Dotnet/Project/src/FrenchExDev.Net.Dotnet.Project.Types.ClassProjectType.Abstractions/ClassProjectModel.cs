using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using FrenchExDev.Net.Dotnet.Project.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.ClassProjectType.Abstractions;

/// <summary>
/// Represents a project model for a class-based project, including metadata, references, and declarations.
/// </summary>
/// <remarks>This class provides a specialized implementation of <see cref="AbstractProjectModel{T}"/> for
/// projects that are structured around class-based declarations. It encapsulates project metadata such as the name,
/// directory, SDK, target framework, and output type, as well as collections of project references, package references,
/// analyzers, and declaration models.</remarks>
public class ClassProjectModel : AbstractProjectModel<ClassProjectModel>
{
    public ClassProjectModel(
        string? name,
        string? directory,
        string? sdk,
        string? targetFramework,
        string? outputType,
        string? langVersion,
        bool? nullable,
        bool? implicitUsings,
        ReferenceList<ProjectReference>? projectReferences,
        ReferenceList<IPackageReference>? packageReferences,
        ReferenceList<IPackageReference>? analyzers,
        Reference<Dictionary<string, object>>? additionalProperties,
        ReferenceList<IDeclarationModel>? declarationModels,
        string? version,
        bool? generatePackageOnBuild,
        string? packageTags,
        string? authors
    ) : base(name, directory, sdk, targetFramework, outputType, langVersion, nullable, implicitUsings, projectReferences, packageReferences, analyzers, additionalProperties, declarationModels, version, generatePackageOnBuild, packageTags, authors)
    {
    }
}
