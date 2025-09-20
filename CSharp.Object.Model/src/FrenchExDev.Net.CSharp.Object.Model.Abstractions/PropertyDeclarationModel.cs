namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a property declaration for a class, struct, or interface, including its modifiers, type, name, accessors, and optional initializer.
/// </summary>
/// <remarks>
/// This model is used for code generation and analysis scenarios where property metadata is required.
/// Example usage:
/// <code>
/// var property = new PropertyDeclarationModel(new List<string> { "public" }, "int", "Id", true, false, "0");
/// </code>
/// </remarks>
public class PropertyDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Initializes a new instance of the PropertyDeclarationModel class with the specified property modifiers, type,
    /// name, accessors, and optional initializer.
    /// </summary>
    /// <param name="modifiers">A list of strings representing the modifiers applied to the property, such as "public", "static", or "virtual".
    /// Cannot be null.</param>
    /// <param name="type">The data type of the property, specified as a string. Cannot be null or empty.</param>
    /// <param name="name">The name of the property. Cannot be null or empty.</param>
    /// <param name="hasGetter">A value indicating whether the property includes a getter accessor. Set to <see langword="true"/> if the
    /// property has a getter; otherwise, <see langword="false"/>.</param>
    /// <param name="hasSetter">A value indicating whether the property includes a setter accessor. Set to <see langword="true"/> if the
    /// property has a setter; otherwise, <see langword="false"/>.</param>
    /// <param name="initializer">An optional string representing the property's initializer value, or null if the property does not have an
    /// initializer.</param>
    public PropertyDeclarationModel(List<string> modifiers, string type, string name, bool hasGetter, bool hasSetter, string? initializer)
    {
        Modifiers = modifiers;
        Type = type;
        Name = name;
        HasGetter = hasGetter;
        HasSetter = hasSetter;
        Initializer = initializer;
    }

    /// <summary>
    /// Gets or sets the list of modifiers applied to the property (e.g., "public", "static").
    /// </summary>
    /// <remarks>
    /// Example: <c>Modifiers = new List&lt;string&gt; { "public", "static" }</c>
    /// </remarks>
    public List<string> Modifiers { get; set; } = [];

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
