namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a model for an enum member declaration in C# code.
/// Encapsulates the member's name, optional value, and any attributes applied to the member.
/// Used for code generation or analysis scenarios.
/// </summary>
public class EnumMemberDeclarationModel : IDeclarationModel
{
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
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();
}
