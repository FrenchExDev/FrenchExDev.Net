namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a model for an enum member declaration in C# code.
/// Encapsulates the member's name, optional value, and any attributes applied to the member.
/// Used for code generation or analysis scenarios.
/// </summary>
public class EnumMemberDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Initializes a new instance of the EnumMemberDeclarationModel class with the specified member name, value, and
    /// attributes.
    /// </summary>
    /// <param name="name">The name of the enumeration member. Cannot be null.</param>
    /// <param name="value">The value assigned to the enumeration member, or null if no explicit value is specified.</param>
    /// <param name="attributes">A list of attributes applied to the enumeration member. Cannot be null.</param>
    public EnumMemberDeclarationModel(string name, string? value, List<AttributeDeclarationModel> attributes)
    {
        Name = name;
        Value = value;
        Attributes = attributes;
    }

    /// <summary>
    /// The name of the enum member (e.g., "Monday").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The value assigned to the enum member, or null if not explicitly set.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// The list of attributes decorating the enum member.
    /// </summary>
    public List<AttributeDeclarationModel> Attributes { get; set; } = [];
}
