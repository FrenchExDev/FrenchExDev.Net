namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a parameter declaration for a method or constructor, including its type, name, and optional default value.
/// </summary>
/// <remarks>
/// This model is used to describe parameters in code generation and analysis scenarios.
/// Example usage:
/// <code>
/// var parameter = new ParameterDeclarationModel
/// {
///     Type = "int",
///     Name = "count",
///     DefaultValue = "0"
/// };
/// </code>
/// </remarks>
public class ParameterDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Gets or sets the type of the parameter (e.g., "int", "string").
    /// </summary>
    /// <remarks>
    /// Example: <c>Type = "int"</c>
    /// </remarks>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    /// <remarks>
    /// Example: <c>Name = "count"</c>
    /// </remarks>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default value of the parameter, or null if none is specified.
    /// </summary>
    /// <remarks>
    /// Example: <c>DefaultValue = "0"</c>
    /// </remarks>
    public string? DefaultValue { get; set; }
}
