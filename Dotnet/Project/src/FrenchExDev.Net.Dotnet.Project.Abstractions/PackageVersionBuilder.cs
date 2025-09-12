using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

public class PackageVersionBuilder : AbstractObjectBuilder<PackageVersion, PackageVersionBuilder>
{
    private string? _version;
    public PackageVersionBuilder Version(string version)
    {
        _version = version;
        return this;
    }
    protected override IObjectBuildResult<PackageVersion> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrWhiteSpace(_version))
        {
            exceptions.Add(nameof(_version), new ArgumentException("Version is required"));
        }
        if (exceptions.Count > 0)
        {
            return Failure(exceptions, visited);
        }
        return Success(new PackageVersion(_version!));
    }
}
