using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for creating <see cref="AttributeDeclarationModel"/> instances.
/// Allows configuration of attribute name and arguments for code generation scenarios.
/// </summary>
public class AttributeDeclarationModelBuilder : DeconstructedAbstractObjectBuilder<AttributeDeclarationModel, AttributeDeclarationModelBuilder>
{
    /// <summary>
    /// Stores the name of the attribute to be built.
    /// </summary>
    private string? _name;

    /// <summary>
    /// Stores the list of arguments for the attribute constructor.
    /// </summary>
    private readonly List<string> _arguments = new();

    /// <summary>
    /// Sets the name of the attribute.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    /// <returns>The current builder instance.</returns>
    public AttributeDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds an argument to the attribute's argument list.
    /// </summary>
    /// <param name="argument">The argument value as a string.</param>
    /// <returns>The current builder instance.</returns>
    public AttributeDeclarationModelBuilder Argument(string argument)
    {
        _arguments.Add(argument);
        return this;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AttributeDeclarationModel"/> using the current attribute name and
    /// arguments.
    /// </summary>
    /// <returns>An <see cref="AttributeDeclarationModel"/> initialized with the specified name and arguments.</returns>
    protected override AttributeDeclarationModel Instantiate()
    {
        ArgumentNullException.ThrowIfNull(_name);

        return new AttributeDeclarationModel
        {
            Name = _name,
            Arguments = _arguments
        };
    }

    /// <summary>
    /// Validates the current object's state and adds any validation exceptions to the specified dictionary.
    /// </summary>
    /// <param name="exceptions">A dictionary to which validation exceptions are added if the object's state is invalid.</param>
    /// <param name="visited">A list of objects that have already been visited during validation to prevent redundant checks or circular
    /// references.</param>
    protected override void Validate(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(nameof(_name), new InvalidOperationException("Attribute name must be provided."));
        }
    }
}
