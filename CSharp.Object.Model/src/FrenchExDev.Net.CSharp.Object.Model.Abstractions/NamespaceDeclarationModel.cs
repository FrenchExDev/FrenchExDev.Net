using FrenchExDev.Net.CSharp.Object.Builder2;

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
    /// Holds the name of the namespace (e.g., "System.Collections.Generic").
    /// </summary>
    private string _name;
    /// <summary>
    /// Holds the scoping configuration for the namespace, which determines the visibility and accessibility of its members.
    /// </summary>
    private NamespaceScoping _scoping;

    /// <summary>
    /// Holds the collection of interface declarations implemented by the type.
    /// </summary>
    private ReferenceList<InterfaceDeclarationModel> _interfaces;

    /// <summary>
    /// Holds an enumerable collection of class declarations contained in the current context.
    /// </summary>
    private ReferenceList<ClassDeclarationModel> _classes;

    /// <summary>
    /// Stores the collection of enum declarations referenced by the containing type.
    /// </summary>
    private ReferenceList<EnumDeclarationModel> _enums;

    /// <summary>
    /// Holds an enumerable collection of struct declarations contained in the current context.
    /// </summary>
    private ReferenceList<StructDeclarationModel> _structs;

    /// <summary>
    /// Stores the collection of nested namespace declarations contained within this namespace.
    /// </summary>
    private ReferenceList<NamespaceDeclarationModel> _nestedNamespaces;

    /// <summary>
    /// Initializes a new instance of the NamespaceDeclarationModel class with the specified name, scoping, and
    /// contained type declarations.
    /// </summary>
    /// <remarks>All type declaration lists must be provided, even if empty. This constructor does not
    /// validate the uniqueness of type names within the namespace.</remarks>
    /// <param name="name">The name of the namespace to declare. Cannot be null or empty.</param>
    /// <param name="scoping">The scoping rules that apply to the namespace declaration.</param>
    /// <param name="interfaces">A list of interface declarations contained within the namespace. Cannot be null.</param>
    /// <param name="classes">A list of class declarations contained within the namespace. Cannot be null.</param>
    /// <param name="enums">A list of enum declarations contained within the namespace. Cannot be null.</param>
    /// <param name="structs">A list of struct declarations contained within the namespace. Cannot be null.</param>
    /// <param name="nestedNamespaces">A list of nested namespace declarations within this namespace. Cannot be null.</param>
    public NamespaceDeclarationModel(
        string name,
        NamespaceScoping scoping,
        ReferenceList<InterfaceDeclarationModel> interfaces,
        ReferenceList<ClassDeclarationModel> classes,
        ReferenceList<EnumDeclarationModel> enums,
        ReferenceList<StructDeclarationModel> structs,
        ReferenceList<NamespaceDeclarationModel> nestedNamespaces
    )
    {
        _name = name;
        _scoping = scoping;
        _interfaces = interfaces;
        _classes = classes;
        _enums = enums;
        _structs = structs;
        _nestedNamespaces = nestedNamespaces;
    }

    /// <summary>
    /// Gets or sets the name of the namespace (e.g., "System.Collections.Generic").
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the scoping configuration for the namespace, which determines the visibility and accessibility of its
    /// members.
    /// </summary>
    public NamespaceScoping? Scoping => _scoping;

    /// <summary>
    /// Gets the collection of interface declarations implemented by the type.
    /// </summary>
    public IEnumerable<InterfaceDeclarationModel> Interfaces => _interfaces.AsEnumerable();

    /// <summary>
    /// Gets an enumerable collection of class declarations contained in the current context.
    /// </summary>
    public IEnumerable<ClassDeclarationModel> Classes => _classes.AsEnumerable();

    /// <summary>
    /// Gets an enumerable collection of enum declaration models contained in the current context.
    /// </summary>
    public IEnumerable<EnumDeclarationModel> Enums => _enums.AsEnumerable();

    /// <summary>
    /// Gets an enumerable collection of struct declarations contained in the current context.
    /// </summary>
    public IEnumerable<StructDeclarationModel> Structs => _structs.AsEnumerable();

    /// <summary>
    /// Gets the collection of nested namespace declarations contained within this namespace.
    /// </summary>
    public IEnumerable<NamespaceDeclarationModel> NestedNamespaces => _nestedNamespaces.AsEnumerable();
}
