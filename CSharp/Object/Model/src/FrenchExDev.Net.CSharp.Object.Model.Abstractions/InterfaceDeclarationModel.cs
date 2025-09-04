namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents an interface declaration, including its name, modifiers, attributes, base interfaces, type parameters, constraints, properties, methods, events, and nested interfaces.
/// </summary>
public class InterfaceDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Gets or sets the name of the interface.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of access modifiers applied to the interface (e.g., public, internal).
    /// </summary>
    public List<InterfaceModifier> Modifiers { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of attributes applied to the interface.
    /// </summary>
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of base interfaces that this interface inherits from.
    /// </summary>
    public List<string> BaseInterfaces { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of type parameters declared by the interface.
    /// </summary>
    public List<TypeParameterDeclarationModel> TypeParameters { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of type parameter constraints for the interface's type parameters.
    /// </summary>
    public List<TypeParameterConstraintModel> TypeParameterConstraints { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of properties declared in the interface.
    /// </summary>
    public List<PropertyDeclarationModel> Properties { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of methods declared in the interface.
    /// </summary>
    public List<MethodDeclarationModel> Methods { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of events declared in the interface.
    /// </summary>
    public List<EventModel> Events { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of nested interfaces declared within this interface.
    /// </summary>
    public List<InterfaceDeclarationModel> NestedInterfaces { get; set; } = new();
}
