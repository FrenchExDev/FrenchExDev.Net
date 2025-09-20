namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a free type parameter constraint declaration, including a main constraint and additional constraints.
/// </summary>
public class FreeTypeParameterConstraintDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Initializes a new instance of the FreeTypeParameterConstraintDeclarationModel class with the specified type
    /// parameter constraint and associated constraint values.
    /// </summary>
    /// <param name="constraint">The type parameter constraint to apply. Specify a value from the TypeParameterConstraintEnum enumeration, or
    /// null if no specific constraint is required.</param>
    /// <param name="constraints">A list of additional constraint values to associate with the type parameter. Cannot be null.</param>
    public FreeTypeParameterConstraintDeclarationModel(TypeParameterConstraintEnum? constraint, List<string> constraints)
    {
        Constraint = constraint;
        Constraints = constraints;
    }

    /// <summary>
    /// Gets or sets the main type parameter constraint (e.g., class, struct, new, notnull).
    /// </summary>
    public TypeParameterConstraintEnum? Constraint { get; set; }

    /// <summary>
    /// Gets or sets the list of additional constraints as strings (e.g., interface names, base types).
    /// </summary>
    public List<string> Constraints { get; set; } = [];
}
