using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class DeclarationHaveNoNameException : Exception
{
    public DeclarationHaveNoNameException() : base("The declaration have no name.")
    {
    }
}

/// <summary>
/// Represents an interface declaration, including its name, modifiers, attributes, base interfaces, type parameters, constraints, properties, methods, events, and nested interfaces.
/// </summary>
public class InterfaceDeclarationModel : IDeclarationModel
{
    public InterfaceDeclarationModel(
        string name,
        IEnumerable<InterfaceModifier> modifiers,
        ReferenceList<AttributeDeclarationModel> attributes,
        ReferenceList<InterfaceDeclarationModel> baseInterfaces,
        ReferenceList<TypeParameterDeclarationModel> typeParameters,
        ReferenceList<TypeParameterConstraintModel> typeParameterConstraints,
        ReferenceList<PropertyDeclarationModel> properties,
        ReferenceList<MethodDeclarationModel> methods,
        ReferenceList<EventDeclarationModel> events,
        ReferenceList<InterfaceDeclarationModel> nestedInterfaces
        )
    {
        _name = name;
        _modifiers = [.. modifiers];
        _attributes = attributes;
        _baseInterfaces = baseInterfaces;
        _typeParameters = typeParameters;
        _typeParameterConstraints = typeParameterConstraints;
        _properties = properties;
        _methods = methods;
        _events = events;
        _nestedInterfaces = nestedInterfaces;
    }

    private string? _name;

    public string Name => _name ?? throw new DeclarationHaveNoNameException();

    private List<InterfaceModifier> _modifiers;

    public IEnumerable<InterfaceModifier> Modifiers => _modifiers;

    private ReferenceList<AttributeDeclarationModel> _attributes;

    public IEnumerable<AttributeDeclarationModel> Attributes => _attributes;

    private ReferenceList<InterfaceDeclarationModel> _baseInterfaces;

    public IEnumerable<InterfaceDeclarationModel> BaseInterfaces => _baseInterfaces;

    private ReferenceList<TypeParameterDeclarationModel> _typeParameters;

    public IEnumerable<TypeParameterDeclarationModel> TypeParameters => _typeParameters;

    private ReferenceList<TypeParameterConstraintModel> _typeParameterConstraints;
    public IEnumerable<TypeParameterConstraintModel> TypeParameterConstraints => _typeParameterConstraints;

    private ReferenceList<PropertyDeclarationModel> _properties;
    public IEnumerable<PropertyDeclarationModel> Properties => _properties;

    private ReferenceList<MethodDeclarationModel> _methods;
    public IEnumerable<MethodDeclarationModel> Methods => _methods;

    private ReferenceList<EventDeclarationModel> _events;
    public IEnumerable<EventDeclarationModel> Events => _events;

    private ReferenceList<InterfaceDeclarationModel> _nestedInterfaces;
    public IEnumerable<InterfaceDeclarationModel> NestedInterfaces => _nestedInterfaces;
}
