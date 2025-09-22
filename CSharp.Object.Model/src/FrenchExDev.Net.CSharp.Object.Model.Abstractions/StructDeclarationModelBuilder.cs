using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="StructDeclarationModel"/> instances.
/// Provides a fluent API to configure struct name, modifiers, and attributes for code generation scenarios.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var builder = new StructDeclarationModelBuilder()
///     .Name("MyStruct")
///     .Modifier(StructModifier.Public)
///     .Attribute(new AttributeDeclarationModel { Name = "Serializable" });
/// var result = builder.Build();
/// </code>
/// </remarks>
public class StructDeclarationModelBuilder : AbstractBuilder<StructDeclarationModel>, IDeclarationModelBuilder
{
    /// <summary>
    /// Stores the name of the struct to be built.
    /// </summary>
    private string? _name;
    /// <summary>
    /// Gets the list of modifiers applied to the struct (e.g., public, readonly).
    /// </summary>
    public List<StructModifier> Modifiers { get; } = [];
    /// <summary>
    /// Stores the list of attributes applied to the struct.
    /// </summary>
    private readonly List<AttributeDeclarationModel> _attributes = [];

    /// <summary>
    /// Sets the name of the struct.
    /// </summary>
    /// <param name="name">The struct name (e.g., "MyStruct").</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Name("MyStruct");
    /// </example>
    public StructDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds a modifier to the struct (e.g., public, readonly).
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Modifier(StructModifier.Public);
    /// </example>
    public StructDeclarationModelBuilder Modifier(StructModifier modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Adds an attribute to the struct.
    /// </summary>
    /// <param name="attribute">The attribute to add.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Attribute(new AttributeDeclarationModel { Name = "Serializable" });
    /// </example>
    public StructDeclarationModelBuilder Attribute(AttributeDeclarationModel attribute)
    {
        _attributes.Add(attribute);
        return this;
    }

    /// <summary>
    /// Performs validation logic for the current object, recording any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any issues found during validation are added to this
    /// collection.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Struct name must be provided."));
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="StructDeclarationModel"/> using the current attributes, modifiers, and
    /// name.
    /// </summary>
    /// <returns>A <see cref="StructDeclarationModel"/> initialized with the configured attributes, modifiers, and name.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the struct name has not been set.</exception>
    protected override StructDeclarationModel Instantiate()
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(_name);

        return new StructDeclarationModel(_name, Modifiers, _attributes);
    }
}
