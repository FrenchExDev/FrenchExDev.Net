namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a type parameter constraint for a generic type, including the type parameter name and its constraints.
/// Used for code generation and analysis scenarios where generic type parameter metadata is required.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var constraintModel = new TypeParameterConstraintModel
/// {
///     TypeParameter = "T",
///     Constraints = new List<FreeTypeParameterConstraintDeclarationModel>
///     {
///         new FreeTypeParameterConstraintDeclarationModel
///         {
///             Constraint = TypeParameterConstraintEnum.Class,
///             Constraints = new List<string> { "IMyInterface" }
///         }
///     }
/// };
/// </code>
/// </remarks>
public class TypeParameterConstraintModel : IDeclarationModel
{
    /// <summary>
    /// Gets or sets the name of the type parameter (e.g., "T").
    /// </summary>
    /// <remarks>
    /// Example: <c>TypeParameter = "T"</c>
    /// </remarks>
    public string TypeParameter { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of constraints applied to the type parameter.
    /// Each constraint can specify a main constraint (e.g., class, struct) and additional constraints (e.g., interfaces).
    /// </summary>
    /// <remarks>
    /// Example: <c>Constraints = new List&lt;FreeTypeParameterConstraintDeclarationModel&gt; { ... }</c>
    /// </remarks>
    public List<FreeTypeParameterConstraintDeclarationModel> Constraints { get; set; } = new();
}
