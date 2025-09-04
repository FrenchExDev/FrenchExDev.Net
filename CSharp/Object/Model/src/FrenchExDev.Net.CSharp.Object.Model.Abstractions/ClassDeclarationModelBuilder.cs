using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for constructing <see cref="ClassDeclarationModel"/> instances.
/// Provides a fluent API to configure class name, modifiers, attributes, base type, implemented interfaces,
/// type parameters, constraints, fields, properties, methods, constructors, and nested classes for code generation scenarios.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
public class ClassDeclarationModelBuilder : AbstractObjectBuilder<ClassDeclarationModel, ClassDeclarationModelBuilder>
{
    // Stores the class name.
    private string? _name;
    // Stores the base type name, if any.
    private string? _baseType;
    // List of class modifiers (e.g., public, abstract, sealed).
    public List<ClassModifier> Modifiers { get; } = new();

    // List of attribute builders for decorating the class.
    private readonly List<AttributeDeclarationModelBuilder> _attributes = new();

    // List of implemented interface names.
    private readonly List<string> _implementedInterfaces = new();

    // List of type parameter builders for generic class definitions.
    private readonly List<TypeParameterDeclarationModelBuilder> _typeParameters = new();

    // List of type parameter constraint builders.
    private readonly List<TypeParameterConstraintModelBuilder> _typeParameterConstraints = new();

    // List of field builders for the class.
    private readonly List<FieldDeclarationModelBuilder> _fields = new();

    // List of property builders for the class.
    private readonly List<PropertyDeclarationModelBuilder> _properties = new();

    // List of method builders for the class.
    private readonly List<MethodDeclarationModelBuilder> _methods = new();

    // List of constructor builders for the class.
    private readonly List<ConstructorDeclarationModelBuilder> _constructors = new();

    // List of nested class builders.
    private readonly List<ClassDeclarationModelBuilder> _nestedClasses = new();

    /// <summary>
    /// Sets the class name.
    /// </summary>
    public ClassDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds a modifier to the class (e.g., public, abstract).
    /// </summary>
    public ClassDeclarationModelBuilder Modifier(ClassModifier modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Adds an attribute to the class using a builder action.
    /// </summary>
    public ClassDeclarationModelBuilder Attribute(Action<AttributeDeclarationModelBuilder> attribute)
    {
        var builder = new AttributeDeclarationModelBuilder();
        attribute(builder);
        _attributes.Add(builder);
        return this;
    }

    /// <summary>
    /// Sets the base type for the class.
    /// </summary>
    public ClassDeclarationModelBuilder BaseType(string baseType)
    {
        _baseType = baseType;
        return this;
    }

    /// <summary>
    /// Adds an implemented interface to the class.
    /// </summary>
    public ClassDeclarationModelBuilder ImplementedInterface(string implementedInterface)
    {
        _implementedInterfaces.Add(implementedInterface);
        return this;
    }

    /// <summary>
    /// Adds a type parameter to the class.
    /// </summary>
    public ClassDeclarationModelBuilder TypeParameter(TypeParameterDeclarationModelBuilder typeParameter)
    {
        _typeParameters.Add(typeParameter);
        return this;
    }

    /// <summary>
    /// Adds a type parameter constraint to the class.
    /// </summary>
    public ClassDeclarationModelBuilder TypeParameterConstraint(TypeParameterConstraintModelBuilder typeParameterConstraint)
    {
        _typeParameterConstraints.Add(typeParameterConstraint);
        return this;
    }

    /// <summary>
    /// Adds a field to the class using a builder action.
    /// </summary>
    public ClassDeclarationModelBuilder Field(Action<FieldDeclarationModelBuilder> field)
    {
        var builder = new FieldDeclarationModelBuilder();
        field(builder);
        _fields.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a property to the class.
    /// </summary>
    public ClassDeclarationModelBuilder Property(PropertyDeclarationModelBuilder property)
    {
        _properties.Add(property);
        return this;
    }

    /// <summary>
    /// Adds a method to the class.
    /// </summary>
    public ClassDeclarationModelBuilder Method(MethodDeclarationModelBuilder method)
    {
        _methods.Add(method);
        return this;
    }

    /// <summary>
    /// Adds a constructor to the class.
    /// </summary>
    public ClassDeclarationModelBuilder Constructor(ConstructorDeclarationModelBuilder constructor)
    {
        _constructors.Add(constructor);
        return this;
    }

    /// <summary>
    /// Adds a nested class to the class.
    /// </summary>
    public ClassDeclarationModelBuilder NestedClass(ClassDeclarationModelBuilder nestedClass)
    {
        _nestedClasses.Add(nestedClass);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="ClassDeclarationModel"/> instance.
    /// Validates required properties and collects any build errors from nested builders.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    protected override IObjectBuildResult<ClassDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        // Build all nested components and collect their results
        var attributes = BuildList<AttributeDeclarationModel, AttributeDeclarationModelBuilder>(_attributes, visited);
        var typeParameters = BuildList<TypeParameterDeclarationModel, TypeParameterDeclarationModelBuilder>(_typeParameters, visited);
        var typeParameterConstraints = BuildList<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>(_typeParameterConstraints, visited);
        var fields = BuildList<FieldDeclarationModel, FieldDeclarationModelBuilder>(_fields, visited);
        var properties = BuildList<PropertyDeclarationModel, PropertyDeclarationModelBuilder>(_properties, visited);
        var methods = BuildList<MethodDeclarationModel, MethodDeclarationModelBuilder>(_methods, visited);
        var constructors = BuildList<ConstructorDeclarationModel, ConstructorDeclarationModelBuilder>(_constructors, visited);
        var nestedClasses = BuildList<ClassDeclarationModel, ClassDeclarationModelBuilder>(_nestedClasses, visited);

        // Collect exceptions from failed nested builds
        AddExceptions<MethodDeclarationModel, MethodDeclarationModelBuilder>(methods, exceptions);
        AddExceptions<AttributeDeclarationModel, AttributeDeclarationModelBuilder>(attributes, exceptions);
        AddExceptions<TypeParameterDeclarationModel, TypeParameterDeclarationModelBuilder>(typeParameters, exceptions);
        AddExceptions<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>(typeParameterConstraints, exceptions);
        AddExceptions<FieldDeclarationModel, FieldDeclarationModelBuilder>(fields, exceptions);
        AddExceptions<PropertyDeclarationModel, PropertyDeclarationModelBuilder>(properties, exceptions);
        AddExceptions<ConstructorDeclarationModel, ConstructorDeclarationModelBuilder>(constructors, exceptions);
        AddExceptions<ClassDeclarationModel, ClassDeclarationModelBuilder>(nestedClasses, exceptions);

        // Validate required class name
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Class name must be provided."));
        }

        // If any errors were collected, return a failure result
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        // Ensure class name is not null
        ArgumentNullException.ThrowIfNull(_name);
        ArgumentNullException.ThrowIfNull(Modifiers);
        ArgumentNullException.ThrowIfNull(_implementedInterfaces);
        ArgumentNullException.ThrowIfNull(_fields);
        ArgumentNullException.ThrowIfNull(_properties);
        ArgumentNullException.ThrowIfNull(_methods);
        ArgumentNullException.ThrowIfNull(_constructors);
        ArgumentNullException.ThrowIfNull(_nestedClasses);

        // Return a successful build result with the constructed ClassDeclarationModel
        return Success(new ClassDeclarationModel
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
