namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class InterfaceDeclarationModel
{
    public string Name { get; set; } = string.Empty;
    public List<InterfaceModifier> Modifiers { get; set; } = new();
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();
    public List<string> BaseInterfaces { get; set; } = new();
    public List<TypeParameterDeclarationModel> TypeParameters { get; set; } = new();
    public List<TypeParameterConstraintModel> TypeParameterConstraints { get; set; } = new();
    public List<PropertyDeclarationModel> Properties { get; set; } = new();
    public List<MethodDeclarationModel> Methods { get; set; } = new();
    public List<EventModel> Events { get; set; } = new();
    public List<InterfaceDeclarationModel> NestedInterfaces { get; set; } = new();
}
