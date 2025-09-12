using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

public class ProjectReferenceBuilder : AbstractObjectBuilder<ProjectReference, ProjectReferenceBuilder>
{
    private IProjectModel? _referencedProject;
    public ProjectReferenceBuilder ReferencedProject(IProjectModel referencedProject)
    {
        _referencedProject = referencedProject;
        return this;
    }

    protected override IObjectBuildResult<ProjectReference> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        if (_referencedProject is null)
        {
            exceptions.Add(new InvalidOperationException("ReferencedProject is required"));
        }
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        return Success(new ProjectReference(_referencedProject ?? throw new InvalidOperationException("ReferencedProject is required")));
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