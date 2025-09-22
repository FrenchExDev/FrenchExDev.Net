using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="FreeTypeParameterConstraintDeclarationModel"/> instances.
/// Provides a fluent interface to set the main constraint and additional constraints for a type parameter.
/// Ensures that at least one constraint is provided before building the model.
/// </summary>
public class FreeTypeParameterConstraintDeclarationModelBuilder : AbstractBuilder<FreeTypeParameterConstraintDeclarationModel>, IDeclarationModelBuilder
{
    /// <summary>
    /// Stores the main type parameter constraint (e.g., class, struct, new, notnull).
    /// </summary>
    private TypeParameterConstraintEnum? _constraint;
    /// <summary>
    /// Stores the list of additional constraints as strings (e.g., interface names, base types).
    /// </summary>
    private readonly List<string> _constraints = [];

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
    /// Performs validation on the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any detected issues are added to this collection.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (_constraint == null && _constraints.Count == 0)
        {
            failures.Failure(nameof(_constraint), new InvalidOperationException("At least one constraint must be provided."));
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="FreeTypeParameterConstraintDeclarationModel"/> initialized with the
    /// current constraint values.
    /// </summary>
    /// <returns>A <see cref="FreeTypeParameterConstraintDeclarationModel"/> containing the constraint and constraints associated
    /// with this declaration.</returns>
    protected override FreeTypeParameterConstraintDeclarationModel Instantiate()
    {
        return new(_constraint, _constraints);
    }
}
