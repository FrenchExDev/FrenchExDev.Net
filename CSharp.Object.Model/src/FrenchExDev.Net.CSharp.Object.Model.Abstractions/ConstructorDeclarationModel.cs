using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a model for a C# constructor declaration.
/// Used for code generation and analysis, this class encapsulates the constructor's modifiers, name, parameters, and body.
/// </summary>
public class ConstructorDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// The list of modifiers applied to the constructor (e.g., public, private, static).
    /// </summary>
    public List<string> Modifiers { get; set; } = new();

    /// <summary>
    /// The name of the constructor, typically matching the class name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The list of parameters for the constructor.
    /// </summary>
    public ReferenceList<ParameterDeclarationModel> Parameters { get; set; } = new();

    /// <summary>
    /// The body of the constructor, represented as a string of C# code.
    /// </summary>
    public string? Body { get; set; }
}
