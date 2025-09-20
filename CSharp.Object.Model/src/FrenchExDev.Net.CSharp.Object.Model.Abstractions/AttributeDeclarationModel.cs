namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a model for an attribute declaration in C# code.
/// Encapsulates the attribute's name and its constructor arguments.
/// Used for code generation or analysis scenarios.
/// </summary>
public class AttributeDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// The name of the attribute (without the 'Attribute' suffix).
    /// <remarks>Should be set to a valid C# identifier. Example: "Serializable".</remarks>
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The list of arguments passed to the attribute's constructor.
    /// <remarks>Arguments are stored as strings and should represent valid C# expressions.</remarks>
    /// </summary>
    public List<string> Arguments { get; set; } = [];
}
