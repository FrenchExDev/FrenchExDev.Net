namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

public record PackageVersionReference(PackageName Name, IPackageVersion PackageVersion) : IPackageReference;
