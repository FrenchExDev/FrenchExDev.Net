using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for creating <see cref="EnumMemberDeclarationModel"/> instances.
/// Provides a fluent interface to set the name, value, and attributes of an enum member.
/// Ensures that the enum member name is provided before building the model.
/// </summary>
public class EnumMemberDeclarationModelBuilder : DeconstructedAbstractObjectBuilder<EnumMemberDeclarationModel, EnumMemberDeclarationModelBuilder>
{
    /// <summary>
    /// Stores the name of the enum member to be created.
    /// </summary>
    private string? _name;
    /// <summary>
    /// Stores the value assigned to the enum member, or null if not set.
    /// </summary>
    private string? _value;
    /// <summary>
    /// Stores the list of attributes applied to the enum member.
    /// </summary>
    private readonly List<AttributeDeclarationModel> _attributes = new();

    /// <summary>
    /// Sets the name of the enum member.
    /// </summary>
    /// <param name="name">The name to assign to the enum member.</param>
    /// <returns>The current builder instance.</returns>
    public EnumMemberDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the value of the enum member.
    /// </summary>
    /// <param name="value">The value to assign to the enum member.</param>
    /// <returns>The current builder instance.</returns>
    public EnumMemberDeclarationModelBuilder Value(string value)
    {
        _value = value;
        return this;
    }

    /// <summary>
    /// Adds an attribute to the enum member.
    /// </summary>
    /// <param name="attribute">The attribute to add.</param>
    /// <returns>The current builder instance.</returns>
    public EnumMemberDeclarationModelBuilder Attribute(AttributeDeclarationModel attribute)
    {
        _attributes.Add(attribute);
        return this;
    }

    /// <summary>
    /// Creates a new <see cref="EnumMemberDeclarationModel"/> instance using the configured name, value, and
    /// attributes.
    /// </summary>
    /// <remarks>Throws an <see cref="ArgumentNullException"/> if the member name is not set. This method is
    /// typically called by derived classes to generate enum member declarations based on the current state.</remarks>
    /// <returns>An <see cref="EnumMemberDeclarationModel"/> initialized with the current member's name, value, and attributes.</returns>
    protected override EnumMemberDeclarationModel Instantiate()
    {
        ArgumentNullException.ThrowIfNull(_name);

        return new EnumMemberDeclarationModel
        {
            Name = _name,
            Value = _value,
            Attributes = _attributes
        };
    }

    /// <summary>
    /// Validates the current enum member and adds any validation errors to the specified exception dictionary.
    /// </summary>
    /// <param name="exceptions">A dictionary to which validation exceptions are added if the enum member is invalid.</param>
    /// <param name="visited">A list of objects that have already been visited during validation to prevent redundant checks or circular
    /// references.</param>
    protected override void Validate(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(nameof(_name), new InvalidOperationException("Enum member name must be provided."));
        }
    }
}
