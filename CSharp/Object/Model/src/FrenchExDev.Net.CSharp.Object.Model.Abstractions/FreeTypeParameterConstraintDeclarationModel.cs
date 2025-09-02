namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class FreeTypeParameterConstraintDeclarationModel : IDeclarationModel
{
    public TypeParameterConstraintEnum? Constraint { get; set; }
    public List<string> Constraints { get; set; } = new();
}
