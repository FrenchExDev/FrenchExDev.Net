using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="PropertyDeclarationModel"/> instances.
/// Provides a fluent API to configure property modifiers, type, name, accessors, and optional initializer for code generation scenarios.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var builder = new PropertyDeclarationModelBuilder()
///     .Modifier("public")
///     .Type("int")
///     .Name("Id")
///     .HasGetter(true)
///     .HasSetter(false)
///     .Initializer("0");
/// var result = builder.Build();
/// </code>
/// </remarks>
public class PropertyDeclarationModelBuilder : AbstractBuilder<PropertyDeclarationModel>
{
    /// <summary>
    /// Stores the list of modifiers applied to the property (e.g., "public", "static").
    /// </summary>
    private readonly List<string> _modifiers = [];
    /// <summary>
    /// Stores the type of the property (e.g., "int", "string").
    /// </summary>
    private string _type = string.Empty;
    /// <summary>
    /// Stores the name of the property.
    /// </summary>
    private string _name = string.Empty;
    /// <summary>
    /// Indicates whether the property has a getter. Defaults to true.
    /// </summary>
    private bool _hasGetter = true;
    /// <summary>
    /// Indicates whether the property has a setter. Defaults to true.
    /// </summary>
    private bool _hasSetter = true;
    /// <summary>
    /// Stores the optional initializer for the property.
    /// </summary>
    private string? _initializer;

    /// <summary>
    /// Adds a modifier to the property (e.g., "public", "static").
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Modifier("public");
    /// </example>
    public PropertyDeclarationModelBuilder Modifier(string modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Sets the type of the property.
    /// </summary>
    /// <param name="type">The property type (e.g., "int").</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Type("int");
    /// </example>
    public PropertyDeclarationModelBuilder Type(string type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Sets the name of the property.
    /// </summary>
    /// <param name="name">The property name (e.g., "Id").</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Name("Id");
    /// </example>
    public PropertyDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets whether the property has a getter.
    /// </summary>
    /// <param name="hasGetter">True if the property should have a getter; otherwise, false.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.HasGetter(true);
    /// </example>
    public PropertyDeclarationModelBuilder HasGetter(bool hasGetter)
    {
        _hasGetter = hasGetter;
        return this;
    }

    /// <summary>
    /// Sets whether the property has a setter.
    /// </summary>
    /// <param name="hasSetter">True if the property should have a setter; otherwise, false.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.HasSetter(false);
    /// </example>
    public PropertyDeclarationModelBuilder HasSetter(bool hasSetter)
    {
        _hasSetter = hasSetter;
        return this;
    }

    /// <summary>
    /// Sets the optional initializer for the property.
    /// </summary>
    /// <param name="initializer">The initializer value, or null if none.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Initializer("0");
    /// </example>
    public PropertyDeclarationModelBuilder Initializer(string? initializer)
    {
        _initializer = initializer;
        return this;
    }

    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        // No additional build steps required for this builder.
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Property name must be provided."));
        }
    }

    protected override PropertyDeclarationModel Instantiate()
    {
        // Ensure the property name is not null before proceeding
        ArgumentNullException.ThrowIfNull(_name);

        // Return a successful build result with the constructed PropertyDeclarationModel
        return new(_modifiers, _type, _name, _hasGetter, _hasSetter, _initializer);
    }
}
