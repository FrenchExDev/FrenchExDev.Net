using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Represents a builder for creating instances of <see cref="BasicModuleInformation"/>.
/// </summary>
public class BasicModuleInformationBuilder : AbstractBuilder<BasicModuleInformation>
{
    /// <summary>
    /// Stores the display name of the module to be built.
    /// </summary>
    private string? _displayName;

    /// <summary>
    /// Stores the name of the module to be built.
    /// </summary>
    private string? _name;

    /// <summary>
    /// Stores the description of the module to be built.
    /// </summary>
    private string? _description;

    /// <summary>
    /// Stores the website URL of the module to be built. This field is optional and defaults to an empty string if not provided.
    /// </summary>
    private string? _website;

    /// <summary>
    /// Stores the documentation URL of the module to be built. This field is optional and defaults to an empty string if not provided.
    /// </summary>
    private string? _documentation;

    /// <summary>
    /// Sets the display name for the module being built.
    /// </summary>
    /// <param name="displayName">The display name to assign to the module. Cannot be null.</param>
    /// <returns>The current instance of <see cref="BasicModuleInformationBuilder"/> to allow method chaining.</returns>
    public BasicModuleInformationBuilder DisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    /// <summary>
    /// Sets the name of the module to be built.
    /// </summary>
    /// <param name="name">The name to assign to the module. Cannot be null.</param>
    /// <returns>The current <see cref="BasicModuleInformationBuilder"/> instance with the updated name.</returns>
    public BasicModuleInformationBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the description for the module information being built.
    /// </summary>
    /// <param name="description">The description text to associate with the module. Can be null or empty if no description is required.</param>
    /// <returns>The current <see cref="BasicModuleInformationBuilder"/> instance to allow method chaining.</returns>
    public BasicModuleInformationBuilder Description(string description)
    {
        _description = description;
        return this;
    }

    /// <summary>
    /// Sets the website URL for the module information builder.
    /// </summary>
    /// <param name="website">The website URL to associate with the module. If null, an empty string is used.</param>
    /// <returns>The current instance of <see cref="BasicModuleInformationBuilder"/> to allow method chaining.</returns>
    public BasicModuleInformationBuilder Website(string website)
    {
        _website = website ?? "";
        return this;
    }

    /// <summary>
    /// Sets the documentation text for the module being built.
    /// </summary>
    /// <param name="documentation">The documentation content to associate with the module. If null, an empty string is used.</param>
    /// <returns>The current <see cref="BasicModuleInformationBuilder"/> instance to allow method chaining.</returns>
    public BasicModuleInformationBuilder Documentation(string documentation)
    {
        _documentation = documentation ?? "";
        return this;
    }

    /// <summary>
    /// Validates that required fields are present and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any missing required fields are added to this collection as
    /// failures.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidDataException($"The {nameof(Name)} field is required."));
        }

        if (string.IsNullOrEmpty(_displayName))
        {
            failures.Failure(nameof(_displayName), new InvalidDataException($"The {nameof(DisplayName)} field is required."));
        }

        if (string.IsNullOrEmpty(_description))
        {
            failures.Failure(nameof(_description), new InvalidDataException($"The {nameof(Description)} field is required."));
        }
    }

    /// <summary>
    /// Creates and returns a new instance of the module information using the configured display name, name,
    /// description, website, and documentation values.
    /// </summary>
    /// <returns>A <see cref="BasicModuleInformation"/> object containing the module's display name, name, description, website,
    /// and documentation. The website and documentation fields will be empty strings if not specified.</returns>
    /// <exception cref="InvalidDataException">Thrown if the display name, name, or description has not been set prior to calling this method.</exception>
    protected override BasicModuleInformation Instantiate()
    {
        return new BasicModuleInformation(
            _displayName ?? throw new InvalidDataException(nameof(_displayName)),
            _name ?? throw new InvalidDataException(nameof(_name)),
            _description ?? throw new InvalidDataException(nameof(_description)),
            _website ?? "",
            _documentation ?? ""
        );
    }
}