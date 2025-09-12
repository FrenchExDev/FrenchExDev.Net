using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class InterfaceDeclarationModelBuilder : AbstractObjectBuilder<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder>
{
    private string? _name;
    public List<InterfaceModifier> Modifiers { get; } = new();
    private readonly List<AttributeDeclarationModelBuilder> _attributes = new();
    private readonly List<string> _baseInterfaces = new();
    private readonly List<TypeParameterDeclarationModelBuilder> _typeParameters = new();
    private readonly List<TypeParameterConstraintModelBuilder> _typeParameterConstraints = new();
    private readonly List<PropertyDeclarationModelBuilder> _properties = new();
    private readonly List<MethodDeclarationModelBuilder> _methods = new();
    private readonly List<EventModelBuilder> _events = new();
    private readonly List<InterfaceDeclarationModelBuilder> _nestedInterfaces = new();

    public InterfaceDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public InterfaceDeclarationModelBuilder Modifier(InterfaceModifier modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }

    public InterfaceDeclarationModelBuilder Attribute(Action<AttributeDeclarationModelBuilder> attribute)
    {
        var builder = new AttributeDeclarationModelBuilder();
        attribute(builder);
        _attributes.Add(builder);
        return this;
    }

    public InterfaceDeclarationModelBuilder BaseInterface(string baseInterface)
    {
        _baseInterfaces.Add(baseInterface);
        return this;
    }

    public InterfaceDeclarationModelBuilder TypeParameter(Action<TypeParameterDeclarationModelBuilder> typeParameter)
    {
        var builder = new TypeParameterDeclarationModelBuilder();
        typeParameter(builder);
        _typeParameters.Add(builder);
        return this;
    }

    public InterfaceDeclarationModelBuilder TypeParameterConstraint(Action<TypeParameterConstraintModelBuilder> typeParameterConstraint)
    {
        var builder = new TypeParameterConstraintModelBuilder();
        typeParameterConstraint(builder);
        _typeParameterConstraints.Add(builder);
        return this;
    }

    public InterfaceDeclarationModelBuilder Property(Action<PropertyDeclarationModelBuilder> property)
    {
        var builder = new PropertyDeclarationModelBuilder();
        property(builder);
        _properties.Add(builder);
        return this;
    }

    public InterfaceDeclarationModelBuilder Method(Action<MethodDeclarationModelBuilder> method)
    {
        var builder = new MethodDeclarationModelBuilder();
        method(builder);
        _methods.Add(builder);
        return this;
    }

    public InterfaceDeclarationModelBuilder Event(Action<EventModelBuilder> eventModel)
    {
        var builder = new EventModelBuilder();
        eventModel(builder);
        _events.Add(builder);
        return this;
    }

    public InterfaceDeclarationModelBuilder NestedInterface(Action<InterfaceDeclarationModelBuilder> nestedInterface)
    {
        var builder = new InterfaceDeclarationModelBuilder();
        nestedInterface(builder);
        _nestedInterfaces.Add(builder);
        return this;
    }


    protected override IObjectBuildResult<InterfaceDeclarationModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        var attributes = BuildBuildList<AttributeDeclarationModel, AttributeDeclarationModelBuilder>(_attributes, visited);
        var typeParameters = BuildBuildList<TypeParameterDeclarationModel, TypeParameterDeclarationModelBuilder>(_typeParameters, visited);
        var typeParameterConstraints = BuildBuildList<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>(_typeParameterConstraints, visited);
        var properties = BuildBuildList<PropertyDeclarationModel, PropertyDeclarationModelBuilder>(_properties, visited);
        var methods = BuildBuildList<MethodDeclarationModel, MethodDeclarationModelBuilder>(_methods, visited);
        var events = BuildBuildList<EventModel, EventModelBuilder>(_events, visited);
        var nestedInterfaces = BuildBuildList<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder>(_nestedInterfaces, visited);

        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(nameof(_name), new InvalidOperationException("Interface name must be provided."));
        }

        AddExceptions<AttributeDeclarationModel, AttributeDeclarationModelBuilder>(nameof(_attributes), attributes, exceptions);
        AddExceptions<TypeParameterDeclarationModel, TypeParameterDeclarationModelBuilder>(nameof(_typeParameters), typeParameters, exceptions);
        AddExceptions<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>(nameof(_typeParameterConstraints), typeParameterConstraints, exceptions);
        AddExceptions<PropertyDeclarationModel, PropertyDeclarationModelBuilder>(nameof(_properties), properties, exceptions);
        AddExceptions<MethodDeclarationModel, MethodDeclarationModelBuilder>(nameof(_methods), methods, exceptions);
        AddExceptions<EventModel, EventModelBuilder>(nameof(events), events, exceptions);
        AddExceptions<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder>(nameof(nestedInterfaces), nestedInterfaces, exceptions);

        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        return Success(new InterfaceDeclarationModel
        {
            Name = _name ?? string.Empty,
            Modifiers = Modifiers,
            Attributes = attributes.ToResultList(),
            BaseInterfaces = _baseInterfaces,
            TypeParameters = typeParameters.ToResultList(),
            TypeParameterConstraints = typeParameterConstraints.ToResultList(),
            Properties = properties.ToResultList(),
            Methods = methods.ToResultList(),
            Events = events.ToResultList(),
            NestedInterfaces = nestedInterfaces.ToResultList()
        });
    }
}
