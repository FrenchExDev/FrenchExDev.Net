namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class ParameterDeclarationModel : IDeclarationModel
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? DefaultValue { get; set; }
}
