namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class PropertyDeclarationModel : IDeclarationModel
{
    public List<string> Modifiers { get; set; } = new();
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool HasGetter { get; set; } = true;
    public bool HasSetter { get; set; } = true;
    public string? Initializer { get; set; }
}
