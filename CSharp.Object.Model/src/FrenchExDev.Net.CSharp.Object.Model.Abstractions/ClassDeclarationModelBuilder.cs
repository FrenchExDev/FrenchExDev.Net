using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for constructing <see cref="ClassDeclarationModel"/> instances.
/// Provides a fluent API to configure class name, modifiers, attributes, base type, implemented interfaces,
/// type parameters, constraints, fields, properties, methods, constructors, and nested classes for code generation scenarios.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
public class ClassDeclarationModelBuilder : AbstractBuilder<ClassDeclarationModel>, IDeclarationModelBuilder
{
    // Stores the class name.
    private string? _name;

    // Stores the base type name, if any.
    private ClassDeclarationModelBuilder? _baseType;

    // List of class modifiers (e.g., public, abstract, sealed).
    private readonly List<ClassModifier> _modifiers = [];

    // List of attribute builders for decorating the class.
    private readonly BuilderList<AttributeDeclarationModel, AttributeDeclarationModelBuilder> _attributes = [];

    // List of implemented interface names.
    private readonly BuilderList<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder> _implementedInterfaces = [];

    // List of type parameter builders for generic class definitions.
    private readonly BuilderList<TypeParameterDeclarationModel, TypeParameterDeclarationModelBuilder> _typeParameters = [];

    // List of type parameter constraint builders.
    private readonly BuilderList<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder> _typeParameterConstraints = [];

    // List of field builders for the class.
    private readonly BuilderList<FieldDeclarationModel, FieldDeclarationModelBuilder> _fields = [];

    // List of property builders for the class.
    private readonly BuilderList<PropertyDeclarationModel, PropertyDeclarationModelBuilder> _properties = [];

    // List of method builders for the class.
    private readonly BuilderList<MethodDeclarationModel, MethodDeclarationModelBuilder> _methods = [];

    // List of constructor builders for the class.
    private readonly BuilderList<ConstructorDeclarationModel, ConstructorDeclarationModelBuilder> _constructors = [];

    // List of nested class builders.
    private readonly BuilderList<ClassDeclarationModel, ClassDeclarationModelBuilder> _nestedClasses = [];

    /// <summary>
    /// Sets the class name.
    /// </summary>
    public ClassDeclarationModelBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds a modifier to the class (e.g., public, abstract).
    /// </summary>
    public ClassDeclarationModelBuilder WithModifier(ClassModifier modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Adds the public access modifier to the class declaration being built.
    /// </summary>
    /// <remarks>This method enables the class to be declared as public when the model is built. Multiple
    /// modifier methods can be chained to configure the class declaration as needed.</remarks>
    /// <returns>The current <see cref="ClassDeclarationModelBuilder"/> instance with the public modifier applied.</returns>
    public ClassDeclarationModelBuilder WithPublicModifier()
    {
        _modifiers.Add(ClassModifier.Public);
        return this;
    }

    /// <summary>
    /// Adds the internal access modifier to the class declaration being built.
    /// </summary>
    /// <remarks>Use this method to specify that the generated class should have internal accessibility. This
    /// method can be chained with other modifier methods to configure the class declaration as needed.</remarks>
    /// <returns>The current <see cref="ClassDeclarationModelBuilder"/> instance with the internal modifier applied.</returns>
    public ClassDeclarationModelBuilder WithInternalModifier()
    {
        _modifiers.Add(ClassModifier.Internal);
        return this;
    }

    /// <summary>
    /// Adds the abstract modifier to the class declaration being built.
    /// </summary>
    /// <remarks>Use this method to specify that the generated class should be abstract. This enables the
    /// class to contain abstract members and prevents direct instantiation.</remarks>
    /// <returns>The current <see cref="ClassDeclarationModelBuilder"/> instance with the abstract modifier applied.</returns>
    public ClassDeclarationModelBuilder WithAbstractModifier()
    {
        _modifiers.Add(ClassModifier.Abstract);
        return this;
    }

    /// <summary>
    /// Adds the sealed modifier to the class declaration being built.
    /// </summary>
    /// <remarks>Use this method to indicate that the generated class should be sealed, preventing further
    /// inheritance. This method can be chained with other modifier methods to configure the class
    /// declaration.</remarks>
    /// <returns>The current <see cref="ClassDeclarationModelBuilder"/> instance with the sealed modifier applied.</returns>
    public ClassDeclarationModelBuilder WithSealedModifier()
    {
        _modifiers.Add(ClassModifier.Sealed);
        return this;
    }

    /// <summary>
    /// Adds an attribute to the class using a builder action.
    /// </summary>
    public ClassDeclarationModelBuilder WithAttribute(Action<AttributeDeclarationModelBuilder> attribute)
    {
        var builder = new AttributeDeclarationModelBuilder();
        attribute(builder);
        _attributes.Add(builder);
        return this;
    }

    /// <summary>
    /// Sets the base type for the class.
    /// </summary>
    public ClassDeclarationModelBuilder WithBaseType(ClassDeclarationModelBuilder baseType)
    {
        _baseType = baseType;
        return this;
    }

    /// <summary>
    /// Adds an implemented interface to the class.
    /// </summary>
    public ClassDeclarationModelBuilder WithImplementedInterface(InterfaceDeclarationModelBuilder implementedInterface)
    {
        _implementedInterfaces.Add(implementedInterface);
        return this;
    }

    /// <summary>
    /// Adds a type parameter to the class.
    /// </summary>
    public ClassDeclarationModelBuilder WithTypeParameter(TypeParameterDeclarationModelBuilder typeParameter)
    {
        _typeParameters.Add(typeParameter);
        return this;
    }

    /// <summary>
    /// Adds a type parameter constraint to the class.
    /// </summary>
    public ClassDeclarationModelBuilder WithTypeParameterConstraint(TypeParameterConstraintModelBuilder typeParameterConstraint)
    {
        _typeParameterConstraints.Add(typeParameterConstraint);
        return this;
    }

    /// <summary>
    /// Adds a field to the class using a builder action.
    /// </summary>
    public ClassDeclarationModelBuilder WithField(Action<FieldDeclarationModelBuilder> field)
    {
        var builder = new FieldDeclarationModelBuilder();
        field(builder);
        _fields.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a property to the class.
    /// </summary>
    public ClassDeclarationModelBuilder WithProperty(PropertyDeclarationModelBuilder property)
    {
        _properties.Add(property);
        return this;
    }

    /// <summary>
    /// Adds a method to the class.
    /// </summary>
    public ClassDeclarationModelBuilder WithMethod(MethodDeclarationModelBuilder method)
    {
        _methods.Add(method);
        return this;
    }

    /// <summary>
    /// Adds a constructor to the class.
    /// </summary>
    public ClassDeclarationModelBuilder WithConstructor(ConstructorDeclarationModelBuilder constructor)
    {
        _constructors.Add(constructor);
        return this;
    }

    /// <summary>
    /// Adds a nested class to the class.
    /// </summary>
    public ClassDeclarationModelBuilder WithNestedClass(ClassDeclarationModelBuilder nestedClass)
    {
        _nestedClasses.Add(nestedClass);
        return this;
    }

    /// <summary>
    /// Builds the internal structure of the type by collecting metadata for its base type, interfaces, attributes, type
    /// parameters, members, and nested types.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track visited objects during the build process to prevent redundant processing and handle
    /// circular references.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        _baseType?.Build(visitedCollector);
        BuildList(_implementedInterfaces, visitedCollector);
        BuildList(_attributes, visitedCollector);
        BuildList(_typeParameters, visitedCollector);
        BuildList(_typeParameterConstraints, visitedCollector);
        BuildList(_fields, visitedCollector);
        BuildList(_properties, visitedCollector);
        BuildList(_methods, visitedCollector);
        BuildList(_constructors, visitedCollector);
        BuildList(_nestedClasses, visitedCollector);
    }

    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new DeclarationHaveNoNameException());
        }
    }

    protected override ClassDeclarationModel Instantiate() => new(_name ?? throw new DeclarationHaveNoNameException(),
            _modifiers, _attributes.AsReferenceList(), _baseType?.Reference(),
            _implementedInterfaces.AsReferenceList(), _typeParameters.AsReferenceList(),
            _typeParameterConstraints.AsReferenceList(), _fields.AsReferenceList(), _properties.AsReferenceList(),
            _methods.AsReferenceList(), _constructors.AsReferenceList(), _nestedClasses.AsReferenceList());
}
