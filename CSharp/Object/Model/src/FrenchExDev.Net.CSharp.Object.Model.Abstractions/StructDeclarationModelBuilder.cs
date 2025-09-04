using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

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
public class StructDeclarationModelBuilder : AbstractObjectBuilder<StructDeclarationModel, StructDeclarationModelBuilder>
{
    /// <summary>
    /// Stores the name of the struct to be built.
    /// </summary>
    private string? _name;
    /// <summary>
    /// Gets the list of modifiers applied to the struct (e.g., public, readonly).
    /// </summary>
    public List<StructModifier> Modifiers { get; } = new();
    /// <summary>
    /// Stores the list of attributes applied to the struct.
    /// </summary>
    private readonly List<AttributeDeclarationModel> _attributes = new();

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
    /// Builds the <see cref="StructDeclarationModel"/> instance, validating required properties.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    /// <remarks>
    /// This method ensures that the struct name is provided. If not, a failure result is returned.
    /// </remarks>
    protected override IObjectBuildResult<StructDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        // Validate that the struct name is provided
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Struct name must be provided."));
        }

        // Return failure if any exceptions were collected
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        // Ensure the struct name is not null before proceeding
        ArgumentNullException.ThrowIfNull(_name);

        // Return a successful build result with the constructed StructDeclarationModel
        return Success(new StructDeclarationModel
        {
            Name = _name,
            Modifiers = Modifiers,
            Attributes = _attributes
        });
    }
}
