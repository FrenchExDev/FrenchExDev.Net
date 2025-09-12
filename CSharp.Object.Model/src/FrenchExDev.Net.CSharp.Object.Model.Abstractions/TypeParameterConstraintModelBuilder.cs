using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using System;
using System.Collections.Generic;
namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for <see cref="TypeParameterConstraintModel"/> objects, allowing fluent configuration of type parameter name and constraints.
/// </summary>
/// <remarks>
/// Use <see cref="TypeParameter"/> to set the type parameter name and <see cref="Constraint"/> to add constraints.
/// Call <see cref="Build"/> to create a validated <see cref="TypeParameterConstraintModel"/> instance.
/// </remarks>
public class TypeParameterConstraintModelBuilder : AbstractObjectBuilder<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>
{
    private string? _typeParameter;
    private readonly List<FreeTypeParameterConstraintDeclarationModel> _constraints = new();
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
    /// Builds the <see cref="TypeParameterConstraintModel"/> instance, validating required fields.
    /// </summary>
    /// <param name="exceptions">A list to collect validation exceptions.</param>
    /// <param name="visited">A list of visited objects for circular reference detection.</param>
    /// <returns>The build result containing the model or validation errors.</returns>
    protected override IObjectBuildResult<TypeParameterConstraintModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_typeParameter))
        {
            exceptions.Add(new InvalidOperationException("Type parameter name must be provided."));
        }

        if (exceptions.Any())           
        {
            return Failure(exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_typeParameter);

        return Success(new TypeParameterConstraintModel
        {
            TypeParameter = _typeParameter,
            Constraints = _constraints
        });
    }
}
