using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for creating <see cref="AttributeDeclarationModel"/> instances.
/// Allows configuration of attribute name and arguments for code generation scenarios.
/// </summary>
public class AttributeDeclarationModelBuilder : AbstractObjectBuilder<AttributeDeclarationModel, AttributeDeclarationModelBuilder>
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
    /// Builds the <see cref="AttributeDeclarationModel"/> instance.
    /// Validates that the attribute name is provided and collects any build errors.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    protected override IObjectBuildResult<AttributeDeclarationModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(nameof(_name), new InvalidOperationException("Attribute name must be provided."));
        }

        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_name);

        return Success(new AttributeDeclarationModel
        {
            Name = _name,
            Arguments = _arguments
        });
    }
}
