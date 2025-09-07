using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for constructing <see cref="EnumDeclarationModel"/> instances.
/// Provides a fluent API to configure enum name, modifiers, attributes, underlying type, and members.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
public class EnumDeclarationModelBuilder : AbstractObjectBuilder<EnumDeclarationModel, EnumDeclarationModelBuilder>
{
    // Stores the enum name.
    private string? _name;
    /// <summary>
    /// List of modifiers applied to the enum (e.g., public, internal).
    /// </summary>
    public List<EnumModifier> Modifiers { get; } = new();

    // List of attributes decorating the enum.
    private readonly List<AttributeDeclarationModel> _attributes = new();

    // Stores the underlying type of the enum (e.g., byte, int).
    private string? _underlyingType;

    // List of members (fields) defined in the enum.
    private readonly List<EnumMemberDeclarationModel> _members = new();

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
    public EnumDeclarationModelBuilder Attribute(AttributeDeclarationModel attribute)
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
    public EnumDeclarationModelBuilder Member(EnumMemberDeclarationModel member)
    {
        _members.Add(member);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="EnumDeclarationModel"/> instance.
    /// Validates required properties and collects any build errors.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    protected override IObjectBuildResult<EnumDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        // Validate required enum name
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Enum name must be provided."));
        }

        // If any errors were collected, return a failure result
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        // Ensure enum name is not null
        ArgumentNullException.ThrowIfNull(_name);

        // Return a successful build result with the constructed EnumDeclarationModel
        return Success(new EnumDeclarationModel
        {
            Name = _name,
            Modifiers = Modifiers,
            Attributes = _attributes,
            UnderlyingType = _underlyingType,
            Members = _members
        });
    }
}
