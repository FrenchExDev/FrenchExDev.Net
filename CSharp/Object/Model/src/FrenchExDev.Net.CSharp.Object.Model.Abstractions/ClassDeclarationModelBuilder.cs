using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class ClassDeclarationModelBuilder : AbstractObjectBuilder<ClassDeclarationModel, ClassDeclarationModelBuilder>
{
    private string? _name;
    private string? _baseType;
    public List<ClassModifier> Modifiers { get; } = new();
    private readonly List<AttributeDeclarationModelBuilder> _attributes = new();
    private readonly List<string> _implementedInterfaces = new();
    private readonly List<TypeParameterDeclarationModelBuilder> _typeParameters = new();
    private readonly List<TypeParameterConstraintModelBuilder> _typeParameterConstraints = new();
    private readonly List<FieldDeclarationModelBuilder> _fields = new();
    private readonly List<PropertyDeclarationModelBuilder> _properties = new();
    private readonly List<MethodDeclarationModelBuilder> _methods = new();
    private readonly List<ConstructorDeclarationModelBuilder> _constructors = new();
    private readonly List<ClassDeclarationModelBuilder> _nestedClasses = new();

    public ClassDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public ClassDeclarationModelBuilder Modifier(ClassModifier modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }

    public ClassDeclarationModelBuilder Attribute(Action<AttributeDeclarationModelBuilder> attribute)
    {
        var builder = new AttributeDeclarationModelBuilder();
        attribute(builder);
        _attributes.Add(builder);
        return this;
    }

    public ClassDeclarationModelBuilder BaseType(string baseType)
    {
        _baseType = baseType;
        return this;
    }

    public ClassDeclarationModelBuilder ImplementedInterface(string implementedInterface)
    {
        _implementedInterfaces.Add(implementedInterface);
        return this;
    }

    public ClassDeclarationModelBuilder TypeParameter(TypeParameterDeclarationModelBuilder typeParameter)
    {
        _typeParameters.Add(typeParameter);
        return this;
    }

    public ClassDeclarationModelBuilder TypeParameterConstraint(TypeParameterConstraintModelBuilder typeParameterConstraint)
    {
        _typeParameterConstraints.Add(typeParameterConstraint);
        return this;
    }

    public ClassDeclarationModelBuilder Field(Action<FieldDeclarationModelBuilder> field)
    {
        var builder = new FieldDeclarationModelBuilder();
        field(builder);
        _fields.Add(builder);
        return this;
    }

    public ClassDeclarationModelBuilder Property(PropertyDeclarationModelBuilder property)
    {
        _properties.Add(property);
        return this;
    }

    public ClassDeclarationModelBuilder Method(MethodDeclarationModelBuilder method)
    {
        _methods.Add(method);
        return this;
    }

    public ClassDeclarationModelBuilder Constructor(ConstructorDeclarationModelBuilder constructor)
    {
        _constructors.Add(constructor);
        return this;
    }

    public ClassDeclarationModelBuilder NestedClass(ClassDeclarationModelBuilder nestedClass)
    {
        _nestedClasses.Add(nestedClass);
        return this;
    }

    protected override IObjectBuildResult<ClassDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        var attributes = _attributes
            .Select(x => x.Build(visited))
            .ToList();

        var typeParameters = _typeParameters
            .Select(x => x.Build(visited))
            .ToList();

        var typeParameterConstraints = _typeParameterConstraints
            .Select(x => x.Build(visited))
            .ToList();

        var fields = _fields
            .Select(x => x.Build(visited))
            .ToList();

        var properties = _properties
            .Select(x => x.Build(visited))
            .ToList();

        var methods = _methods
            .Select(x => x.Build(visited))
            .ToList();

        var constructors = _constructors
            .Select(x => x.Build(visited))
            .ToList();


        var nestedClasses = _nestedClasses
            .Select(x => x.Build(visited))
            .ToList();

        if (fields.OfType<FailureObjectBuildResult<FieldDeclarationModel, FieldDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(fields
                .OfType<FailureObjectBuildResult<FieldDeclarationModel, FieldDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (properties.OfType<FailureObjectBuildResult<PropertyDeclarationModel, PropertyDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(properties
                .OfType<FailureObjectBuildResult<PropertyDeclarationModel, PropertyDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (methods.OfType<FailureObjectBuildResult<MethodDeclarationModel, MethodDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(methods
                .OfType<FailureObjectBuildResult<MethodDeclarationModel, MethodDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (attributes.OfType<FailureObjectBuildResult<AttributeDeclarationModel, AttributeDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(attributes
                .OfType<FailureObjectBuildResult<AttributeDeclarationModel, AttributeDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (typeParameters.OfType<FailureObjectBuildResult<TypeParameterDeclarationModel, TypeParameterDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(typeParameters
                .OfType<FailureObjectBuildResult<TypeParameterDeclarationModel, TypeParameterDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (typeParameterConstraints.OfType<FailureObjectBuildResult<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>>().Any())
        {
            exceptions.AddRange(typeParameterConstraints
                .OfType<FailureObjectBuildResult<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Class name must be provided."));
        }

        if (nestedClasses.OfType<FailureObjectBuildResult<ClassDeclarationModel, ClassDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(nestedClasses
                .OfType<FailureObjectBuildResult<ClassDeclarationModel, ClassDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (constructors.OfType<FailureObjectBuildResult<ConstructorDeclarationModel, ConstructorDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(constructors
                .OfType<FailureObjectBuildResult<ConstructorDeclarationModel, ConstructorDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<ClassDeclarationModel, ClassDeclarationModelBuilder>(this, exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_name);

        return new SuccessObjectBuildResult<ClassDeclarationModel>(new ClassDeclarationModel
        {
            Name = _name,
            BaseType = _baseType,
            Modifiers = Modifiers,
            ImplementedInterfaces = _implementedInterfaces,
            Attributes = attributes.ToResultList(),
            TypeParameters = typeParameters.ToResultList(),
            TypeParameterConstraints = typeParameterConstraints.ToResultList(),
            Fields = fields.ToResultList(),
            Properties = properties.ToResultList(),
            Methods = methods.ToResultList(),
            Constructors = constructors.ToResultList(),
            NestedClasses = nestedClasses.ToResultList()
        });
    }
}
