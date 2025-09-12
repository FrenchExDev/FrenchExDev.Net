using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for constructing <see cref="ConstructorDeclarationModel"/> instances.
/// This class provides a fluent API to configure the constructor's name, modifiers, parameters, and body for code generation scenarios.
/// It validates required properties and produces a build result indicating success or failure.
/// </summary>
public class ConstructorDeclarationModelBuilder : AbstractObjectBuilder<ConstructorDeclarationModel, ConstructorDeclarationModelBuilder>
{
    /// <summary>
    /// Stores the name of the constructor, typically matching the class name.
    /// </summary>
    private string? _name;

    /// <summary>
    /// The list of modifiers applied to the constructor (e.g., public, private, static).
    /// </summary>
    public List<string> Modifiers { get; } = new();

    /// <summary>
    /// The list of parameters for the constructor.
    /// </summary>
    private readonly List<ParameterDeclarationModelBuilder> _parameters = new();

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
    /// Builds the <see cref="ConstructorDeclarationModel"/> instance.
    /// Validates that the constructor name is provided and collects any build errors.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    protected override IObjectBuildResult<ConstructorDeclarationModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        var parameters = BuildList<ParameterDeclarationModel, ParameterDeclarationModelBuilder>(_parameters, visited);
        AddExceptions<ParameterDeclarationModel, ParameterDeclarationModelBuilder>(nameof(_parameters).ToMemberName(), parameters, exceptions);

        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(nameof(_name), new InvalidOperationException("Constructor name must be provided."));
        }

        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_name);

        return Success(new ConstructorDeclarationModel
        {
            Name = _name,
            Modifiers = Modifiers,
            Parameters = parameters.OfType<SuccessObjectBuildResult<ParameterDeclarationModel>>().Select(x => x.Result).ToList(),
            Body = _body
        });
    }
}
