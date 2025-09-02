namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class FieldDeclarationModel : IDeclarationModel
{
    public List<string> Modifiers { get; set; } = new();
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Initializer { get; set; }
}
