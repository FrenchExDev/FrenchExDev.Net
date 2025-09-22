using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for constructing <see cref="ConstructorDeclarationModel"/> instances.
/// This class provides a fluent API to configure the constructor's name, modifiers, parameters, and body for code generation scenarios.
/// It validates required properties and produces a build result indicating success or failure.
/// </summary>
public class ConstructorDeclarationModelBuilder : AbstractBuilder<ConstructorDeclarationModel>, IDeclarationModelBuilder
{
    /// <summary>
    /// Stores the name of the constructor, typically matching the class name.
    /// </summary>
    private string? _name;

    /// <summary>
    /// The list of modifiers applied to the constructor (e.g., public, private, static).
    /// </summary>
    public List<string> Modifiers { get; } = [];

    /// <summary>
    /// The list of parameters for the constructor.
    /// </summary>
    private readonly BuilderList<ParameterDeclarationModel, ParameterDeclarationModelBuilder> _parameters = [];

    /// <summary>
    /// The body of the constructor, represented as a string of C# code.
    /// </summary>
    private string? _body;

    /// <summary>
    /// Sets the name of the constructor.
    /// </summary>
    /// <param name="name">The constructor name, usually the class name.</param>
    /// <returns>The current builder instance.</returns>
    public ConstructorDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds a modifier to the constructor (e.g., public, private, static).
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>The current builder instance.</returns>
    public ConstructorDeclarationModelBuilder Modifier(string modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Adds a parameter to the constructor.
    /// </summary>
    /// <param name="parameter">The parameter model to add.</param>
    /// <returns>The current builder instance.</returns>
    public ConstructorDeclarationModelBuilder Parameter(Action<ParameterDeclarationModelBuilder> body)
    {
        var builder = new ParameterDeclarationModelBuilder();
        body(builder);
        _parameters.Add(builder);
        return this;
    }

    /// <summary>
    /// Sets the body of the constructor as a string of C# code.
    /// </summary>
    /// <param name="body">The constructor body code.</param>
    /// <returns>The current builder instance.</returns>
    public ConstructorDeclarationModelBuilder Body(string body)
    {
        _body = body;
        return this;
    }

    /// <summary>
    /// Performs the core build operation using the specified collection of visited objects.
    /// </summary>
    /// <param name="visitedCollector">A dictionary that tracks objects already visited during the build process. Used to prevent redundant processing
    /// and handle object references.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        BuildList(_parameters, visitedCollector);
    }

    /// <summary>
    /// Performs validation logic specific to the derived constructor, recording any failures encountered.
    /// </summary>
    /// <remarks>Override this method in derived classes to implement custom validation rules. Validation
    /// failures should be added to the <paramref name="failures"/> dictionary to ensure they are reported
    /// appropriately.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each failure is recorded with its associated context and
    /// exception.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new DeclarationHaveNoNameException());
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="ConstructorDeclarationModel"/> using the current property values.
    /// </summary>
    /// <remarks>Throws an <see cref="ArgumentNullException"/> if the name property is null. The returned
    /// model reflects the current state of the properties at the time of invocation.</remarks>
    /// <returns>A <see cref="ConstructorDeclarationModel"/> initialized with the specified name, modifiers, parameters, and
    /// body.</returns>
    protected override ConstructorDeclarationModel Instantiate() => new()
    {
        Name = _name ?? throw new DeclarationHaveNoNameException(),
        Modifiers = Modifiers,
        Parameters = _parameters.AsReferenceList(),
        Body = _body
    };
}
