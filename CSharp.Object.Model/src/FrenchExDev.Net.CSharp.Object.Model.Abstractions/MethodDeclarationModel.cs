namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a method declaration, including its modifiers, return type, name, parameters, body, and attributes.
/// </summary>
public class MethodDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Gets or sets the list of modifiers applied to the method (e.g., public, static).
    /// </summary>
    public List<string> Modifiers { get; set; } = new();

    /// <summary>
    /// Gets or sets the return type of the method (e.g., int, void).
    /// </summary>
    public string ReturnType { get; set; } = "void";

    /// <summary>
    /// Gets or sets the name of the method.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of parameters for the method.
    /// </summary>
    public List<ParameterDeclarationModel> Parameters { get; set; } = new();

    /// <summary>
    /// Gets or sets the body of the method as a string. Null if the method is abstract or interface.
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Gets or sets the list of attributes applied to the method.
    /// </summary>
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();
}
