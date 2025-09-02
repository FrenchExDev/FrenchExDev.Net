namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class TypeParameterConstraintModel : IDeclarationModel
{
    public string TypeParameter { get; set; } = string.Empty;
    public List<FreeTypeParameterConstraintDeclarationModel> Constraints { get; set; } = new();
}
