using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a model for a C# class declaration, including its name, modifiers, attributes, base type, implemented interfaces,
/// type parameters, constraints, fields, properties, methods, constructors, and nested classes.
/// This model is used for code generation and analysis scenarios.
/// </summary>
public class ClassDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClassDeclarationModel"/> class with the specified class name, modifiers,
    /// attributes, base type, implemented interfaces, type parameters, constraints, members, and nested classes.
    /// <remarks>All collections should be non-null and properly initialized before use.</remarks>
    /// </summary>
    /// <param name="name">The name of the class being declared. Cannot be null.</param>
    /// <param name="modifiers">A list of modifiers applied to the class, such as public, abstract, or sealed.</param>
    /// <param name="attributes">A list of attribute declarations applied to the class.</param>
    /// <param name="baseType">A reference to the base class that this class inherits from, or null if there is no base class.</param>
    /// <param name="implementedInterfaces">A list of interfaces implemented by the class.</param>
    /// <param name="typeParameters">A list of type parameters declared for the class, if it is generic.</param>
    /// <param name="typeParameterConstraints">A list of constraints applied to the class's type parameters.</param>
    /// <param name="fields">A list of field declarations defined in the class.</param>
    /// <param name="properties">A list of property declarations defined in the class.</param>
    /// <param name="methods">A list of method declarations defined in the class.</param>
    /// <param name="constructors">A list of constructor declarations for the class.</param>
    /// <param name="nestedClasses">A list of nested class declarations contained within this class.</param>
    public ClassDeclarationModel(string name, List<ClassModifier> modifiers, ReferenceList<AttributeDeclarationModel> attributes,
        Reference<ClassDeclarationModel>? baseType, ReferenceList<InterfaceDeclarationModel> implementedInterfaces,
        ReferenceList<TypeParameterDeclarationModel> typeParameters, ReferenceList<TypeParameterConstraintModel> typeParameterConstraints,
        ReferenceList<FieldDeclarationModel> fields, ReferenceList<PropertyDeclarationModel> properties, ReferenceList<MethodDeclarationModel> methods,
        ReferenceList<ConstructorDeclarationModel> constructors, ReferenceList<ClassDeclarationModel> nestedClasses
    )
    {
        _name = name;
        _modifiers = modifiers;
        _attributes = attributes;
        _baseType = baseType;
        _implementedInterfaces = implementedInterfaces;
        _typeParameters = typeParameters;
        _typeParameterConstraints = typeParameterConstraints;
        _fields = fields;
        _properties = properties;
        _methods = methods;
        _constructors = constructors;
        _nestedClasses = nestedClasses;
    }

    /// <summary>
    /// The name of the class.
    /// <remarks>Should be a valid C# identifier and unique within its namespace.</remarks>
    /// </summary>
    private string _name;

    /// <summary>
    /// Gets the name of the class.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// The list of modifiers applied to the class (e.g., public, abstract, sealed).
    /// <remarks>Modifiers control accessibility, inheritance, and other class behaviors.</remarks>
    /// </summary>
    private List<ClassModifier> _modifiers;

    /// <summary>
    /// Gets the modifiers applied to the class as a read-only enumerable.
    /// </summary>
    public IEnumerable<ClassModifier> Modifiers => _modifiers.AsReadOnly();

    /// <summary>
    /// Gets an enumerable collection of attribute declarations that have been resolved for the current model.
    /// <remarks>Attributes provide metadata and can influence code generation or runtime behavior.</remarks>
    /// </summary>
    private ReferenceList<AttributeDeclarationModel> _attributes;

    /// <summary>
    /// Gets the resolved attribute declarations for the class.
    /// </summary>
    public IEnumerable<AttributeDeclarationModel> Attributes => _attributes.AsEnumerable();

    /// <summary>
    /// The base type that the class inherits from, if any.
    /// <remarks>Null if the class does not inherit from another class.</remarks>
    /// </summary>
    private Reference<ClassDeclarationModel>? _baseType;

    /// <summary>
    /// Gets the resolved base type for this class declaration.
    /// <remarks>Throws <see cref="InvalidOperationException"/> if the base type reference is not resolved. Ensure type resolution before accessing.</remarks>
    /// </summary>
    public ClassDeclarationModel BaseType => _baseType?.Resolved() ?? throw new InvalidOperationException("Base type reference is not set.");

    /// <summary>
    /// The collection of interfaces implemented by this class.
    /// <remarks>Order reflects declaration order in source code.</remarks>
    /// </summary>
    private ReferenceList<InterfaceDeclarationModel> _implementedInterfaces;

    /// <summary>
    /// Gets the resolved interfaces implemented by this class.
    /// </summary>
    public IEnumerable<InterfaceDeclarationModel> ImplementedInterfaces => _implementedInterfaces.AsEnumerable();

    /// <summary>
    /// The collection of type parameter declarations associated with this class.
    /// <remarks>Relevant for generic classes.</remarks>
    /// </summary>
    private ReferenceList<TypeParameterDeclarationModel> _typeParameters;

    /// <summary>
    /// Gets the resolved type parameters for this class.
    /// </summary>
    public IEnumerable<TypeParameterDeclarationModel> TypeParameters => _typeParameters.AsEnumerable();

    /// <summary>
    /// The collection of constraints applied to the type parameters of the class.
    /// <remarks>Constraints restrict the types that can be used as type arguments.</remarks>
    /// </summary>
    private ReferenceList<TypeParameterConstraintModel> _typeParameterConstraints;

    /// <summary>
    /// Gets the resolved type parameter constraints for this class.
    /// </summary>
    public IEnumerable<TypeParameterConstraintModel> TypeParameterConstraints => _typeParameterConstraints.AsEnumerable();

    /// <summary>
    /// The collection of field declarations defined in the class.
    /// <remarks>Fields represent data members and should be properly initialized.</remarks>
    /// </summary>
    private ReferenceList<FieldDeclarationModel> _fields;

    /// <summary>
    /// Gets the resolved field declarations for this class.
    /// </summary>
    public IEnumerable<FieldDeclarationModel> Fields => _fields.AsEnumerable();

    /// <summary>
    /// The collection of property declarations defined in the class.
    /// <remarks>Properties provide controlled access to class data.</remarks>
    /// </summary>
    private ReferenceList<PropertyDeclarationModel> _properties;

    /// <summary>
    /// Gets the resolved property declarations for this class.
    /// </summary>
    public IEnumerable<PropertyDeclarationModel> Properties => _properties.AsEnumerable();

    /// <summary>
    /// The collection of method declarations defined in the class.
    /// <remarks>Methods implement class behavior and logic.</remarks>
    /// </summary>
    private ReferenceList<MethodDeclarationModel> _methods;

    /// <summary>
    /// Gets the resolved method declarations for this class.
    /// </summary>
    public IEnumerable<MethodDeclarationModel> Methods => _methods.AsEnumerable();

    /// <summary>
    /// The collection of constructor declarations for the class.
    /// <remarks>Constructors initialize class instances and may have parameters.</remarks>
    /// </summary>
    private ReferenceList<ConstructorDeclarationModel> _constructors;

    /// <summary>
    /// Gets the resolved constructor declarations for this class.
    /// </summary>
    public IEnumerable<ConstructorDeclarationModel> Constructors => _constructors.AsEnumerable();

    /// <summary>
    /// The collection of nested class declarations contained within this class.
    /// <remarks>Nested classes are useful for encapsulating related types.</remarks>
    /// </summary>
    public ReferenceList<ClassDeclarationModel> _nestedClasses { get; set; } = new();

    /// <summary>
    /// Gets the resolved nested class declarations for this class.
    /// </summary>
    public IEnumerable<ClassDeclarationModel> NestedClasses => _nestedClasses.AsEnumerable();
}
