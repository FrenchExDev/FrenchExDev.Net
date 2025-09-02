namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class AttributeDeclarationModel : IDeclarationModel
{
    public string Name { get; set; } = string.Empty;
    public List<string> Arguments { get; set; } = new();
}
