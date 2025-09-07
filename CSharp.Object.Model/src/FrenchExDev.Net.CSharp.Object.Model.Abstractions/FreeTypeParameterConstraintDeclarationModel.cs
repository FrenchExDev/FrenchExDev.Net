namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a free type parameter constraint declaration, including a main constraint and additional constraints.
/// </summary>
public class FreeTypeParameterConstraintDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Gets or sets the main type parameter constraint (e.g., class, struct, new, notnull).
    /// </summary>
    public TypeParameterConstraintEnum? Constraint { get; set; }

    /// <summary>
    /// Gets or sets the list of additional constraints as strings (e.g., interface names, base types).
    /// </summary>
    public List<string> Constraints { get; set; } = new();
}
