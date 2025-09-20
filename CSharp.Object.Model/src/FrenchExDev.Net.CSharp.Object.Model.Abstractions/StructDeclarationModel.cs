namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a struct declaration, including its name, modifiers, and attributes.
/// Used for code generation and analysis scenarios where struct metadata is required.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var structModel = new StructDeclarationModel("MyStruct", new List&lt;StructModifier&gt; { StructModifier.Public, StructModifier.ReadOnly }, new List&lt;AttributeDeclarationModel&gt; { new AttributeDeclarationModel { Name = "Serializable" } });
/// </code>
/// </remarks>
public class StructDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Initializes a new instance of the StructDeclarationModel class with the specified name, modifiers, and
    /// attributes.
    /// </summary>
    /// <param name="name">The name of the struct declaration. Cannot be null or empty.</param>
    /// <param name="modifiers">A list of modifiers applied to the struct, such as visibility or other keywords. Cannot be null.</param>
    /// <param name="attributes">A list of attribute declarations associated with the struct. Cannot be null.</param>
    public StructDeclarationModel(string name, List<StructModifier> modifiers, List<AttributeDeclarationModel> attributes)
    {
        Name = name;
        Modifiers = modifiers;
        Attributes = attributes;
    }

    /// <summary>
    /// Gets or sets the name of the struct (e.g., "MyStruct").
    /// </summary>
    /// <remarks>
    /// Example: <c>Name = "MyStruct"</c>
    /// </remarks>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of modifiers applied to the struct (e.g., public, readonly).
    /// </summary>
    /// <remarks>
    /// Example: <c>Modifiers = new List&lt;StructModifier&gt; { StructModifier.Public, StructModifier.ReadOnly }</c>
    /// </remarks>
    public List<StructModifier> Modifiers { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of attributes applied to the struct.
    /// </summary>
    /// <remarks>
    /// Example: <c>Attributes = new List&lt;AttributeDeclarationModel&gt; { new AttributeDeclarationModel { Name = "Serializable" } }</c>
    /// </remarks>
    public List<AttributeDeclarationModel> Attributes { get; set; } = [];
}
