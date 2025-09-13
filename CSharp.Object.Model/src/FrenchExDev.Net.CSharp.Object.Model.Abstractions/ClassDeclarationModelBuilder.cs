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
    public List<ClassModifier> Modifiers { get; } = [];

    // List of attribute builders for decorating the class.
    private readonly List<AttributeDeclarationModelBuilder> _attributes = [];

    // List of implemented interface names.
    private readonly List<string> _implementedInterfaces = [];

    // List of type parameter builders for generic class definitions.
    private readonly List<TypeParameterDeclarationModelBuilder> _typeParameters = [];

    // List of type parameter constraint builders.
    private readonly List<TypeParameterConstraintModelBuilder> _typeParameterConstraints = [];

    // List of field builders for the class.
    private readonly List<FieldDeclarationModelBuilder> _fields = [];

    // List of property builders for the class.
    private readonly List<PropertyDeclarationModelBuilder> _properties = [];

    // List of method builders for the class.
    private readonly List<MethodDeclarationModelBuilder> _methods = [];

    // List of constructor builders for the class.
    private readonly List<ConstructorDeclarationModelBuilder> _constructors = [];

    // List of nested class builders.
    private readonly List<ClassDeclarationModelBuilder> _nestedClasses = [];

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
    protected override IObjectBuildResult<ClassDeclarationModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        // Build all nested components and collect their results
        var attributes = BuildBuildListAndVisitForExceptions<AttributeDeclarationModel, AttributeDeclarationModelBuilder>(_attributes, visited, nameof(_attributes), exceptions);
        var typeParameters = BuildBuildListAndVisitForExceptions<TypeParameterDeclarationModel, TypeParameterDeclarationModelBuilder>(_typeParameters, visited, nameof(_typeParameters), exceptions);
        var typeParameterConstraints = BuildBuildListAndVisitForExceptions<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>(_typeParameterConstraints, visited, nameof(_typeParameterConstraints), exceptions);
        var fields = BuildBuildListAndVisitForExceptions<FieldDeclarationModel, FieldDeclarationModelBuilder>(_fields, visited, nameof(_fields), exceptions);
        var properties = BuildBuildListAndVisitForExceptions<PropertyDeclarationModel, PropertyDeclarationModelBuilder>(_properties, visited, nameof(_properties), exceptions);
        var methods = BuildBuildListAndVisitForExceptions<MethodDeclarationModel, MethodDeclarationModelBuilder>(_methods, visited, nameof(_methods), exceptions);
        var constructors = BuildBuildListAndVisitForExceptions<ConstructorDeclarationModel, ConstructorDeclarationModelBuilder>(_constructors, visited, nameof(_constructors), exceptions);
        var nestedClasses = BuildBuildListAndVisitForExceptions<ClassDeclarationModel, ClassDeclarationModelBuilder>(_nestedClasses, visited, nameof(_nestedClasses), exceptions);

        // Validate required class name
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(nameof(_name), new InvalidOperationException("Class name must be provided."));
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
