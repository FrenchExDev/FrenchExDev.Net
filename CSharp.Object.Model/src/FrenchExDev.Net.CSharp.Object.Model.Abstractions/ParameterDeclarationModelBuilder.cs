using FrenchExDev.Net.CSharp.Object.Builder2;

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
public class ParameterDeclarationModelBuilder : AbstractBuilder<ParameterDeclarationModel>
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
    /// Validates the parameter's configuration and records any detected validation failures.
    /// </summary>
    /// <remarks>Validation ensures that required properties such as parameter name and type are set. If a
    /// default value is specified, a parameter name must also be provided. Any validation errors are added to the
    /// failures dictionary for further inspection.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures encountered during the validation process. Each failure is
    /// recorded with its associated parameter name and exception.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (_defaultValue is not null && string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_defaultValue), new InvalidOperationException("Parameter name must be provided when a default value is set."));
        }

        if (string.IsNullOrEmpty(_type))
        {
            failures.Failure(nameof(_type), new InvalidOperationException("Parameter type must be provided."));
        }

        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Parameter name must be provided."));
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="ParameterDeclarationModel"/> using the configured type, name, and default
    /// value.
    /// </summary>
    /// <returns>A <see cref="ParameterDeclarationModel"/> initialized with the specified type, name, and default value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the parameter type or name has not been provided.</exception>
    protected override ParameterDeclarationModel Instantiate()
    {
        return new(
            _type ?? throw new InvalidOperationException("Parameter type must be provided."),
            _name ?? throw new InvalidOperationException("Parameter name must be provided."),
            _defaultValue);
    }
}