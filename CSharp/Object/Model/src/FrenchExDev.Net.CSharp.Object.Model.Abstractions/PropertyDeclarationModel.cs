namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a property declaration for a class, struct, or interface, including its modifiers, type, name, accessors, and optional initializer.
/// </summary>
/// <remarks>
/// This model is used for code generation and analysis scenarios where property metadata is required.
/// Example usage:
/// <code>
/// var property = new PropertyDeclarationModel
/// {
///     Modifiers = new List<string> { "public" },
///     Type = "int",
///     Name = "Id",
///     HasGetter = true,
///     HasSetter = false,
///     Initializer = "0"
/// };
/// </code>
/// </remarks>
public class PropertyDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Gets or sets the list of modifiers applied to the property (e.g., "public", "static").
    /// </summary>
    /// <remarks>
    /// Example: <c>Modifiers = new List&lt;string&gt; { "public", "static" }</c>
    /// </remarks>
    public List<string> Modifiers { get; set; } = new();

    /// <summary>
    /// Gets or sets the type of the property (e.g., "int", "string").
    /// </summary>
    /// <remarks>
    /// Example: <c>Type = "string"</c>
    /// </remarks>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the property.
    /// </summary>
    /// <remarks>
    /// Example: <c>Name = "Id"</c>
    /// </remarks>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the property has a getter.
    /// </summary>
    /// <remarks>
    /// Example: <c>HasGetter = true</c>
    /// </remarks>
    public bool HasGetter { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the property has a setter.
    /// </summary>
    /// <remarks>
    /// Example: <c>HasSetter = false</c>
    /// </remarks>
    public bool HasSetter { get; set; } = true;

    /// <summary>
    /// Gets or sets the optional initializer for the property, or null if none is specified.
    /// </summary>
    /// <remarks>
    /// Example: <c>Initializer = "0"</c>
    /// </remarks>
    public string? Initializer { get; set; }
}
