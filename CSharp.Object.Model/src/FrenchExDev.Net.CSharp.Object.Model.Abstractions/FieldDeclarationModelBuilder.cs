using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Provides a builder for constructing instances of <see cref="FieldDeclarationModel"/>.
/// </summary>
/// <remarks>
/// This builder allows the configuration of field modifiers, type, name, and an optional initializer for
/// creating a <see cref="FieldDeclarationModel"/>. Use the provided fluent methods to configure the field declaration
/// before calling <see cref="DeconstructedAbstractObjectBuilder{TObject, TBuilder}.Build"/> to generate the final model.
/// </remarks>
public class FieldDeclarationModelBuilder : AbstractBuilder<FieldDeclarationModel>, IDeclarationModelBuilder
{
    /// <summary>
    /// Stores the list of modifiers applied to the field (e.g., public, static).
    /// </summary>
    private readonly List<string> _modifiers = [];
    /// <summary>
    /// Stores the type of the field (e.g., int, string).
    /// </summary>
    private string _type = string.Empty;
    /// <summary>
    /// Stores the name of the field.
    /// </summary>
    private string _name = string.Empty;
    /// <summary>
    /// Stores the optional initializer for the field.
    /// </summary>
    private string? _initializer;

    /// <summary>
    /// Adds a modifier to the field (e.g., public, static).
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>The current builder instance.</returns>
    public FieldDeclarationModelBuilder WithModifier(string modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Sets the type of the field.
    /// </summary>
    /// <param name="type">The field type.</param>
    /// <returns>The current builder instance.</returns>
    public FieldDeclarationModelBuilder WithType(string type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Sets the name of the field.
    /// </summary>
    /// <param name="name">The field name.</param>
    /// <returns>The current builder instance.</returns>
    public FieldDeclarationModelBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the optional initializer for the field.
    /// </summary>
    /// <param name="initializer">The initializer value, or null if none.</param>
    /// <returns>The current builder instance.</returns>
    public FieldDeclarationModelBuilder Initializer(string? initializer)
    {
        _initializer = initializer;
        return this;
    }

    /// <summary>
    /// Performs validation on the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any issues found during validation are added to this
    /// collection.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        // Validate that the field name is provided
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Field name must be provided."));
        }
    }

    /// <summary>
    /// Creates and returns a new instance of <see cref="FieldDeclarationModel"/> using the configured field name, type,
    /// modifiers, and optional initializer.
    /// </summary>
    /// <remarks>Throws an <see cref="ArgumentNullException"/> if the field name, type, or modifiers have not
    /// been set prior to calling this method.</remarks>
    /// <returns>A <see cref="FieldDeclarationModel"/> representing the field declaration with the specified properties.</returns>
    protected override FieldDeclarationModel Instantiate()
    {
        // Ensure required fields are not null
        ArgumentNullException.ThrowIfNull(_name);
        ArgumentNullException.ThrowIfNull(_type);
        ArgumentNullException.ThrowIfNull(_modifiers);

        // Return a successful build result with the constructed FieldDeclarationModel
        return new(_modifiers, _type, _name, _initializer);
    }
}
