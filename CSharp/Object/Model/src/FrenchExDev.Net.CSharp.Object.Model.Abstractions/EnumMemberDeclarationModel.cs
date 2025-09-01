namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class EnumMemberDeclarationModel
{
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();
}
