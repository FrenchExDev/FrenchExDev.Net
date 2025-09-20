using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a model for an enum declaration in C# code.
/// Encapsulates the enum's name, modifiers, attributes, underlying type, and its members.
/// Used for code generation or analysis scenarios.
/// </summary>
public class EnumDeclarationModel : IDeclarationModel
{
    public EnumDeclarationModel(string name, List<EnumModifier> modifiers, ReferenceList<AttributeDeclarationModel> attributes, string? underlyingType, ReferenceList<EnumMemberDeclarationModel> members)
    {
        Name = name;
        Modifiers = modifiers;
        Attributes = attributes;
        UnderlyingType = underlyingType;
        Members = members;
    }

    /// <summary>
    /// The name of the enum (e.g., "DayOfWeek").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The list of modifiers applied to the enum (e.g., public, internal).
    /// </summary>
    public List<EnumModifier> Modifiers { get; set; } = [];

    /// <summary>
    /// The list of attributes decorating the enum.
    /// </summary>
    public ReferenceList<AttributeDeclarationModel> Attributes { get; set; } = [];

    /// <summary>
    /// The underlying type of the enum (e.g., "byte", "int"), or null for default.
    /// </summary>
    public string? UnderlyingType { get; set; }

    /// <summary>
    /// The list of members (fields) defined in the enum.
    /// </summary>
    public ReferenceList<EnumMemberDeclarationModel> Members { get; set; } = [];
}
