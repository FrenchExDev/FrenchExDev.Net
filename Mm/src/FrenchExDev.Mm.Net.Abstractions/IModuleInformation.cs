namespace FrenchExDev.Mm.Net.Abstractions;

/// <summary>
/// Represents basic information about a module.
/// </summary>
public interface IModuleInformation
{
    /// <summary>
    /// Gets the display name of the module.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets the name of the module.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the module
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the website URL of the module.
    /// </summary>
    string Website { get; }

    /// <summary>
    /// Gets the documentation URL of the module.
    /// </summary>
    string Documentation { get; }
}

public class BasicModuleInformation : IModuleInformation
{
    private readonly string _displayName;
    private readonly string _name;
    private readonly string _description;
    private readonly string _website;
    private readonly string _documentation;

    public string DisplayName => _displayName;

    public string Name => _name;

    public string Description => _description;

    public string Website => _website;

    public string Documentation => _documentation;

    public BasicModuleInformation(string displayName, string name, string description, string website = "", string documentation = "")
    {
        ArgumentNullException.ThrowIfNull(displayName);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(description);

        _displayName = displayName;
        _name = name;
        _description = description;
        _website = website;
        _documentation = documentation;
    }
}

/// <summary>
/// Represents a builder for creating instances of <see cref="BasicModuleInformation"/>.
/// </summary>
public class BasicModuleInformationBuilder
{
    private string? _displayName;
    private string? _name;
    private string? _description;
    private string? _website;
    private string? _documentation;

    public BasicModuleInformationBuilder DisplayName(string displayName)
    {
        ArgumentNullException.ThrowIfNull(displayName);
        _displayName = displayName;
        return this;
    }

    public BasicModuleInformationBuilder Name(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        _name = name;
        return this;
    }

    public BasicModuleInformationBuilder Description(string description)
    {
        ArgumentNullException.ThrowIfNull(description);
        _description = description;
        return this;
    }

    public BasicModuleInformationBuilder Website(string website)
    {
        _website = website ?? "";
        return this;
    }

    public BasicModuleInformationBuilder Documentation(string documentation)
    {
        _documentation = documentation ?? "";
        return this;
    }

    public BasicModuleInformation Build()
    {
        return new BasicModuleInformation(
            _displayName ?? throw new ArgumentNullException(nameof(_displayName)),
            _name ?? throw new ArgumentNullException(nameof(_name)),
            _description ?? throw new ArgumentNullException(nameof(_description)),
            _website ?? string.Empty,
            _documentation ?? string.Empty);
    }
}