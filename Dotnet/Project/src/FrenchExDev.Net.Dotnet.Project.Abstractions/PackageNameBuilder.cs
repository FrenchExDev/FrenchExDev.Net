using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

public class PackageNameBuilder : AbstractObjectBuilder<PackageName, PackageNameBuilder>
{
    private string? _name;
    public PackageNameBuilder Name(string name)
    {
        _name = name;
        return this;
    }
    protected override IObjectBuildResult<PackageName> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            exceptions.Add(new ArgumentException("Name is required"));
        }
        if (exceptions.Count > 0)
        {
            return Failure(exceptions, visited);
        }
        return Success(new PackageName(_name!));
    }
}

//}

//public class ClassProjectModel : AbstractProjectModel<ClassProjectModel>
//{
//}


//public class DesktopProjectModel : AbstractProjectModel<DesktopProjectModel>
//{

//}

//public class CliProjectModel : AbstractProjectModel<CliProjectModel>
//{

//}

//public class WebApiProjectModel : AbstractProjectModel<WebApiProjectModel>
//{

//}

//public class WebWorkerProjectModel : AbstractProjectModel<WebWorkerProjectModel>
//{

//}