using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for <see cref="TypeParameterConstraintModel"/> objects, allowing fluent configuration of type parameter name and constraints.
/// </summary>
/// <remarks>
/// Use <see cref="TypeParameter"/> to set the type parameter name and <see cref="Constraint"/> to add constraints.
/// Call <see cref="Build"/> to create a validated <see cref="TypeParameterConstraintModel"/> instance.
/// </remarks>
public class TypeParameterConstraintModelBuilder : AbstractBuilder<TypeParameterConstraintModel>
{
    private string? _typeParameter;
    private readonly List<FreeTypeParameterConstraintDeclarationModel> _constraints = [];
    /// <summary>
    /// Sets the type parameter name for the constraint model.
    /// </summary>
    /// <param name="typeParameter">The name of the type parameter.</param>
    /// <returns>The builder instance for chaining.</returns>
    public TypeParameterConstraintModelBuilder TypeParameter(string typeParameter)
    {
        _typeParameter = typeParameter;
        return this;
    }
    /// <summary>
    /// Adds a constraint to the type parameter.
    /// </summary>
    /// <param name="constraint">A constraint declaration model.</param>
    /// <returns>The builder instance for chaining.</returns>
    public TypeParameterConstraintModelBuilder Constraint(FreeTypeParameterConstraintDeclarationModel constraint)
    {
        _constraints.Add(constraint);
        return this;
    }

    /// <summary>
    /// Validate the builder's state, ensuring required properties are set.
    /// </summary>
    /// <param name="visitedCollector"></param>
    /// <param name="failures"></param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_typeParameter))
        {
            failures.Failure(nameof(_typeParameter), new InvalidOperationException("Type parameter name must be provided."));
        }
    }

    /// <summary>
    /// Creates a new instance of the type parameter constraint model using the current type parameter and its
    /// associated constraints.
    /// </summary>
    /// <returns>A <see cref="TypeParameterConstraintModel"/> containing the type parameter and its constraints.</returns>
    protected override TypeParameterConstraintModel Instantiate()
    {
        ArgumentNullException.ThrowIfNull(_typeParameter);

        return new(_typeParameter, _constraints);
    }
}
