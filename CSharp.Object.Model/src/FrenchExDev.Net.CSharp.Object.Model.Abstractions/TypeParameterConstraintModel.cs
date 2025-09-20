namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;


/// <summary>
/// Represents the constraints applied to a generic type parameter, including its name and associated constraint
/// declarations.
/// </summary>
/// <remarks>Use this class to model and inspect the requirements imposed on a type parameter in generic type or
/// method definitions. Constraints may include base type requirements, interface implementations, or special modifiers
/// such as 'class', 'struct', or 'new()'.</remarks>
public class TypeParameterConstraintModel : IDeclarationModel
{
    /// <summary>
    /// Initializes a new instance of the TypeParameterConstraintModel class with the specified type parameter name and
    /// associated constraints.
    /// </summary>
    /// <param name="typeParameter">The name of the type parameter to which the constraints apply. Cannot be null or empty.</param>
    /// <param name="constraints">A list of constraint declarations that define the requirements for the type parameter. Cannot be null.</param>
    public TypeParameterConstraintModel(string typeParameter, List<FreeTypeParameterConstraintDeclarationModel> constraints)
    {
        TypeParameter = typeParameter;
        Constraints = constraints;
    }

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
    public List<FreeTypeParameterConstraintDeclarationModel> Constraints { get; set; } = [];
}
