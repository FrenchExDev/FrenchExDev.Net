using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for creating <see cref="EnumMemberDeclarationModel"/> instances.
/// Provides a fluent interface to set the name, value, and attributes of an enum member.
/// Ensures that the enum member name is provided before building the model.
/// </summary>
public class EnumMemberDeclarationModelBuilder : AbstractObjectBuilder<EnumMemberDeclarationModel, EnumMemberDeclarationModelBuilder>
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
    /// Builds the <see cref="EnumMemberDeclarationModel"/> instance, validating that the name is provided.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    protected override IObjectBuildResult<EnumMemberDeclarationModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        // Validate that the enum member name is provided
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Enum member name must be provided."));
        }

        // If there are any exceptions, return a failure result
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        // Ensure the name is not null before creating the model
        ArgumentNullException.ThrowIfNull(_name);

        // Return a successful build result with the constructed EnumMemberDeclarationModel
        return Success(new EnumMemberDeclarationModel
        {
            Name = _name,
            Value = _value,
            Attributes = _attributes
        });
    }
}
