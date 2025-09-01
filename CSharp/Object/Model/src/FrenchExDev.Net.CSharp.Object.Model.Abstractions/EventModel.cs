namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class EventModel
{
    public List<string> Modifiers { get; set; } = new();
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();
}
