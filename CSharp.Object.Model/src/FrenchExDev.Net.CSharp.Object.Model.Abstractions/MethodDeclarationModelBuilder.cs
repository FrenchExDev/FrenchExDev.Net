using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="MethodDeclarationModel"/> instances.
/// Provides a fluent interface to set the method's name, modifiers, return type, parameters, body, and attributes.
/// Ensures that the method name is provided and validates all parameters and attributes before building the model.
/// </summary>
public class MethodDeclarationModelBuilder : AbstractObjectBuilder<MethodDeclarationModel, MethodDeclarationModelBuilder>
{
    /// <summary>
    /// Stores the name of the method to be created.
    /// </summary>
    private string? _name;
    /// <summary>
    /// Gets the list of modifiers applied to the method (e.g., public, static).
    /// </summary>
    public List<string> Modifiers { get; } = new();
    /// <summary>
    /// Stores the return type of the method. Defaults to "void".
    /// </summary>
    private string _returnType = "void";
    /// <summary>
    /// Stores the list of parameter builders for the method.
    /// </summary>
    private readonly List<ParameterDeclarationModelBuilder> _parameters = new();
    /// <summary>
    /// Stores the body of the method as a string, or null if not set.
    /// </summary>
    private string? _body;
    /// <summary>
    /// Stores the list of attribute builders for the method.
    /// </summary>
    private readonly List<AttributeDeclarationModelBuilder> _attributes = new();

    /// <summary>
    /// Sets the name of the method.
    /// </summary>
    /// <param name="name">The name to assign to the method.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds a modifier to the method (e.g., public, static).
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder Modifier(string modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Sets the return type of the method.
    /// </summary>
    /// <param name="returnType">The return type to set.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder ReturnType(string returnType)
    {
        _returnType = returnType;
        return this;
    }

    /// <summary>
    /// Adds a parameter to the method using a builder action.
    /// </summary>
    /// <param name="body">An action to configure the parameter builder.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder Parameter(Action<ParameterDeclarationModelBuilder> body)
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
    public MethodDeclarationModelBuilder Body(string body)
    {
        _body = body;
        return this;
    }

    /// <summary>
    /// Adds an attribute to the method using a builder action.
    /// </summary>
    /// <param name="body">An action to configure the attribute builder.</param>
    /// <returns>The current builder instance.</returns>
    public MethodDeclarationModelBuilder Attribute(Action<AttributeDeclarationModelBuilder> body)
    {
        var builder = new AttributeDeclarationModelBuilder();
        body(builder);
        _attributes.Add(builder);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="MethodDeclarationModel"/> instance, validating the name, parameters, and attributes.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    protected override IObjectBuildResult<MethodDeclarationModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        // Validate that the method name is provided
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(nameof(_name), new InvalidOperationException("Method name must be provided."));
        }

        // Build all parameters and collect their results
        var buildParameters = BuildBuildList<ParameterDeclarationModel, ParameterDeclarationModelBuilder>(_parameters, visited);
        AddExceptions<ParameterDeclarationModel, ParameterDeclarationModelBuilder>(nameof(_parameters), buildParameters, exceptions);

        // Build all attributes and collect their results
        var attributes = BuildBuildList<AttributeDeclarationModel, AttributeDeclarationModelBuilder>(_attributes, visited);
        AddExceptions<AttributeDeclarationModel, AttributeDeclarationModelBuilder>(nameof(_attributes), attributes, exceptions);

        // Return failure if any exceptions were collected during parameter or attribute building
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        // Ensure the name is not null before proceeding
        ArgumentNullException.ThrowIfNull(_name);

        // Return a successful build result with the constructed MethodDeclarationModel
        return Success(new MethodDeclarationModel
        {
            Body = _body,
            Name = _name,
            Modifiers = Modifiers,
            ReturnType = _returnType,
            Parameters = buildParameters.ToResultList<ParameterDeclarationModel>(),
            Attributes = attributes.ToResultList<AttributeDeclarationModel>()
        });
    }
}
