using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for creating <see cref="AttributeDeclarationModel"/> instances.
/// Allows configuration of attribute name and arguments for code generation scenarios.
/// </summary>
public class AttributeDeclarationModelBuilder : AbstractBuilder<AttributeDeclarationModel>, IDeclarationModelBuilder
{
    /// <summary>
    /// Stores the name of the attribute to be built.
    /// </summary>
    private string? _name;

    /// <summary>
    /// Stores the list of arguments for the attribute constructor.
    /// </summary>
    private readonly List<string> _arguments = [];

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
    protected override AttributeDeclarationModel Instantiate() => new()
    {
        Name = _name ?? throw new DeclarationHaveNoNameException(),
        Arguments = _arguments
    };

    /// <summary>
    /// Performs validation on the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any issues found during validation are added to this
    /// collection.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Attribute name must be provided."));
        }
    }
}
