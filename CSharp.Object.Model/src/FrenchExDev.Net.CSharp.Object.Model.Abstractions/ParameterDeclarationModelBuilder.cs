using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="ParameterDeclarationModel"/> instances.
/// Provides a fluent API to configure the parameter's type, name, and optional default value for code generation scenarios.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var builder = new ParameterDeclarationModelBuilder()
///     .Type("int")
///     .Name("count")
///     .DefaultValue("0");
/// var result = builder.Build();
/// </code>
/// </remarks>
public class ParameterDeclarationModelBuilder : AbstractObjectBuilder<ParameterDeclarationModel, ParameterDeclarationModelBuilder>
{
    /// <summary>
    /// Stores the type of the parameter to be built (e.g., "int", "string").
    /// </summary>
    private string? _type;
    /// <summary>
    /// Stores the name of the parameter to be built.
    /// </summary>
    private string? _name;
    /// <summary>
    /// Stores the optional default value for the parameter.
    /// </summary>
    private string? _defaultValue;

    /// <summary>
    /// Sets the type of the parameter.
    /// </summary>
    /// <param name="type">The parameter type (e.g., "int").</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Type("int");
    /// </example>
    public ParameterDeclarationModelBuilder Type(string type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Sets the name of the parameter.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "count").</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Name("count");
    /// </example>
    public ParameterDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the default value of the parameter.
    /// </summary>
    /// <param name="defaultValue">The default value (e.g., "0").</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.DefaultValue("0");
    /// </example>
    public ParameterDeclarationModelBuilder DefaultValue(string defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    /// <summary>
    /// Builds the <see cref="ParameterDeclarationModel"/> instance, validating required properties.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    /// <remarks>
    /// This method ensures that both the type and name are provided. If either is missing, a failure result is returned.
    /// </remarks>
    protected override IObjectBuildResult<ParameterDeclarationModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        // Validate that the parameter type is provided
        if (string.IsNullOrEmpty(_type))
        {
            exceptions.Add(new InvalidOperationException("Parameter type must be provided."));
        }

        // Validate that the parameter name is provided
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Parameter name must be provided."));
        }

        // Return failure if any exceptions were collected
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        // Ensure required fields are not null before proceeding
        ArgumentNullException.ThrowIfNull(_type);
        ArgumentNullException.ThrowIfNull(_name);

        // Return a successful build result with the constructed ParameterDeclarationModel
        return Success(new ParameterDeclarationModel
        {
            Type = _type,
            Name = _name,
            DefaultValue = _defaultValue
        });
    }
}