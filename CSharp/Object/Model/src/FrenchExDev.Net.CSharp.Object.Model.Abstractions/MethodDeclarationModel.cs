namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class MethodDeclarationModel
{
    public List<string> Modifiers { get; set; } = new();
    public string ReturnType { get; set; } = "void";
    public string Name { get; set; } = string.Empty;
    public List<ParameterDeclarationModel> Parameters { get; set; } = new();
    public string? Body { get; set; }
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();
}
