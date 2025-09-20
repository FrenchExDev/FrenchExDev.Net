using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Provides a builder for constructing instances of the TypeParameterDeclarationModel class, allowing configuration of
/// type parameter declarations for code generation or analysis.
/// </summary>
/// <remarks>Use this builder to set the name of a type parameter before creating a TypeParameterDeclarationModel
/// instance. The builder enforces that a valid name is provided prior to instantiation. This class is not thread-safe;
/// concurrent access should be synchronized if used across multiple threads.</remarks>
public class TypeParameterDeclarationModelBuilder : AbstractBuilder<TypeParameterDeclarationModel>
{
    /// <summary>
    /// Stores the name of the type parameter to be created.
    /// </summary>
    private string? _name;

    /// <summary>
    /// Sets the name of the type parameter for the model being built.
    /// </summary>
    /// <param name="name">The name to assign to the type parameter. Cannot be null or empty.</param>
    /// <returns>The current <see cref="TypeParameterDeclarationModelBuilder"/> instance to allow method chaining.</returns>
    public TypeParameterDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Creates a new instance of <see cref="TypeParameterDeclarationModel"/> using the current type parameter name.
    /// </summary>
    /// <returns>A <see cref="TypeParameterDeclarationModel"/> initialized with the type parameter name. The <c>Name</c> property
    /// will be set to the current value.</returns>
    protected override TypeParameterDeclarationModel Instantiate()
    {
        ArgumentNullException.ThrowIfNull(_name);

        return new(_name);
    }

    /// <summary>
    /// Performs validation on the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any issues found during validation are added to this
    /// collection.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Type parameter name must be provided."));
        }
    }
}
