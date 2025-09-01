namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class StructDeclarationModel
{
    public string Name { get; set; } = string.Empty;
    public List<StructModifier> Modifiers { get; set; } = new();
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();
}
