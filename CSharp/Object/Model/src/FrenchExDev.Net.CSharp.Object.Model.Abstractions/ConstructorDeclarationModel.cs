namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class ConstructorDeclarationModel
{
    public List<string> Modifiers { get; set; } = new();
    public string Name { get; set; } = string.Empty;
    public List<ParameterDeclarationModel> Parameters { get; set; } = new();
    public string? Body { get; set; }
}
