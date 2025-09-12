using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Provides a builder for constructing instances of <see cref="FieldDeclarationModel"/>.
/// </summary>
/// <remarks>
/// This builder allows the configuration of field modifiers, type, name, and an optional initializer for
/// creating a <see cref="FieldDeclarationModel"/>. Use the provided fluent methods to configure the field declaration
/// before calling <see cref="AbstractObjectBuilder{TObject, TBuilder}.Build"/> to generate the final model.
/// </remarks>
public class FieldDeclarationModelBuilder : AbstractObjectBuilder<FieldDeclarationModel, FieldDeclarationModelBuilder>
{
    /// <summary>
    /// Stores the list of modifiers applied to the field (e.g., public, static).
    /// </summary>
    private readonly List<string> _modifiers = new();
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
    public FieldDeclarationModelBuilder Modifier(string modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Sets the type of the field.
    /// </summary>
    /// <param name="type">The field type.</param>
    /// <returns>The current builder instance.</returns>
    public FieldDeclarationModelBuilder Type(string type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Sets the name of the field.
    /// </summary>
    /// <param name="name">The field name.</param>
    /// <returns>The current builder instance.</returns>
    public FieldDeclarationModelBuilder Name(string name)
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
    /// Builds the <see cref="FieldDeclarationModel"/> instance, validating required properties.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    protected override IObjectBuildResult<FieldDeclarationModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        // Validate that the field name is provided
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Field name must be provided."));
        }

        // If there are any exceptions, return a failure result
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        // Ensure required fields are not null
        ArgumentNullException.ThrowIfNull(_name);
        ArgumentNullException.ThrowIfNull(_type);
        ArgumentNullException.ThrowIfNull(_modifiers);

        // Return a successful build result with the constructed FieldDeclarationModel
        return Success(new FieldDeclarationModel
        {
            Modifiers = _modifiers,
            Type = _type,
            Name = _name,
            Initializer = _initializer
        });
    }
}
