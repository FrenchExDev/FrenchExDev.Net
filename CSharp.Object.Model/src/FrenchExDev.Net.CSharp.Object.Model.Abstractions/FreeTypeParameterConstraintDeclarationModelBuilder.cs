using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="FreeTypeParameterConstraintDeclarationModel"/> instances.
/// Provides a fluent interface to set the main constraint and additional constraints for a type parameter.
/// Ensures that at least one constraint is provided before building the model.
/// </summary>
public class FreeTypeParameterConstraintDeclarationModelBuilder : AbstractObjectBuilder<FreeTypeParameterConstraintDeclarationModel, FreeTypeParameterConstraintDeclarationModelBuilder>
{
    /// <summary>
    /// Stores the main type parameter constraint (e.g., class, struct, new, notnull).
    /// </summary>
    private TypeParameterConstraintEnum? _constraint;
    /// <summary>
    /// Stores the list of additional constraints as strings (e.g., interface names, base types).
    /// </summary>
    private readonly List<string> _constraints = new();

    /// <summary>
    /// Sets the main type parameter constraint.
    /// </summary>
    /// <param name="constraint">The main constraint to set.</param>
    /// <returns>The current builder instance.</returns>
    public FreeTypeParameterConstraintDeclarationModelBuilder Constraint(TypeParameterConstraintEnum constraint)
    {
        _constraint = constraint;
        return this;
    }

    /// <summary>
    /// Adds an additional constraint as a string (e.g., interface or base type name).
    /// </summary>
    /// <param name="constraint">The constraint to add.</param>
    /// <returns>The current builder instance.</returns>
    public FreeTypeParameterConstraintDeclarationModelBuilder Type(string constraint)
    {
        _constraints.Add(constraint);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="FreeTypeParameterConstraintDeclarationModel"/> instance, validating that at least one constraint is provided.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    protected override IObjectBuildResult<FreeTypeParameterConstraintDeclarationModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        // Validate that at least one constraint is provided
        if (_constraint == null && !_constraints.Any())
        {
            exceptions.Add(nameof(_constraint), new InvalidOperationException("At least one constraint must be provided."));
        }

        // If there are any exceptions, return a failure result
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        return Success(new FreeTypeParameterConstraintDeclarationModel
        {
            Constraint = _constraint,
            Constraints = _constraints
        });
    }
}
