namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a model for a C# namespace declaration, including its name, contained interfaces, classes, enums, structs, and nested namespaces.
/// This model is used for code generation and analysis scenarios.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var model = new NamespaceDeclarationModel
/// {
///     Name = "MyCompany.MyProduct.Core",
///     Classes = new List<ClassDeclarationModel>
///     {
///         new ClassDeclarationModel { Name = "MyClass" }
///     },
///     Enums = new List<EnumDeclarationModel>
///     {
///         new EnumDeclarationModel { Name = "MyEnum" }
///     }
/// };
/// </code>
/// </remarks>
public class NamespaceDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Gets or sets the name of the namespace (e.g., "System.Collections.Generic").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of interfaces declared in this namespace.
    /// </summary>
    /// <remarks>
    /// Example: <c>Interfaces = new List&lt;InterfaceDeclarationModel&gt; { new InterfaceDeclarationModel { Name = "IMyInterface" } }</c>
    /// </remarks>
    public List<InterfaceDeclarationModel> Interfaces { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of classes declared in this namespace.
    /// </summary>
    /// <remarks>
    /// Example: <c>Classes = new List&lt;ClassDeclarationModel&gt; { new ClassDeclarationModel { Name = "MyClass" } }</c>
    /// </remarks>
    public List<ClassDeclarationModel> Classes { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of enums declared in this namespace.
    /// </summary>
    /// <remarks>
    /// Example: <c>Enums = new List&lt;EnumDeclarationModel&gt; { new EnumDeclarationModel { Name = "MyEnum" } }</c>
    /// </remarks>
    public List<EnumDeclarationModel> Enums { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of structs declared in this namespace.
    /// </summary>
    /// <remarks>
    /// Example: <c>Structs = new List&lt;StructDeclarationModel&gt; { new StructDeclarationModel { Name = "MyStruct" } }</c>
    /// </remarks>
    public List<StructDeclarationModel> Structs { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of nested namespaces declared within this namespace.
    /// </summary>
    /// <remarks>
    /// Example: <c>NestedNamespaces = new List&lt;NamespaceDeclarationModel&gt; { new NamespaceDeclarationModel { Name = "MyCompany.MyProduct.Core.SubNamespace" } }</c>
    /// </remarks>
    public List<NamespaceDeclarationModel> NestedNamespaces { get; set; } = new();
}
