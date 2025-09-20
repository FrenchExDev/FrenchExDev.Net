namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a model for a field declaration, including its modifiers, type, name, and optional initializer.
/// </summary>
/// <remarks>This class is typically used to describe field declarations in code analysis or code generation
/// scenarios. It provides properties to access and modify the components of a field, such as its access modifiers, data
/// type, identifier, and initializer expression if present.</remarks>
public class FieldDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Initializes a new instance of the FieldDeclarationModel class with the specified modifiers, type, name, and
    /// optional initializer.
    /// </summary>
    /// <param name="modifiers">A list of modifiers applied to the field, such as "public", "static", or "readonly". Cannot be null.</param>
    /// <param name="type">The data type of the field, represented as a string. Cannot be null or empty.</param>
    /// <param name="name">The name of the field. Cannot be null or empty.</param>
    /// <param name="initializer">The optional initializer value for the field. Specify null if the field does not have an initializer.</param>
    public FieldDeclarationModel(List<string> modifiers, string type, string name, string? initializer)
    {
        Modifiers = modifiers;
        Type = type;
        Name = name;
        Initializer = initializer;
    }

    /// <summary>
    /// Gets or sets the collection of modifier strings associated with the current instance.
    /// </summary>
    public List<string> Modifiers { get; set; } = [];

    /// <summary>
    /// Gets or sets the type identifier associated with the object.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name associated with the object.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the initializer expression used to set the initial value of the member.
    /// </summary>
    public string? Initializer { get; set; }
}
