namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class EnumDeclarationModel
{
    public string Name { get; set; } = string.Empty;
    public List<EnumModifier> Modifiers { get; set; } = new();
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();
    public string? UnderlyingType { get; set; }
    public List<EnumMemberDeclarationModel> Members { get; set; } = new();
}
