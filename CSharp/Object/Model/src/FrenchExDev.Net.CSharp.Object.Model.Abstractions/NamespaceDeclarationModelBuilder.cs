using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class NamespaceDeclarationModelBuilder : AbstractObjectBuilder<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>
{
    private string? _name;
    private readonly List<InterfaceDeclarationModelBuilder> _interfaces = new();
    private readonly List<ClassDeclarationModelBuilder> _classes = new();
    private readonly List<EnumDeclarationModelBuilder> _enums = new();
    private readonly List<StructDeclarationModelBuilder> _structs = new();
    private readonly List<NamespaceDeclarationModelBuilder> _nestedNamespaces = new();

    public NamespaceDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }
    public NamespaceDeclarationModelBuilder Interface(Action<InterfaceDeclarationModelBuilder> interfaceBuilder)
    {
        var builder = new InterfaceDeclarationModelBuilder();
        interfaceBuilder(builder);
        _interfaces.Add(builder);
        return this;
    }
    public NamespaceDeclarationModelBuilder Class(Action<ClassDeclarationModelBuilder> classBuilder)
    {
        var builder = new ClassDeclarationModelBuilder();
        classBuilder(builder);
        _classes.Add(builder);
        return this;
    }
    public NamespaceDeclarationModelBuilder Enum(Action<EnumDeclarationModelBuilder> enumBuilder)
    {
        var builder = new EnumDeclarationModelBuilder();
        enumBuilder(builder);
        _enums.Add(builder);
        return this;
    }
    public NamespaceDeclarationModelBuilder Struct(Action<StructDeclarationModelBuilder> structBuilder)
    {
        var builder = new StructDeclarationModelBuilder();
        structBuilder(builder);
        _structs.Add(builder);
        return this;
    }
    public NamespaceDeclarationModelBuilder NestedNamespace(Action<NamespaceDeclarationModelBuilder> namespaceBuilder)
    {
        var builder = new NamespaceDeclarationModelBuilder();
        namespaceBuilder(builder);
        _nestedNamespaces.Add(builder);
        return this;
    }
    protected override IObjectBuildResult<NamespaceDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        var interfaces = _interfaces
            .Select(x => x.Build(visited))
            .ToList();

        if (interfaces.OfType<FailureObjectBuildResult<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(interfaces
                .OfType<FailureObjectBuildResult<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }


        var classes = _classes
            .Select(x => x.Build(visited))
            .ToList();

        if (classes.OfType<FailureObjectBuildResult<ClassDeclarationModel, ClassDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(classes
                .OfType<FailureObjectBuildResult<ClassDeclarationModel, ClassDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        var enums = _enums
            .Select(x => x.Build(visited))
            .ToList();

        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Namespace name must be provided."));
        }

        var structs = _structs
            .Select(x => x.Build(visited))
            .ToList();

        if (structs.OfType<FailureObjectBuildResult<StructDeclarationModel, StructDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(structs
                .OfType<FailureObjectBuildResult<StructDeclarationModel, StructDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        var nestedNamespaces = _nestedNamespaces
            .Select(x => x.Build(visited))
            .ToList();

        if (nestedNamespaces.OfType<FailureObjectBuildResult<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(nestedNamespaces
                .OfType<FailureObjectBuildResult<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>(this, exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_name);

        return new SuccessObjectBuildResult<NamespaceDeclarationModel>(new NamespaceDeclarationModel
        {
            Name = _name,
            Interfaces = interfaces.ToResultList(),
            Classes = classes.ToResultList(),
            Enums = enums.ToResultList(),
            Structs = structs.ToResultList(),
            NestedNamespaces = nestedNamespaces.ToResultList()
        });
    }
}
