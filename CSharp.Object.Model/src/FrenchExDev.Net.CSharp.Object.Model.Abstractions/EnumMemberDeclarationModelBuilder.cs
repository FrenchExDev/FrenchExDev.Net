using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for creating <see cref="EnumMemberDeclarationModel"/> instances.
/// Provides a fluent interface to set the name, value, and attributes of an enum member.
/// Ensures that the enum member name is provided before building the model.
/// </summary>
public class EnumMemberDeclarationModelBuilder : AbstractBuilder<EnumMemberDeclarationModel>, IDeclarationModelBuilder
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
    private readonly List<AttributeDeclarationModel> _attributes = [];

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

        return new(_name, _value, _attributes);
    }

    /// <summary>
    /// Performs validation on the current enum member and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any issues found during validation are added to this
    /// collection.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Enum member name must be provided."));
        }
    }
}
