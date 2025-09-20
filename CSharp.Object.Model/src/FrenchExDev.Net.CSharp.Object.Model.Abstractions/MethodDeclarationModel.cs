using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a method declaration, including its modifiers, return type, name, parameters, body, and attributes.
/// </summary>
public class MethodDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Initializes a new instance of the MethodDeclarationModel class with the specified method name, modifiers, return
    /// type, parameters, attributes, and body.
    /// </summary>
    /// <param name="name">The name of the method to be represented by this model. Cannot be null or empty.</param>
    /// <param name="modifiers">A list of modifiers applied to the method, such as 'public', 'static', or 'virtual'. May be empty if no
    /// modifiers are present.</param>
    /// <param name="returnType">The return type of the method, specified as a string. Cannot be null or empty.</param>
    /// <param name="parameters">A collection of parameter declarations for the method. May be empty if the method has no parameters.</param>
    /// <param name="attributes">A collection of attribute declarations applied to the method. May be empty if the method has no attributes.</param>
    /// <param name="body">The body of the method, represented as a string. May be empty for abstract or interface methods.</param>
    public MethodDeclarationModel(string name, List<string> modifiers, string returnType, ReferenceList<ParameterDeclarationModel> parameters, ReferenceList<AttributeDeclarationModel> attributes, string? body)
    {
        _name = name;
        _modifiers = modifiers;
        _returnType = returnType;
        _parameters = parameters;
        _attributes = attributes;
        _body = body;
    }

    /// <summary>
    /// Gets or sets the list of modifiers applied to the method (e.g., public, static).
    /// </summary>
    private List<string> _modifiers;

    /// <summary>
    /// Enumerable of modifiers applied to the method (e.g., public, static).
    /// </summary>
    public IEnumerable<string> Modifiers => _modifiers;

    /// <summary>
    /// Gets or sets the return type of the method (e.g., int, void).
    /// </summary>
    private string _returnType;

    /// <summary>
    /// Gets the name of the return type associated with the current context.
    /// </summary>
    public string ReturnType => _returnType;

    /// <summary>
    /// Gets or sets the name of the method.
    /// </summary>
    private string _name;

    /// <summary>
    /// Gets the name associated with this instance.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Stores the list of parameter declarations associated with the method.
    /// </summary>
    private ReferenceList<ParameterDeclarationModel> _parameters;

    /// <summary>
    /// Gets the collection of parameter declarations associated with the current model.
    /// </summary>
    public IEnumerable<ParameterDeclarationModel> Parameters => _parameters;

    /// <summary>
    /// Gets or sets the body of the method as a string. Null if the method is abstract or interface.
    /// </summary>
    private string? _body;

    /// <summary>
    /// Gets the body content as a string. Returns an empty string if no body is set.
    /// </summary>
    public string Body => _body ?? string.Empty;

    /// <summary>
    /// Stores the list of attribute declarations associated with the method.
    /// </summary>
    private ReferenceList<AttributeDeclarationModel> _attributes;

    /// <summary>
    /// Gets the collection of attribute declarations associated with this model.
    /// </summary>
    public IEnumerable<AttributeDeclarationModel> Attributes => _attributes;
}
