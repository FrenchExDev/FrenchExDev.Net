using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for constructing <see cref="EnumDeclarationModel"/> instances.
/// Provides a fluent API to configure enum name, modifiers, attributes, underlying type, and members.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
public class EnumDeclarationModelBuilder : AbstractBuilder<EnumDeclarationModel>, IDeclarationModelBuilder
{
    // Stores the enum name.
    private string? _name;
    /// <summary>
    /// List of modifiers applied to the enum (e.g., public, internal).
    /// </summary>
    public List<EnumModifier> Modifiers { get; } = [];

    // List of attributes decorating the enum.
    private readonly BuilderList<AttributeDeclarationModel, AttributeDeclarationModelBuilder> _attributes = [];

    // Stores the underlying type of the enum (e.g., byte, int).
    private string? _underlyingType;

    // List of members (fields) defined in the enum.
    private readonly BuilderList<EnumMemberDeclarationModel, EnumMemberDeclarationModelBuilder> _members = [];

    /// <summary>
    /// Sets the enum name.
    /// </summary>
    /// <param name="name">The name of the enum.</param>
    /// <returns>The builder instance for chaining.</returns>
    public EnumDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds a modifier to the enum (e.g., public, internal).
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>The builder instance for chaining.</returns>
    public EnumDeclarationModelBuilder Modifier(EnumModifier modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Adds an attribute to the enum.
    /// </summary>
    /// <param name="attribute">The attribute to add.</param>
    /// <returns>The builder instance for chaining.</returns>
    public EnumDeclarationModelBuilder Attribute(AttributeDeclarationModelBuilder attribute)
    {
        _attributes.Add(attribute);
        return this;
    }

    /// <summary>
    /// Sets the underlying type of the enum (e.g., byte, int).
    /// </summary>
    /// <param name="underlyingType">The underlying type as a string.</param>
    /// <returns>The builder instance for chaining.</returns>
    public EnumDeclarationModelBuilder UnderlyingType(string underlyingType)
    {
        _underlyingType = underlyingType;
        return this;
    }

    /// <summary>
    /// Adds a member (field) to the enum.
    /// </summary>
    /// <param name="member">The enum member to add.</param>
    /// <returns>The builder instance for chaining.</returns>
    public EnumDeclarationModelBuilder Member(EnumMemberDeclarationModelBuilder member)
    {
        _members.Add(member);
        return this;
    }

    /// <summary>
    /// Creates a new instance of <see cref="EnumDeclarationModel"/> using the current builder state.
    /// </summary>
    /// <remarks>Throws an <see cref="ArgumentNullException"/> if the name has not been set. This method is
    /// typically called by consumers of the builder to finalize and retrieve the constructed enum
    /// declaration.</remarks>
    /// <returns>An <see cref="EnumDeclarationModel"/> initialized with the specified name, modifiers, attributes, underlying
    /// type, and members.</returns>
    protected override EnumDeclarationModel Instantiate()
    {
        ArgumentNullException.ThrowIfNull(_name);

        return new(_name, Modifiers, _attributes.AsReferenceList(), _underlyingType, _members.AsReferenceList());
    }

    /// <summary>
    /// Builds the internal representation of the object by processing its attributes and members, tracking visited
    /// objects to prevent redundant processing.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to record objects that have already been visited during the build process. This helps avoid
    /// processing the same object multiple times and prevents circular references.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        BuildList(_attributes, visitedCollector);
        BuildList(_members, visitedCollector);
    }

    /// <summary>
    /// Performs validation on the current enum object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any issues found during validation are added to this
    /// collection.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Enum name must be provided."));
        }
    }
}
