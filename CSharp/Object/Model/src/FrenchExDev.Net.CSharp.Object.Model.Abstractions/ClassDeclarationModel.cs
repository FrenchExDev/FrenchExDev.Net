namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class ClassDeclarationModel
{
    public string Name { get; set; } = string.Empty;
    public List<ClassModifier> Modifiers { get; set; } = new();
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();
    public string? BaseType { get; set; }
    public List<string> ImplementedInterfaces { get; set; } = new();
    public List<TypeParameterDeclarationModel> TypeParameters { get; set; } = new();
    public List<TypeParameterConstraintModel> TypeParameterConstraints { get; set; } = new();
    public List<FieldDeclarationModel> Fields { get; set; } = new();
    public List<PropertyDeclarationModel> Properties { get; set; } = new();
    public List<MethodDeclarationModel> Methods { get; set; } = new();
    public List<ConstructorDeclarationModel> Constructors { get; set; } = new();
    public List<ClassDeclarationModel> NestedClasses { get; set; } = new();
}
