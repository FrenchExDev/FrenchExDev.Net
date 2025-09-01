namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class NamespaceDeclarationModel
{
    public string Name { get; set; } = string.Empty;
    public List<InterfaceDeclarationModel> Interfaces { get; set; } = new();
    public List<ClassDeclarationModel> Classes { get; set; } = new();
    public List<EnumDeclarationModel> Enums { get; set; } = new();
    public List<StructDeclarationModel> Structs { get; set; } = new();
    public List<NamespaceDeclarationModel> NestedNamespaces { get; set; } = new();
}
