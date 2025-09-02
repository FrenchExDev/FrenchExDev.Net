namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a model for a C# class declaration, including its name, modifiers, attributes, base type, implemented interfaces,
/// type parameters, constraints, fields, properties, methods, constructors, and nested classes.
/// This model is used for code generation and analysis scenarios.
/// </summary>
public class ClassDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// The name of the class.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The list of modifiers applied to the class (e.g., public, abstract, sealed).
    /// </summary>
    public List<ClassModifier> Modifiers { get; set; } = new();

    /// <summary>
    /// The list of attributes decorating the class.
    /// </summary>
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();

    /// <summary>
    /// The base type that the class inherits from, if any.
    /// </summary>
    public string? BaseType { get; set; }

    /// <summary>
    /// The list of interfaces implemented by the class.
    /// </summary>
    public List<string> ImplementedInterfaces { get; set; } = new();

    /// <summary>
    /// The list of type parameters for generic class definitions.
    /// </summary>
    public List<TypeParameterDeclarationModel> TypeParameters { get; set; } = new();

    /// <summary>
    /// The list of constraints applied to the type parameters.
    /// </summary>
    public List<TypeParameterConstraintModel> TypeParameterConstraints { get; set; } = new();

    /// <summary>
    /// The list of fields declared in the class.
    /// </summary>
    public List<FieldDeclarationModel> Fields { get; set; } = new();

    /// <summary>
    /// The list of properties declared in the class.
    /// </summary>
    public List<PropertyDeclarationModel> Properties { get; set; } = new();

    /// <summary>
    /// The list of methods declared in the class.
    /// </summary>
    public List<MethodDeclarationModel> Methods { get; set; } = new();

    /// <summary>
    /// The list of constructors for the class.
    /// </summary>
    public List<ConstructorDeclarationModel> Constructors { get; set; } = new();

    /// <summary>
    /// The list of nested classes declared within this class.
    /// </summary>
    public List<ClassDeclarationModel> NestedClasses { get; set; } = new();
}
