using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Model.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

public abstract class AbstractProjectModelBuilder<TProjectModel, TBuilder> : AbstractObjectBuilder<TProjectModel, TBuilder>
    where TProjectModel : IProjectModel
    where TBuilder : AbstractProjectModelBuilder<TProjectModel, TBuilder>
{
    private string? _name;
    private string? _directory;
    private string? _sdk;
    private string? _targetFramework;
    private string? _outputType;
    private string? _langVersion;
    private bool? _nullable;
    private bool? _implicitUsings;
    private List<ProjectReference> _projectReferences = new();
    private List<IPackageReference> _packageReferences = new();
    private List<IPackageReference> _analyzers = new();
    private Dictionary<string, object> _additionalProperties = new();
    private List<IDeclarationModel> _declarationModels = new();

    public TBuilder Name(string name)
    {
        _name = name;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder Directory(string directory)
    {
        _directory = directory;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder Sdk(string sdk)
    {
        _sdk = sdk;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder TargetFramework(string targetFramework)
    {
        _targetFramework = targetFramework;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder OutputType(string outputType)
    {
        _outputType = outputType;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder LangVersion(string langVersion)
    {
        _langVersion = langVersion;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder Nullable(bool nullable)
    {
        _nullable = nullable;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder ImplicitUsings(bool implicitUsings)
    {
        _implicitUsings = implicitUsings;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder ProjectReferences(List<ProjectReference> projectReferences)
    {
        _projectReferences = projectReferences;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder PackageReferences(List<IPackageReference> packageReferences)
    {
        _packageReferences = packageReferences;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder Analyzers(List<IPackageReference> analyzers)
    {
        _analyzers = analyzers;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder AdditionalProperties(Dictionary<string, object> additionalProperties)
    {
        _additionalProperties = additionalProperties;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder DeclarationModels(List<IDeclarationModel> declarationModels)
    {
        _declarationModels = declarationModels;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }
}
