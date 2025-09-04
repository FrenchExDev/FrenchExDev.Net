using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="TypeParameterConstraintModel"/> instances.
/// Provides a fluent API to configure the type parameter name and its constraints for code generation scenarios.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var builder = new TypeParameterConstraintModelBuilder()
///     .TypeParameter("T")
///     .Constraint(new FreeTypeParameterConstraintDeclarationModel
///     {
///         Constraint = TypeParameterConstraintEnum.Class,
///         Constraints = new List<string> { "IMyInterface" }
///     });
/// var result = builder.Build();
/// </code>
/// </remarks>
public class TypeParameterConstraintModelBuilder : AbstractObjectBuilder<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>
{
    /// <summary>
    /// Stores the name of the type parameter to be constrained (e.g., "T").
    /// </summary>
    private string? _typeParameter;
    /// <summary>
    /// Stores the list of constraints applied to the type parameter.
    /// </summary>
    private readonly List<FreeTypeParameterConstraintDeclarationModel> _constraints = new();

    /// <summary>
    /// Sets the name of the type parameter.
    /// </summary>
    /// <param name="typeParameter">The type parameter name (e.g., "T").</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.TypeParameter("T");
    /// </example>
    public TypeParameterConstraintModelBuilder TypeParameter(string typeParameter)
    {
        _typeParameter = typeParameter;
        return this;
    }

    /// <summary>
    /// Adds a constraint to the type parameter.
    /// </summary>
    /// <param name="constraint">The constraint to add (e.g., class, struct, interface name).</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Constraint(new FreeTypeParameterConstraintDeclarationModel { Constraint = TypeParameterConstraintEnum.Class });
    /// </example>
    public TypeParameterConstraintModelBuilder Constraint(FreeTypeParameterConstraintDeclarationModel constraint)
    {
        _constraints.Add(constraint);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="TypeParameterConstraintModel"/> instance, validating required properties.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    /// <remarks>
    /// This method ensures that the type parameter name is provided. If not, a failure result is returned.
    /// </remarks>
    protected override IObjectBuildResult<TypeParameterConstraintModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        // Validate that the type parameter name is provided
        if (string.IsNullOrEmpty(_typeParameter))
        {
            exceptions.Add(new InvalidOperationException("Type parameter name must be provided."));
        }

        // Return failure if any exceptions were collected
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        // Ensure the type parameter name is not null before proceeding
        ArgumentNullException.ThrowIfNull(_typeParameter);

        // Return a successful build result with the constructed TypeParameterConstraintModel
        return Success(new TypeParameterConstraintModel
        {
            TypeParameter = _typeParameter,
            Constraints = _constraints
        });
    }
}
