namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Represents basic information about a module.
/// </summary>
/// <param name="name"></param>
/// <param name="displayName"></param>
/// <param name="description"></param>
/// <param name="website"></param>
/// <param name="documentation"></param>
public class BasicModuleInformation(string name, string displayName, string? description = null, string? website = null, string? documentation = null) : IModuleInformation
{
    /// <summary>
    /// Read-only property for the display name of the module.
    /// </summary>
    public string DisplayName => displayName;

    /// <summary>
    /// Read-only property for the name of the module.
    /// </summary>
    public string Name => name;

    /// <summary>
    /// Read-only property for the description of the module.
    /// </summary>
    public string? Description => description;

    /// <summary>
    /// Gets the website URL of the module.
    /// </summary>
    public string? Website => website;

    /// <summary>
    /// Gets the documentation associated with the module.
    /// </summary>
    public string? Documentation => documentation;
}