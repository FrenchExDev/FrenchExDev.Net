using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="MethodDeclarationModel"/> instances.
/// Provides a fluent interface to set the method's name, modifiers, return type, parameters, body, and attributes.
/// Ensures that the method name is provided and validates all parameters and attributes before building the model.
/// </summary>
public class MethodDeclarationModelBuilder : AbstractBuilder<MethodDeclarationModel>
{
    /// <summary>
    /// Stores the name of the method to be created.
    /// </summary>
    private string? _name;

    /// <summary>
    /// Gets the list of modifiers applied to the method (e.g., public, static).
    /// </summary>
    private List<string> _modifiers = [];

    /// <summary>
    /// Stores the return type of the method. Defaults to "void".
    /// </summary>
    private string _returnType = "void";

    /// <summary>
    /// Stores the list of parameter builders for the method.
    /// </summary>
    private readonly BuilderList<ParameterDeclarationModel, ParameterDeclarationModelBuilder> _parameters = [];

    /// <summary>
    /// Stores the body of the method as a string, or null if not set.
    /// </summary>
    private string? _body;

    /// <summary>
    /// Stores the list of attribute builders for the method.
    /// </summary>
    private readonly BuilderList<AttributeDeclarationModel, AttributeDeclarationModelBuilder> _attributes = [];

    /// <summary>
    /// Sets the name of the method.
    /// </summary>
    /// <param name="name">The name to assign to the method.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds a modifier to the method (e.g., public, static).
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder WithModifier(string modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Sets the return type of the method.
    /// </summary>
    /// <param name="returnType">The return type to set.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder WithReturnType(string returnType)
    {
        _returnType = returnType;
        return this;
    }

    /// <summary>
    /// Adds a parameter to the method using a builder action.
    /// </summary>
    /// <param name="body">An action to configure the parameter builder.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder WithParameter(Action<ParameterDeclarationModelBuilder> body)
    {
        var builder = new ParameterDeclarationModelBuilder();
        body(builder);
        _parameters.Add(builder);
        return this;
    }

    /// <summary>
    /// Sets the body of the method.
    /// </summary>
    /// <param name="body">The method body as a string.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }

    /// <summary>
    /// Adds an attribute to the method using a builder action.
    /// </summary>
    /// <param name="body">An action to configure the attribute builder.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder WithAttribute(Action<AttributeDeclarationModelBuilder> body)
    {
        var builder = new AttributeDeclarationModelBuilder();
        body(builder);
        _attributes.Add(builder);
        return this;
    }

    /// <summary>
    /// Builds the internal representation of the object by processing its parameters and attributes.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during the build process. This helps prevent
    /// redundant processing and circular references.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        BuildList(_parameters, visitedCollector);
        BuildList(_attributes, visitedCollector);
    }

    /// <summary>
    /// Performs validation on the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any issues found during validation are added to this
    /// collection.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Method name must be provided."));
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="MethodDeclarationModel"/> using the current declaration's name, modifiers,
    /// return type, parameters, and attributes.
    /// </summary>
    /// <returns>A <see cref="MethodDeclarationModel"/> representing the method declaration with the specified name, modifiers,
    /// return type, parameters, and attributes.</returns>
    /// <exception cref="DeclarationHaveNoNameException">Thrown if the declaration does not have a name.</exception>
    protected override MethodDeclarationModel Instantiate()
    {
        return new(_name ?? throw new DeclarationHaveNoNameException(), _modifiers, _returnType,
            _parameters.AsReferenceList(), _attributes.AsReferenceList(), _body);
    }
}
